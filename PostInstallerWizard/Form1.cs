using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
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
        Control WizSelectServerSource;
        Control WizReviewSettings;

        Control CurrentPage;

        bool ServerAllreadyConfigured;
        bool InstallNewInstance;
        bool AdvancedMode;

        // Actions
        bool ActionDownloadSQLSetup;
        bool ActionInstallSQL;
        bool ActionSetupDatabase;
        bool ActionConfigureExistingInstance;
        bool ActionCheckSchema;

        public Form1()
        {
            // Check to see if settings.xml allready exists
            if (File.Exists(Path.Combine(FileSystemWalker.PublicRootDirectory, "settings.xml")))
            {
                ServerAllreadyConfigured = true;
            }
            else
            {
                ServerAllreadyConfigured = false;
            }

            // Set some bools
            InstallNewInstance = true;
            AdvancedMode = false;

            ActionDownloadSQLSetup = false;
            ActionInstallSQL = false;
            ActionSetupDatabase = false;
            ActionConfigureExistingInstance = false;
            ActionCheckSchema = false;

            CurrentPage = null;
            
            InitializeComponent();

            // Create wizard pages
            WizStart = new Wiz_Start();
            WizSelectServer = new Wiz_SelectServer();
            WizSelectExistingServer = new Wiz_SelectExistingServer();
            WizSelectServerSource = new Wiz_SelectServerSource();
            WizReviewSettings = new Wiz_ReviewSettings();
            WizEnd = new Wiz_End();
            
            // Set first page as active
            NextPage();
        }



        /// <summary>
        /// Wizard page change logic
        /// </summary>
        private void NextPage()
        {
            if (CurrentPage == null)
            {
                if (ServerAllreadyConfigured)
                {
                    (WizStart as Wiz_Start).cbAdvanced.Visible = true;
                    (WizStart as Wiz_Start).lMessage1.Text = "Existing installation detected!";
                    (WizStart as Wiz_Start).lMessage2.Text = "This wizard will upgrade the database to the latest version!";
                }
                else
                {
                    (WizStart as Wiz_Start).cbAdvanced.Visible = false;
                    (WizStart as Wiz_Start).lMessage1.Text = "New installation detected!";
                    (WizStart as Wiz_Start).lMessage2.Text = "This wizard will guide you through installing SQL Server!";
                }
                SetWizardPage(WizStart);
                return;
            }

            if (CurrentPage == WizStart)
            {
                AdvancedMode = (WizStart as Wiz_Start).cbAdvanced.Checked;

                if ((!ServerAllreadyConfigured) || (AdvancedMode == true))
                {
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
                        SetWizardPage(WizSelectServerSource);
                    }
                }
                else
                {
                    ActionConfigureExistingInstance = false;
                    ActionSetupDatabase = false;
                    ActionDownloadSQLSetup = false;
                    ActionInstallSQL = false;
                    ActionCheckSchema = true;
                    SetupActionTextBox();
                    SetWizardPage(WizReviewSettings);
                }

                return;
            }

            if (CurrentPage == WizSelectServer)
            {
                if ((WizSelectServer as Wiz_SelectServer).rbInstall.Checked == true)
                {
                    InstallNewInstance = true;
                    SetWizardPage(WizSelectServerSource);
                }
                else
                {
                    InstallNewInstance = false;
                    SetWizardPage(WizSelectExistingServer);
                }
                return;
            }

            if (CurrentPage == WizSelectExistingServer)
            {
                ActionConfigureExistingInstance = true;
                ActionSetupDatabase = true;
                ActionDownloadSQLSetup = false;
                ActionInstallSQL = false;
                ActionCheckSchema = false;

                SetupActionTextBox();
                SetWizardPage(WizReviewSettings);
                return;
            }
            
            if (CurrentPage ==  WizSelectServerSource)
            {
                if ((WizSelectServerSource as Wiz_SelectServerSource).rbDownloadSQLInstaller.Checked)
                {
                    ActionDownloadSQLSetup = true;
                }
                else
                {
                    ActionDownloadSQLSetup = false;
                }

                ActionConfigureExistingInstance = false;
                ActionInstallSQL = true;
                ActionSetupDatabase = true;

                SetupActionTextBox();
                SetWizardPage(WizReviewSettings);
                return;
            }

            if (CurrentPage == WizReviewSettings)
            {
                if (ActionDownloadSQLSetup) DownloadInstaller("");
                if (ActionInstallSQL) RunSQLSetup();
                if (ActionSetupDatabase) ConfigureSQL();
                if (ActionCheckSchema) UpgradeSchema();

                buttonNext.Text = "Finish";
                buttonBack.Enabled = false;
                SetWizardPage(WizEnd);
                return;
            }

            if (CurrentPage == WizEnd)
            {
                this.Close();
            }
        }

        private void SetupActionTextBox()
        {
            StringBuilder txt = new StringBuilder();
            if (ActionDownloadSQLSetup) txt.AppendLine("Download SQL from Microsoft.");
            if (ActionInstallSQL) txt.AppendLine("Install SQL Server as OML default instance.");
            if (ActionSetupDatabase) txt.AppendLine("Setup OML Database.");
            if (ActionConfigureExistingInstance) txt.AppendLine("Configure OML to use existing server.");
            if (ActionCheckSchema) txt.AppendLine("Check OML Database version and upgrade if required.");
               
            (WizReviewSettings as Wiz_ReviewSettings).tbActions.Text = txt.ToString();
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
            int Line2 = 256;
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

            formGraphics.DrawString("Welcome", Font, BlackBrush, 10, 10);

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

        private void DownloadInstaller(string url)
        {
            // todo : mike : Find if 32 or 64 bit os and download the correct version
            MessageBox.Show("Download installer goes here");
        }

        private void RunSQLSetup()
        {
            // todo : mike : Find if 32 or 64 bit os and install the correct version
            MessageBox.Show("Run SQL Installer goes here");
        }

        private void UpgradeSchema()
        {
            MessageBox.Show("Upgrade schema goes here");
        }

        private void ConfigureSQL()
        {
            // Create sql accounts

            // Create Database

            // Run schema query
            MessageBox.Show("Configure sql goes here");
        }
    }
}
