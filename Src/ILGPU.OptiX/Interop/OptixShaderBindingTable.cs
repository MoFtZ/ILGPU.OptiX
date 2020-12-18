// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixShaderBindingTable.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

namespace ILGPU.OptiX.Interop
{
    [CLSCompliant(false)]
    public struct OptixShaderBindingTable
    {
        /// <summary>
        /// Device address of the SBT record of the ray gen program to start launch at.
        /// The address must be a multiple of OPTIX_SBT_RECORD_ALIGNMENT.
        /// </summary>
        public IntPtr RaygenRecord;

        /// <summary>
        /// Device address of the SBT record of the exception program. The address must
        /// be a multiple of OPTIX_SBT_RECORD_ALIGNMENT.
        /// </summary>
        public IntPtr ExceptionRecord;

        /// <summary>
        /// Arrays of SBT records for miss programs. The base address and the stride must
        /// be a multiple of OPTIX_SBT_RECORD_ALIGNMENT.
        /// </summary>
        public IntPtr MissRecordBase;
        public uint MissRecordStrideInBytes;
        public uint MissRecordCount;

        /// <summary>
        /// Arrays of SBT records for hit groups. The base address and the stride must be
        /// a multiple of OPTIX_SBT_RECORD_ALIGNMENT.
        /// </summary>
        public IntPtr HitgroupRecordBase;
        public uint HitgroupRecordStrideInBytes;
        public uint HitgroupRecordCount;

        /// <summary>
        /// Arrays of SBT records for callable programs. If the base address is not null,
        /// the stride and count must not be zero. If the base address is null, then the
        /// count needs to zero. The base address and the stride must be a multiple of
        /// OPTIX_SBT_RECORD_ALIGNMENT.
        /// </summary>
        public IntPtr CallablesRecordBase;
        public uint CallablesRecordStrideInBytes;
        public uint CallablesRecordCount;
    }
}
