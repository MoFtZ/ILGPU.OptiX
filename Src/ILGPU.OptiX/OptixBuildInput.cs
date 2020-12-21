using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ILGPU.OptiX
{
    public enum OptixBuildInputType
    {
        /// Triangle inputs. \see #OptixBuildInputTriangleArray
        OPTIX_BUILD_INPUT_TYPE_TRIANGLES = 0x2141,
        /// Custom primitive inputs. \see #OptixBuildInputCustomPrimitiveArray
        OPTIX_BUILD_INPUT_TYPE_CUSTOM_PRIMITIVES = 0x2142,
        /// Instance inputs. \see #OptixBuildInputInstanceArray
        OPTIX_BUILD_INPUT_TYPE_INSTANCES = 0x2143,
        /// Instance pointer inputs. \see #OptixBuildInputInstanceArray
        OPTIX_BUILD_INPUT_TYPE_INSTANCE_POINTERS = 0x2144,
        /// Curve inputs. \see #OptixBuildInputCurveArray
        OPTIX_BUILD_INPUT_TYPE_CURVES = 0x2145
    }

    public enum OptixVertexFormat
    {
        OPTIX_VERTEX_FORMAT_NONE = 0x0000,  // No vertices
        OPTIX_VERTEX_FORMAT_FLOAT3 = 0x2121,// Vertices are represented by three floats
        OPTIX_VERTEX_FORMAT_FLOAT2 = 0x2122,// Vertices are represented by two floats
        OPTIX_VERTEX_FORMAT_HALF3 = 0x2123, // Vertices are represented by three halfs
        OPTIX_VERTEX_FORMAT_HALF2 = 0x2124, // Vertices are represented by two halfs
        OPTIX_VERTEX_FORMAT_SNORM16_3 = 0x2125,
        OPTIX_VERTEX_FORMAT_SNORM16_2 = 0x2126
    }

    public enum OptixIndicesFormat
    {
        // No indices, this format must only be used in combination with triangle soups, i.e., numIndexTriplets must be zero
        OPTIX_INDICES_FORMAT_NONE = 0,
        // Three shorts
        OPTIX_INDICES_FORMAT_UNSIGNED_SHORT3 = 0x2102,
        // Three ints
        OPTIX_INDICES_FORMAT_UNSIGNED_INT3 = 0x2103
    }

    public enum OptixTransformFormat
    {
        OPTIX_TRANSFORM_FORMAT_NONE = 0,       // no transform, default for zero initialization
        OPTIX_TRANSFORM_FORMAT_MATRIX_FLOAT12 = 0x21E1,  // 3x4 row major affine matrix
    }

    public enum OptixPrimitiveType
    {
        /// Custom primitive.
        OPTIX_PRIMITIVE_TYPE_CUSTOM = 0x2500,
        /// B-spline curve of degree 2 with circular cross-section.
        OPTIX_PRIMITIVE_TYPE_ROUND_QUADRATIC_BSPLINE = 0x2501,
        /// B-spline curve of degree 3 with circular cross-section.
        OPTIX_PRIMITIVE_TYPE_ROUND_CUBIC_BSPLINE = 0x2502,
        /// Piecewise linear curve with circular cross-section.
        OPTIX_PRIMITIVE_TYPE_ROUND_LINEAR = 0x2503,
        /// Triangle.
        OPTIX_PRIMITIVE_TYPE_TRIANGLE = 0x2531,
    }

    [StructLayout(LayoutKind.Explicit)]
    [CLSCompliant(false)]
    public unsafe struct OptixBuildInput
    {
        [FieldOffset(0)]
        public OptixBuildInputType type;
        [FieldOffset(4)]
        public OptixBuildInputTriangleArray triangleArray;
        [FieldOffset(4)]
        public OptixBuildInputCurveArray curveArray;
        [FieldOffset(4)]
        public OptixBuildInputCustomPrimitiveArray customPrimitiveArray;
        [FieldOffset(4)]
        public OptixBuildInputInstanceArray instanceArray;
        [FieldOffset(4)]
        public fixed byte pad[1024];
    }

    [CLSCompliant(false)]
    public unsafe struct OptixBuildInputTriangleArray
    {
        public IntPtr vertexBuffers;
        public uint numVerticies;
        public OptixVertexFormat vertexFormat;
        public uint vertexStrideInBytes;

        public IntPtr indexBuffer;
        public uint numIndexTriplets;
        public OptixIndicesFormat indexFormat;
        public uint indexStrideInBytes;

        public IntPtr preTransform;

        public uint* flags;

        public uint numSbtRecords;
        public IntPtr sbtIndexOffsetBuffer;
        public uint sbtIndexOffsetSizeInBytes;
        public uint sbtIndexOffsetStrideInBytes;
        public uint primitiveIndexOffset;

        public OptixTransformFormat transformFormat;
    }

    [CLSCompliant(false)]
    public unsafe struct OptixBuildInputCurveArray
    {
        public OptixPrimitiveType curveType;
        public uint numPrimitives;
 
        public IntPtr vertexBuffers;
        public uint numVerticies;
        public uint vertexStrideInBytes;
 
        public IntPtr widthBuffers;
        public uint widthStrideInBytes;
 
        //according to optix_7_types.h this is reserved for future use
        public IntPtr normalBuffers;
        public uint normalStrideInBytes;
 
        public uint flag;
        
        public uint primitiveIndexOffset;
    }

    [CLSCompliant(false)]
    public unsafe struct OptixBuildInputCustomPrimitiveArray
    {
        public IntPtr aabbBuffers;
        public uint numPrimitives;
        public uint stride;

        public uint* flags;
 
        public uint numSbtRecords;
        public IntPtr sbtIndexOffsetBuffer;
        public uint sbtIndexOffsetSizeInBytes;
        public uint sbtIndexOffsetStrideInBytes;
        public uint primitiveIndexOffset;
    }

    [CLSCompliant(false)]
    public struct OptixBuildInputInstanceArray
    {
        public IntPtr instances;
        public uint numInstances;
    }
}
