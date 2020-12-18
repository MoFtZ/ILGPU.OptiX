// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixKernel.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Util;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Abstraction of an OptiX module and program group. We associate a single ILGPU
    /// kernel with one module and one program group. This is due to the way that ILGPU
    /// currently compiles kernels - PTX code is generated per function.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class OptixKernel : DisposeBase
    {
        #region Static

        public static readonly string RAYGEN_PREFIX = "__raygen__";
        public static readonly string MISS_PREFIX = "__miss__";
        public static readonly string CLOSESTHIT_PREFIX = "__closesthit__";
        public static readonly string ANYHIT_PREFIX = "__anyhit__";
        public static readonly string INTERSECTION_PREFIX = "__intersection__";

        #endregion

        #region Properties

        /// <summary>
        /// The OptiX module.
        /// </summary>
        public OptixModule[] Modules { get; private set; }

        /// <summary>
        /// The OptiX program group.
        /// </summary>
        public OptixProgramGroup ProgramGroup { get; private set; }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new OptiX kernel.
        /// </summary>
        /// <param name="module">The OptiX module.</param>
        /// <param name="programGroup">The OptiX program group.</param>
        public OptixKernel(OptixModule[] modules, OptixProgramGroup programGroup)
        {
            Modules = modules;
            ProgramGroup = programGroup;
        }

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ProgramGroup?.Dispose();
                ProgramGroup = null;

                if (Modules != null)
                {
                    foreach (var module in Modules)
                        module?.Dispose();
                    Modules = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
