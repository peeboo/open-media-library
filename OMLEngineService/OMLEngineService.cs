using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;

using OMLEngine;

namespace OMLEngineService
{
    public partial class OMLEngineService : ServiceBase
    {
        //private IList<Title> titles;
        //private ServiceHost _transcodingServiceHost, _titleCollectionServiceHost;
        //private ServiceHost _transcodingServiceHost, _trailersProxyServiecHost;
        private ServiceHost _transcodingServiceHost;
        private TrailersProxyService _trailersProxyService;

        public OMLEngineService()
        {
            this.ServiceName = @"OMLEngineService";

            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;

            //titles = new List<Title>(TitleCollectionManager.GetAllTitles());
        }        

        #region overridden control methods (start, stop, pause, continue, etc)
        protected override void OnStart(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            WriteToLog(System.Diagnostics.EventLogEntryType.Information, "OMLEngineService Start");

            _transcodingServiceHost = WCFUtilites.StartService(EventSource, typeof(TranscodingService));
            //_titleCollectionServiceHost = WCFUtilites.StartService(EventSource, typeof(TitleCollectionAPI));
            //_trailersProxyServiecHost = WCFUtilites.StartService(EventSource, typeof(TrailersProxyService));
            _trailersProxyService = new TrailersProxyService();

        }

        protected override void OnStop()
        {
            WCFUtilites.StopService(EventSource, ref _transcodingServiceHost);
            //WCFUtilites.StopService(EventSource, ref _trailersProxyServiecHost);
            _trailersProxyService.Server.Stop();
            //WCFUtilites.StopService(EventSource, ref _titleCollectionServiceHost);
            WriteToLog(System.Diagnostics.EventLogEntryType.Information, "OMLEngineService Stop");
        }
        #endregion

        const string EventSource = "OMLEngineService";
        internal static void WriteToLog(System.Diagnostics.EventLogEntryType type, string msg, params object[] args)
        {
            WCFUtilites.WriteToLog(EventSource, type, msg, args);
        }
    }
}
