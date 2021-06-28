// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixException.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Represents an OptixException exception that can be thrown by the OptiX runtime.
    /// </summary>
    [Serializable]
    public sealed class OptixException : Exception
    {
        #region Instance

        /// <summary>
        /// Constructs a new OptiX exception.
        /// </summary>
        public OptixException()
            : this(OptixResult.OPTIX_ERROR_UNKNOWN)
        { }

        /// <summary>
        /// Constructs a new OptiX exception.
        /// </summary>
        /// <param name="errorCode">The OptiX runtime error.</param>
        public OptixException(OptixResult optixResult)
            : base()
        {
            OptixResult = optixResult;
        }

        /// <summary>
        /// Constructs a new OptiX exception.
        /// </summary>
        /// <param name="errorCode">The OptiX runtime error.</param>
        /// <param name="message">The exception message.</param>
        public OptixException(OptixResult optixResult, string message)
            : base(message)
        {
            OptixResult = optixResult;
        }

        /// <summary>
        /// Constructs a new OptiX exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public OptixException(string message)
            : base(message)
        {
            OptixResult = OptixResult.OPTIX_ERROR_UNKNOWN;
        }

        /// <summary>
        /// Constructs a new OptiX exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OptixException(string message, Exception innerException)
            : base(message, innerException)
        {
            OptixResult = OptixResult.OPTIX_ERROR_UNKNOWN;
        }

        /// <summary cref="Exception(SerializationInfo, StreamingContext)"/>
        private OptixException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OptixResult = (OptixResult)info.GetInt32(nameof(OptixResult));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the error.
        /// </summary>
        public OptixResult OptixResult { get; }

        #endregion

        #region Methods

        /// <summary cref="Exception.GetObjectData(SerializationInfo, StreamingContext)"/>
#if !NET5_0
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
#endif
        public override void GetObjectData(
            SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(OptixResult), (int)OptixResult);
        }

        /// <summary>
        /// Checks the given status and throws an exception in case of an error.
        /// </summary>
        /// <param name="optixResult">The OptiX result code to check.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfFailed(OptixResult optixResult)
        {
            if (optixResult != OptixResult.OPTIX_SUCCESS)
                throw new OptixException(optixResult);
        }

        /// <summary>
        /// Checks the given status and throws an exception in case of an error.
        /// </summary>
        /// <param name="optixResult">The OptiX result code to check.</param>
        /// <param name="message">The exception message.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfFailed(OptixResult optixResult, string message)
        {
            if (optixResult != OptixResult.OPTIX_SUCCESS)
                throw new OptixException(optixResult, message);
        }

        #endregion
    }
}
