// ---------------------------------------------------------------------------------------
//                                    ILGPU Samples
//                        Copyright (c) 2020-2022 ILGPU Project
//                                    www.ilgpu.net
//
// File: Program.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU;
using ILGPU.OptiX;
using ILGPU.Runtime.Cuda;

namespace Sample01
{
    public class Program
    {
        static void Main()
        {
            using var context = Context.Create(b => b.Cuda().InitOptiX());
            using var accelerator = context.CreateCudaAccelerator(0);
        }
    }
}
