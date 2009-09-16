using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace PostInstallerWizard
{
    public partial class Wiz_End_SQLInstall_Fail : UserControl
    {
        public Wiz_End_SQLInstall_Fail()
        {
            InitializeComponent();
        }

        private void buttonOpenLogFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Log\Summary.txt"))
            {
                Process pr = new Process();
                pr.StartInfo.UseShellExecute = true;
                pr.StartInfo.FileName = @"C:\Program Files\Microsoft SQL Server\100\Setup Bootstrap\Log\Summary.txt";
                pr.Start();
                pr.WaitForExit();
            }
        }
    }
}
