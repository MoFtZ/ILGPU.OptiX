// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixBuildInput.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace ILGPU.OptiX
{
    public enum OptixBuildInputType
    {
        /// <summary>
        /// Triangle inputs.
        /// </summary>
        OPTIX_BUILD_INPUT_TYPE_TRIANGLES = 0x2141,

        /// <summary>
        /// Custom primitive inputs.
        /// </summary>
        OPTIX_BUILD_INPUT_TYPE_CUSTOM_PRIMITIVES = 0x2142,

        /// <summary>
        /// Instance inputs.
        /// </summary>
        OPTIX_BUILD_INPUT_TYPE_INSTANCES = 0x2143,

        /// <summary>
        /// Instance pointer inputs.
        /// </summary>
        OPTIX_BUILD_INPUT_TYPE_INSTANCE_POINTERS = 0x2144,

        /// <summary>
        /// Curve inputs.
        /// </summary>
        OPTIX_BUILD_INPUT_TYPE_CURVES = 0x2145
    }

    public enum OptixVertexFormat
    {
        /// <summary>
        /// No vertices
        /// </summary>
        OPTIX_VERTEX_FORMAT_NONE = 0x0000,

        /// <summary>
        /// Vertices are represented by three floats
        /// </summary>
        OPTIX_VERTEX_FORMAT_FLOAT3 = 0x2121,

        /// <summary>
        /// Vertices are represented by two floats
        /// </summary>
        OPTIX_VERTEX_FORMAT_FLOAT2 = 0x2122,

        /// <summary>
        /// Vertices are represented by three halfs
        /// </summary>
        OPTIX_VERTEX_FORMAT_HALF3 = 0x2123,

        /// <summary>
        /// Vertices are represented by two halfs
        /// </summary>
        OPTIX_VERTEX_FORMAT_HALF2 = 0x2124,
        OPTIX_VERTEX_FORMAT_SNORM16_3 = 0x2125,
        OPTIX_VERTEX_FORMAT_SNORM16_2 = 0x2126
    }

    public enum OptixIndicesFormat
    {
        /// <summary>
        /// No indices, this format must only be used in combination with triangle soups
        /// i.e., numIndexTriplets must be zero
        /// </summary>
        OPTIX_INDICES_FORMAT_NONE = 0,

        /// <summary>
        /// // Three shorts.
        /// </summary>
        OPTIX_INDICES_FORMAT_UNSIGNED_SHORT3 = 0x2102,

        /// <summary>
        /// Three ints.
        /// </summary>
        OPTIX_INDICES_FORMAT_UNSIGNED_INT3 = 0x2103
    }

    public enum OptixTransformFormat
    {
        /// <summary>
        /// No transform, default for zero initialization.
        /// </summary>
        OPTIX_TRANSFORM_FORMAT_NONE = 0,

        /// <summary>
        /// 3x4 row major affine matrix.
        /// </summary>
        OPTIX_TRANSFORM_FORMAT_MATRIX_FLOAT12 = 0x21E1,
    }

    public enum OptixPrimitiveType
    {
        /// <summary>
        /// Custom primitive.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_CUSTOM = 0x2500,

        /// <summary>
        /// B-spline curve of degree 2 with circular cross-section.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_ROUND_QUADRATIC_BSPLINE = 0x2501,

        /// <summary>
        /// B-spline curve of degree 3 with circular cross-section.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_ROUND_CUBIC_BSPLINE = 0x2502,

        /// <summary>
        /// Piecewise linear curve with circular cross-section.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_ROUND_LINEAR = 0x2503,

        /// <summary>
        /// Triangle.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_TRIANGLE = 0x2531,
    }

    [StructLayout(LayoutKind.Explicit)]
    [CLSCompliant(false)]
    public unsafe struct OptixBuildInput
    {
        [FieldOffset(0)]
        public OptixBuildInputType Type;

        [FieldOffset(8)]
        public OptixBuildInputTriangleArray TriangleArray;

        [FieldOffset(8)]
        public OptixBuildInputCurveArray CurveArray;

        [FieldOffset(8)]
        public OptixBuildInputCustomPrimitiveArray CustomPrimitiveArray;

        [FieldOffset(8)]
        public OptixBuildInputInstanceArray InstanceArray;

        [FieldOffset(8)]
        public fixed byte Pad[1024];
    }

    [CLSCompliant(false)]
    public unsafe struct OptixBuildInputTriangleArray
    {
        public IntPtr VertexBuffers;
        public uint NumVerticies;
        public OptixVertexFormat VertexFormat;
        public uint VertexStrideInBytes;

        public IntPtr IndexBuffer;
        public uint NumIndexTriplets;
        public OptixIndicesFormat IndexFormat;
        public uint IndexStrideInBytes;

        public IntPtr PreTransform;

        public uint* Flags;

        public uint NumSbtRecords;
        public IntPtr SbtIndexOffsetBuffer;
        public uint SbtIndexOffsetSizeInBytes;
        public uint SbtIndexOffsetStrideInBytes;
        public uint PrimitiveIndexOffset;

        public OptixTransformFormat TransformFormat;
    }

    [CLSCompliant(false)]
    public unsafe struct OptixBuildInputCurveArray
    {
        public OptixPrimitiveType CurveType;
        public uint NumPrimitives;

        public IntPtr VertexBuffers;
        public uint NumVerticies;
        public uint VertexStrideInBytes;

        public IntPtr WidthBuffers;
        public uint WidthStrideInBytes;

        //according to optix_7_types.h this is reserved for future use
        public IntPtr NormalBuffers;
        public uint NormalStrideInBytes;

        public uint Flag;

        public uint PrimitiveIndexOffset;
    }

    [CLSCompliant(false)]
    public unsafe struct OptixBuildInputCustomPrimitiveArray
    {
        public IntPtr AabbBuffers;
        public uint NumPrimitives;
        public uint Stride;

        public uint* Flags;

        public uint NumSbtRecords;
        public IntPtr SbtIndexOffsetBuffer;
        public uint SbtIndexOffsetSizeInBytes;
        public uint SbtIndexOffsetStrideInBytes;
        public uint PrimitiveIndexOffset;
    }

    [CLSCompliant(false)]
    public struct OptixBuildInputInstanceArray
    {
        public IntPtr Instances;
        public uint NumInstances;
    }
}

#pragma warning restore CA1008 // Enums should have zero value
#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1815 // Override equals and operator equals on value types
