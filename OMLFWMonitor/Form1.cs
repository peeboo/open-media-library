using OMLFileWatcher;
using OMLLogging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Data.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OMLFWMonitor
{
    public partial class Form1 : Form
    {
        OMLFileWatcher.OMLFileWatcher fw;
        RegistryKey rkOML;
        List<String> watches = new List<String>();
        String[] watch;
        Boolean _logging = false;
        Boolean subdirs;

        public Form1()
        {
            InitializeComponent();
            if (!InitFileWatch()) { SetMsg("Nothing to watch"); };
        }

        public Boolean InitFileWatch()
        {
            fw = new OMLFileWatcher.OMLFileWatcher();
            rkOML = Registry.CurrentUser.OpenSubKey(@"Software");
            watches.AddRange(rkOML.GetSubKeyNames());
            rkOML.Close();
            if (!watches.Contains(@"OMLz")) { return false; }
            watches.Clear();
            rkOML = Registry.CurrentUser.OpenSubKey(@"Software\OML");
            watch = (String[]) rkOML.GetValue("Watch");
            watches.AddRange(watch);
            rkOML.Close();
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
            fw.Changed += new OMLFileWatcher.OMLFileWatcher.eChanged(fw_Changed);
            fw.Created += new OMLFileWatcher.OMLFileWatcher.eCreated(fw_Changed);
            fw.Deleted += new OMLFileWatcher.OMLFileWatcher.eDeleted(fw_Changed);
            fw.Renamed += new OMLFileWatcher.OMLFileWatcher.eRenamed(fw_Renamed);
            return true;
        }

        public Boolean logging
        {
            get
            {
                rkOML = Registry.CurrentUser.OpenSubKey(@"Software\OML");
                if (!Boolean.TryParse((string)rkOML.GetValue("logging", _logging), out _logging))
                {
                    _logging = false;
                }
                return _logging;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fw.EnableRaisingEvents = true;
        }
        private delegate void SetText(string sMsg);

        private void SetMsg(string sMsg)
        {
            this.textBox1.Text += sMsg + System.Environment.NewLine;
            if (logging) { LogInfo.WriteStatusLog(sMsg); }
        }

        private void fw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            SetText wryt = new SetText(SetMsg);
            string msg = String.Format("{0}: {1}", e.ChangeType.ToString(), e.FullPath);
            this.Invoke(wryt, msg);
        }
        private void fw_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            SetText wryt = new SetText(SetMsg);
            string msg = String.Format("{0}: {1} to {2}", e.ChangeType.ToString(), e.OldFullPath, e.FullPath);
            this.Invoke(wryt, msg);
        }

    }
}