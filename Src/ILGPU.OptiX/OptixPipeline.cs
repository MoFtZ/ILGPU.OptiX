// ---------------------------------------------------------------------------------------
//                                     ILGPU OptiX
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: OptixPipeline.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU.Util;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Wrapper over OptiX pipeline.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class OptixPipeline : DisposeBase
    {
        #region Properties

        /// <summary>
        /// The native OptiX pipeline.
        /// </summary>
        public IntPtr PipelinePtr { get; private set; }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new pipeline wrapper.
        /// </summary>
        /// <param name="pipelinePtr">The OptiX pipeline.</param>
        public OptixPipeline(IntPtr pipelinePtr)
        {
            if (pipelinePtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(pipelinePtr));
            PipelinePtr = pipelinePtr;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures the pipeline stack size.
        /// </summary>
        /// <param name="directCallableStackSizeFromTraversal">
        /// The direct stack size requirement for direct callables invoked from IS or AH.
        /// </param>
        /// <param name="directCallableStackSizeFromState">
        /// The direct stack size requirement for direct callables invoked from RG, MS,
        /// or CH.</param>
        /// <param name="continuationStackSize">
        /// The continuation stack requirement.
        /// </param>
        /// <param name="maxTraversableGraphDepth">
        /// The maximum depth of a traversable graph passed to trace.</param>
        /// <returns>The OptiX result.</returns>
        public void SetStackSize(
            uint directCallableStackSizeFromTraversal,
            uint directCallableStackSizeFromState,
            uint continuationStackSize,
            uint maxTraversableGraphDepth)
        {
            OptixException.ThrowIfFailed(
                OptixAPI.Current.PipelineSetStackSize(
                    PipelinePtr,
                    directCallableStackSizeFromTraversal,
                    directCallableStackSizeFromState,
                    continuationStackSize,
                    maxTraversableGraphDepth));
        }

        #endregion

        #region IDisposable

        /// <summary cref="DisposeBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (PipelinePtr != IntPtr.Zero)
                {
                    OptixAPI.Current.PipelineDestroy(PipelinePtr);
                    PipelinePtr = IntPtr.Zero;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
