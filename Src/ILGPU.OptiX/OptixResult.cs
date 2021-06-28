// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixResult.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

#pragma warning disable CA1707 // Identifiers should not contain underscores

namespace ILGPU.OptiX
{
    public enum OptixResult
    {
        OPTIX_SUCCESS = 0,
        OPTIX_ERROR_INVALID_VALUE = 7001,
        OPTIX_ERROR_HOST_OUT_OF_MEMORY = 7002,
        OPTIX_ERROR_INVALID_OPERATION = 7003,
        OPTIX_ERROR_FILE_IO_ERROR = 7004,
        OPTIX_ERROR_INVALID_FILE_FORMAT = 7005,
        OPTIX_ERROR_DISK_CACHE_INVALID_PATH = 7010,
        OPTIX_ERROR_DISK_CACHE_PERMISSION_ERROR = 7011,
        OPTIX_ERROR_DISK_CACHE_DATABASE_ERROR = 7012,
        OPTIX_ERROR_DISK_CACHE_INVALID_DATA = 7013,
        OPTIX_ERROR_LAUNCH_FAILURE = 7050,
        OPTIX_ERROR_INVALID_DEVICE_CONTEXT = 7051,
        OPTIX_ERROR_CUDA_NOT_INITIALIZED = 7052,
        OPTIX_ERROR_VALIDATION_FAILURE = 7053,
        OPTIX_ERROR_INVALID_PTX = 7200,
        OPTIX_ERROR_INVALID_LAUNCH_PARAMETER = 7201,
        OPTIX_ERROR_INVALID_PAYLOAD_ACCESS = 7202,
        OPTIX_ERROR_INVALID_ATTRIBUTE_ACCESS = 7203,
        OPTIX_ERROR_INVALID_FUNCTION_USE = 7204,
        OPTIX_ERROR_INVALID_FUNCTION_ARGUMENTS = 7205,
        OPTIX_ERROR_PIPELINE_OUT_OF_CONSTANT_MEMORY = 7250,
        OPTIX_ERROR_PIPELINE_LINK_ERROR = 7251,
        OPTIX_ERROR_INTERNAL_COMPILER_ERROR = 7299,
        OPTIX_ERROR_DENOISER_MODEL_NOT_SET = 7300,
        OPTIX_ERROR_DENOISER_NOT_INITIALIZED = 7301,
        OPTIX_ERROR_ACCEL_NOT_COMPATIBLE = 7400,
        OPTIX_ERROR_NOT_SUPPORTED = 7800,
        OPTIX_ERROR_UNSUPPORTED_ABI_VERSION = 7801,
        OPTIX_ERROR_FUNCTION_TABLE_SIZE_MISMATCH = 7802,
        OPTIX_ERROR_INVALID_ENTRY_FUNCTION_OPTIONS = 7803,
        OPTIX_ERROR_LIBRARY_NOT_FOUND = 7804,
        OPTIX_ERROR_ENTRY_SYMBOL_NOT_FOUND = 7805,
        OPTIX_ERROR_LIBRARY_UNLOAD_FAILURE = 7806,
        OPTIX_ERROR_CUDA_ERROR = 7900,
        OPTIX_ERROR_INTERNAL_ERROR = 7990,
        OPTIX_ERROR_UNKNOWN = 7999,
    }
}

#pragma warning restore CA1707 // Identifiers should not contain underscores
