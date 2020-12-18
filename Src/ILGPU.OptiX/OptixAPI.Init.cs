// ---------------------------------------------------------------------------------------
//                                      ILGPU.OptiX
//                        Copyright (c) 2020 ILGPU OptiX Project
//                                    www.ilgpu.net
//
// File: OptixAPI.Init.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.OptiX.Util;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace ILGPU.OptiX
{
    partial class OptixAPI
    {
        #region Static

        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
        public const int OPTIX_ABI_VERSION = 41;

        private delegate OptixResult OptixQueryFunctionTable(
            int abiId,
            uint numOptions,
            IntPtr optionKeys,
            IntPtr optionValues,
            IntPtr functionTable,
            nuint sizeOfTable);

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the Optix API.
        /// </summary>
        /// <returns>The OptiX result.</returns>
        public OptixResult Init()
        {
            hmodule = LoadOptixDLL();
            if (hmodule == IntPtr.Zero)
                return OptixResult.OPTIX_ERROR_LIBRARY_NOT_FOUND;

            var proc = NativeMethods.GetProcAddressA(hmodule, "optixQueryFunctionTable");
            if (proc == IntPtr.Zero)
                return OptixResult.OPTIX_ERROR_ENTRY_SYMBOL_NOT_FOUND;

            var functionTableSize = Marshal.SizeOf<OptixFunctionTable>();
            using var functionTablePtr = SafeHGlobal.AllocHGlobal(functionTableSize);

            var query =
                Marshal.GetDelegateForFunctionPointer<OptixQueryFunctionTable>(proc);
            var result = query(
                OPTIX_ABI_VERSION,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                functionTablePtr,
                (nuint)functionTableSize);
            functionTable = Marshal.PtrToStructure<OptixFunctionTable>(functionTablePtr);
            return result;
        }

        /// <summary>
        /// Uninitializes the OptiX API.
        /// </summary>
        /// <returns>The OptiX result.</returns>
        public OptixResult Uninit()
        {
            functionTable = default;

            if (hmodule != IntPtr.Zero)
            {
                NativeMethods.FreeLibrary(hmodule);
                hmodule = IntPtr.Zero;
            }

            return OptixResult.OPTIX_SUCCESS;
        }

        /// <summary>
        /// Loads the OptiX DLL.
        /// </summary>
        /// <returns>A handle to the loaded DLL module.</returns>
        private static IntPtr LoadOptixDLL()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadOptixWindowsDLL("nvoptix.dll");
            }
            else
            {
                return NativeMethods.LoadLibrary("libnvoptix.so.1");
            }
        }

        /// <summary>
        /// Loads the OptiX DLL on the Windows platform.
        /// </summary>
        /// <returns>A handle to the loaded DLL module.</returns>
        private static IntPtr LoadOptixWindowsDLL(string filename)
        {
            var systemPath = Path.Combine(Environment.SystemDirectory, filename);
            var handle = NativeMethods.LoadLibrary(systemPath);
            if (handle != IntPtr.Zero)
                return handle;

            return LoadOptixWindowsDLLFromConfigurationManager(filename);
        }

        /// <summary>
        /// Attempts to load the OptiX DLL from the Configuration Manager.
        /// </summary>
        /// <param name="filename">The name of the OptiX DLL to load.</param>
        /// <returns>A handle to the loaded DLL module.</returns>
        private static IntPtr LoadOptixWindowsDLLFromConfigurationManager(string filename)
        {
            // If we didn't find it, go looking in the register store.  Since nvoptix.dll
            // doesn't have its own registry entry, we are going to look for the opengl
            // driver which lives next to nvoptix.dll.  0 (null) will be returned if any
            // errors occured.
            foreach (var deviceName in GetDeviceNames())
            {
                try
                {
                    var driverPath = GetDeviceOpenGLDriverPath(deviceName);
                    var basePath = Path.GetDirectoryName(driverPath);
                    var dllPath = Path.Combine(basePath, filename);
                    var handle = NativeMethods.LoadLibrary(dllPath);
                    if (handle != IntPtr.Zero)
                        return handle;
                }
                catch (Exception)
                {
                    // Continue to the next device if errors are encountered.
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Enumerate the registry store for installed OpenGL devices.
        /// </summary>
        /// <returns>List of device names.</returns>
        private static IEnumerable<string> GetDeviceNames()
        {
            const string DeviceInstanceIdentifiersGUID =
                "{4d36e968-e325-11ce-bfc1-08002be10318}";
            const uint DeviceListFlags =
                NativeMethods.CM_GETIDLIST_FILTER_CLASS |
                NativeMethods.CM_GETIDLIST_FILTER_PRESENT;

            // Returns the required size to retrieve the device list.
            uint deviceListSize = 0;
            if (NativeMethods.CM_Get_Device_ID_List_Size(
                ref deviceListSize,
                DeviceInstanceIdentifiersGUID,
                DeviceListFlags)
                != NativeMethods.CR_SUCCESS)
            {
                return Array.Empty<string>();
            }

            // Returns a list of null-terminated strings.
            // The list itself is double null-terminated.
            var buffer = new char[deviceListSize];
            if (NativeMethods.CM_Get_Device_ID_List(
                DeviceInstanceIdentifiersGUID,
                buffer,
                deviceListSize,
                DeviceListFlags)
                != NativeMethods.CR_SUCCESS)
            {
                return Array.Empty<string>();
            }

            var bufferStr = new string(buffer);
            return bufferStr.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Retrieves the path to the OpenGL driver DLL for the specified device.
        /// </summary>
        /// <param name="deviceName">The device name.</param>
        /// <returns>The path to the driver DLL.</returns>
        private static string GetDeviceOpenGLDriverPath(string deviceName)
        {
            const string ValueName = "OpenGLDriverName";

            var devNode = IntPtr.Zero;
            if (NativeMethods.CM_Locate_DevNode(
                ref devNode,
                deviceName,
                NativeMethods.CM_LOCATE_DEVNODE_NORMAL)
                != NativeMethods.CR_SUCCESS)
            {
                return string.Empty;
            }

            if (NativeMethods.CM_Open_DevNode_Key(
                devNode,
                NativeMethods.KEY_QUERY_VALUE,
                0,
                NativeMethods.RegDisposition_OpenExisting,
                out var regKeyPtr,
                NativeMethods.CM_REGISTRY_SOFTWARE)
                != NativeMethods.CR_SUCCESS)
            {
                return string.Empty;
            }

            using var regKeyHandle = new SafeRegistryHandle(regKeyPtr, ownsHandle: true);
            var regKey = RegistryKey.FromHandle(regKeyHandle);
            var valueKind = regKey.GetValueKind(ValueName);
            if (valueKind == RegistryValueKind.MultiString)
            {
                string path = ((string[])regKey.GetValue(ValueName))[0];
                return path;
            }
            else if (valueKind == RegistryValueKind.String)
            {
                string path = (string)regKey.GetValue(ValueName);
                return path;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Native Methods

        private static partial class NativeMethods
        {
            #region Library Loader

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport(
                "kernel32.dll",
                EntryPoint = "GetProcAddress",
                CharSet = CharSet.Ansi)]
            [SuppressMessage(
                "Globalization",
                "CA2101:Specify marshaling for P/Invoke string arguments",
                Justification = "OptiX does not work with GetProcAddressW")]
            public static extern IntPtr GetProcAddressA(
                IntPtr hModule,
                string lpProcName);

            [DllImport("kernel32.dll")]
            public static extern bool FreeLibrary(IntPtr hModule);

            #endregion

            #region Configuration Manager

            /// <summary>
            /// Configuration Manager CONFIGRET return status codes.
            /// </summary>
            public const uint CR_SUCCESS = 0x00000000;

            /// <summary>
            /// Flags for CM_Get_Device_ID_List, CM_Get_Device_ID_List_Size.
            /// </summary>
            public const uint CM_GETIDLIST_FILTER_PRESENT = 0x00000100;
            public const uint CM_GETIDLIST_FILTER_CLASS = 0x00000200;

            /// <summary>
            /// Flags for CM_Locate_DevNode.
            /// </summary>
            public const uint CM_LOCATE_DEVNODE_NORMAL = 0x00000000;

            /// <summary>
            /// Registry disposition values.
            /// (specified in call to CM_Open_DevNode_Key and CM_Open_Class_Key).
            /// </summary>
            public const uint RegDisposition_OpenExisting = 0x00000001;

            /// <summary>
            /// Registry Branch Locations (for CM_Open_DevNode_Key).
            /// </summary>
            public const uint CM_REGISTRY_SOFTWARE = 0x00000001;

            /// <summary>
            /// Registry Specific Access Rights.
            /// </summary>
            public const uint KEY_QUERY_VALUE = 0x0001;

            [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
            public static extern int CM_Get_Device_ID_List_Size(
                ref uint idListlen,
                string lpFilter,
                uint ulFlags);

            [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
            public static extern int CM_Get_Device_ID_List(
                string lpFilter,
                char[] buffer,
                uint bufferLen,
                uint ulFlags);

            [DllImport("setupapi.dll", CharSet = CharSet.Unicode)]
            public static extern int CM_Locate_DevNode(
                ref IntPtr devNode,
                string deviceName,
                uint ulFlags);

            [DllImport("setupapi.dll")]
            public static extern int CM_Open_DevNode_Key(
                IntPtr devNode,
                uint samDesired,
                ulong ulHardwareProfile,
                ulong Disposition,
                out IntPtr phkDevice,
                uint ulFlags);

            #endregion
        }

        #endregion
    }
}
