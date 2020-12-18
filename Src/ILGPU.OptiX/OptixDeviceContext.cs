// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixDeviceContext.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Backends;
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
        public CudaAccelerator Accelerator { get; private set; }

        /// <summary>
        /// Custom PTX backend.
        /// </summary>
        public PTXBackend Backend { get; private set; }

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

            // WORKAROUND: Use a custom PTX backend that supports ISA 6.4 because
            // OptiX does not like anything newer.
            if (Accelerator.InstructionSet > PTXInstructionSet.ISA_64)
            {
                Backend = new PTXBackend(
                    Accelerator.Context,
                    Accelerator.Capabilities,
                    Accelerator.Architecture,
                    PTXInstructionSet.ISA_64);
            }
            else
            {
                Backend = Accelerator.Backend;
            }
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

                // NB: Do not dispose of the Cuda accelerator. We are holding a
                // reference to it for convenience only.
                Accelerator = null;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
