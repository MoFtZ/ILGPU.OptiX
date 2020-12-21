using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGPU.OptiX
{
    [CLSCompliant(false)]
    public enum OptixBuildFlags : uint
    {
        OPTIX_BUILD_FLAG_NONE = 0,
        OPTIX_BUILD_FLAG_ALLOW_UPDATE = 1u << 0,
        OPTIX_BUILD_FLAG_ALLOW_COMPACTION = 1u << 1,
        OPTIX_BUILD_FLAG_PREFER_FAST_TRACE = 1u << 2,
        OPTIX_BUILD_FLAG_PREFER_FAST_BUILD = 1u << 3,
        OPTIX_BUILD_FLAG_ALLOW_RANDOM_VERTEX_ACCESS = 1u << 4,
    }

    public enum OptixBuildOperation
    {
        OPTIX_BUILD_OPERATION_BUILD = 0x2161,
        OPTIX_BUILD_OPERATION_UPDATE = 0x2162,
    }

    [CLSCompliant(false)]
    public enum OptixMotionFlags : uint
    {
        OPTIX_MOTION_FLAG_NONE = 0u,
        OPTIX_MOTION_FLAG_START_VANISH = 1u << 0,
        OPTIX_MOTION_FLAG_END_VANISH = 1u << 1
    }

    [CLSCompliant(false)]
    public struct OptixMotionOptions
    {
        public ushort numKeys;
        public ushort flags;
        public float timeBegin;
        public float timeEnd;
    }

    [CLSCompliant(false)]
    public struct OptixAccelBuildOptions
    {
        public uint buildFlags;
        public OptixBuildOperation operation;
        public OptixMotionOptions motionOptions;
    }

    [CLSCompliant(false)]
    public struct OptixAccelBufferSizes
    {
        public ulong outputSizeInBytes;
        public ulong tempSizeInBytes;
        public ulong tempUpdateSizeInBytes;
    }
}
