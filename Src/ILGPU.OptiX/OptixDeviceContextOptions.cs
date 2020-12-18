// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixDeviceContextOptions.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;

namespace ILGPU.OptiX
{
    [CLSCompliant(false)]
    public struct OptixDeviceContextOptions
    {
        /// <summary>
        /// Function pointer used when OptiX wishes to generate messages.
        /// </summary>
        public IntPtr LogCallbackFunction;

        /// <summary>
        /// Pointer stored and passed to logCallbackFunction when a message is generated.
        /// </summary>
        public IntPtr LogCallbackData;

        /// <summary>
        /// Maximum callback level to generate message for (see #OptixLogCallback).
        /// </summary>
        public int LogCallbackLevel;

        /// <summary>
        /// Validation mode of context.
        /// </summary>
        public OptixDeviceContextValidationMode ValidationMode;
    }
}
