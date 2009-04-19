using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;

using OMLEngine;

namespace OMLEngineService
{
    public partial class OMLEngineService : ServiceBase
    {
        private List<Title> titles;
        private ServiceHost _transcodingServiceHost, _titleCollectionServiceHost;

        public OMLEngineService()
        {
            this.ServiceName = @"OMLEngineService";

            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;

            titles = new List<Title>(TitleCollectionManager.GetAllTitles());
        }        

        #region overridden control methods (start, stop, pause, continue, etc)
        protected override void OnStart(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            WriteToLog(EventLogEntryType.Information, "OMLEngineService Start");

            _transcodingServiceHost = WCFUtilites.StartService(EventSource, typeof(TranscodingService));
            //_titleCollectionServiceHost = WCFUtilites.StartService(EventSource, typeof(TitleCollectionAPI));
        }

        protected override void OnStop()
        {
            WCFUtilites.StopService(EventSource, ref _transcodingServiceHost);
            WCFUtilites.StopService(EventSource, ref _titleCollectionServiceHost);
            WriteToLog(EventLogEntryType.Information, "OMLEngineService Stop");
        }
        #endregion

        const string EventSource = "OMLEngineService";
        internal static void WriteToLog(EventLogEntryType type, string msg, params object[] args)
        {
            WCFUtilites.WriteToLog(EventSource, type, msg, args);
        }
    }
}
