// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixPrimitiveTypeFlags.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

namespace ILGPU.OptiX
{
    [Flags]
    [SuppressMessage("Usage", "CA2217:Do not mark enums with FlagsAttribute")]
    public enum OptixPrimitiveTypeFlags
    {
        /// <summary>
        /// Custom primitive.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_FLAGS_CUSTOM = 1 << 0,

        /// <summary>
        /// B-spline curve of degree 2 with circular cross-section.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_FLAGS_ROUND_QUADRATIC_BSPLINE = 1 << 1,

        /// <summary>
        /// B-spline curve of degree 3 with circular cross-section.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_FLAGS_ROUND_CUBIC_BSPLINE = 1 << 2,

        /// <summary>
        /// Piecewise linear curve with circular cross-section.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_FLAGS_ROUND_LINEAR = 1 << 3,

        /// <summary>
        /// Triangle.
        /// </summary>
        OPTIX_PRIMITIVE_TYPE_FLAGS_TRIANGLE = 1 << 31,
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
