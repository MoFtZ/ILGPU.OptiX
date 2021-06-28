// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixModuleCompileOptions.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace ILGPU.OptiX
{
    [CLSCompliant(false)]
    public struct OptixModuleCompileOptions
    {
        /// <summary>
        /// Maximum number of registers allowed when compiling to SASS.
        /// Set to 0 for no explicit limit. May vary within a pipeline.
        /// </summary>
        public int MaxRegisterCount;

        /// <summary>
        /// Optimization level. May vary within a pipeline.
        /// </summary>
        public OptixCompileOptimizationLevel OptimizationLevel;

        /// <summary>
        /// Generate debug information.
        /// </summary>
        public OptixCompileDebugLevel DebugLevel;

        /// <summary>
        /// Ingored if numBoundValues is set to 0
        /// Pointer to array of <see cref="OptixModuleCompileBoundValueEntry"/>.
        /// </summary>
        public IntPtr BoundValues;

        /// <summary>
        /// set to 0 if unused
        /// </summary>
        public uint NumBoundValues;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1815 // Override equals and operator equals on value types
