// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixLaunchExtensions.cs
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

namespace ILGPU.OptiX
{
    /// <summary>
    /// Convenience functions for launching.
    /// </summary>
    public static class OptixLaunchExtensions
    {
        [CLSCompliant(false)]
        public static void OptixLaunch<TLaunchParams>(
            this CudaAccelerator accelerator,
            OptixPipeline pipeline,
            TLaunchParams launchParams,
            OptixShaderBindingTable sbt,
            uint width,
            uint height,
            uint depth)
            where TLaunchParams : unmanaged
        {
            OptixLaunch(
                accelerator,
                accelerator.DefaultStream as CudaStream,
                pipeline,
                launchParams,
                sbt,
                width,
                height,
                depth);
        }

        [CLSCompliant(false)]
        public static void OptixLaunch<TLaunchParams>(
            this CudaAccelerator accelerator,
            CudaStream stream,
            OptixPipeline pipeline,
            TLaunchParams launchParams,
            OptixShaderBindingTable sbt,
            uint width,
            uint height,
            uint depth)
            where TLaunchParams : unmanaged
        {
            if (accelerator == null)
                throw new ArgumentNullException(nameof(accelerator));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (pipeline == null)
                throw new ArgumentNullException(nameof(pipeline));

            var launchParamsBuffer = accelerator.Allocate1D<TLaunchParams>(1);
            launchParamsBuffer.View.CopyFromCPU(stream, ref launchParams, 1);

            var sbtPtr = SafeHGlobal.AllocHGlobal(Marshal.SizeOf<OptixShaderBindingTable>());
            Marshal.StructureToPtr(sbt, sbtPtr, false);

            OptixException.ThrowIfFailed(
                OptixAPI.Current.Launch(
                    pipeline.PipelinePtr,
                    (accelerator.DefaultStream as CudaStream).StreamPtr,
                    launchParamsBuffer.NativePtr,
                    (uint)Marshal.SizeOf<TLaunchParams>(),
                    sbtPtr,
                    width,
                    height,
                    depth));
        }
    }
}
