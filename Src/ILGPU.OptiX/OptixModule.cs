// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixModule.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Util;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Wrapper over OptiX module.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class OptixModule : DisposeBase
    {
        #region Properties

        /// <summary>
        /// The native OptiX module.
        /// </summary>
        public IntPtr ModulePtr { get; private set; }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new module wrapper.
        /// </summary>
        /// <param name="modulePtr">The OptiX module.</param>
        public OptixModule(IntPtr modulePtr)
        {
            if (modulePtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(modulePtr));
            ModulePtr = modulePtr;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Transfers ownership into a new instance.
        /// </summary>
        /// <returns>The new instance.</returns>
        public OptixModule Transfer()
        {
            var result = new OptixModule(ModulePtr);
            ModulePtr = IntPtr.Zero;
            return result;
        }

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ModulePtr != IntPtr.Zero)
                {
                    OptixAPI.Current.DeviceContextDestroy(ModulePtr);
                    ModulePtr = IntPtr.Zero;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
