// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixPipelineCompileOptions.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ILGPU.OptiX
{
    public struct OptixPipelineCompileOptions
    {
        /// <summary>
        /// Boolean value indicating whether motion blur could be used
        /// </summary>
        public int UsesMotionBlur;

        /// <summary>
        /// Traversable graph bitfield. See OptixTraversableGraphFlags
        /// </summary>
        public OptixTraversableGraphFlags TraversableGraphFlags;

        /// <summary>
        /// How much storage, in 32b words, to make available for the payload, [0..8]
        /// </summary>
        public int NumPayloadValues;

        /// <summary>
        /// How much storage, in 32b words, to make available for the attributes. The
        /// minimum number is 2. Values below that will automatically be changed to 2.
        /// [2..8]
        /// </summary>
        public int NumAttributeValues;

        /// <summary>
        /// A bitmask of OptixExceptionFlags indicating which exceptions are enabled.
        /// </summary>
        public OptixExceptionFlags ExceptionFlags;

        /// <summary>
        /// The name of the pipeline parameter variable.  If 0, no pipeline parameter
        /// will be available. This will be ignored if the launch param variable was
        /// optimized out or was not found in the modules linked to the pipeline.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string PipelineLaunchParamsVariableName;

        /// <summary>
        /// Bit field enabling primitive types. See OptixPrimitiveTypeFlags.
        /// Setting to zero corresponds to enabling OPTIX_PRIMITIVE_TYPE_FLAGS_CUSTOM and
        /// OPTIX_PRIMITIVE_TYPE_FLAGS_TRIANGLE.
        /// </summary>
        public OptixPrimitiveTypeFlags UsesPrimitiveTypeFlags;
    }
}
