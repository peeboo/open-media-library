﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OMLEngine.FileSystem
{
    /// <summary>
    /// The NetworkScanner class is used to enumerate servers on the network.
    /// This classes was copied from here - http://www.codeproject.com/KB/cs/csenumnetworkresources.aspx
    /// </summary>
    ///    
    public static class NetworkScanner
    {
        #region Enums
        private enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        };

        private enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        };

        private enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        };

        private enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        };

        enum ErrorCodes
        {
            NO_ERROR = 0,
            ERROR_NO_MORE_ITEMS = 259
        };
        #endregion

        [DllImport("Mpr.dll", EntryPoint = "WNetOpenEnumA", CallingConvention = CallingConvention.Winapi)]
        private static extern ErrorCodes WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NETRESOURCE p, out IntPtr lphEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetCloseEnum", CallingConvention = CallingConvention.Winapi)]
        private static extern ErrorCodes WNetCloseEnum(IntPtr hEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetEnumResourceA", CallingConvention = CallingConvention.Winapi)]
        private static extern ErrorCodes WNetEnumResource(IntPtr hEnum, ref uint lpcCount, IntPtr buffer, ref uint lpBufferSize);

        public static IEnumerable<string> GetAllAvailableNetworkShares()
        {
            NETRESOURCE pRsrc = new NETRESOURCE();

            return EnumerateServers(pRsrc, ResourceScope.RESOURCE_GLOBALNET, ResourceType.RESOURCETYPE_DISK, ResourceUsage.RESOURCEUSAGE_ALL, ResourceDisplayType.RESOURCEDISPLAYTYPE_SHARE);
        }

        private static IEnumerable<string> EnumerateServers(NETRESOURCE pRsrc, ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType)
        {            
            uint bufferSize = 16384;
            IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
            IntPtr handle = IntPtr.Zero;
            ErrorCodes result;
            uint cEntries = 1;

            result = WNetOpenEnum(scope, type, usage, pRsrc, out handle);

            if (result == ErrorCodes.NO_ERROR)
            {
                do
                {
                    result = WNetEnumResource(handle, ref cEntries, buffer, ref	bufferSize);

                    if (result == ErrorCodes.NO_ERROR)
                    {
                        Marshal.PtrToStructure(buffer, pRsrc);

                        if (pRsrc.dwDisplayType == displayType)
                            yield return pRsrc.lpRemoteName;

                        if ((pRsrc.dwUsage & ResourceUsage.RESOURCEUSAGE_CONTAINER) == ResourceUsage.RESOURCEUSAGE_CONTAINER)
                        {
                            foreach( string share in EnumerateServers(pRsrc, scope, type, usage, displayType))
                                yield return share;
                        }
                    }
                    else if (result != ErrorCodes.ERROR_NO_MORE_ITEMS)
                        break;
                } while (result != ErrorCodes.ERROR_NO_MORE_ITEMS);

                WNetCloseEnum(handle);
            }

            Marshal.FreeHGlobal((IntPtr)buffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        };
    }
}
