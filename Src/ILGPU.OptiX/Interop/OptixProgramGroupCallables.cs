// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroupCallables.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1815 // Override equals and operator equals on value types

namespace ILGPU.OptiX.Interop
{
    public struct OptixProgramGroupCallables
    {
        /// <summary>
        /// Module holding the direct callable (DC) program.
        /// </summary>
        public IntPtr ModuleDC;

        /// <summary>
        /// Entry function name of the direct callable (DC) program.
        /// </summary>
        public IntPtr EntryFunctionNameDC;

        /// <summary>
        /// Module holding the continuation callable (CC) program.
        /// </summary>
        public IntPtr ModuleCC;

        /// <summary>
        /// Entry function name of the continuation callable (CC) program.
        /// </summary>
        public IntPtr entryFunctionNameCC;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
#pragma warning restore CA1815 // Override equals and operator equals on value types
