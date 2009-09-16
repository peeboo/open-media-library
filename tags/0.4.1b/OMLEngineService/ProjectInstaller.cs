using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace OMLEngineService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.NetworkService;
            service = new ServiceInstaller();
            service.ServiceName = "OMLEngineService";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
