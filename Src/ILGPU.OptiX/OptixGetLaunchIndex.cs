// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixGetLaunchIndex.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Backends.PTX;
using ILGPU.IR;
using ILGPU.IR.Intrinsics;
using ILGPU.IR.Values;
using System;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Provides the functionality of the OptixGetLaunchIndex built-in function.
    /// </summary>
    [CLSCompliant(false)]
    public static class OptixGetLaunchIndex
    {
        [IntrinsicImplementation]
        public static uint X() => default;

        [IntrinsicImplementation]
        public static uint Y() => default;

        [IntrinsicImplementation]
        public static uint Z() => default;

        public static class Generator
        {
            /// <summary>
            /// The Cuda (PTX) implementation.
            /// </summary>
            /// <remarks>
            /// Note that this function signature corresponds to the PTX-backend specific
            /// delegate type <see cref="PTXIntrinsic.Handler"/>.
            /// </remarks>
            public static void GeneratePTXCode(
               PTXBackend backend,
               PTXCodeGenerator codeGenerator,
               Value value)
            {
                var methodCall = value as MethodCall;
                var register = codeGenerator.AllocateHardware(value);
                var registerName = PTXRegisterAllocator.GetStringRepresentation(register);
                var sourceName = methodCall.Target.Source.Name.ToLower();
                codeGenerator.Builder.AppendLine(
                    $"call (%{registerName}), _optix_get_launch_index_{sourceName}, ();");
            }
        }
    }
}
