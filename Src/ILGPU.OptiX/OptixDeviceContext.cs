// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixDeviceContext.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU.Backends.PTX;
using ILGPU.Runtime.Cuda;
using ILGPU.Util;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Wrapper for an OptiX device context.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class OptixDeviceContext : DisposeBase
    {
        #region Properties

        /// <summary>
        /// The native OptiX device context.
        /// </summary>
        public IntPtr DeviceContextPtr { get; private set; }

        /// <summary>
        /// The Cuda accelerator.
        /// </summary>
        public CudaAccelerator Accelerator { get; }

        /// <summary>
        /// The PTX backend.
        /// </summary>
        public PTXBackend Backend =>
            Accelerator.Backend;

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new device context wrapper.
        /// </summary>
        /// <param name="accelerator">The Cuda accelerator.</param>
        /// <param name="deviceContextPtr">The OptiX device context.</param>
        public OptixDeviceContext(CudaAccelerator accelerator, IntPtr deviceContextPtr)
        {
            if (deviceContextPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(deviceContextPtr));
            Accelerator = accelerator
                ?? throw new ArgumentNullException(nameof(accelerator));
            DeviceContextPtr = deviceContextPtr;
        }

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DeviceContextPtr != IntPtr.Zero)
                {
                    OptixAPI.Current.DeviceContextDestroy(DeviceContextPtr);
                    DeviceContextPtr = IntPtr.Zero;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
