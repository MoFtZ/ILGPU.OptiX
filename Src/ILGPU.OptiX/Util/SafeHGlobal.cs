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
        public static SafeHGlobal Alloc(int cb) =>
            new SafeHGlobal(Marshal.AllocHGlobal(cb));

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        public static SafeHGlobal Alloc<T>() =>
            Alloc<T>(1);

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        /// <param name="numElements">The number of elements of size T.</param>
        public static SafeHGlobal Alloc<T>(int numElements) =>
            Alloc(Marshal.SizeOf<T>() * numElements);

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        /// <param name="element">Single element to marshal.</param>
        public static SafeHGlobal AllocFrom<T>(T element)
            where T : struct
        {
            T[] elements = new T[1] { element };
            return AllocFrom<T>(elements.AsSpan());
        }

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        /// <param name="elements">Elements to marshal.</param>
        public static SafeHGlobal AllocFrom<T>(ReadOnlySpan<T> elements)
            where T : struct
        {
            var elementSize = Marshal.SizeOf<T>();
            var handle = Alloc(elementSize * elements.Length);

            IntPtr nextPtr = handle;
            foreach (var element in elements)
            {
                Marshal.StructureToPtr(element, nextPtr, false);
                nextPtr += elementSize;
            }
            return handle;
        }

        /// <summary>
        /// Convenience function to allocate a block of memory.
        /// </summary>
        public static SafeHGlobal FromString(string? str) =>
            new SafeHGlobal(Marshal.StringToHGlobalAnsi(str));

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
