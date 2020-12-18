// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroupHitgroup.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

namespace ILGPU.OptiX.Interop
{
    public struct OptixProgramGroupHitgroup
    {
        /// <summary>
        /// Module holding the closest hit (CH) program.
        /// </summary>
        public IntPtr ModuleCH;

        /// <summary>
        /// Entry function name of the closest hit (CH) program.
        /// </summary>
        public IntPtr EntryFunctionNameCH;

        /// <summary>
        /// Module holding the any hit (AH) program.
        /// </summary>
        public IntPtr ModuleAH;

        /// <summary>
        /// Entry function name of the any hit (AH) program.
        /// </summary>
        public IntPtr EntryFunctionNameAH;

        /// <summary>
        /// Module holding the intersection (Is) program.
        /// </summary>
        public IntPtr ModuleIS;

        /// <summary>
        /// Entry function name of the intersection (IS) program.
        /// </summary>
        public IntPtr EntryFunctionNameIS;
    }
}
