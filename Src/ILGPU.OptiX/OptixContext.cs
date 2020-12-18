// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixContext.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Backends.PTX;
using ILGPU.IR.Intrinsics;
using ILGPU.Runtime.Cuda;
using System;
using System.Reflection;

namespace ILGPU.OptiX
{
    public static class OptixContext
    {
        /// <summary>
        /// Initializes OptiX library.
        /// </summary>
        /// <param name="accelerator">The CUDA accelerator</param>
        /// <returns>The CUDA accelerator.</returns>
        public static CudaAccelerator InitOptiX(this CudaAccelerator accelerator)
        {
            OptixException.ThrowIfFailed(OptixAPI.Current.Init());

            // Register the Cuda instrinsics for OptiX
            var remappingType = typeof(OptixGetLaunchIndex);
            var methodInfo = remappingType.GetMethod(
                nameof(OptixGetLaunchIndex.X),
                BindingFlags.Public | BindingFlags.Static);
            accelerator.Context.IntrinsicManager.RegisterMethod(
                methodInfo,
                new PTXIntrinsic(
                    typeof(OptixGetLaunchIndex.Generator),
                    nameof(OptixGetLaunchIndex.Generator.GeneratePTXCode),
                    IntrinsicImplementationMode.GenerateCode));
            methodInfo = remappingType.GetMethod(
                nameof(OptixGetLaunchIndex.Y),
                BindingFlags.Public | BindingFlags.Static);
            accelerator.Context.IntrinsicManager.RegisterMethod(
                methodInfo,
                new PTXIntrinsic(
                    typeof(OptixGetLaunchIndex.Generator),
                    nameof(OptixGetLaunchIndex.Generator.GeneratePTXCode),
                    IntrinsicImplementationMode.GenerateCode));
            methodInfo = remappingType.GetMethod(
                nameof(OptixGetLaunchIndex.Z),
                BindingFlags.Public | BindingFlags.Static);
            accelerator.Context.IntrinsicManager.RegisterMethod(
                methodInfo,
                new PTXIntrinsic(
                    typeof(OptixGetLaunchIndex.Generator),
                    nameof(OptixGetLaunchIndex.Generator.GeneratePTXCode),
                    IntrinsicImplementationMode.GenerateCode));

            return accelerator;
        }

        /// <summary>
        /// Uniinitializes OptiX library.
        /// </summary>
        /// <param name="accelerator">The CUDA accelerator</param>
        /// <returns>The CUDA accelerator.</returns>
        public static CudaAccelerator UninitOptiX(this CudaAccelerator accelerator)
        {
            OptixException.ThrowIfFailed(OptixAPI.Current.Uninit());
            return accelerator;
        }

        /// <summary>
        /// Creates a new OptiX device context.
        /// </summary>
        /// <param name="accelerator">The CUDA accelerator.</param>
        /// <param name="options">The device context options.</param>
        /// <returns>The device context.</returns>
        [CLSCompliant(false)]
        public static OptixDeviceContext CreateDeviceContext(
            this CudaAccelerator accelerator,
            OptixDeviceContextOptions options = default)
        {
            OptixException.ThrowIfFailed(
                OptixAPI.Current.DeviceContextCreate(
                    accelerator.ContextPtr,
                    options,
                    out var deviceContext));
            return new OptixDeviceContext(accelerator, deviceContext);
        }
    }
}
