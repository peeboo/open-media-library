using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Management;
using System.Threading;
using System.ServiceModel;
using OMLEngine;
using OMLEngineService;

namespace TranscoderTester
{
    public partial class TranscoderTester : Form, ITranscodingNotifyingService
    {
        public ServiceHost sHost;
        public string NotifierURI;
        bool TestActive;
        bool TranscoderSuccess;
        string MovieFileName;

        public TranscoderTester()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                foreach (string arg in args)
                {
                    MovieFileName = arg;
                }

                TestActive = false;
                TranscoderSuccess = false;
                timer1.Enabled = true;
                buttonClose.Enabled = false;
            }
            else
            {
                textBoxTT.AppendText("No media file selected. Please select a title and retry!");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string ServiceName = "OMLEngineService";
            timer1.Enabled = false;
            if (!TestActive)
            {
                // Test is not currently running. Start it.
                if (TestTranscoding(ServiceName))
                {
                    TestActive = true;
                    timer1.Interval = 60000; // Set timer to 60 seconds
                    timer1.Enabled = true;
                }
                else
                {
                    buttonClose.Enabled = true;
                }
            }
            else
            {
                timer1.Enabled = false;
                if (!TranscoderSuccess)
                {
                    AddTextString("Transcoder has not reported buffer ready in required time!");
                }
                Stop();
                RestartService(ServiceName);
                buttonClose.Enabled = true;
            }
        }

        private void AddTextString(string str)
        {
            textBoxTT.AppendText(str);
            textBoxTT.AppendText(Environment.NewLine);
            textBoxTT.Select(textBoxTT.Text.Length, 0);
            
        }

        private bool TestTranscoding(string ServiceName)
        {
            // Check service
            if (!CheckService(ServiceName)) { return false; }

            // Connect to transcode service
            if (!OpenConnectionToService()) { return false; }
            TranscodingNotifyingService.OnMediaSourceStatusChanged += TranscodingNotifyingService_OnMediaSourceStatusChanged;

            TranscodeFile(MovieFileName);

            return true;
        }


