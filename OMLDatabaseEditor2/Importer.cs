using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using OMLEngine;
using OMLSDK;
using System.Threading;

namespace OMLDatabaseEditor
{
    public partial class Importer : DevExpress.XtraEditors.XtraForm
    {
        OMLPlugin importer;
        string[] work;

        public Importer(OMLPlugin _importer)
        {
            InitializeComponent();

            sbOK.Enabled = false;
            sbCancel.Enabled = false;

            importer = _importer;

            // Present the folder/file select dialog. 
            work = importer.GetWork();


            if (work != null)
            {
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.RunWorkerAsync();
                sbCancel.Enabled = true;
            }
            else
            {
                lbStatus.Text = "Cancelled";
                sbOK.Enabled = true;
            }
        }

        private void ShowNonFatalErrors(string[] errors)
        {
            Controls.NonFatalErrors nfe = new Controls.NonFatalErrors();
            nfe.LoadErrors(errors);
            nfe.Show();
        }

        public void LoadTitlesIntoDatabase(OMLPlugin plugin)
        {
            bool Cancelled = false;
            try
            {
                Utilities.DebugLine("[OMLImporter] Titles loaded, beginning Import process");
                IList<Title> titles = plugin.GetTitles();
                Utilities.DebugLine("[OMLImporter] " + titles.Count + " titles found in input file");

                int totalNumberOfTitles = 0;
                int numberOfTitlesAdded = 0;
                int numberOfTitlesSkipped = 0;

                bool YesToAll = true;// false;

                foreach (Title t in titles)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        // Abort the operation
                        Cancelled = true;
                        break;
                    }

                    backgroundWorker1.ReportProgress(100 * totalNumberOfTitles / titles.Count, "Saving title : " + t.Name);

                    totalNumberOfTitles++;

                    if (TitleCollectionManager.ContainsDisks(t.Disks))
                    {
                        numberOfTitlesSkipped++;
                        continue;
                    }

                    if (!YesToAll)
                    {
                        //TODO: Need to show a UI that let's the User decide whether to import all titles or be selective about it

                        /*Console.WriteLine("Would you like to add this title? (y/n/a)");
                        string response = Console.ReadLine();
                        switch (response.ToUpper())
                        {
                            case "Y":
                                mainTitleCollection.Add(t);
                                numberOfTitlesAdded++;
                                break;
                            case "N":
                                numberOfTitlesSkipped++;
                                break;
                            case "A":
                                YesToAll = true;
                                mainTitleCollection.Add(t);
                                numberOfTitlesAdded++;
                                break;
                            default:
                                break;
                        }*/
                    }
                    else
                    {
                        TitleCollectionManager.AddTitle(t);
                        numberOfTitlesAdded++;
                    }
                }

                backgroundWorker1.ReportProgress(100, "Finalising...");

                TitleCollectionManager.SaveTitleUpdates();

                if (Cancelled)
                {
                    backgroundWorker1.ReportProgress(100, "Cancelled : Added " + numberOfTitlesAdded + "  Skipped " + numberOfTitlesSkipped);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Complete : Added " + numberOfTitlesAdded + "  Skipped " + numberOfTitlesSkipped);
                }

                plugin.GetTitles().Clear();
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("Exception in LoadTitlesIntoDatabase: {0}", e.Message);
                Utilities.DebugLine("[OMLImporter] Exception in LoadTitlesIntoDatabase: " + e.Message);
            }
        }

        private void sbCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            sbCancel.Enabled = false;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbStatus.Text = e.UserState.ToString();
            pbProgress.Position = e.ProgressPercentage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0, "Starting");

            //string[] work = importer.GetWork();
            if (work != null)
            {
                backgroundWorker1.ReportProgress(0, "Scanning titles");
                importer.DoWork(work);
                backgroundWorker1.ReportProgress(0, "Saving titles");
                LoadTitlesIntoDatabase(importer);
            }

            string[] nonFatalErrors = importer.GetErrors;
            if (nonFatalErrors.Length > 0)
                ShowNonFatalErrors(nonFatalErrors);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sbCancel.Enabled = false;
            sbOK.Enabled = true;
        }
    }
}