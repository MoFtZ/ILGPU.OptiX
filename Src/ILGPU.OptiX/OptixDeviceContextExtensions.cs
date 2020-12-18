// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixDeviceContextExtensions.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.OptiX.Interop;
using ILGPU.OptiX.Util;
using System;
using System.Runtime.InteropServices;

// disable: max_line_length

namespace ILGPU.OptiX
{
    /// <summary>
    /// Extension functions for OptixDeviceContext.
    /// </summary>
    public static class OptixDeviceContextExtensions
    {
        /// <summary>
        /// Creates a new OptiX raygen kernel.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="raygenKernel">The raygen kernel.</param>
        /// <param name="moduleCompileOptions">The module compile options.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <returns>The raygen kernel.</returns>
        [CLSCompliant(false)]
        public static OptixKernel CreateRaygenKernel<TLaunchParams>(
            this OptixDeviceContext deviceContext,
            Action<TLaunchParams> raygenKernel,
            OptixModuleCompileOptions moduleCompileOptions,
            OptixPipelineCompileOptions pipelineCompileOptions)
            where TLaunchParams : unmanaged
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            using var module = deviceContext.CreateModule(
                raygenKernel,
                OptixKernel.RAYGEN_PREFIX,
                moduleCompileOptions,
                pipelineCompileOptions,
                out var raygenEntryFunctionName);
            using var name = SafeHGlobal.StringToHGlobalAnsi(raygenEntryFunctionName);
            using var programGroup = deviceContext.CreateProgramGroup(
                new OptixProgramGroupDesc()
                {
                    Kind = OptixProgramGroupKind.OPTIX_PROGRAM_GROUP_KIND_RAYGEN,
                    Raygen =
                        new OptixProgramGroupSingleModule()
                        {
                            Module = module.ModulePtr,
                            EntryFunctionName = name
                        }
                });
            return new OptixKernel(
                new[] { module.Transfer() },
                programGroup.Transfer());
        }

        /// <summary>
        /// Creates a new OptiX miss kernel.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="missKernel">The miss kernel.</param>
        /// <param name="moduleCompileOptions">The module compile options.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <returns>The miss kernel.</returns>
        [CLSCompliant(false)]
        public static OptixKernel CreateMissKernel<TLaunchParams>(
            this OptixDeviceContext deviceContext,
            Action<TLaunchParams> missKernel,
            OptixModuleCompileOptions moduleCompileOptions,
            OptixPipelineCompileOptions pipelineCompileOptions)
            where TLaunchParams : unmanaged
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            using var module = deviceContext.CreateModule(
                missKernel,
                OptixKernel.MISS_PREFIX,
                moduleCompileOptions,
                pipelineCompileOptions,
                out var missEntryFunctionName);
            using var name = SafeHGlobal.StringToHGlobalAnsi(missEntryFunctionName);

