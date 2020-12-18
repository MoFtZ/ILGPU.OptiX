// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixModuleCompileBoundValueEntry.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

namespace ILGPU.OptiX
{
    [CLSCompliant(false)]
    public struct OptixModuleCompileBoundValueEntry
    {
        public nuint PipelineParamOffsetInBytes;
        public nuint SizeInBytes;

        public IntPtr BoundValuePtr;

        // optional string to display, set to 0 if unused.  If unused,
        // OptiX will report the annotation as "No annotation"
        public IntPtr Annotation;
    }
}
