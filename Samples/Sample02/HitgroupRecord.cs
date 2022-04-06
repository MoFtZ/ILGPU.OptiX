﻿// ---------------------------------------------------------------------------------------
//                                    ILGPU Samples
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: HitgroupRecord.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using ILGPU.OptiX;

namespace Sample02
{
    [StructLayout(LayoutKind.Sequential, Pack = OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT, Size = 48)]
    public unsafe struct HitgroupRecord
    {
        public fixed byte Header[OptixAPI.OPTIX_SBT_RECORD_HEADER_SIZE];
        public int ObjectID;
    }
}
