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

        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)
        {
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
            OMLApplication app = new OMLApplication(s_session, host);
            app.Startup();
        }
    }
}