        private bool CheckService(string servicename)
        {
            ServiceController sc = new ServiceController(servicename);

            try
            {
                AddTextString("Service '" + sc.ServiceName + @"' found!");
            }
            catch (Exception ex)
            {
                AddTextString("Cannot find the service '" + servicename + "'.");
                return false;
            }

            try
            {
                // Check the service startup type
                string path = "Win32_Service.Name='" + servicename + "'";
                ManagementPath p = new ManagementPath(path);
                ManagementObject ManagementObj = new ManagementObject(p);
                AddTextString("Statup type " + ManagementObj["StartMode"].ToString());

                if ((ManagementObj["StartMode"].ToString() == "Disabled") ||
                    (ManagementObj["StartMode"].ToString() == "Manual"))
                {
                    object[] parameters = new object[1];
                    parameters[0] = "Automatic";
                    AddTextString("Setting statup type to Automatic.");
                    ManagementObj.InvokeMethod("ChangeStartMode", parameters);
                }

                // Check is the service is running and start if required.
                AddTextString("The service is currently " + sc.Status.ToString().ToLower() + ".");
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    AddTextString("");
                    AddTextString("Starting service.");
                    sc.Start();

                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(1000);
                        sc.Refresh();
                        AddTextString("The service is currently " + sc.Status.ToString().ToLower() + ".");
                        if (sc.Status == ServiceControllerStatus.Running) { break; }
                        if (sc.Status == ServiceControllerStatus.Stopped) { break; }
                    }


                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        AddTextString("Service started.");
                        return true;
                    }
                    else
                    {
                        AddTextString("Service failed to start.");
                        return false;
                    }

                }
            }
            catch
            {
                AddTextString("A problem has occured when trying to check the service. Vista UAC can cause this!");
            }
            AddTextString("");
            return true;
        }


        private bool OpenConnectionToService()
        {
            if (sHost != null)
                return false;

            try
            {
                AddTextString("Creating transcoder notifier.");

                NotifierURI = GetNotifierUri();
                sHost = new ServiceHost(typeof(TranscodingNotifyingService), new Uri(NotifierURI));
                sHost.AddServiceEndpoint(typeof(ITranscodingNotifyingService), WCFUtilites.NetTcpBinding(), NotifierURI);
                sHost.Open();

                AddTextString("Creating transcoder service proxy.");

                var host = NewTranscodingServiceProxy();

                AddTextString("Registering notifier with service.");

                host.Channel.RegisterNotifyer(NotifierURI, true);

                AddTextString("Successfully connected to transcoder service.");
                AddTextString("");
                return true;
            }
            catch
            {
                AddTextString("Failed to connect to transcoder service. Check Windows firewall.");
                AddTextString("");
                return false;
            }
            return true;
        }


        private void RestartService(string servicename)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                AddTextString("Restarting transcoder service.");
                ServiceController sc = new ServiceController(servicename);
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                AddTextString("Transcoder service stopped.");

                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running);
                AddTextString("Transcoder started.");

                this.Cursor = Cursors.Default;
            }
            catch
            {
                AddTextString("Unable to restart transcoding service. Vista UAC can cause this!");
            }
        }


        void TranscodingNotifyingService_OnMediaSourceStatusChanged(string key, TranscodingStatus status)
        {
            timer1.Stop();
            AddTextString("Transcoder reports " + key + " - " + status.ToString());
            AddTextString("");

            if (status.ToString().Contains("BufferReady"))
            {
                AddTextString("Congratulations, the transcoder appears to be working fine!!");
                AddTextString("");
                TranscoderSuccess = true;
                timer1.Interval = 100;
            } 
            
            if (status.ToString().Contains("Done"))
            {
                AddTextString("Congratulations, the transcoder appears to be working fine!!");
                AddTextString("");
                TranscoderSuccess = true;
                timer1.Interval = 100;
            }

            if (status.ToString().Contains("Error"))
            {
                AddTextString("Bad news, the transcoder reported an error. Try a different title (AVI, MPG or MKV are good)!!");
                AddTextString("");
                TranscoderSuccess = false;
                timer1.Interval = 100;
            }

            timer1.Start();
        }


        void TranscodeFile(string filename)
        {
            Disk disk = new Disk("Test", filename, VideoFormat.MKV);
            MediaSource source = new MediaSource(disk);
            AddTextString("Starting transcode job on " + source.Disk);
            var host = TranscodingNotifyingService.NewTranscodingServiceProxy();
            host.Channel.Transcode(source);
            AddTextString("");
        }


        public string GetNotifierUri()
        {
            return string.Format("net.tcp://localhost:{0}/OMLTNS", WCFUtilites.FreeTcpPort());
        }


        public MyClientBase<ITranscodingService> NewTranscodingServiceProxy()
        {
            return new MyClientBase<ITranscodingService>(WCFUtilites.NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8321/OMLTS"));
        }


        public MyClientBase<ITranscodingNotifyingService> NewTranscodingNotifyingServiceProxy(string uri)
        {
            return new MyClientBase<ITranscodingNotifyingService>(WCFUtilites.NetTcpBinding(), new EndpointAddress(uri));
        }


        public void Stop()
        {
            if (sHost == null) return;

            AddTextString("Unregistering Notifier.");
            try
            {
                var host = NewTranscodingServiceProxy();
                host.Channel.RegisterNotifyer(NotifierURI, false);
                AddTextString("Notifier unregistered.");
            }
            catch
            {
                AddTextString("Failed to Unregister Notifier.");
            }
            sHost.Close();
            sHost = null;
            AddTextString("");
        }


        public event MediaSourceStatusChanged OnMediaSourceStatusChanged;


        public void StatusChanged(string key, TranscodingStatus status)
        {
            if (OnMediaSourceStatusChanged != null) OnMediaSourceStatusChanged(key, status);
        }
       
 
        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
