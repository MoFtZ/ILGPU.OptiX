// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixAccelBuildOptions.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1028 // Enum Storage should be Int32
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace ILGPU.OptiX
{
    [CLSCompliant(false)]
    [Flags]
    [SuppressMessage(
        "Naming",
        "CA1711:Identifiers should not have incorrect suffix")]
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

    public enum OptixAccelPropertyType
    {
        OPTIX_PROPERTY_TYPE_COMPACTED_SIZE = 0x2181,
        OPTIX_PROPERTY_TYPE_AABBS = 0x2182,
    }

    [CLSCompliant(false)]
    [Flags]
    [SuppressMessage(
        "Naming",
        "CA1711:Identifiers should not have incorrect suffix")]
    public enum OptixMotionFlags : uint
    {
        OPTIX_MOTION_FLAG_NONE = 0u,
        OPTIX_MOTION_FLAG_START_VANISH = 1u << 0,
        OPTIX_MOTION_FLAG_END_VANISH = 1u << 1
    }

    [CLSCompliant(false)]
    public struct OptixMotionOptions
    {
        public ushort NumKeys;
        public ushort Flags;
        public float TimeBegin;
        public float TimeEnd;
    }

    [CLSCompliant(false)]
    public struct OptixAccelBuildOptions
    {
        public OptixBuildFlags BuildFlags;
        public OptixBuildOperation Operation;
        public OptixMotionOptions MotionOptions;
    }

    [CLSCompliant(false)]
    public struct OptixAccelBufferSizes
    {
        public ulong OutputSizeInBytes;
        public ulong TempSizeInBytes;
        public ulong TempUpdateSizeInBytes;
    }

    public struct OptixAccelEmitDesc
    {
        public IntPtr Result;
        public OptixAccelPropertyType Type;
    }
}

#pragma warning restore CA1008 // Enums should have zero value
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1028 // Enum Storage should be Int32
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1815 // Override equals and operator equals on value types
