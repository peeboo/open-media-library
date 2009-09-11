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
using System.ServiceProcess;
using OMLEngine;

namespace PostInstallerWizard
{
    // Database management logic
    //
    // Is there an existing 'settings.xml', if so oml has allready been setup
    // In this case we simply need to verify we can connect to the db and check
    // the schema version (upgrade if required)
    //
    //
    //
    //
    //
    enum WizardActions
    {
        CheckAndUpgradeSchema,
        InstallSQLServer,
        SetupForRemoteServer

    }
    public partial class Form1 : Form
    {
        List<Control> WizPages = new List<Control>();

        Control WizStart;
        Control WizSelectServer;
        Control WizEnd;
        Control WizEndSQLInstallFail;
        Control WizSelectExistingServer;

        Control CurrentPage;

        WizardActions WizardAction;

        bool ServerInstall;             // Does the install include the server components
        bool ServerAllreadyInstalled;
        bool InstallNewInstance;
        bool AdvancedMode;

        string servername;
        string sapassword;
        string instancename;

        string ScriptsPath;

        public Form1()
        {
            ServerInstall = false;

            // Check if this a server installation. This needs to be changed to a better method.
            // Maybe a registry setting in wix to indicate server install
            if ((File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x86_ENU.exe")) ||
                (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x64_ENU.exe")))
            {
                ServerInstall = true;
            }

            if ((File.Exists("C:\\Program Files\\OpenMediaLibrary\\SQLInstaller\\SQLEXPR_x86_ENU.exe")) ||
                (File.Exists("C:\\Program Files\\OpenMediaLibrary\\SQLInstaller\\SQLEXPR_x64_ENU.exe")))
            {
                ServerInstall = true;
            }

            // Check the command line for the 'client' override
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                if (string.Compare(Environment.GetCommandLineArgs()[1], "client", true) == 0)
                {
                    ServerInstall = false;
                }
            }

            // Does the OML instance allready exists
            if (ServerInstall)
            {
                ServerAllreadyInstalled = CheckSQLExists();
            }
            else
            {
                ServerAllreadyInstalled = false;
            }


            // Set some bools
            InstallNewInstance = true;
            AdvancedMode = false;

            CurrentPage = null;

            // Default database info
            sapassword = "R3WztB4#9";
            instancename = "oml";
            servername = "localhost";

            // Find the script path. Also include hack to find scripts if running from VS rather than c:\program files....
            ScriptsPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller";
            if (Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\..\\..\\SQL Scripts"))
            {
                ScriptsPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\..\\..\\SQL Scripts";
            }

            InitializeComponent();

            // Create wizard pages
            WizStart = new Wiz_Start();
            WizSelectServer = new Wiz_SelectServer();
            WizSelectExistingServer = new Wiz_SelectExistingServer();
            WizEnd = new Wiz_End();
            WizEndSQLInstallFail = new Wiz_End_SQLInstall_Fail();
            
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
                if (OMLEngine.DatabaseManagement.DatabaseInformation.ConfigFileExists)
                {
                    OMLEngine.Utilities.DebugLine("[PostInstallerWizard] settings.xml file exists, assume SQL is allready setup");

                    // Good news, this must be an upgrade. Just need to check  
                    // we can connect and check the schema version
                    (WizStart as Wiz_Start).lMessage1.Text = "This wizard has detected an existing installation of OML!";
                    (WizStart as Wiz_Start).lMessage2.Text = "Press Next to begin checking the database.";

                    sapassword = OMLEngine.DatabaseManagement.DatabaseInformation.SAPassword;
                    instancename = OMLEngine.DatabaseManagement.DatabaseInformation.SQLInstanceName;
                    servername = OMLEngine.DatabaseManagement.DatabaseInformation.SQLServerName;

                    WizardAction = WizardActions.CheckAndUpgradeSchema;
                }
                else
                {
                    OMLEngine.Utilities.DebugLine("[PostInstallerWizard] settings.xml does not exist, install SQL");
                    // This must be a new install. Is it a server install
                    if (ServerInstall)
                    {
                        // Server installation
                        if (ServerAllreadyInstalled)
                        {
                            (WizStart as Wiz_Start).cbAdvanced.Visible = false;
                            (WizStart as Wiz_Start).lMessage1.Text = "This wizard has detected a previous installation of OML!";
                            (WizStart as Wiz_Start).lMessage2.Text = "Press Next to begin checking the database.";
                            WizardAction = WizardActions.CheckAndUpgradeSchema;
                        }
                        else
                        {
                            (WizStart as Wiz_Start).lMessage1.Text = "This wizard will install and prepare SQL Server for OML!";
                            (WizStart as Wiz_Start).lMessage2.Text = "Press Next to begin installing SQL Server.";
                            WizardAction = WizardActions.InstallSQLServer;
                        }
                    }
                    else
                    {
                        // Client installation
                        (WizStart as Wiz_Start).lMessage1.Text = "This wizard will assist in setting up OML for a remote server!";
                        (WizStart as Wiz_Start).lMessage2.Text = "Press Next to select a server.";
                        WizardAction = WizardActions.SetupForRemoteServer;
                    }
                }
                SetWizardPage(WizStart);
                return;
            }


            if (CurrentPage == WizStart)
            {
                AdvancedMode = (WizStart as Wiz_Start).cbAdvanced.Checked;

                if (WizardAction == WizardActions.CheckAndUpgradeSchema)
                {
                    // Check & Upgrade Schema
                    CheckAndUpgradeSchema();
                    buttonNext.Text = "Finish";
                    buttonBack.Enabled = false;
                    SetWizardPage(WizEnd);
                }
                if (WizardAction == WizardActions.InstallSQLServer)
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
                        if (RunSQLSetup())
                        {
                            WriteSettings();
                            ConfigureSQL();
                            SetWizardPage(WizEnd);
                        }
                        else
                        {
                            SetWizardPage(WizEndSQLInstallFail);
                        }
                    }
                }
                if (WizardAction == WizardActions.SetupForRemoteServer)
                {
                    ShowSelectExistingServerPage(AdvancedMode);
                }
                return;
            }

            if (CurrentPage == WizSelectServer)
            {
                if ((WizSelectServer as Wiz_SelectServer).rbInstall.Checked == true)
                {
                    WriteSettings();
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

                // Check & Upgrade Schema
                CheckAndUpgradeSchema();
                SetWizardPage(WizEnd);
                return;
            }
           

            if (CurrentPage == WizEnd)
            {
                WriteSettings();
                this.Close();
            }

            if (CurrentPage == WizEndSQLInstallFail)
            {
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

            //foreach (string server in OMLEngine.FileSystem.NetworkScanner.GetAllAvailableNetworkDevices())
            {
            //    (WizSelectExistingServer as Wiz_SelectExistingServer).cbServers.Items.Add(server.Trim('\\'));
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
            // Manage forward / next buttons
            if (newpage == WizStart)
            {
                buttonBack.Enabled = false;
            }
            else
            {
                buttonBack.Enabled = true;
            }

            if ((newpage == WizEnd) || (newpage == WizEndSQLInstallFail))
            {
                buttonNext.Text = "Finish";
                buttonBack.Enabled = false;
            }
            else
            {
                buttonNext.Text = "Next";
                buttonBack.Enabled = true;
            }

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
            const string instance = "MSSQL$OML";
            //const string instance = "MSSQLSERVER";

            try
            {
                // Enumerate all SQL instances on system
                ManagementObjectSearcher getAllSQLInstances =
                    new ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement10",
                    "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 " +
                    " and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')");

                ManagementObjectCollection resultsAll = getAllSQLInstances.Get();
                
                foreach (ManagementObject service in resultsAll)
                {
                    OMLEngine.Utilities.DebugLine("[PostInstallerWizard] Emumerating SQL Server Instance : " + service.ToString());
                }


                // Enumerate OML Names instance

                ManagementObjectSearcher getOMLInstance =
                   new ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement10",
                   "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 " +
                   " and ServiceName = '" + instance + "'" +
                   " and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')");

                ManagementObjectCollection resultsOML = getOMLInstance.Get();

                // If nothing is returned, SQL isn't installed.
                if (resultsOML.Count == 0)
                {
                    OMLEngine.Utilities.DebugLine("[PostInstallerWizard] No OML SQL Servers Not Found");
                    return false;
                }

                foreach (ManagementObject service in resultsOML)
                {
                    OMLEngine.Utilities.DebugLine("[PostInstallerWizard] Found OML SQL Server Instance : " + service.ToString());
                }

                return true;
            }
            catch (ManagementException e)
            {
                return false;
            }
        }
        
        
        private static StringBuilder prOutput = null;
        private static StringBuilder prError = null;
        private bool RunSQLSetup()
        {
            Process pr = new Process();

            if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x86_ENU.exe"))
            {
                pr.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x86_ENU.exe";
                pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLConfigNoTools_x32.ini\"";
            }
            else
            {

                if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x64_ENU.exe"))
                {
                    pr.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x64_ENU.exe";
                    pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller\\SQLConfigNoTools_x64.ini\"";
                }
                else
                {
                    MessageBox.Show("Cannot find the SQL installers!", "Error");
                    return false;
                }
            }

            // Attempt to capture stdout & stderr, doesn't seem to work but leaving code in
            // just incase it works but is buggy

            // Set UseShellExecute to false for redirection.
            pr.StartInfo.UseShellExecute = false;

            // Setup stdout capture
            pr.StartInfo.RedirectStandardOutput = true;
            pr.OutputDataReceived += new DataReceivedEventHandler(NetOutputDataHandler);
            prOutput = new StringBuilder();

            // Setup stderr capture
            pr.StartInfo.RedirectStandardError = true;
            pr.ErrorDataReceived += new DataReceivedEventHandler(NetErrorDataHandler);
            prError = new StringBuilder();

            pr.Start();

            // Start the asynchronous read of the standard output & stderr stream.
            pr.BeginOutputReadLine();
            pr.BeginErrorReadLine();

            pr.WaitForExit();

            int ExitCode = pr.ExitCode;

            if (ExitCode == 0)
                return true;
            else
            {
                MessageBox.Show("SQL installer reported error code " + ExitCode.ToString(), "Error");
                return false;
            }

        }

        private static void NetOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                // Add the text to the collected output.
                prOutput.Append(Environment.NewLine + "  " + outLine.Data);
            }
        }

