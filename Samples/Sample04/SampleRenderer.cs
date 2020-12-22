using ILGPU;
using ILGPU.OptiX;
using ILGPU.OptiX.Interop;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace Sample04
{
    [StructLayout(LayoutKind.Sequential, Pack = OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT, Size = 48)]
    public unsafe struct RaygenRecord
    {
        public fixed byte Header[OptixAPI.OPTIX_SBT_RECORD_HEADER_SIZE];
        // just a dummy value - later examples will use more interesting
        // data here
        public int ObjectID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT, Size = 48)]
    public unsafe struct MissRecord
    {
        public fixed byte Header[OptixAPI.OPTIX_SBT_RECORD_HEADER_SIZE];
        // just a dummy value - later examples will use more interesting
        // data here
        public int ObjectID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT, Size = 48)]
    public unsafe struct HitgroupRecord
    {
        public fixed byte Header[OptixAPI.OPTIX_SBT_RECORD_HEADER_SIZE];
        // just a dummy value - later examples will use more interesting
        // data here
        public int ObjectID;
    }

    public class SampleRenderer
    {
        int width;
        int height;
        MainWindow window;

        Context context;
        CudaAccelerator accelerator;
        OptixDeviceContext deviceContext;

        OptixModuleCompileOptions moduleCompileOptions;
        OptixPipelineCompileOptions pipelineCompileOptions;
        OptixPipelineLinkOptions pipelineLinkOptions;

        OptixKernel raygenKernel;
        OptixKernel missKernel;
        OptixKernel hitgroupKernel;

        OptixKernel[] raygenKernels;
        OptixKernel[] missKernels;
        OptixKernel[] hitgroupKernels;
        OptixKernel[] allKernels;

        OptixPipeline pipeline;

        RaygenRecord[] raygenRecordsArray;
        MissRecord[] missRecordsArray;
        HitgroupRecord[] hitgroupRecordsArray;

        MemoryBuffer<RaygenRecord> raygenRecordsBuffer;
        MemoryBuffer<MissRecord> missRecordsBuffer;
        MemoryBuffer<HitgroupRecord> hitgroupRecordsBuffer;

        OptixShaderBindingTable sbt;

        MemoryBuffer<byte> colorBuffer0;
        MemoryBuffer<byte> colorBuffer1;

        byte[] colorArray;

        LaunchParams launchParams;

        Action<Index1, int, int, ArrayView<byte>, ArrayView<byte>> flipBitmap;

        //world data
        Camera camera;
        TriangleMesh model;
        MemoryBuffer<byte> asBuffer; 
        IntPtr traversable;

        public unsafe SampleRenderer(int width, int height, MainWindow window)
        {
            this.window = window;

            //init optix
            context = new Context();
            accelerator = new CudaAccelerator(context).InitOptiX();
            deviceContext = accelerator.CreateDeviceContext();

            moduleCompileOptions = new OptixModuleCompileOptions()
                {
                    MaxRegisterCount = 50,
                    OptimizationLevel = OptixCompileOptimizationLevel.OPTIX_COMPILE_OPTIMIZATION_DEFAULT,
                    DebugLevel = OptixCompileDebugLevel.OPTIX_COMPILE_DEBUG_LEVEL_NONE
                };

            pipelineCompileOptions = new OptixPipelineCompileOptions()
                {
                    TraversableGraphFlags = OptixTraversableGraphFlags.OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_SINGLE_GAS,
                    NumPayloadValues = 2,
                    NumAttributeValues = 2,
                    ExceptionFlags = OptixExceptionFlags.OPTIX_EXCEPTION_FLAG_NONE,
                    PipelineLaunchParamsVariableName = OptixLaunchParams.VariableName
                };

            pipelineLinkOptions = new OptixPipelineLinkOptions()
                {
                    MaxTraceDepth = 2
                };

            raygenKernel = deviceContext.CreateRaygenKernel<LaunchParams>(
                devicePrograms.__raygen__renderFrame,
                moduleCompileOptions,
                pipelineCompileOptions);

            missKernel = deviceContext.CreateMissKernel<LaunchParams>(
                devicePrograms.__miss__radiance,
                moduleCompileOptions,
                pipelineCompileOptions);

            hitgroupKernel = deviceContext.CreateHitgroupKernel<LaunchParams>(
                devicePrograms.__closest__radiance,
                devicePrograms.__anyhit__radiance,
                null,
                moduleCompileOptions,
                pipelineCompileOptions);

            raygenKernels = new[] { raygenKernel };
            missKernels = new[] { missKernel };
            hitgroupKernels = new[] { hitgroupKernel };
            allKernels = (raygenKernels.Concat(missKernels).Concat(hitgroupKernels)).ToArray();

            pipeline = deviceContext.CreatePipeline(
                pipelineCompileOptions,
                pipelineLinkOptions,
                allKernels.Select(x => x.ProgramGroup).ToArray());

            pipeline.SetStackSize(
                2 * 1024,
                2 * 1024,
                2 * 1024,
                1);


            // Setup SBT.
            raygenRecordsArray = OptixSbt.PackRecords<RaygenRecord>(raygenKernels);
            missRecordsArray = OptixSbt.PackRecords<MissRecord>(missKernels);
            hitgroupRecordsArray = OptixSbt.PackRecords<HitgroupRecord>(hitgroupKernels);

            raygenRecordsBuffer = accelerator.Allocate(raygenRecordsArray);
            missRecordsBuffer = accelerator.Allocate(missRecordsArray);
            hitgroupRecordsBuffer = accelerator.Allocate(hitgroupRecordsArray);

            sbt = new OptixShaderBindingTable()
                {
                    RaygenRecord = raygenRecordsBuffer.NativePtr,
                    MissRecordBase = missRecordsBuffer.NativePtr,
                    MissRecordStrideInBytes = (uint)Marshal.SizeOf<MissRecord>(),
                    MissRecordCount = (uint)missRecordsBuffer.Length,
                    HitgroupRecordBase = hitgroupRecordsBuffer.NativePtr,
                    HitgroupRecordStrideInBytes = (uint)Marshal.SizeOf<HitgroupRecord>(),
                    HitgroupRecordCount = (uint)hitgroupRecordsBuffer.Length
                };


            model = new TriangleMesh(accelerator);
            model.AddCube(new Affine3f(new Vec3(0, -1.5f, 0), new Vec3(10, 0.1f, 10)));
            model.AddCube(new Affine3f(new Vec3(0, 0, 0), new Vec3(2, 2, 2)));

            //                      from                   at                 up                                no hit color  vfov  scale
            camera = new Camera(new Vec3(-10, 2, -12), new Vec3(0, 0, 0), new Vec3(0, 1, 0), width, height, new Vec3(1, 1, 1), 40f, 10f);

            traversable = buildAccel(model);

            resize(width, height);
            flipBitmap = accelerator.LoadAutoGroupedStreamKernel<Index1, int, int, ArrayView<byte>, ArrayView<byte>>(devicePrograms.flipBitmap);
        }

        public void Dispose()
        {
            hitgroupRecordsBuffer.Dispose();
            missRecordsBuffer.Dispose();
            raygenRecordsBuffer.Dispose();

            pipeline.Dispose();

            hitgroupKernel.Dispose();
            missKernel.Dispose();
            raygenKernel.Dispose();

            deviceContext.Dispose();
            accelerator.Dispose();
            context.Dispose();
        }

        public unsafe void resize(int width, int height)
        {
            if (width != 0 && height != 0)
            {
                this.width = width;
                this.height = height;
                
                colorBuffer0 = accelerator.AllocateZero<byte>(width * height * sizeof(uint));
                colorBuffer1 = accelerator.AllocateZero<byte>(width * height * sizeof(uint));

                colorArray = new byte[colorBuffer0.Length];
                launchParams = new LaunchParams()
                {
                    ColorBuffer = (uint*)colorBuffer0.NativePtr,
                    camera = this.camera,
                    traversable = this.traversable
                };
            }
        }

        public unsafe IntPtr buildAccel(TriangleMesh model)
        {
            IntPtr asHandle = IntPtr.Zero;

            OptixBuildInput triangleInput = new OptixBuildInput()
            {
                type = OptixBuildInputType.OPTIX_BUILD_INPUT_TYPE_TRIANGLES,
            };

            var vertexBuffers = stackalloc IntPtr[1];
            vertexBuffers[0] = model.d_vertexBuffer.NativePtr;

            triangleInput.triangleArray.vertexFormat = OptixVertexFormat.OPTIX_VERTEX_FORMAT_FLOAT3;
            triangleInput.triangleArray.vertexStrideInBytes = (uint)sizeof(Vec3);
            triangleInput.triangleArray.numVerticies = (uint)model.vertexBuffer.Count;
            triangleInput.triangleArray.vertexBuffers = (IntPtr)vertexBuffers;

            triangleInput.triangleArray.indexFormat = OptixIndicesFormat.OPTIX_INDICES_FORMAT_UNSIGNED_INT3;
            triangleInput.triangleArray.indexStrideInBytes = (uint)sizeof(Vec3i);
            triangleInput.triangleArray.numIndexTriplets = (uint)model.triangleIndexBuffer.Count;
            triangleInput.triangleArray.indexBuffer = model.d_vertexBuffer.NativePtr;

            var triangleInputFlags = stackalloc uint[1];
            triangleInput.triangleArray.flags = triangleInputFlags;
            triangleInput.triangleArray.numSbtRecords = 1;
            triangleInput.triangleArray.sbtIndexOffsetBuffer = IntPtr.Zero;
            triangleInput.triangleArray.sbtIndexOffsetSizeInBytes = 0;
            triangleInput.triangleArray.sbtIndexOffsetStrideInBytes = 0;

            OptixAccelBuildOptions accelOptions = new OptixAccelBuildOptions()
            {
                buildFlags = (uint)OptixBuildFlags.OPTIX_BUILD_FLAG_NONE | (uint)OptixBuildFlags.OPTIX_BUILD_FLAG_ALLOW_COMPACTION,
                operation = OptixBuildOperation.OPTIX_BUILD_OPERATION_BUILD
            };
            accelOptions.motionOptions.numKeys = 1;

            OptixAccelBufferSizes blasBufferSizes = deviceContext.AccelComputeMemoryUsage(accelOptions, triangleInput);

            using MemoryBuffer<ulong> compactedSizeBuffer = accelerator.Allocate<ulong>(1);

            OptixAccelEmitDesc[] emitDesc = {
                new OptixAccelEmitDesc()
                {
                    type = OptixAccelPropertyType.OPTIX_PROPERTY_TYPE_COMPACTED_SIZE,
                    result = compactedSizeBuffer.NativePtr
                }
            };

            OptixBuildInput[] buildInputs =
            {
                triangleInput
            };

            using MemoryBuffer<byte> tempBuffer = accelerator.Allocate<byte>((long)blasBufferSizes.tempSizeInBytes);
            using MemoryBuffer<byte> outputBuffer = accelerator.Allocate<byte>((long)blasBufferSizes.outputSizeInBytes);

            asHandle = deviceContext.AccelBuild((CudaStream)accelerator.DefaultStream, accelOptions, buildInputs, blasBufferSizes, tempBuffer, outputBuffer, emitDesc);

            compactedSizeBuffer.CopyTo(out ulong compactedSize, 0);
            asBuffer = accelerator.Allocate<byte>((long)compactedSize);
            //breaks here


            return asHandle;
        }

        public void setCamera(Camera camera)
        {
        }

        public void render()
        {
            // Launch pipeline.

            launchParams.FrameID++;

            accelerator.OptixLaunch(
                accelerator.DefaultStream as CudaStream,
                pipeline,
                launchParams,
                sbt,
                (uint)width,
                (uint)height,
                1);

            //need to flip bitmap because of wpf weirdness
            flipBitmap(new Index1(width * height), width, height, colorBuffer0, colorBuffer1);
            accelerator.Synchronize();

            // Write output
            colorBuffer1.CopyTo(colorArray, 0, 0, colorBuffer1.Length);

            //draws colorArray to window and waits for completion avoiding locking
            Application.Current.Dispatcher.Invoke(() => { window.draw(ref colorArray); });

            Thread.Sleep(10);
        }
    }
}
