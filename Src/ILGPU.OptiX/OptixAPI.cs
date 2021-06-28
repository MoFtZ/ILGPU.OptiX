// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixAPI.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.OptiX.Interop;
using ILGPU.OptiX.Util;
using ILGPU.Util;
using System;
using System.Runtime.InteropServices;
using System.Text;

// disable: max_line_length

namespace ILGPU.OptiX
{
    /// <summary>
    /// Wrapper for the OptiX library functions.
    /// </summary>
    public sealed partial class OptixAPI : DisposeBase
    {
        #region Static

        public const int OPTIX_SBT_RECORD_ALIGNMENT = 16;
        public const int OPTIX_SBT_RECORD_HEADER_SIZE = 32;

        private const int DEFAULT_LOG_SIZE = 2048;

        /// <summary>
        /// The current instance of the API.
        /// </summary>
        public static OptixAPI Current { get; } = new OptixAPI();

        #endregion

        #region Instance

        /// <summary>
        /// The OptiX DLL handle.
        /// </summary>
        private IntPtr hmodule = IntPtr.Zero;

        /// <summary>
        /// The OptiX function table.
        /// </summary>
        private OptixFunctionTable functionTable;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new OptiX device context.
        /// </summary>
        /// <param name="cudaContext">The CUDA context.</param>
        /// <param name="options">The OptiX device context options.</param>
        /// <param name="deviceContext">Filled in with the new device context.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public OptixResult DeviceContextCreate(
            IntPtr cudaContext,
            OptixDeviceContextOptions options,
            out IntPtr deviceContext)
        {
            var func = Marshal.GetDelegateForFunctionPointer<DeviceContextCreate>(functionTable.OptixDeviceContextCreate);
            return func(cudaContext, options, out deviceContext);
        }

        /// <summary>
        /// Destroys the OptiX device context.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <returns>The OptiX result.</returns>
        public OptixResult DeviceContextDestroy(IntPtr deviceContext)
        {
            var func = Marshal.GetDelegateForFunctionPointer<DeviceContextDestroy>(functionTable.OptixDeviceContextDestroy);
            return func(deviceContext);
        }

