using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Diagnostics;
using OMLEngine;

namespace PostInstallerWizard
{
    public partial class Form1 : Form
    {
        List<Control> WizPages = new List<Control>();

        Control WizStart;
        Control WizSelectServer;
        Control WizEnd;
        Control WizSelectExistingServer;

        Control CurrentPage;

        bool ServerInstall;
        bool ServerAllreadyInstalled;
        bool InstallNewInstance;
        bool AdvancedMode;

        string servername;
        string sapassword;
        string instancename;

        public Form1()
        {
            ServerInstall = false;
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                if (string.Compare(Environment.GetCommandLineArgs()[1], "server",true) == 0)
                {
                    ServerInstall = true;
                }
            }

            ServerAllreadyInstalled = CheckSQLExists();

            // Set some bools
            InstallNewInstance = true;
            AdvancedMode = false;

            CurrentPage = null;

            // Default database info
            sapassword = "omladmin";
            instancename = "oml";
            servername = "localhost";


            InitializeComponent();

            // Create wizard pages
            WizStart = new Wiz_Start();
            WizSelectServer = new Wiz_SelectServer();
            WizSelectExistingServer = new Wiz_SelectExistingServer();
            WizEnd = new Wiz_End();
            
            // Set first page as active
            NextPage();
        }


        /// <summary>
        /// Wizard page change logic
        /// </summary>
        private void NextPage()
        {
            // Only support oml installed databses at the moment
            (WizStart as Wiz_Start).cbAdvanced.Visible = false;

            if (CurrentPage == null)
            {
                if (ServerInstall)
                {
                    // Server installation
                    if (ServerAllreadyInstalled)
                    {  
                        (WizStart as Wiz_Start).cbAdvanced.Visible = false;
                        (WizStart as Wiz_Start).lMessage1.Text = "This wizard will upgrade the database to the latest version!";
                        (WizStart as Wiz_Start).lMessage2.Text = "Press Next to begin upgrading the database.";
                    }
                    else
                    {
                        (WizStart as Wiz_Start).lMessage1.Text = "This wizard will install and prepare SQL Server for OML!";
                        (WizStart as Wiz_Start).lMessage2.Text = "Press Next to begin installing SQL Server.";
                    }
                }
                else
                {
                    // Client installation
                    (WizStart as Wiz_Start).lMessage1.Text = "This wizard will assist in setting up OML for a remote server!";
                    (WizStart as Wiz_Start).lMessage2.Text = "Press Next to select a server.";
                }
                SetWizardPage(WizStart);
                return;
            }


            if (CurrentPage == WizStart)
            {
                AdvancedMode = (WizStart as Wiz_Start).cbAdvanced.Checked;

                if (ServerInstall && ServerAllreadyInstalled)
                {
                    // Upgrade Schema
                    UpgradeSchema();
                    buttonNext.Text = "Finish";
                    buttonBack.Enabled = false;
                    SetWizardPage(WizEnd);
                }
                if (ServerInstall && !ServerAllreadyInstalled)
                {
                    // Install SQL 
                    if (AdvancedMode == true)
                    {
                        if (InstallNewInstance)
                        {
                            (WizSelectServer as Wiz_SelectServer).rbInstall.Checked = true;
                            (WizSelectServer as Wiz_SelectServer).rbUseExistingInstance.Checked = false;
                        }
                        else
                        {
                            (WizSelectServer as Wiz_SelectServer).rbInstall.Checked = false;
                            (WizSelectServer as Wiz_SelectServer).rbUseExistingInstance.Checked = true;
                        }

                        SetWizardPage(WizSelectServer);
                    }
                    else
                    {
                        RunSQLSetup();
                        ConfigureSQL();
                        buttonNext.Text = "Finish";
                        buttonBack.Enabled = false;
                        SetWizardPage(WizEnd);
                    }
                }
                if (!ServerInstall)
                {
                    ShowSelectExistingServerPage(AdvancedMode);
                }
                return;
            }

            if (CurrentPage == WizSelectServer)
            {
                if ((WizSelectServer as Wiz_SelectServer).rbInstall.Checked == true)
                {
                    RunSQLSetup();
                    ConfigureSQL();
                    buttonNext.Text = "Finish";
                    buttonBack.Enabled = false;
                    SetWizardPage(WizEnd);
                }
                else
                {
                    ShowSelectExistingServerPage(true);
                    InstallNewInstance = false;
                }
                return;
            }

 
            if (CurrentPage == WizSelectExistingServer)
            {
                servername = (WizSelectExistingServer as Wiz_SelectExistingServer).cbServers.Text;
                if (AdvancedMode)
                {
                    sapassword = (WizSelectExistingServer as Wiz_SelectExistingServer).teSAPwd.Text;
                    instancename = (WizSelectExistingServer as Wiz_SelectExistingServer).teInstance.Text;
                }
                buttonNext.Text = "Finish";
                buttonBack.Enabled = false;
                SetWizardPage(WizEnd);
                return;
            }
           

            if (CurrentPage == WizEnd)
            {
                WriteSettings();
                this.Close();
            }
        }

 
        #region Forward & next buttons and wizard page management
        /// <summary>
        /// Next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            NextPage();
        }

        /// <summary>
        /// Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (WizPages.Count > 1) WizPages.RemoveAt(WizPages.Count - 1); 
            SetWizardPage(WizPages[WizPages.Count - 1]);
        }

        private void ShowSelectExistingServerPage(bool advanced)
        {
            PleaseWait pw = new PleaseWait();
            pw.Show();
            pw.SetMessage("Finding servers on network.");

            (WizSelectExistingServer as Wiz_SelectExistingServer).cbServers.Items.Clear();
            (WizSelectExistingServer as Wiz_SelectExistingServer).cbServers.Items.Add("localhost");

            foreach (string server in OMLEngine.FileSystem.NetworkScanner.GetAllAvailableNetworkDevices())
            {
                (WizSelectExistingServer as Wiz_SelectExistingServer).cbServers.Items.Add(server.Trim('\\'));
            }
            pw.Hide();
            pw.Dispose();
            pw = null;

            (WizSelectExistingServer as Wiz_SelectExistingServer).ShowAdvancedFields(advanced);
            SetWizardPage(WizSelectExistingServer);
        }

        /// <summary>
        /// Loads up the requested wizard page
        /// </summary>
        /// <param name="newpage"></param>
        private void SetWizardPage(Control newpage)
        {
            CurrentPage = newpage;

            if (WizPages.Count > 0)
            {
                if (WizPages[WizPages.Count - 1] != newpage)
                {
                    WizPages.Add(newpage);
                }
            }
            else
            {
                WizPages.Add(newpage);
            }

            if (!WizPanel.Controls.Contains(newpage))
                 WizPanel.Controls.Add(newpage);

            newpage.Visible = true;

            foreach (Control page in WizPanel.Controls)
            {
                if (page != newpage)
                    page.Visible = false;
            }
        }
        #endregion

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int Line1 = 40;
            int Line2 = 230;
            System.Drawing.Pen DarkPen = new System.Drawing.Pen(System.Drawing.Color.DarkGray);
            System.Drawing.Pen WhitePen = new System.Drawing.Pen(System.Drawing.Color.White);
            System.Drawing.Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.Font Font = new Font("Arial", 16);

            System.Drawing.Graphics formGraphics = this.CreateGraphics();

            formGraphics.FillRectangle(WhiteBrush, 0, 0, 500, Line1);

            formGraphics.DrawLine(DarkPen, 0, Line1, 500, Line1);
            formGraphics.DrawLine(WhitePen, 0, Line1 + 1, 500, Line1 + 1);
            formGraphics.DrawLine(DarkPen, 0, Line2, 500, Line2);
            formGraphics.DrawLine(WhitePen, 0, Line2 + 1, 500, Line2 + 1);

            formGraphics.DrawString("Welcome to OML Database Setup ", Font, BlackBrush, 10, 10);

            DarkPen.Dispose();
            WhitePen.Dispose();
            WhiteBrush.Dispose();
            BlackBrush.Dispose();
            WhitePen.Dispose();

            formGraphics.Dispose();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool CheckSQLExists()
        {
            const string instance = "MSSQL$OML9";

            try
            {
                ManagementObjectSearcher getSqlExpress =
                    new ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement10",
                    "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and ServiceName = '" 
                    + instance + "' and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')");

                // If nothing is returned, SQL isn't installed.
                if (getSqlExpress.Get().Count==0)
                {
                    return false;
                }

                return true;
            }
            catch (ManagementException e)
            {
                return false;
            }
        }

        private bool RunSQLSetup()
        {
            Process pr = new Process();

            if (File.Exists(@"SQLInstaller\SQLEXPR_x86_ENU.exe"))
            {
                pr.StartInfo.FileName = @"SQLInstaller\SQLEXPR_x86_ENU.exe";
                pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLConfigNoTools_x32.ini\"";
                pr.Start();
                pr.WaitForExit();
                return true;
            }
            else
                if (File.Exists(@"SQLInstaller\SQLEXPR_x64_ENU.exe"))
                {
                    pr.StartInfo.FileName = @"SQLInstaller\SQLEXPR_x86_ENU.exe";
                    pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLConfigNoTools_x64.ini\"";
                    pr.Start();
                    pr.WaitForExit();
                    return true;
                }
                else
                {
                    MessageBox.Show("Cannot find the SQL installers!", "Error");
                    return false;
                }
        }

        private void UpgradeSchema()
        {
            MessageBox.Show("Upgrade schema goes here");
        }

        private void ConfigureSQL()
        {
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
            dbm.CreateOMLDatabase();
            dbm.RunSQLScript(Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\Title Database.sql");
            dbm.CreateOMLUser();
        }

        private void WriteSettings()
        {
            OMLEngine.DatabaseManagement.DatabaseInformation.DatabaseName = "oml";
            OMLEngine.DatabaseManagement.DatabaseInformation.OMLUserAcct = "oml";
            OMLEngine.DatabaseManagement.DatabaseInformation.OMLUserPassword = "oml";
            OMLEngine.DatabaseManagement.DatabaseInformation.SAPassword = sapassword;
            OMLEngine.DatabaseManagement.DatabaseInformation.SQLInstanceName = instancename;
            OMLEngine.DatabaseManagement.DatabaseInformation.SQLServerName = servername;
            OMLEngine.DatabaseManagement.DatabaseInformation.SaveSettings();
        }
    }
}
