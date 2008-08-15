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
        OMLApplication app;
        private string _id;

        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)
        {
            _id = entryPointInfo["id"].ToString();
            OMLEngine.Utilities.RawSetup();
            OMLApplication.DebugLine("[Launch] Initialize()");
        }

        ~MyAddIn()
        {
            OMLApplication.DebugLine("[Launch] Destroy");
        }

        public void Uninitialize()
        {
            OMLApplication.DebugLine("[Launch] Uninitialize");
        }

        public void Launch(AddInHost host)
        {
            OMLApplication.DebugLine("[Launch] Launch called");
            s_session = new HistoryOrientedPageSession();

            if (app == null)
            {
                OMLApplication.DebugLine("[Launch] No Application found, creating...");
                app = new OMLApplication(s_session, host);
            }

            app.Startup(_id);
        }
    }
}