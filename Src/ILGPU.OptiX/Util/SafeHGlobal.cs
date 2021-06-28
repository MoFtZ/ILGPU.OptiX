// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: SafeHGlobal.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Util;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ILGPU.OptiX.Util
{
    /// <summary>
    /// Convenience wrapper around memory handle.
    /// </summary>
    public sealed class SafeHGlobal : DisposeBase
    {
        #region Static

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        public static SafeHGlobal AllocHGlobal(int cb)
        {
            return new SafeHGlobal(Marshal.AllocHGlobal(cb));
        }

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        public static SafeHGlobal StringToHGlobalAnsi(string? str)
        {
            return new SafeHGlobal(Marshal.StringToHGlobalAnsi(str));
        }

        #endregion

        #region Properties

        /// <summary>
        /// The underlying memory handle.
        /// </summary>
        public IntPtr NativePtr { get; private set; }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new wrapper over the memory handle.
        /// </summary>
        public SafeHGlobal(IntPtr nativePtr)
        {
            NativePtr = nativePtr;
        }

        [SuppressMessage(
            "Design",
            "CA1062:Validate arguments of public methods",
            Justification = "Should not throw within implicit operator")]
        public static implicit operator IntPtr(SafeHGlobal s) =>
            s.NativePtr;

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (NativePtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(NativePtr);
                    NativePtr = IntPtr.Zero;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}

#pragma warning restore CA2225 // Operator overloads have named alternates