            using var programGroup = deviceContext.CreateProgramGroup(
                new OptixProgramGroupDesc()
                {
                    Kind = OptixProgramGroupKind.OPTIX_PROGRAM_GROUP_KIND_MISS,
                    Miss =
                        new OptixProgramGroupSingleModule()
                        {
                            Module = module.ModulePtr,
                            EntryFunctionName = name
                        }
                });
            return new OptixKernel(
                new[] { module.Transfer() },
                programGroup.Transfer());
        }

        /// <summary>
        /// Creates a new OptiX hitgroup kernel.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="closestHitKernel">The closest hit kernel.</param>
        /// <param name="anyHitKernel">The any hit kernel.</param>
        /// <param name="intersectionKernel">The intersection kernel.</param>
        /// <param name="moduleCompileOptions">The module compile options.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <returns>The hitgroup kernel.</returns>
        [CLSCompliant(false)]
        public static OptixKernel CreateHitgroupKernel<TLaunchParams>(
            this OptixDeviceContext deviceContext,
            Action<TLaunchParams> closestHitKernel,
            Action<TLaunchParams> anyHitKernel,
            Action<TLaunchParams> intersectionKernel,
            OptixModuleCompileOptions moduleCompileOptions,
            OptixPipelineCompileOptions pipelineCompileOptions)
            where TLaunchParams : unmanaged
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            using var closestHitModule =
                deviceContext.CreateModule(
                    closestHitKernel,
                    OptixKernel.CLOSESTHIT_PREFIX,
                    moduleCompileOptions,
                    pipelineCompileOptions,
                    out var closestHitEntryFunctionName);
            using var chName = SafeHGlobal.StringToHGlobalAnsi(closestHitEntryFunctionName);

            using var anyHitModule =
                deviceContext.CreateModule(
                   anyHitKernel,
                    OptixKernel.ANYHIT_PREFIX,
                    moduleCompileOptions,
                    pipelineCompileOptions,
                    out var anyHitEntryFunctionName);
            using var ahName = SafeHGlobal.StringToHGlobalAnsi(anyHitEntryFunctionName);

            using var intersectionModule =
                deviceContext.CreateModule(
                    intersectionKernel,
                    OptixKernel.INTERSECTION_PREFIX,
                    moduleCompileOptions,
                    pipelineCompileOptions,
                    out var intersectionEntryFunctionName);
            using var isName = SafeHGlobal.StringToHGlobalAnsi(intersectionEntryFunctionName);

            using var programGroup = deviceContext.CreateProgramGroup(
                new OptixProgramGroupDesc()
                {
                    Kind = OptixProgramGroupKind.OPTIX_PROGRAM_GROUP_KIND_HITGROUP,
                    Hitgroup =
                        new OptixProgramGroupHitgroup()
                        {
                            ModuleCH = closestHitModule?.ModulePtr ?? IntPtr.Zero,
                            EntryFunctionNameCH = chName,
                            ModuleAH = anyHitModule?.ModulePtr ?? IntPtr.Zero,
                            EntryFunctionNameAH = ahName,
                            ModuleIS = intersectionModule?.ModulePtr ?? IntPtr.Zero,
                            EntryFunctionNameIS = isName
                        }
                });

            return new OptixKernel(
                new[]
                {
                    closestHitModule?.Transfer(),
                    anyHitModule?.Transfer(),
                    intersectionModule?.Transfer()
                },
                programGroup.Transfer());
        }

        /// <summary>
        /// Creates a new OptiX module.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="kernelPrefix">The prefix to the kernel name.</param>
        /// <param name="moduleCompileOptions">The module compile options.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <param name="entryFunctionName">Filled in with the function name.</param>
        /// <returns>The module.</returns>
        private static OptixModule CreateModule<TLaunchParams>(
            this OptixDeviceContext deviceContext,
            Action<TLaunchParams> kernel,
            string kernelPrefix,
            OptixModuleCompileOptions moduleCompileOptions,
            OptixPipelineCompileOptions pipelineCompileOptions,
            out string entryFunctionName)
            where TLaunchParams : unmanaged
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            var m = kernel?.Method;
            if (m == null)
            {
                entryFunctionName = null;
                return null;
            }

            // Compile the action into PTX
            var ptxAssembly =
                deviceContext.GeneratePTX(
                    kernel,
                    kernelPrefix,
                    out entryFunctionName);

            return deviceContext.CreateModule(
                moduleCompileOptions,
                pipelineCompileOptions,
                ptxAssembly);
        }

        /// <summary>
        /// Creates a new OptiX module.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="moduleCompileOptions">The module compile options.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <param name="ptxString">The module PTX code.</param>
        /// <param name="functionName">The entry function name.</param>
        /// <returns>The module.</returns>
        [CLSCompliant(false)]
        public static OptixModule CreateModule(
            this OptixDeviceContext deviceContext,
            OptixModuleCompileOptions moduleCompileOptions,
            OptixPipelineCompileOptions pipelineCompileOptions,
            string ptxString)
        {
            var result = OptixAPI.Current.ModuleCreateFromPTX(
                deviceContext.DeviceContextPtr,
                moduleCompileOptions,
                pipelineCompileOptions,
                ptxString,
                out var module,
                out var logString);
            OptixException.ThrowIfFailed(result, logString);
            return new OptixModule(module);
        }

        /// <summary>
        /// Creates a new OptiX program group.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="programDescriptions">The program group descriptions.</param>
        /// <returns>The program group.</returns>
        [CLSCompliant(false)]
        public static OptixProgramGroup CreateProgramGroup(
            this OptixDeviceContext deviceContext,
            params OptixProgramGroupDesc[] programDescriptions)
        {
            return deviceContext.CreateProgramGroup(default, programDescriptions);
        }

        /// <summary>
        /// Creates a new OptiX program group.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="programGroupOptions">The program group options.</param>
        /// <param name="programDescriptions">The program group descriptions.</param>
        /// <returns>The program group.</returns>
        [CLSCompliant(false)]
        public static OptixProgramGroup CreateProgramGroup(
            this OptixDeviceContext deviceContext,
            OptixProgramGroupOptions programGroupOptions,
            params OptixProgramGroupDesc[] programDescriptions)
        {
            var result =
                OptixAPI.Current.ProgramGroupCreate(
                    deviceContext.DeviceContextPtr,
                    programDescriptions,
                    programGroupOptions,
                    out var programGroup,
                    out var logString);
            OptixException.ThrowIfFailed(result, logString);
            return new OptixProgramGroup(programGroup);
        }

        /// <summary>
        /// Creates a new OptiX pipeline.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <param name="pipelineLinkOptions">The pipeline link options.</param>
        /// <param name="programGroups">The program groups.</param>
        /// <returns>The pipeline.</returns>
        [CLSCompliant(false)]
        public static OptixPipeline CreatePipeline(
            this OptixDeviceContext deviceContext,
            OptixPipelineCompileOptions pipelineCompileOptions,
            OptixPipelineLinkOptions pipelineLinkOptions,
            params OptixProgramGroup[] programGroups)
        {
            using var programGroupsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * programGroups.Length);
            IntPtr nextPtr = programGroupsPtr;

            foreach (var programGroup in programGroups)
            {
                Marshal.WriteIntPtr(nextPtr, programGroup.ProgramGroupPtr);
                nextPtr += Marshal.SizeOf<IntPtr>();
            }

            var result = OptixAPI.Current.PipelineCreate(
                deviceContext.DeviceContextPtr,
                pipelineCompileOptions,
                pipelineLinkOptions,
                programGroupsPtr,
                (uint)programGroups.Length,
                out var pipeline,
                out var logString);
            OptixException.ThrowIfFailed(result, logString);
            return new OptixPipeline(pipeline);
        }
    }
}
