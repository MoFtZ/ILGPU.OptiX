// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixCompileOptimizationLevel.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace ILGPU.OptiX
{
    public enum OptixCompileOptimizationLevel
    {
        /// <summary>
        /// Default is to run all optimizations.
        /// </summary>
        OPTIX_COMPILE_OPTIMIZATION_DEFAULT = 0,

        /// <summary>
        /// No optimizations.
        /// </summary>
        OPTIX_COMPILE_OPTIMIZATION_LEVEL_0 = 0x2340,

        /// <summary>
        /// Some optimizations.
        /// </summary>
        OPTIX_COMPILE_OPTIMIZATION_LEVEL_1 = 0x2341,

        /// <summary>
        /// Most optimizations.
        /// </summary>
        OPTIX_COMPILE_OPTIMIZATION_LEVEL_2 = 0x2342,

        /// <summary>
        /// All optimizations.
        /// </summary>
        OPTIX_COMPILE_OPTIMIZATION_LEVEL_3 = 0x2343,
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
