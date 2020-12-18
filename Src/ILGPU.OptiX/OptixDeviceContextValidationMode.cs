// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixDeviceContextValidationMode.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace ILGPU.OptiX
{
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    [CLSCompliant(false)]
    public enum OptixDeviceContextValidationMode : uint
    {
        OPTIX_DEVICE_CONTEXT_VALIDATION_MODE_OFF = 0,
        OPTIX_DEVICE_CONTEXT_VALIDATION_MODE_ALL = 0xFFFFFFFF
    }
}