        private static void NetErrorDataHandler(object sendingProcess, DataReceivedEventArgs errLine)
        {
            // Write the error text to the file if there is something
            // to write and an error file has been specified.

            if (!String.IsNullOrEmpty(errLine.Data))
            {
                prError.Append(Environment.NewLine + "  " + errLine.Data);
            }
        }



        private void CheckAndUpgradeSchema()
        {
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();

            OMLEngine.DatabaseManagement.DatabaseInformation.SQLState state = dbm.CheckDatabase();

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBNotFound)
            {
                MessageBox.Show("Detected SQL Server but cannot find the database. Click OK to create the database.", "Databse problem", MessageBoxButtons.OK);
                // OML Instance but OML database does not exist
                dbm.ConfigureSQL(ScriptsPath);
                dbm.UpgradeSchemaVersion(ScriptsPath);

                // Retest the connection
                state = dbm.CheckDatabase();
            }

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired)
            {
                MessageBox.Show("Detected the OML Database but it requires updating. Click OK to update the database.", "Databse problem", MessageBoxButtons.OK);
                dbm.UpgradeSchemaVersion(ScriptsPath);
   
                // Retest the connection
                state = dbm.CheckDatabase();
            }

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OK)
            {
                MessageBox.Show("The database appears all fine.", "Database status", MessageBoxButtons.OK);
                return;
            }
        }

        private void ConfigureSQL()
        {
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
            dbm.ConfigureSQL(ScriptsPath);
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
