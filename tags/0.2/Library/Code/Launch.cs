using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.MediaCenter.Hosting;

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

            try
            {
                if (File.Exists(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt"))
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.Append);
                else
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.OpenOrCreate);

                Trace.Listeners.Add(new TextWriterTraceListener(Log));
                Trace.AutoFlush = true;
                OMLApplication.DebugLine("[Launch] Initialize()");
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("[Launch] Error: " + e.Message);
            }
        }

        ~MyAddIn()
        {
            OMLApplication.DebugLine("[Launch] Destroy");
            Trace.Flush();
            Log.Close();
        }

        public void Uninitialize()
        {
            OMLApplication.DebugLine("[Launch] Uninitialize");
            Trace.Flush();
            Log.Close();
        }

        public void Launch(AddInHost host)
        {
            OMLApplication.DebugLine("[Launch] Launch called");
            s_session = new HistoryOrientedPageSession();
            OMLApplication app = new OMLApplication(s_session, host);
            app.Startup();
        }
    }
}