using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using System.Text;

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

        public static IEnumerable<string> GetAllAvailableNetworkDevices()
        {
            NETRESOURCE pRsrc = new NETRESOURCE();

            return EnumerateServers(pRsrc, ResourceScope.RESOURCE_GLOBALNET, ResourceType.RESOURCETYPE_DISK, ResourceUsage.RESOURCEUSAGE_ALL, ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER);
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

        public enum DriveTypes
        {
            Unknown = 0,
            NoRootDirectory = 1,
            RemovableDisk = 2,
            LocalDisk = 3,
            NetworkDrive = 4,
            CompactDisc = 5,
            RAMDisk = 6,
        }

        /// <summary>
        /// Will return true if the drive letter is a mapped network drive and output the UNC path
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <param name="UNCPath"></param>
        /// <returns></returns>
        public static bool IsMappedDrive(char driveLetter, out string UNCPath)
        {
            bool isUNCPath = false;

            // Make sure the drive exists. If it is mapped continue, otherwise just return
            // the unc path
            if (!Directory.Exists(driveLetter.ToString() + ":\\"))
            {
                UNCPath = driveLetter.ToString() + ":\\";
                return isUNCPath;
            }                        

            ManagementObject management = new ManagementObject();

            management.Path = new ManagementPath("Win32_LogicalDisk='" + driveLetter + ":'");                       

            if (Convert.ToInt32(management["DriveType"]) == 4)
            {
                isUNCPath = true;
                UNCPath = Convert.ToString(management["ProviderName"]);
            }
            else
                UNCPath = driveLetter.ToString() + ":\\";

            return isUNCPath;
        }

        /// <summary>
        /// Query the drive letter passed to find if the drive is a local drive or a network drive
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns>enum of DriveTypes depending on the path passed</returns>
        static public DriveTypes GetDriveType(char driveLetter)
        {
            ManagementObject management = new ManagementObject();

            management.Path = new ManagementPath("Win32_LogicalDisk='" + driveLetter + ":'");

            return (DriveTypes)Convert.ToInt32(management["DriveType"]);
        }

        /// <summary>
        /// Finds the drive type of the passed in path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>enum of DriveTypes depending on the path passed</returns>
        static public DriveTypes GetDriveType(string path)
        {
            if (!string.IsNullOrEmpty(path)
                && !path.StartsWith("\\\\")
                && path.Length > 2
                && path[1] == ':')
            {
                // It is a local drive. Find if it is a local drive or mapped network drive
                return GetDriveType(path[0]);
            }
            else
            {
                // It is a UNC path
                return DriveTypes.NetworkDrive;
            }
        }

        /// <summary>
        /// This will validate the path supplied and convert it to a unc path if 
        /// path is a network mapped drive or if there is a local share available
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public string FixPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";

            switch (GetDriveType(path))
            {
                case DriveTypes.NetworkDrive: 
                    // Check if this is a mapped drive
                    string uncPath;
                    // if it's an attached network drive convert it to a UNC path
                    if (IsMappedDrive(path[0], out uncPath))
                    {
                        return uncPath + "\\" + path.Remove(0, 2).TrimStart('\\');
                    }
                    else
                    {
                        return path;
                    }
                    break;

                case DriveTypes.LocalDisk:
                    // Attempt to find a local share containing this path - convert to unc path
                    string newpath = GetUniversalPath(path);
                    if (!string.IsNullOrEmpty(newpath)) return newpath;
                    break;

                default:
                    return path;
                    break;
            }
            return path;
        }

        #region This code borrowed from a public forum posting on www.pinvoke.net
        static public string GetUniversalPath(string path)
        {
            bool IncludeAdminShares = false;

            string universalPath = "";
            try
            {
                GetNetShares gns = new GetNetShares();

                // First attempt - ignore administrative shares (eg c$)
                foreach (GetNetShares.ShareInfo shareInfo in gns.EnumNetShares("127.0.0.1"))
                {
                    if (((shareInfo.ShareType & (uint)GetNetShares.SHARE_TYPE.Special) == 0))
                    {
                        if (string.Compare(path.Substring(0, shareInfo.Path.Length), shareInfo.Path, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return Path.Combine(String.Concat(@"\\", Environment.MachineName, @"\", shareInfo.ShareName),
                             path.Replace(shareInfo.Path, "").TrimStart('\\'));
                        }
                    }
                }

                // Second attempt - include administrative shares (eg c$) if enabled
                if (IncludeAdminShares)
                {
                    foreach (GetNetShares.ShareInfo shareInfo in gns.EnumNetShares("127.0.0.1"))
                    {
                        if (((shareInfo.ShareType) == 0)
                            || (IncludeAdminShares))
                        {
                            if (string.Compare(path.Substring(0, shareInfo.Path.Length), shareInfo.Path, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                return Path.Combine(String.Concat(@"\\", Environment.MachineName, @"\", shareInfo.ShareName),
                                 path.Replace(shareInfo.Path, "").TrimStart('\\'));
                            }
                        }
                    }
                }
            }
            catch
            {
                universalPath = path;
            }

            return universalPath;
        }

        class GetNetShares
        {
            #region External Calls
            [DllImport("Netapi32.dll", SetLastError = true)]
            static extern int NetApiBufferFree(IntPtr Buffer);
            [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
            private static extern int NetShareEnum(
                 StringBuilder ServerName,
                 int level,
                 ref IntPtr bufPtr,
                 uint prefmaxlen,
                 ref int entriesread,
                 ref int totalentries,
                 ref int resume_handle
                 );
            #endregion
            #region External Structures
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct ShareInfo
            {
                public string ShareName;
                public uint ShareType;
                public string Remark;
                public uint Permissions;
                public uint MaxUses;
                public uint CurrentUses;
                public string Path;
                public string Password;

                public ShareInfo(string netname, uint type, string remark, uint permissions, uint max_uses, uint current_uses, string path, string password)
                {
                    ShareName = netname;
                    ShareType = type;
                    Remark = remark;
                    Permissions = permissions;
                    MaxUses = max_uses;
                    CurrentUses = current_uses;
                    Path = path;
                    Password = password;
                }
            }
            #endregion

            const uint MAX_PREFERRED_LENGTH = 0xFFFFFFFF;
            const int SuccessCode = 0;
            private enum NetErrorResults : uint
            {
                Success = 0,
                BASE = 2100,
                UnknownDevDir = (BASE + 16),
                DuplicateShare = (BASE + 18),
                BufTooSmall = (BASE + 23),
            }
            public enum SHARE_TYPE : uint
            {
                DiskTree = 0,
                PrintQ = 1,
                Device = 2,
                IPC = 3,
                Special = 0x80000000,
            }
            public List<ShareInfo> EnumNetShares(string Server)
            {
                List<ShareInfo> ShareInfos = new List<ShareInfo>();
                int entriesread = 0;
                int totalentries = 0;
                int resume_handle = 0;
                int nStructSize = Marshal.SizeOf(typeof(ShareInfo));
                IntPtr bufPtr = IntPtr.Zero;
                StringBuilder server = new StringBuilder(Server);
                int ret = NetShareEnum(server, 2, ref bufPtr, MAX_PREFERRED_LENGTH, ref entriesread, ref totalentries, ref resume_handle);
                if (ret == SuccessCode)
                {
                    IntPtr currentPtr = bufPtr;
                    for (int i = 0; i < entriesread; i++)
                    {
                        ShareInfo shi1 = (ShareInfo)Marshal.PtrToStructure(currentPtr, typeof(ShareInfo));
                        ShareInfos.Add(shi1);
                        currentPtr = new IntPtr(currentPtr.ToInt32() + nStructSize);
                    }
                    NetApiBufferFree(bufPtr);
                    return ShareInfos;
                }
                else
                {
                    return new List<ShareInfo>();
                }
            }
        }
        #endregion
    }
}
