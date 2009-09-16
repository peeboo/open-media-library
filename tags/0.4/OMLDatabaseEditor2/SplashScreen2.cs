using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor
{
    public partial class SplashScreen2 : DevExpress.XtraEditors.XtraForm
    {    
        // Threading

        static SplashScreen2 ms_frmSplash = null;
        static Thread ms_oThread = null;

        public delegate void ProgressDelegate(int progress);
        public delegate void StatusDelegate(string status);
        public delegate void CloseDelegate();

        public SplashScreen2()
        {
            InitializeComponent();
        }

        static private void ShowForm()
        {
            ms_frmSplash = new SplashScreen2();
            Application.Run(ms_frmSplash);
        }

        static public void ShowSplashScreen()
        {
            // Make sure it is only launched once.

            if (ms_frmSplash != null)
                return;
            ms_oThread = new Thread(new ThreadStart(SplashScreen2.ShowForm));
            ms_oThread.IsBackground = true;
            ms_oThread.ApartmentState = ApartmentState.STA;
            ms_oThread.Start();
            Thread.Sleep(100);
        }

        static public void CloseForm()
        {
            if (ms_frmSplash != null && ms_frmSplash.IsDisposed == false)
            {
                ms_frmSplash._CloseForm();
            }
            ms_oThread = null;  // we do not need these any more.
            ms_frmSplash = null;
        }

        private void _CloseForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CloseDelegate(this._CloseForm));
            }
            else
            {
                this.Close();
            }
        }

        private void _SetProgress(int progress)
        {
            if (progressBarControl1.InvokeRequired)
            {
                this.progressBarControl1.Invoke(new ProgressDelegate(this._SetProgress), new Object[] { progress });
            }
            else
            {
                progressBarControl1.Position = progress;
            }
        }

        private void _SetStatus(string status)
        {
            if (labelControlStatus.InvokeRequired)
            {
                this.labelControlStatus.Invoke(new StatusDelegate(this._SetStatus), new Object[] { status });
            }
            else
            {
                labelControlStatus.Text = status;
            }
        }

        static public void SetStatus(int progress, string status)
        {
            if (ms_frmSplash != null)
            {
                ms_frmSplash._SetStatus(status);
                ms_frmSplash._SetProgress(progress);
            }
        }
    }
}