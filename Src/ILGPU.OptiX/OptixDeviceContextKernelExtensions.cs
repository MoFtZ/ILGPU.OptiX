// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixDeviceContextKernelExtensions.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Backends.EntryPoints;
using ILGPU.Backends.PTX;
using System;
using System.Text.RegularExpressions;

// disable: max_line_length

namespace ILGPU.OptiX
{
    /// <summary>
    /// Extension functions for OptixDeviceContext.
    /// </summary>
    public static class OptixDeviceContextKernelExtensions
    {
        /// <summary>
        /// Creates a PTX for the supplied kernel.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="kernelPrefix">The prefix to the kernel name.</param>
        /// <param name="entryFunctionName">Filled in with the function name.</param>
        /// <returns>The module.</returns>
        internal static string GeneratePTX<TLaunchParams>(
            this OptixDeviceContext deviceContext,
            Action<TLaunchParams> kernel,
            string kernelPrefix,
            out string entryFunctionName)
            where TLaunchParams : unmanaged
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            // Compile the action into PTX
            var m = kernel.Method;
            var backend = deviceContext.Backend;
            var entryPointDesc = EntryPointDescription.FromExplicitlyGroupedKernel(m);
            var ptx = backend.Compile(entryPointDesc, default) as PTXCompiledKernel;

            // WORKAROUND: OptiX loads the launch parameters into constant memory.
            // We generate a new entry point kernel that will load the launch parameters
            // and pass it into the ILGPU kernel.
            //
            // WORKAROUND: ILGPU uses a standard name for the kernel entry point.
            // This causes issues with duplicate name conflicts in OptiX, so rename
            // the ILGPU entry point.
            entryFunctionName = $"{kernelPrefix}{kernel.Method.Name}";
            var altKernelName = $"_{entryFunctionName}";

            var entryPointKernel = GenerateEntryKernel<TLaunchParams>(
                backend,
                entryFunctionName,
                altKernelName);
            var ptxAssembly = ptx.PTXAssembly.Replace(
                $" .entry {PTXCompiledKernel.EntryName}",
                $" .func {altKernelName}");

            ptxAssembly += entryPointKernel;
            return ptxAssembly;
        }

        internal static string GenerateEntryKernel<TLaunchParams>(
            PTXBackend backend,
            string entryFunctionName,
            string altKernelName)
            where TLaunchParams : unmanaged
        {
            // Load the dummy entry point kernel to work out the details of the
            // launch parameters.
            Action<TLaunchParams> x = EntryPointKernel;
            var placeholderKernel =
                backend.Compile(
                    EntryPointDescription.FromExplicitlyGroupedKernel(x.Method),
                    default) as PTXCompiledKernel;

            var ptx = placeholderKernel.PTXAssembly;

            // Strip anything before the entry point.
            ptx = ptx.Substring(ptx.IndexOf(".visible .entry"));

            // Renaming the entry point to the preferred name.
            ptx = ptx.Replace(
                $" .entry {PTXCompiledKernel.EntryName}",
                $" .entry {entryFunctionName}");

            // Identify the single function parameter of the kernel.

            var match = Regex.Match(ptx, "[.]param [.]align 8 [.]b8 (.+)[[](.+)[]]");
            if (!match.Success)
                throw new NotSupportedException();

            var line = match.Groups[0].Value;
            var variableName = match.Groups[1].Value;
            var variableSize = match.Groups[2].Value;

            // Remove the function parameter.
            ptx = ptx.Replace(line, string.Empty);

            // Change the PTX instructions for loading from each of the launch parameter
            // fields, and replace with a load from constant memory, and a store to a
            // register.
            ptx = Regex.Replace(
                ptx,
                "ld[.]param[.](\\S+)\\s+(\\S+),\\s(\\S+);",
                $"ld.const.$1 $2, $3;{Environment.NewLine}st.param.$1 $3, $2;");

            // Rename the original variable to the OptixLaunchParams variable name.
            ptx = ptx.Replace(variableName, OptixLaunchParams.VariableName);

            // Find the first load instruction, and inject a new parameter.
            const string ParamName = "callParam0";
            ptx = ptx.Insert(
                ptx.IndexOf("ld.const"),
                $".param .align 8 .b8 {ParamName}[{variableSize}];{Environment.NewLine}");

            // Replace all the store instructions to use the new parameter.
            ptx = Regex.Replace(ptx, "(st[.]param[.].+[[])([^]+])+", $"$1{ParamName}");

            // Inject a call to the renamed ILGPU kernel.
            ptx = ptx.Replace("ret;", $"call {altKernelName}, ({ParamName});");

            // Inject the OptiX launch parameter variable name into constant memory.
            ptx = ptx.Insert(0, $"{Environment.NewLine}" +
                $".const .align 8 .b8 {OptixLaunchParams.VariableName}[{variableSize}];{Environment.NewLine}");
            return ptx;
        }

        public static void EntryPointKernel<TLaunchParams>(TLaunchParams launchParams)
            where TLaunchParams : unmanaged
        { }
    }
}
