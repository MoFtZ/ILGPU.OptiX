// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroupKind.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

namespace ILGPU.OptiX
{
    /// <summary>
    /// Distinguishes different kinds of program groups.
    /// </summary>
    public enum OptixProgramGroupKind
    {
        /// <summary>
        /// Program group containing a raygen (RG) program.
        /// </summary>
        OPTIX_PROGRAM_GROUP_KIND_RAYGEN = 0x2421,

        /// <summary>
        /// Program group containing a miss (MS) program.
        /// </summary>
        OPTIX_PROGRAM_GROUP_KIND_MISS = 0x2422,

        /// <summary>
        /// Program group containing an exception (EX) program.
        /// </summary>
        OPTIX_PROGRAM_GROUP_KIND_EXCEPTION = 0x2423,

        /// <summary>
        /// Program group containing an intersection (IS), any hit (AH), and/or closest
        /// hit (CH) program.
        /// </summary>
        OPTIX_PROGRAM_GROUP_KIND_HITGROUP = 0x2424,

        /// <summary>
        /// Program group containing a direct (DC) or continuation (CC) callable program
        /// \see OptixProgramGroupCallables, #OptixProgramGroupDesc::callables
        /// </summary>
        OPTIX_PROGRAM_GROUP_KIND_CALLABLES = 0x2425
    }
}
