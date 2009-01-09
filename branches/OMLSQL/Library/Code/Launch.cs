using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.MediaCenter.Hosting;
using Microsoft.Win32;

namespace Library
{
    public class MyAddIn : IAddInModule, IAddInEntryPoint
    {
        private static HistoryOrientedPageSession s_session;
        OMLApplication app;
        string _id;

        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)
        {
            if (entryPointInfo.ContainsKey("Context"))
                _id = (string)entryPointInfo["Context"];

            OMLEngine.Utilities.RawSetup();
            OMLApplication.DebugLine("[Launch] Initialize()");
        }

        ~MyAddIn()
        {
            // todo : solomon : this is blowing up because the sql object is already
            // disposed - in general cleanup in desctructor should be avoided
            //OMLApplication.Current.Uninitialize();
            OMLApplication.DebugLine("[Launch] Destroy");
        }

        public void Uninitialize()
        {
            OMLApplication.Current.Uninitialize();
            OMLApplication.DebugLine("[Launch] Uninitialize");
        }

        public void Launch(AddInHost host)
        {
            OMLApplication.DebugLine("[Launch] Launch called");
            DeleteOldRegistryKey(host);
            s_session = new HistoryOrientedPageSession();

            if (app == null)
            {
                OMLApplication.DebugLine("[Launch] No Application found, creating...");
                app = new OMLApplication(s_session, host);
            }

            app.Startup(_id);
        }

        private void DeleteOldRegistryKey(AddInHost host)
        {
            // remove old HKCU key that caused settings to appear on the TV+Movies menu strip
            if (!host.MediaCenterEnvironment.Capabilities.ContainsKey("Console"))
            {
                try
                {
                    RegistryKey delKeyValue = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\" +
                                                                              @"Media Center\Extensibility\Categories\Services\" +
                                                                              @"Movies\{543d0438-b10d-43d8-a20d-f0c96db4e6bd}", true);
                    if (delKeyValue != null)
                    {
                        OMLApplication.DebugLine("[Launch] Deleting old registry key value");
                        delKeyValue.DeleteValue(@"UseCount");
                    }
                }
                catch (Exception) { } // we don't really care, just delete it if you find it

                try
                {
                    RegistryKey delSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\" +
                                                                            @"Media Center\Extensibility\Categories\Services\Movies", true);
                    if (delSubKey != null)
                    {
                        OMLApplication.DebugLine("[Launch] Deleting old registry key");
                        delSubKey.DeleteSubKey(@"{543d0438-b10d-43d8-a20d-f0c96db4e6bd}");
                    }
                }
                catch (Exception) { } // we don't really care, just delete it if you find it
            }
        }
    }
}