        /// <summary>
        /// Creates a new OptiX module.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="moduleCompileOptions">The module compile options.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <param name="ptxString">The module PTX code.</param>
        /// <param name="module">Filled in with the new module.</param>
        /// <param name="logString">Filled in with the log string.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public unsafe OptixResult ModuleCreateFromPTX(
            IntPtr deviceContext,
            OptixModuleCompileOptions moduleCompileOptions,
            OptixPipelineCompileOptions pipelineCompileOptions,
            string ptxString,
            out IntPtr module,
            out string logString)
        {
            var func = Marshal.GetDelegateForFunctionPointer<ModuleCreateFromPTX>(functionTable.OptixModuleCreateFromPTX);

            using var moduleCompileOptionsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixModuleCompileOptions>());
            Marshal.StructureToPtr(moduleCompileOptions, moduleCompileOptionsPtr, false);

            using var pipelineCompileOptionsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixPipelineCompileOptions>());
            Marshal.StructureToPtr(pipelineCompileOptions, pipelineCompileOptionsPtr, false);

            var ptxStringBytes = Encoding.UTF8.GetBytes(ptxString);
            var logBytes = new byte[DEFAULT_LOG_SIZE];
            fixed (byte* ptxStringPtr = ptxStringBytes)
            fixed (byte* logPtr = logBytes)
            {
                nuint logLength = (nuint)logBytes.Length;
                var result = func(
                    deviceContext,
                    moduleCompileOptionsPtr,
                    pipelineCompileOptionsPtr,
                    (IntPtr)ptxStringPtr,
                    (nuint)ptxStringBytes.Length,
                    (IntPtr)logPtr,
                    ref logLength,
                    out module
                );
                if (result != OptixResult.OPTIX_SUCCESS)
                {
                    logString = Encoding.UTF8.GetString(logPtr, (int)logLength);
                }
                else
                {
                    logString = string.Empty;
                }
                return result;
            }
        }

        /// <summary>
        /// Destroys the OptiX module.
        /// </summary>
        /// <param name="module">The OptiX module.</param>
        /// <returns>The OptiX result.</returns>
        public OptixResult ModuleDestroy(IntPtr module)
        {
            var func = Marshal.GetDelegateForFunctionPointer<ModuleDestroy>(functionTable.OptixModuleDestroy);
            return func(module);
        }

        /// <summary>
        /// Creates a new OptiX program group.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="programDescriptions">The program group descriptions.</param>
        /// <param name="programGroupOptions">The program group options.</param>
        /// <param name="programGroup">Filled in with the new program group.</param>
        /// <param name="logString">Filled in with the log string.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public unsafe OptixResult ProgramGroupCreate(
            IntPtr deviceContext,
            OptixProgramGroupDesc[] programDescriptions,
            OptixProgramGroupOptions programGroupOptions,
            out IntPtr programGroup,
            out string logString)
        {
            var func = Marshal.GetDelegateForFunctionPointer<ProgramGroupCreate>(functionTable.OptixProgramGroupCreate);

            using var programGroupOptionsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixProgramGroupOptions>());
            Marshal.StructureToPtr(programGroupOptions, programGroupOptionsPtr, false);

            using var programDescriptionsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixProgramGroupDesc>() * programDescriptions.Length);
            IntPtr nextPtr = programDescriptionsPtr;
            foreach (var programDescription in programDescriptions)
            {
                Marshal.StructureToPtr(programDescription, nextPtr, false);
                nextPtr += Marshal.SizeOf<OptixProgramGroupDesc>();
            }

            var logBytes = new byte[DEFAULT_LOG_SIZE];
            fixed (byte* logPtr = logBytes)
            {
                nuint logLength = (nuint)logBytes.Length;
                var result = func(
                    deviceContext,
                    programDescriptionsPtr,
                    (uint)programDescriptions.Length,
                    programGroupOptionsPtr,
                    (IntPtr)logPtr,
                    ref logLength,
                    out programGroup
                );
                if (result != OptixResult.OPTIX_SUCCESS)
                {
                    logString = Encoding.UTF8.GetString(logPtr, (int)logLength);
                }
                else
                {
                    logString = string.Empty;
                }
                return result;
            }
        }

        /// <summary>
        /// Destroys the OptiX program group.
        /// </summary>
        /// <param name="programGroup">The OptiX program group.</param>
        /// <returns>The OptiX result.</returns>
        public OptixResult ProgramGroupDestroy(IntPtr programGroup)
        {
            var func = Marshal.GetDelegateForFunctionPointer<ProgramGroupDestroy>(functionTable.OptixProgramGroupDestroy);
            return func(programGroup);
        }

        /// <summary>
        /// Creates a new OptiX pipeline.
        /// </summary>
        /// <param name="deviceContext">The OptiX device context.</param>
        /// <param name="pipelineCompileOptions">The pipeline compile options.</param>
        /// <param name="pipelineLinkOptions">The pipeline link options.</param>
        /// <param name="programGroups">The program groups.</param>
        /// <param name="numProgramGroups">The number of program groups.</param>
        /// <param name="pipeline">Filled in with the new pipeline.</param>
        /// <param name="logString">Filled in with the log string.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public unsafe OptixResult PipelineCreate(
            IntPtr deviceContext,
            OptixPipelineCompileOptions pipelineCompileOptions,
            OptixPipelineLinkOptions pipelineLinkOptions,
            IntPtr programGroups,
            uint numProgramGroups,
            out IntPtr pipeline,
            out string logString)
        {
            var func = Marshal.GetDelegateForFunctionPointer<PipelineCreate>(functionTable.OptixPipelineCreate);

            using var pipelineCompileOptionsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixPipelineCompileOptions>());
            Marshal.StructureToPtr(pipelineCompileOptions, pipelineCompileOptionsPtr, false);

            using var pipelineLinkOptionsPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixPipelineLinkOptions>());
            Marshal.StructureToPtr(pipelineLinkOptions, pipelineLinkOptionsPtr, false);

            var logBytes = new byte[DEFAULT_LOG_SIZE];
            fixed (byte* logPtr = logBytes)
            {
                nuint logLength = (nuint)logBytes.Length;
                var result = func(
                    deviceContext,
                    pipelineCompileOptionsPtr,
                    pipelineLinkOptionsPtr,
                    programGroups,
                    numProgramGroups,
                    (IntPtr)logPtr,
                    ref logLength,
                    out pipeline
                );
                if (result != OptixResult.OPTIX_SUCCESS)
                {
                    logString = Encoding.UTF8.GetString(logPtr, (int)logLength);
                }
                else
                {
                    logString = string.Empty;
                }
                return result;
            }
        }

        /// <summary>
        /// Destroys the OptiX program group.
        /// </summary>
        /// <param name="pipeline">The OptiX pipeline.</param>
        /// <returns>The OptiX result.</returns>
        public OptixResult PipelineDestroy(IntPtr pipeline)
        {
            var func = Marshal.GetDelegateForFunctionPointer<PipelineDestroy>(functionTable.OptixPipelineDestroy);
            return func(pipeline);
        }

        /// <summary>
        /// Configures the pipeline stack size.
        /// </summary>
        /// <param name="pipeline">The OptiX pipeline.</param>
        /// <param name="directCallableStackSizeFromTraversal">
        /// The direct stack size requirement for direct callables invoked from IS or AH.
        /// </param>
        /// <param name="directCallableStackSizeFromState">
        /// The direct stack size requirement for direct callables invoked from RG, MS,
        /// or CH.</param>
        /// <param name="continuationStackSize">
        /// The continuation stack requirement.
        /// </param>
        /// <param name="maxTraversableGraphDepth">
        /// The maximum depth of a traversable graph passed to trace.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public OptixResult PipelineSetStackSize(
            IntPtr pipeline,
            uint directCallableStackSizeFromTraversal,
            uint directCallableStackSizeFromState,
            uint continuationStackSize,
            uint maxTraversableGraphDepth)
        {
            var func = Marshal.GetDelegateForFunctionPointer<PipelineSetStackSize>(functionTable.OptixPipelineSetStackSize);
            return func(
                pipeline,
                directCallableStackSizeFromTraversal,
                directCallableStackSizeFromState,
                continuationStackSize,
                maxTraversableGraphDepth);
        }

        /// <summary>
        /// Configures the pipeline stack size.
        /// </summary>
        /// <param name="programGroup">The OptiX program group.</param>
        /// <param name="sbtRecordHeaderHostPointer">
        /// Filled in with the result SBT record header.
        /// </param>
        /// <returns>The OptiX result.</returns>
        public OptixResult SbtRecordPackHeader(
            IntPtr programGroup,
            IntPtr sbtRecordHeaderHostPointer)
        {
            var func = Marshal.GetDelegateForFunctionPointer<SbtRecordPackHeader>(functionTable.OptixSbtRecordPackHeader);
            return func(
                programGroup,
                sbtRecordHeaderHostPointer);
        }

        /// <summary>
        /// Launches the OptiX pipeline.
        /// </summary>
        /// <param name="pipeline">The OptiX pipeline.</param>
        /// <param name="stream">The CUDA stream.</param>
        /// <param name="pipelineParams">The pipeline parameters.</param>
        /// <param name="pipelineParamsSize">The pipeline parameters size.</param>
        /// <param name="sbt">The shader binding table.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public OptixResult Launch(
            IntPtr pipeline,
            IntPtr stream,
            IntPtr pipelineParams,
            uint pipelineParamsSize,
            IntPtr sbt,
            uint width,
            uint height,
            uint depth)
        {
            var func = Marshal.GetDelegateForFunctionPointer<Launch>(functionTable.OptixLaunch);
            return func(
                pipeline,
                stream,
                pipelineParams,
                pipelineParamsSize,
                sbt,
                width,
                height,
                depth);
        }

        /// <summary>
        /// Calculates accelleration structure size
        /// </summary>
        /// <param name="context">The OptiX device context.</param>
        /// <param name="accelOptions">The acceleration structure build options.</param>
        /// <param name="buildInputs">The build inputs.</param>
        /// <param name="numBuildInputs">The build inputs count.</param>
        /// <param name="bufferSizes">The acceleration structure size output.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public OptixResult AccelComputeMemoryUsage(
        IntPtr context,
        IntPtr accelOptions,
        IntPtr buildInputs,
        uint numBuildInputs,
        IntPtr bufferSizes)
        {
            var func = Marshal.GetDelegateForFunctionPointer<AccelComputeMemoryUsage>(functionTable.OptixAccelComputeMemoryUsage);

            return func(
                context,
                accelOptions,
                buildInputs,
                numBuildInputs,
                bufferSizes);
        }

        /// <summary>
        /// Builds Acceleration Structure
        /// </summary>
        /// <param name="context">The OptiX device context.</param>
        /// <param name="stream">The CUDA stream.</param>
        /// <param name="accelOptions">The acceleration structure build options.</param>
        /// <param name="buildInputs">The build inputs.</param>
        /// <param name="numBuildInputs">The build inputs count.</param>
        /// <param name="tempBuffer">The temp build buffer, after this call the temp buffer is filled with garbage.</param>
        /// <param name="tempBufferSizeInBytes">The temp buffer size.</param>
        /// <param name="outputBuffer">The build output buffer.</param>
        /// <param name="outputBufferSizeInBytes">The output buffer size.</param>
        /// <param name="outputHandle">The OptixTraversableHandle pointer.</param>
        /// <param name="emittedProperties">The acceleration structure emitted properties.</param>
        /// <param name="numEmittedProperties">Emitted properties count.</param>
        /// <returns>The OptiX result.</returns>
        [CLSCompliant(false)]
        public OptixResult AccelBuild(
        IntPtr context,
        IntPtr stream,
        IntPtr accelOptions,
        IntPtr buildInputs,
        uint numBuildInputs,
        IntPtr tempBuffer,
        ulong tempBufferSizeInBytes,
        IntPtr outputBuffer,
        ulong outputBufferSizeInBytes,
        IntPtr outputHandle,
        IntPtr emittedProperties,
        uint numEmittedProperties)
        {
            var func = Marshal.GetDelegateForFunctionPointer<AccelBuild>(functionTable.OptixAccelBuild);

            return func(
                context,
                stream,
                accelOptions,
                buildInputs,
                numBuildInputs,
                tempBuffer,
                tempBufferSizeInBytes,
                outputBuffer,
                outputBufferSizeInBytes,
                outputHandle,
                emittedProperties,
                numEmittedProperties);
        }

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Uninit();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
