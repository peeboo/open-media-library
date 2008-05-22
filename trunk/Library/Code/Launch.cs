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

            try
            {
                if (File.Exists(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt"))
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.Truncate);
                else
                    Log = new FileStream(OMLEngine.FileSystemWalker.LogDirectory + "\\debug.txt", FileMode.OpenOrCreate);

                Trace.Listeners.Add(new TextWriterTraceListener(Log));
                Trace.WriteLine("Launch:Initialize()");
            }
            catch (IOException)
            { }
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