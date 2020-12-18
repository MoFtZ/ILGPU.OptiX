// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixCompileDebugLevel.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

namespace ILGPU.OptiX
{
    public enum OptixCompileDebugLevel
    {
        /// <summary>
        /// Default currently is to add line info.
        /// </summary>
        OPTIX_COMPILE_DEBUG_LEVEL_DEFAULT = 0,

        /// <summary>
        /// No debug information.
        /// </summary>
        OPTIX_COMPILE_DEBUG_LEVEL_NONE = 0x2350,

        /// <summary>
        /// Generate lineinfo only.
        /// </summary>
        OPTIX_COMPILE_DEBUG_LEVEL_LINEINFO = 0x2351,

        /// <summary>
        /// Generate dwarf debug information.
        /// </summary>
        OPTIX_COMPILE_DEBUG_LEVEL_FULL = 0x2352,
    }
}
