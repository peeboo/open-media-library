using System;
using System.ServiceProcess;
using OMLEngine;

namespace OMLEngineService
{
    public partial class OMLEngineService : ServiceBase
    {
        private TitleCollection tc;

        public OMLEngineService()
        {
            this.ServiceName = "OMLEngine Service";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            tc = new TitleCollection();
            tc.loadTitleCollection();
        }

        protected override void OnStop()
        {
        }
    }
}
