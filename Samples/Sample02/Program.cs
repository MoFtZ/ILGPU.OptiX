// ---------------------------------------------------------------------------------------
//                                    ILGPU Samples
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: Program.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU;
using ILGPU.OptiX;
using ILGPU.OptiX.Interop;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sample02
{
    public class Program
    {
        public unsafe static void __raygen__renderFrame(LaunchParams launchParams)
        {
            var ix = OptixGetLaunchIndex.X;
            var iy = OptixGetLaunchIndex.Y;

            uint r = (ix % 256);
            uint g = (iy % 256);
            uint b = ((ix + iy) % 256);

            // convert to 32-bit rgba value (we explicitly set alpha to 0xff
            // to make stb_image_write happy ...
            uint rgba = 0xff000000 | (r << 0) | (g << 8) | (b << 16);

            // and write to frame buffer ...
            long fbIndex = ix + iy * launchParams.FbSizeX;
            launchParams.ColorBuffer[fbIndex] = rgba;
        }

        public static void __miss__radiance(LaunchParams launchParams)
        { }

        public static void __closest__radiance(LaunchParams launchParams)
        { }

        public static void __anyhit__radiance(LaunchParams launchParams)
        { }

        unsafe static void Main()
        {
            using var context = Context.Create(b => b.Cuda().InitOptiX());
            using var accelerator = context.CreateCudaAccelerator(0);
            using var deviceContext = accelerator.CreateDeviceContext();

            // Setup OptiX pipeline.
            var moduleCompileOptions =
                new OptixModuleCompileOptions()
                {
                    MaxRegisterCount = 50,
                    OptimizationLevel = OptixCompileOptimizationLevel.OPTIX_COMPILE_OPTIMIZATION_DEFAULT,
                    DebugLevel = OptixCompileDebugLevel.OPTIX_COMPILE_DEBUG_LEVEL_NONE
                };

            var pipelineCompileOptions =
                new OptixPipelineCompileOptions()
                {
                    TraversableGraphFlags = OptixTraversableGraphFlags.OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_SINGLE_GAS,
                    NumPayloadValues = 2,
                    NumAttributeValues = 2,
                    ExceptionFlags = OptixExceptionFlags.OPTIX_EXCEPTION_FLAG_NONE,
                    PipelineLaunchParamsVariableName = OptixLaunchParams.VariableName
                };

            var pipelineLinkOptions =
                new OptixPipelineLinkOptions()
                {
                    MaxTraceDepth = 2
                };

            using var raygenKernel = deviceContext.CreateRaygenKernel<LaunchParams>(
                __raygen__renderFrame,
                moduleCompileOptions,
                pipelineCompileOptions);

            using var missKernel = deviceContext.CreateMissKernel<LaunchParams>(
                __miss__radiance,
                moduleCompileOptions,
                pipelineCompileOptions);

            using var hitgroupKernel = deviceContext.CreateHitgroupKernel<LaunchParams>(
                __closest__radiance,
                __anyhit__radiance,
                null,
                moduleCompileOptions,
                pipelineCompileOptions);

            var raygenKernels = new[] { raygenKernel };
            var missKernels = new[] { missKernel };
            var hitgroupKernels = new[] { hitgroupKernel };
            var allKernels = raygenKernels.Concat(missKernels).Concat(hitgroupKernels);

            using var pipeline = deviceContext.CreatePipeline(
                pipelineCompileOptions,
                pipelineLinkOptions,
                allKernels.Select(x => x.ProgramGroup).ToArray());

            pipeline.SetStackSize(
                2 * 1024,
                2 * 1024,
                2 * 1024,
                1);

            // Setup SBT.
            var raygenRecordsArray = OptixSbt.PackRecords<RaygenRecord>(raygenKernels);
            var missRecordsArray = OptixSbt.PackRecords<MissRecord>(missKernels);
            var hitgroupRecordsArray = OptixSbt.PackRecords<HitgroupRecord>(hitgroupKernels);
            using var raygenRecordsBuffer = accelerator.Allocate1D(raygenRecordsArray);
            using var missRecordsBuffer = accelerator.Allocate1D(missRecordsArray);
            using var hitgroupRecordsBuffer = accelerator.Allocate1D(hitgroupRecordsArray);
            var sbt =
                new OptixShaderBindingTable()
                {
                    RaygenRecord = raygenRecordsBuffer.NativePtr,
                    MissRecordBase = missRecordsBuffer.NativePtr,
                    MissRecordStrideInBytes = (uint)Marshal.SizeOf<MissRecord>(),
                    MissRecordCount = (uint)missRecordsBuffer.Length,
                    HitgroupRecordBase = hitgroupRecordsBuffer.NativePtr,
                    HitgroupRecordStrideInBytes = (uint)Marshal.SizeOf<HitgroupRecord>(),
                    HitgroupRecordCount = (uint)hitgroupRecordsBuffer.Length
                };

            // Setup launch parameters.
            var sizeX = 1200;
            var sizeY = 1024;
            using var colorBuffer = accelerator.Allocate1D<byte>(sizeX * sizeY * sizeof(uint));
            colorBuffer.MemSetToZero();

            var launchParams =
                new LaunchParams()
                {
                    ColorBuffer = (uint*)colorBuffer.NativePtr,
                    FbSizeX = sizeX,
                    FbSizeY = sizeY
                };

            // Launch pipeline.
            accelerator.OptixLaunch(
                accelerator.DefaultStream,
                pipeline,
                launchParams,
                sbt,
                (uint)launchParams.FbSizeX,
                (uint)launchParams.FbSizeY,
                1);
            accelerator.Synchronize();

            // Write output.
            var outputArray = colorBuffer.GetAsArray1D();
            using var pngStream = File.OpenWrite("sample02.png");
            var writer = new StbImageWriteSharp.ImageWriter();
            writer.WritePng(
                outputArray,
                launchParams.FbSizeX,
                launchParams.FbSizeY,
                StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha,
                pngStream);
        }
    }
}
