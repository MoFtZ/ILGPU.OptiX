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
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
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
                            ModuleCH = closestHitModule.ModulePtr,
                            EntryFunctionNameCH = chName,
                            ModuleAH = anyHitModule.ModulePtr,
                            EntryFunctionNameAH = ahName,
                            ModuleIS = intersectionModule.ModulePtr,
                            EntryFunctionNameIS = isName
                        }
                });

            return new OptixKernel(
                new[]
                {
                    closestHitModule.Transfer(),
                    anyHitModule.Transfer(),
                    intersectionModule.Transfer()
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
            out string? entryFunctionName)
            where TLaunchParams : unmanaged
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            if (kernel?.Method == null)
            {
                entryFunctionName = null;
                return OptixModule.CreateEmpty();
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
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

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
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

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
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

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

        /// <summary>
        /// Calculates accelleration structure size
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="accelOptions">The acceleration structure build options.</param>
        /// <param name="buildInputs">The build inputs.</param>
        /// <returns>The acceleration structure size output.</returns>
        [CLSCompliant(false)]
        public unsafe static OptixAccelBufferSizes AccelComputeMemoryUsage(
            this OptixDeviceContext deviceContext,
            OptixAccelBuildOptions accelOptions,
            params OptixBuildInput[] buildInputs)
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));

            using var accelBuildOptions = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixAccelBuildOptions>());
            Marshal.StructureToPtr(accelOptions, accelBuildOptions, false);
            Trace.WriteLine(Marshal.SizeOf<OptixBuildInput>());
            using var accelBuildInputs = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixBuildInput>() * buildInputs.Length);
            IntPtr nextPtr = accelBuildInputs;
            foreach (var buildInput in buildInputs)
            {
                Marshal.StructureToPtr(buildInput, nextPtr, false);
                nextPtr += Marshal.SizeOf<OptixBuildInput>();
            }

            var bufferSizes = new OptixAccelBufferSizes[1];
            fixed (OptixAccelBufferSizes* bufferSizesPtr = bufferSizes)
            {
                var result = OptixAPI.Current.AccelComputeMemoryUsage(
                    deviceContext.DeviceContextPtr,
                    accelBuildOptions,
                    accelBuildInputs,
                    (uint)buildInputs.Length,
                    (IntPtr)bufferSizesPtr);

                OptixException.ThrowIfFailed(result);
            }
            return bufferSizes[0];
        }

        /// <summary>
        /// Builds Acceleration Structure
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="stream">The current cuda stream.</param>
        /// <param name="accelOptions">The acceleration structure build options.</param>
        /// <param name="buildInputs">The build inputs.</param>
        /// <param name="tempBuffer">The temp build buffer, after this call the temp buffer is filled with garbage.</param>
        /// <param name="outputBuffer">The build output buffer.</param>
        /// <returns>The output device pointer</returns>
        [CLSCompliant(false)]
        public unsafe static IntPtr AccelBuild(
            this OptixDeviceContext deviceContext,
            AcceleratorStream stream,
            OptixAccelBuildOptions accelOptions,
            ReadOnlySpan<OptixBuildInput> buildInputs,
            ArrayView1D<byte, Stride1D.Dense> tempBuffer,
            ArrayView1D<byte, Stride1D.Dense> outputBuffer,
            ReadOnlySpan<OptixAccelEmitDesc> emittedProperties)
        {
            if (deviceContext == null)
                throw new ArgumentNullException(nameof(deviceContext));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (stream is not CudaStream cudaStream)
                throw new ArgumentOutOfRangeException(nameof(stream));

            using var accelBuildOptions = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixAccelBuildOptions>());
            Marshal.StructureToPtr(accelOptions, accelBuildOptions, false);

            using var accelBuildInputs = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixBuildInput>() * buildInputs.Length);
            IntPtr nextPtr = accelBuildInputs;
            foreach (var buildInput in buildInputs)
            {
                Marshal.StructureToPtr(buildInput, nextPtr, false);
                nextPtr += Marshal.SizeOf<OptixBuildInput>();
            }

            using var emittedPropertiesInputs = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixAccelEmitDesc>() * emittedProperties.Length);
            IntPtr nextPtr1 = emittedPropertiesInputs;
            foreach (var emittedProperty in emittedProperties)
            {
                Marshal.StructureToPtr(emittedProperty, nextPtr1, false);
                nextPtr1 += Marshal.SizeOf<OptixAccelEmitDesc>();
            }

            var asHandle = new IntPtr[1];
            fixed (IntPtr* asHandlePtr = asHandle)
            {
                var result = OptixAPI.Current.AccelBuild(
                    deviceContext.DeviceContextPtr,
                    cudaStream.StreamPtr,
                    accelBuildOptions,
                    accelBuildInputs,
                    (uint)buildInputs.Length,
                    tempBuffer.BaseView.LoadEffectiveAddressAsPtr(),
                    (ulong)tempBuffer.LengthInBytes,
                    outputBuffer.BaseView.LoadEffectiveAddressAsPtr(),
                    (ulong)outputBuffer.LengthInBytes,
                    (IntPtr)asHandlePtr,
                    emittedPropertiesInputs,
                    (uint)emittedProperties.Length);

                OptixException.ThrowIfFailed(result);
            }

            return asHandle[0];
        }
    }
}
