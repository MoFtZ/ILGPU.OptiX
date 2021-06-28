// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020-2021 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixSbt.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.OptiX.Resources;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ILGPU.OptiX
{
    /// <summary>
    /// Helper functions for constructing OptixShaderBindingTable.
    /// </summary>
    public static class OptixSbt
    {
        [CLSCompliant(false)]
        public static TRecord[] PackRecords<TRecord>(IReadOnlyList<OptixKernel> kernels)
            where TRecord : unmanaged
        {
            if (kernels == null)
                throw new ArgumentNullException(nameof(kernels));
            ThrowIfMisalignedRecord<TRecord>();

            var recordsArray = new TRecord[kernels.Count];
            unsafe
            {
                fixed (TRecord* record = recordsArray)
                {
                    for (var i = 0; i < recordsArray.Length; i++)
                    {
                        var kernel = kernels[i];
                        var currRecord = new IntPtr(&record[i]);

                        OptixAPI.Current.SbtRecordPackHeader(
                            kernel.ProgramGroup.ProgramGroupPtr,
                            currRecord);
                    }
                }
            }

            return recordsArray;
        }

        /// <summary>
        /// Checks that the SBT record is the correct size/alignment.
        /// </summary>
        /// <typeparam name="TRecord"></typeparam>
        private static void ThrowIfMisalignedRecord<TRecord>()
            where TRecord : unmanaged
        {
            if (Marshal.SizeOf<TRecord>() % OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT != 0)
            {
                throw new OptixException(
                    OptixResult.OPTIX_ERROR_VALIDATION_FAILURE,
                    string.Format(
                        ErrorMessages.MisalignedSbtRecord,
                        nameof(OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT),
                        OptixAPI.OPTIX_SBT_RECORD_ALIGNMENT));
            }
        }
    }
}
