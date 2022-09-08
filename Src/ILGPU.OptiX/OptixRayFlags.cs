// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                           Copyright (c) 2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixRayFlags.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using System;

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

// disable: max_line_length

namespace ILGPU.OptiX
{
    [Flags]
    public enum OptixRayFlags
    {
        /// <summary>
        /// No change from the behavior configured for the individual AS.
        /// </summary>
        OPTIX_RAY_FLAG_NONE = 0,

        /// <summary>
        /// Disables anyhit programs for the ray.
        /// Overrides OPTIX_INSTANCE_FLAG_ENFORCE_ANYHIT.
        /// This flag is mutually exclusive with OPTIX_RAY_FLAG_ENFORCE_ANYHIT,
        /// OPTIX_RAY_FLAG_CULL_DISABLED_ANYHIT, OPTIX_RAY_FLAG_CULL_ENFORCED_ANYHIT.
        /// </summary>
        OPTIX_RAY_FLAG_DISABLE_ANYHIT = 1 << 0,

        /// <summary>
        /// Forces anyhit program execution for the ray.
        /// Overrides OPTIX_GEOMETRY_FLAG_DISABLE_ANYHIT as well as OPTIX_INSTANCE_FLAG_DISABLE_ANYHIT.
        /// This flag is mutually exclusive with OPTIX_RAY_FLAG_DISABLE_ANYHIT,
        /// OPTIX_RAY_FLAG_CULL_DISABLED_ANYHIT, OPTIX_RAY_FLAG_CULL_ENFORCED_ANYHIT.
        /// </summary>
        OPTIX_RAY_FLAG_ENFORCE_ANYHIT = 1 << 1,

        /// <summary>
        /// Terminates the ray after the first hit and executes
        /// the closesthit program of that hit.
        /// </summary>
        OPTIX_RAY_FLAG_TERMINATE_ON_FIRST_HIT = 1 << 2,

        /// <summary>
        /// Disables closesthit programs for the ray, but still executes miss program in case of a miss.
        /// </summary>
        OPTIX_RAY_FLAG_DISABLE_CLOSESTHIT = 1 << 3,

        /// <summary>
        /// Do not intersect triangle back faces
        /// (respects a possible face change due to instance flag
        /// OPTIX_INSTANCE_FLAG_FLIP_TRIANGLE_FACING).
        /// This flag is mutually exclusive with OPTIX_RAY_FLAG_CULL_FRONT_FACING_TRIANGLES.
        /// </summary>
        OPTIX_RAY_FLAG_CULL_BACK_FACING_TRIANGLES = 1 << 4,

        /// <summary>
        /// Do not intersect triangle front faces
        /// (respects a possible face change due to instance flag
        /// OPTIX_INSTANCE_FLAG_FLIP_TRIANGLE_FACING).
        /// This flag is mutually exclusive with OPTIX_RAY_FLAG_CULL_BACK_FACING_TRIANGLES.
        /// </summary>
        OPTIX_RAY_FLAG_CULL_FRONT_FACING_TRIANGLES = 1 << 5,

        /// <summary>
        /// Do not intersect geometry which disables anyhit programs
        /// (due to setting geometry flag OPTIX_GEOMETRY_FLAG_DISABLE_ANYHIT or
        /// instance flag OPTIX_INSTANCE_FLAG_DISABLE_ANYHIT).
        /// This flag is mutually exclusive with OPTIX_RAY_FLAG_CULL_ENFORCED_ANYHIT,
        /// OPTIX_RAY_FLAG_ENFORCE_ANYHIT, OPTIX_RAY_FLAG_DISABLE_ANYHIT.
        /// </summary>
        OPTIX_RAY_FLAG_CULL_DISABLED_ANYHIT = 1 << 6,

        /// <summary>
        /// Do not intersect geometry which have an enabled anyhit program
        /// (due to not setting geometry flag OPTIX_GEOMETRY_FLAG_DISABLE_ANYHIT or
        /// setting instance flag OPTIX_INSTANCE_FLAG_ENFORCE_ANYHIT).
        /// This flag is mutually exclusive with OPTIX_RAY_FLAG_CULL_DISABLED_ANYHIT,
        /// OPTIX_RAY_FLAG_ENFORCE_ANYHIT, OPTIX_RAY_FLAG_DISABLE_ANYHIT.
        /// </summary>
        OPTIX_RAY_FLAG_CULL_ENFORCED_ANYHIT = 1 << 7
    }
}

#pragma warning restore CA1008 // Enums should have zero value
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
