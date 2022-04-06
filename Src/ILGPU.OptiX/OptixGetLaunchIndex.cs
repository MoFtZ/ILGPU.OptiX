// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixGetLaunchIndex.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU.Runtime.Cuda;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Provides the functionality of the OptixGetLaunchIndex built-in function.
    /// </summary>
    [CLSCompliant(false)]
    public static class OptixGetLaunchIndex
    {
        public static uint X
        {
            get
            {
                CudaAsm.Emit("call (%0), _optix_get_launch_index_x, ();", out uint x);
                return x;
            }
        }

        public static uint Y
        {
            get
            {
                CudaAsm.Emit("call (%0), _optix_get_launch_index_y, ();", out uint y);
                return y;
            }
        }

        public static uint Z
        {
            get
            {
                CudaAsm.Emit("call (%0), _optix_get_launch_index_z, ();", out uint z);
                return z;
            }
        }
    }
}
