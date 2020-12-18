using ILGPU.Util;
using System;
using System.Runtime.InteropServices;

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
        public static SafeHGlobal StringToHGlobalAnsi(string str)
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
        /// <param name="ptr"></param>
        public SafeHGlobal(IntPtr ptr)
        {
            NativePtr = ptr;
        }

        public static implicit operator IntPtr(SafeHGlobal s) => s.NativePtr;

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
