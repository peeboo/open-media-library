using OMLFileWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;

namespace OMLFWService
{
    public partial class OMLFWService : ServiceBase
    {
        OMLFileWatcher.OMLFileWatcher fw;
        RegistryKey rkOML;
        String[] watches;
        String[] watch;
        Boolean logging = false;
        Boolean subdirs;

        public OMLFWService()
        {
            InitializeComponent();

            fw = new OMLFileWatcher.OMLFileWatcher();

            RegistryKey rkCU = Registry.CurrentUser;
            RegistryKey rkSoftware = rkCU.OpenSubKey(@"Software");
            rkOML = rkSoftware.OpenSubKey(@"OML");
            rkSoftware.Close();
            rkCU.Close();
            if (!Boolean.TryParse((string)rkOML.GetValue("logging", logging), out logging))
            {
                logging = false;
            }
            watches = (String[])rkOML.GetValue("Watch");
            foreach (string set in watches)
            {
                watch = set.Split(';');
                switch (watch.Length)
                {
                    case 1:
                    case 2:
                        fw.AddWatch(watch[0], watch[1]);
                        break;
                    case 3:
                        if (!Boolean.TryParse(watch[2], out subdirs))
                        {
                            subdirs = false;
                        }
                        fw.AddWatch(watch[0], watch[1], subdirs);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            fw.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            fw.EnableRaisingEvents = false;
        }

    }

}
