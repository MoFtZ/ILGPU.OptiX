﻿// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixModule.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
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
        #region Static

        /// <summary>
        /// Constructs an empty module wrapper.
        /// </summary>
        public static OptixModule CreateEmpty() =>
            new OptixModule(IntPtr.Zero, allowNullPtr: true);

        #endregion

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
            : this(modulePtr, allowNullPtr: false)
        { }

        /// <summary>
        /// Constructs a new module wrapper.
        /// </summary>
        /// <param name="modulePtr">The OptiX module.</param>
        /// <param name="allowNullPtr">Indicates if IntPtr.Zero is allowed.</param>
        private OptixModule(IntPtr modulePtr, bool allowNullPtr)
        {
            if (!allowNullPtr && modulePtr == IntPtr.Zero)
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
            var result = new OptixModule(ModulePtr, allowNullPtr: true);
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
