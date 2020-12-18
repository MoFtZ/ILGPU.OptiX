// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroupDesc.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ILGPU.OptiX.Interop
{
    [StructLayout(LayoutKind.Explicit)]
    public struct OptixProgramGroupDesc
    {
        /// <summary>
        /// The kind of program group.
        /// </summary>
        [FieldOffset(0)]
        public OptixProgramGroupKind Kind;

        [FieldOffset(4)]
        public OptixProgramGroupFlags Flags;

        [FieldOffset(8)]
        public OptixProgramGroupSingleModule Raygen;
        [FieldOffset(8)]
        public OptixProgramGroupSingleModule Miss;
        [FieldOffset(8)]
        public OptixProgramGroupSingleModule Exception;
        [FieldOffset(8)]
        public OptixProgramGroupCallables Callables;
        [FieldOffset(8)]
        public OptixProgramGroupHitgroup Hitgroup;
    }
}
