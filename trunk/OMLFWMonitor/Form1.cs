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
        bool _logging = false;
        bool subdirs;

        public Form1()
        {
            InitializeComponent();
            if (!InitFileWatch())
                SetMsg("Nothing to watch");
        }

        public bool InitFileWatch()
        {
            List<string> watches = new List<string>();

            fw = new OMLFileWatcher.OMLFileWatcher();
            using (RegistryKey rkOML = Registry.CurrentUser.OpenSubKey(@"Software"))
                watches.AddRange(rkOML.GetSubKeyNames());
            if (!watches.Contains(@"OMLz"))
                return false;

            watches.Clear();
            using (RegistryKey rkOML = Registry.CurrentUser.OpenSubKey(@"Software\OML"))
                watches.AddRange((string[])rkOML.GetValue("Watch"));

            foreach (string set in watches)
            {
                string[] watch = set.Split(';');
                switch (watch.Length)
                {
                    case 1:
                    case 2:
                        fw.AddWatch(watch[0], watch[1]);
                        break;
                    case 3:
                        if (!bool.TryParse(watch[2], out subdirs))
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

        public bool logging
        {
            get
            {
                using (RegistryKey rkOML = Registry.CurrentUser.OpenSubKey(@"Software\OML"))
                {
                    if (rkOML != null && bool.TryParse((string)rkOML.GetValue("logging", _logging), out _logging) == false)
                    {
                        _logging = false;
                    }
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
            if (logging) 
                LogInfo.WriteStatusLog(sMsg);
        }

        private void fw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            SetText wryt = new SetText(SetMsg);
            string msg = string.Format("{0}: {1}", e.ChangeType.ToString(), e.FullPath);
            this.Invoke(wryt, msg);
        }

        private void fw_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            SetText wryt = new SetText(SetMsg);
            string msg = string.Format("{0}: {1} to {2}", e.ChangeType.ToString(), e.OldFullPath, e.FullPath);
            this.Invoke(wryt, msg);
        }

    }
}