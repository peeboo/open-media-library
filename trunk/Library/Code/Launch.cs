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
            if (OMLApplication.Current != null)
            {
                if (OMLApplication.Current.IsExtender && this.imp != null)
                    this.imp.Leave();

                OMLApplication.Current.Uninitialize();
                OMLApplication.DebugLine("[Launch] Uninitialize");
            }
        }

        private void GoToLoader()
        {
            s_session.GoToPageWithoutHistory("resx://Library/Library.Resources/V3_Loader", new Dictionary<string, object>());
        }

        private void BeginStart(object host)
        {
            OMLApplication.DebugLine("[Launch] Launch called");
            if (TestDBConnection((AddInHost)host))
            {
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
            else
            {
                //just going to do a retry for now-need to allow db change later...
                List<string> buttons = new List<string>();
                buttons.Add("Retry");
                buttons.Add("Cancel");
                Microsoft.MediaCenter.DialogResult res = ((AddInHost)host).MediaCenterEnvironment.Dialog("Unable to communicate with database. Would you like to retry or cancel?", "OPEN MEDIA LIBRARY", buttons, -1, true, null, delegate(Microsoft.MediaCenter.DialogResult dialogResult) { });
                if ((int)res == 100)
                {
                    Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.BeginStart), (object)host, new TimeSpan(1));
                }
                else
                {
                    s_session.Close();
                }
            }
        }

        public void Launch(AddInHost host)
        {
            //_id = "FirstRun";
            if (_id == "FirstRun")
            {
                CheckFirstRun(host);
            }
            else
            {
                s_session = new Library.Code.V3.HistoryOrientedPageSessionEx();
                GoToLoader();
                Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.BeginStart), (object)host, new TimeSpan(1));
            }
        }

        private bool TestDBConnection(AddInHost host)
        {
            try
            {
                //return OMLEngine.Settings.OMLSettings.IsConnected;
                OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
                OMLEngine.DatabaseManagement.DatabaseInformation.SQLState state = dbm.CheckDatabase();
                switch (state)
                {
                    case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OK:
                        return true;
                    case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionCodeOlderThanSchema:
                        throw (new Exception("The OML client is an older version than the database. Please upgrade the client components!"));
                    //is this right?
                    case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionNotFound:
                        return true;
                    case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.LoginFailure:
                        return false;
                    default:
                        throw (new Exception(OMLEngine.DatabaseManagement.DatabaseInformation.LastSQLError));
                }
            }
            catch (Exception ex)
            {
                host.MediaCenterEnvironment.Dialog(
                ex.Message,
                "CONNECTION FAILED", Microsoft.MediaCenter.DialogButtons.Ok, 5, false);
                return false;
                
                //this._session.Close();
            }
            return true;
        }
        private void CheckFirstRun(AddInHost host)
        {
            if (Properties.Settings.Default.ShowFirstRunPrompt == true)
            {
                List<string> buttons = new List<string>();
                buttons.Add("Configure OML");
                buttons.Add("No Thanks");
                Microsoft.MediaCenter.MediaCenterEnvironment env = host.MediaCenterEnvironment;
                string dialogText = "Would you like to configure Open Media Library now? To run this wizard later you can access OML from the program library.";
                Microsoft.MediaCenter.DialogResult res = env.Dialog(dialogText, "OPEN MEDIA LIBRARY", buttons, -1, true, null, delegate(Microsoft.MediaCenter.DialogResult dialogResult) { });
                if ((int)res == 100)
                {
                    //tmp until firstrun page
                    Properties.Settings.Default.ShowFirstRunPrompt = false;
                    Properties.Settings.Default.Save();

                    //run setup
                    s_session = new Library.Code.V3.HistoryOrientedPageSessionEx();
                    GoToLoader();
                    Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.BeginStart), (object)host, new TimeSpan(1));
                }
                else
                {
                    Properties.Settings.Default.ShowFirstRunPrompt = false;
                    Properties.Settings.Default.Save();
                    //env.Dialog("You can access OML from the program library.", "OPEN MEDIA LIBRARY", Microsoft.MediaCenter.DialogButtons.Ok, -1, true);
                }
                
            }
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