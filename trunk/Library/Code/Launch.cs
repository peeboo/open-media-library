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
            Log = new FileStream("C:\\debug.txt", FileMode.OpenOrCreate|FileMode.Truncate);
            Trace.Listeners.Add(new TextWriterTraceListener(Log));
            Trace.WriteLine("Launch:Initialize()");
        }

        public void Uninitialize()
        {
            Trace.Flush();
            Log.Close();
        }

        public void Launch(AddInHost host)
        {
            Trace.WriteLine("Launch:Launch()");
            s_session = new HistoryOrientedPageSession();
            Application app = new Application(s_session, host);
            app.GoToMenu();
        }
    }
}