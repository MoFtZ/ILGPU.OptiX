// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixPipelineLinkOptions.cs
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
    public struct OptixPipelineLinkOptions
    {
        /// <summary>
        /// Maximum trace recursion depth. 0 means a ray generation program can be
        /// launched, but can't trace any rays. The maximum allowed value is 31.
        /// </summary>
        public uint MaxTraceDepth;

        /// <summary>
        /// Generate debug information.
        /// </summary>
        public OptixCompileDebugLevel DebugLevel;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1815 // Override equals and operator equals on value types
