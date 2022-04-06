// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroupFlags.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using System;

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

namespace ILGPU.OptiX
{
    [Flags]
    public enum OptixProgramGroupFlags
    {
        /// <summary>
        /// Currently there are no flags.
        /// </summary>
        OPTIX_PROGRAM_GROUP_FLAGS_NONE = 0
    }
}


#pragma warning restore CA1008 // Enums should have zero value
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
