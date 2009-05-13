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
        private Impersonator imp;
        private static Library.Code.V3.HistoryOrientedPageSessionEx s_session;
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
            if (OMLApplication.Current.IsExtender && this.imp != null)
                this.imp.Leave();

            OMLApplication.Current.Uninitialize();
            OMLApplication.DebugLine("[Launch] Uninitialize");
        }

        private void GoToLoader()
        {
            s_session.GoToPageWithoutHistory("resx://Library/Library.Resources/V3_Loader", new Dictionary<string, object>());
        }

        private void BeginStart(object host)
        {
            OMLApplication.DebugLine("[Launch] Launch called");
            DeleteOldRegistryKey((AddInHost)host);

            if (app == null)
            {
                OMLApplication.DebugLine("[Launch] No Application found, creating...");
                app = new OMLApplication(s_session, (AddInHost)host);
            }

            try
            {
                if (OMLApplication.Current.IsExtender)
                {
                    this.imp = new Impersonator();
                    this.imp.Enter();
                }
            }
            catch (Exception ex)
            {
                OMLApplication.DebugLine("Exception during this.imp.Enter(): {0}", ex);
            }

            app.Startup(_id);
        }

        public void Launch(AddInHost host)
        {
            s_session = new Library.Code.V3.HistoryOrientedPageSessionEx();
            GoToLoader();
            Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.BeginStart), (object)host, new TimeSpan(1));
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