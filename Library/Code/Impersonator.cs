using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ComponentModel;
using System.Security.Permissions;

namespace Library
{
    public class Impersonator
    {
        // Fields
        private WindowsImpersonationContext impContext;
        private IntPtr _token;

        private const int LOGON32_LOGON_INTERACTIVE = 2,
                          LOGON32_LOGON_NETWORK = 3,
                          LOGON32_PROVIDER_WINNT50 = 3,
                          LOGON32_PROVIDER_DEFAULT = 0,
                          LOGON32_LOGON_BATCH = 4,
                          LOGON32_LOGON_SERVICE = 5;

        // Methods
        public Impersonator()
        {
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Enter()
        {
            if (!this.IsImpersonating)
            {
                this._token = new IntPtr(0);
                try
                {
                    string domain = System.Environment.UserDomainName;
                    string username = OMLEngine.Properties.Settings.Default.ImpersonationUsername;
                    string password = OMLEngine.Properties.Settings.Default.ImpersonationPassword;

                    this._token = IntPtr.Zero;
                    if (!LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref this._token))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    this.impContext = new WindowsIdentity(this._token).Impersonate();
                }
                catch (Exception e)
                {
                    OMLApplication.DebugLine("[Impersonator] Failed to impersonate: {0}", e.Message);
                }
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Leave()
        {
            if (this.IsImpersonating)
            {
                this.impContext.Undo();
                if (this._token != IntPtr.Zero)
                {
                    CloseHandle(this._token);
                }
                this.impContext = null;
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // Properties
        protected bool IsImpersonating
        {
            get
            {
                return (this.impContext != null);
            }
        }
    }
}

