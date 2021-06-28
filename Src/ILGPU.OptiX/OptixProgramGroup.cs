// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixProgramGroup.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Util;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Wrapper over OptiX program group.
    /// </summary>
    public sealed class OptixProgramGroup : DisposeBase
    {
        #region Properties

        /// <summary>
        /// The native OptiX program group.
        /// </summary>
        public IntPtr ProgramGroupPtr { get; private set; }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new program group wrapper.
        /// </summary>
        /// <param name="programGroupPtr">The OptiX program group.</param>
        public OptixProgramGroup(IntPtr programGroupPtr)
        {
            if (programGroupPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(programGroupPtr));
            ProgramGroupPtr = programGroupPtr;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Transfers ownership into a new instance.
        /// </summary>
        /// <returns>The new instance.</returns>
        public OptixProgramGroup Transfer()
        {
            var result = new OptixProgramGroup(ProgramGroupPtr);
            ProgramGroupPtr = IntPtr.Zero;
            return result;
        }

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ProgramGroupPtr != IntPtr.Zero)
                {
                    OptixAPI.Current.ProgramGroupDestroy(ProgramGroupPtr);
                    ProgramGroupPtr = IntPtr.Zero;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
