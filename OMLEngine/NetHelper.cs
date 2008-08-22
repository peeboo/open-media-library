using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;

namespace OMLEngine
{
    public class NetworkHelper
    {
        // Used to Map UNC from a Windows Service

        #region Constants
        //NetResource Scope
        private const int RESOURCE_CONNECTED = 0x00000001;
        private const int RESOURCE_GLOBALNET = 0x00000002;
        private const int RESOURCE_REMEMBERED = 0x00000003;

        //NetResource Type
        private const int RESOURCETYPE_ANY = 0x00000000;
        private const int RESOURCETYPE_DISK = 0x00000001;
        private const int RESOURCETYPE_PRINT = 0x00000002;

        //NetResource Usage
        private const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        private const int RESOURCEUSAGE_CONTAINER = 0x00000002;


        //NetResource Display Type
        private const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        private const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        private const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        private const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        private const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        private const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;

        //Flags
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;
        private const int CONNECT_UPDATE_RECENT = 0x00000002;
        private const int CONNECT_TEMPORARY = 0x00000004;
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_NEED_DRIVE = 0x00000020;

        #endregion

        #region NetResource Structure
        [StructLayout(LayoutKind.Sequential)]
        private struct NetResource
        {
            public int Scope;
            public int Type;
            public int DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }
        #endregion

        #region Win32 Functions
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WNetAddConnection2A(ref NetResource netresource, string password, string username, int flags);

        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WNetCancelConnection2(string drivename, int flag, bool force);

        #endregion

        public bool WNetAddConnection(string LocalDrive, string NetworkFolderPath, string User, string Password, bool Force)
        {
            bool success = false;

            try
            {
                NetResource netresource = new NetResource();
                netresource.Scope = RESOURCE_GLOBALNET;
                netresource.Type = RESOURCETYPE_DISK;
                netresource.Usage = RESOURCEUSAGE_CONNECTABLE;
                netresource.DisplayType = RESOURCEDISPLAYTYPE_SHARE;
                netresource.LocalName = LocalDrive;
                netresource.RemoteName = NetworkFolderPath;
                netresource.Comment = "";
                netresource.Provider = "";

                int Flag = CONNECT_UPDATE_PROFILE;

                if (Force)
                {
                    success = WNetCancelConnection(LocalDrive, true);
                }

                int result = WNetAddConnection2A(ref netresource, Password, User, Flag);

                if (result > 0)
                {
                    throw new System.ComponentModel.Win32Exception(result);
                }
                success = true;

            }
            catch (Exception e)
            {
                //lib.Echo("Error: " + e.Message, myLib.MsgType.FAIL);
            }

            return success;
        }

        public bool WNetCancelConnection(string LocalDrive, bool Force)
        {
            bool success = false;
            try
            {
                int result = WNetCancelConnection2(LocalDrive, CONNECT_UPDATE_PROFILE, Force);
                if (result > 0)
                {
                    throw new System.ComponentModel.Win32Exception(result);
                }
                success = true;

            }
            catch (Exception e)
            {
                //lib.Echo("Error:" + e.Message, myLib.MsgType.FAIL);
            }
            return success;
        }
    }
}
