// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixFunctionTable.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

#pragma warning disable CS0649 // Field is never assigned to

namespace ILGPU.OptiX
{
    internal struct OptixFunctionTable
    {
        public IntPtr OptixGetErrorName;
        public IntPtr OptixGetErrorString;
        public IntPtr OptixDeviceContextCreate;
        public IntPtr OptixDeviceContextDestroy;
        public IntPtr OptixDeviceContextGetProperty;
        public IntPtr OptixDeviceContextSetLogCallback;
        public IntPtr OptixDeviceContextSetCacheEnabled;
        public IntPtr OptixDeviceContextSetCacheLocation;
        public IntPtr OptixDeviceContextSetCacheDatabaseSizes;
        public IntPtr OptixDeviceContextGetCacheEnabled;
        public IntPtr OptixDeviceContextGetCacheLocation;
        public IntPtr OptixDeviceContextGetCacheDatabaseSizes;
        public IntPtr OptixModuleCreateFromPTX;
        public IntPtr OptixModuleDestroy;
        public IntPtr OptixBuiltinISModuleGet;
        public IntPtr OptixProgramGroupCreate;
        public IntPtr OptixProgramGroupDestroy;
        public IntPtr OptixProgramGroupGetStackSize;
        public IntPtr OptixPipelineCreate;
        public IntPtr OptixPipelineDestroy;
        public IntPtr OptixPipelineSetStackSize;
        public IntPtr OptixAccelComputeMemoryUsage;
        public IntPtr OptixAccelBuild;
        public IntPtr OptixAccelGetRelocationInfo;
        public IntPtr OptixAccelCheckRelocationCompatibility;
        public IntPtr OptixAccelRelocate;
        public IntPtr OptixAccelCompact;
        public IntPtr OptixConvertPointerToTraversableHandle;
        public IntPtr OptixSbtRecordPackHeader;
        public IntPtr OptixLaunch;
        public IntPtr OptixDenoiserCreate;
        public IntPtr OptixDenoiserDestroy;
        public IntPtr OptixDenoiserComputeMemoryResources;
        public IntPtr OptixDenoiserSetup;
        public IntPtr OptixDenoiserInvoke;
        public IntPtr OptixDenoiserSetModel;
        public IntPtr OptixDenoiserComputeIntensity;
        public IntPtr OptixDenoiserComputeAverageColor;
    }

    internal delegate OptixResult DeviceContextCreate(
        IntPtr cudaContext,
        OptixDeviceContextOptions options,
        out IntPtr deviceContext);

    internal delegate OptixResult DeviceContextDestroy(IntPtr deviceContext);

    internal delegate OptixResult ModuleCreateFromPTX(
        IntPtr deviceContext,
        IntPtr moduleCompileOptions,
        IntPtr pipelineCompileOptions,
        IntPtr ptxString,
        nuint ptxStringSize,
        IntPtr logString,
        ref nuint logStringSize,
        out IntPtr module);

    internal delegate OptixResult ModuleDestroy(IntPtr module);

    internal delegate OptixResult ProgramGroupCreate(
        IntPtr deviceContext,
        IntPtr programDescriptions,
        uint numProgramGroups,
        IntPtr programGroupOptions,
        IntPtr logString,
        ref nuint logStringSize,
        out IntPtr programGroups);

    internal delegate OptixResult ProgramGroupDestroy(IntPtr programGroup);

    internal delegate OptixResult PipelineCreate(
        IntPtr deviceContext,
        IntPtr pipelineCompileOptions,
        IntPtr pipelineLinkOptions,
        IntPtr programGroups,
        uint numProgramGroups,
        IntPtr logString,
        ref nuint logStringSize,
        out IntPtr pipeline);
    internal delegate OptixResult PipelineDestroy(IntPtr pipeline);

    internal delegate OptixResult PipelineSetStackSize(
        IntPtr pipeline,
        uint directCallableStackSizeFromTraversal,
        uint directCallableStackSizeFromState,
        uint continuationStackSize,
        uint maxTraversableGraphDepth);

    internal delegate OptixResult SbtRecordPackHeader(
        IntPtr programGroup,
        IntPtr sbtRecordHeaderHostPtr);

    internal delegate OptixResult Launch(
        IntPtr pipeline,
        IntPtr stream,
        IntPtr pipelineParams,
        uint pipelineParamsSize,
        IntPtr sbt,
        uint width,
        uint height,
        uint depth);
}

#pragma warning restore CS0649 // Field is never assigned to
