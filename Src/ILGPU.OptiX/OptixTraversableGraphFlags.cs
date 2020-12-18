// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixTraversableGraphFlags.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

namespace ILGPU.OptiX
{
    public enum OptixTraversableGraphFlags
    {
        /// <summary>
        ///  Used to signal that any traversable graphs is valid.
        ///  This flag is mutually exclusive with all other flags.
        /// </summary>
        OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_ANY = 0,

        /// <summary>
        ///  Used to signal that a traversable graph of a single Geometry Acceleration
        ///  Structure (GAS) without any transforms is valid. This flag may be combined
        ///  with other flags except for OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_ANY.
        /// </summary>
        OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_SINGLE_GAS = 1 << 0,

        /// <summary>
        ///  Used to signal that a traversable graph of a single Instance Acceleration
        ///  Structure (IAS) directly connected to Geometry Acceleration Structure (GAS)
        ///  traversables without transform traversables in between is valid.  This flag
        ///  may be combined with other flags except for
        ///  OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_ANY.
        /// </summary>
        OPTIX_TRAVERSABLE_GRAPH_FLAG_ALLOW_SINGLE_LEVEL_INSTANCING = 1 << 1,
    }
}
