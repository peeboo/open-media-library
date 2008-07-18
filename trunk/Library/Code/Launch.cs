using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.Hosting;
using System.IO;
using System.Diagnostics;

namespace Library
{
    public class MyAddIn : IAddInModule, IAddInEntryPoint
    {
        private static HistoryOrientedPageSession s_session;
        public TextWriterTraceListener tl;
        public FileStream Log;

        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)
        {
            OMLEngine.Utilities.RawSetup();
            StartLog();
        }

        public void StartLog()
        {
            try
            {
                if (File.Exists(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt"))
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.Truncate);
                else
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.OpenOrCreate);

                tl = new TextWriterTraceListener(Log);
                Trace.Listeners.Add(tl);
                OMLApplication.DebugLine("Launch:Initialize()");
            }
            catch (Exception e)
            {
                AddInHost.Current.MediaCenterEnvironment.Dialog("Error opening logifle: " + e.Message,
                                                                "Log Error", Microsoft.MediaCenter.DialogButtons.Ok, 0, true);
            }
        }

        public void RestartLog()
        {
            try
            {
                if (!Log.CanWrite)
                {
                    Trace.Listeners.Remove(tl);
                    Log.Close();
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.Truncate);
                    tl = new TextWriterTraceListener(Log);
                    Trace.Listeners.Add(tl);
                    OMLApplication.DebugLine("Launch:Initialize()");
                }
            }
            catch (Exception e)
            {
                AddInHost.Current.MediaCenterEnvironment.Dialog("Error opening logifle: " + e.Message,
                                                                "Log Error", Microsoft.MediaCenter.DialogButtons.Ok, 0, true);
            }
        }

        ~MyAddIn()
        {
            Trace.Flush();
            Log.Close();
        }

        public void Uninitialize()
        {
            Trace.Flush();
            Log.Close();
        }

        public void Launch(AddInHost host)
        {
            s_session = new HistoryOrientedPageSession();
            OMLApplication app = new OMLApplication(s_session, host);
            app.Startup();
        }
    }
}