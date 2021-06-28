// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroupSingleModule.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace ILGPU.OptiX.Interop
{
    public struct OptixProgramGroupSingleModule
    {
        /// <summary>
        /// Module holding single program.
        /// </summary>
        public IntPtr Module;

        /// <summary>
        /// Entry function name of the single program.
        /// </summary>
        public IntPtr EntryFunctionName;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1815 // Override equals and operator equals on value types
