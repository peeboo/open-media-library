using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.IO;
using System.ServiceProcess;
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using FileDownloader;
using System.Data.Sql;

namespace OMLCustomWiXAction {
    public class CustomActions {
        private static string MediaInfoX64Url = @"http://www.openmedialibrary.org/OMLInstallerFiles/MediaInfox64.dll";
        private static string MediaInfoX86Url = @"http://www.openmedialibrary.org/OMLInstallerFiles/MediaInfoi386.dll";
        private static string MediaInfoLocalPath = @"c:\program files\openmedialibrary\MediaInfo.dll";

        private static string MEncoderUrl = @"http://www.openmedialibrary.org/OMLInstallerFiles/mencoder-1.0rc2-4.2.1.exe";
        private static string MEncoderPath = @"c:\program files\openmedialibrary\MEncoder.exe";

        private static string UserManualUrl = @"http://www.openmedialibrary.org/OMLInstallerFiles/Open_Media_Library_User_Manual.pdf";
        private static string UserManualHelpPath = @"c:\program files\openmedialibrary\Help";
        private static string UserManualPath = @"c:\program files\openmedialibrary\Help\Open_Media_Library_User_Manual.pdf";

        [CustomAction]
        public static ActionResult StartOMLEngineService(Session session) {
            session.Log("Inside StartOMLEngineService");
            try {
                ServiceController omlengineController = new ServiceController(@"OMLEngineService");
                TimeSpan timeout = TimeSpan.FromSeconds(20);
                omlengineController.Start();
                omlengineController.WaitForStatus(ServiceControllerStatus.Running, timeout);
                omlengineController.Close();
                session.Log("StartOMLEngineService CA: Success");
                return ActionResult.Success;
            } catch (Exception e) {
                session.Log(string.Format("Error starting OMLEngineService: {0}", e.Message));
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult StartOMLFWService(Session session) {
            try {
                ServiceController omlfsserviceController = new ServiceController(@"OMLFWService");
                TimeSpan timeout = TimeSpan.FromSeconds(10);
                omlfsserviceController.Start();
                omlfsserviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
                omlfsserviceController.Close();
            } catch (Exception e) {
                session.Log(string.Format("An error occured starting the OMLFW Service: {0}", e.Message));
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ReserveTrailersUrl(Session session) {
            session.Log("Inside ReserveTrailersUrl");
            try {
                SecurityIdentifier sid;

                //add users in case someone runs the service as themselves
                sid = new SecurityIdentifier("S-1-5-32-545");//users
                URLReservation.UrlReservation rev = new URLReservation.UrlReservation("http://127.0.0.1:8484/3f0850a7-0fd7-4cbf-b8dc-c7f7ea31534e/");
                rev.AddSecurityIdentifier(sid);

                //add system because that is the default
                sid = new SecurityIdentifier("S-1-5-18");//system
                rev.AddSecurityIdentifier(sid);
                rev.Create();
                session.Log("ReserveTrailersUrl CA: Success");
                return ActionResult.Success;

            } catch (Exception e) {
                session.Log("Error in ReserveTrailersUrl: {0}", e.Message);
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult ActivateNetTcpPortSharing(Session session) {
            try {
                System.ServiceModel.Activation.Configuration.NetTcpSection ntSection = new System.ServiceModel.Activation.Configuration.NetTcpSection();
                ntSection.TeredoEnabled = true;
                session.Log("ActivateNetTcpPortSharing: Success");
                return ActionResult.Success;
            } catch (Exception e) {
                session.Log(string.Format("Failed to activate NetTcpPortSharing: {0}", e.Message));
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult DownloadAndInstallMediaInfo(Session session) {
            string type = Environment.GetEnvironmentVariable(@"PROCESSOR_ARCHITECTURE");
            string miUrl = type.ToUpperInvariant().Contains("86")
                ? CustomActions.MediaInfoX86Url
                : CustomActions.MediaInfoX64Url;

            DownloadEngine miEngine = new DownloadEngine(miUrl);

            miEngine.Log += (s) => {
                int pct = Convert.ToInt32((Convert.ToDouble(Int32.Parse(s)) / Convert.ToDouble(miEngine.TotalBytes)) * 100);
                session.Log(string.Format("MediaInfo: {0}", pct));
            };
            bool miDownloaded = miEngine.Download();
            if (!miDownloaded) {
                session.Log("MediaInfo Failed to download");
                return ActionResult.Failure;
            }
            session.Log("File is: {0}", miEngine.DownloadedFile);
            try {
                File.Copy(miEngine.DownloadedFile, CustomActions.MediaInfoLocalPath, true);
            } catch (Exception ex) {
                session.Log("MediaInfo Error: {0}", ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DownloadAndInstallMEncoder(Session session) {
            DownloadEngine meEngine = new DownloadEngine(CustomActions.MEncoderUrl);
            meEngine.Log += (s) => {
                int pct = Convert.ToInt32((Convert.ToDouble(Int32.Parse(s)) / Convert.ToDouble(meEngine.TotalBytes)) * 100);
                session.Log(string.Format("MEncoder: {0}", pct));
            };
            bool meDownloaded = meEngine.Download();
            if (!meDownloaded) {
                session.Log("MEncoder Failed to download");
                return ActionResult.Failure;
            }
            session.Log("File is: {0}", meEngine.DownloadedFile);
            try {
                File.Copy(meEngine.DownloadedFile, CustomActions.MEncoderPath, true);
            } catch (Exception ex) {
                session.Log("MEncoder Error: {0}", ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DownloadAndInstallUserManual(Session session) {
            DownloadEngine umEngine = new DownloadEngine(CustomActions.UserManualUrl);
            umEngine.Log += (s) => {
                int pct = Convert.ToInt32((Convert.ToDouble(Int32.Parse(s)) / Convert.ToDouble(umEngine.TotalBytes)) * 100);
                session.Log(string.Format("PDF: {0}", pct));
            };
            bool umDownloaded = umEngine.Download();
            if (!umDownloaded) {
                session.Log("Open_Media_Library_User_Manual Failed to download");
                return ActionResult.Failure;
            }
            session.Log("File is: {0}", umEngine.DownloadedFile);
            try {
                if (Directory.Exists(CustomActions.UserManualHelpPath)) {
                    session.Log(@"Creating folder: {0}", CustomActions.UserManualHelpPath);
                    Directory.CreateDirectory(CustomActions.UserManualHelpPath);
                    session.Log("setting access controls");
                //    DirectoryInfo dInfo = new DirectoryInfo(CustomActions.UserManualHelpPath);
                //    System.Security.AccessControl.DirectorySecurity dSec = dInfo.GetAccessControl();
                //    dSec.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
                //        "Users", System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow
                //        ));
                //    dInfo.SetAccessControl(dSec);
                }
                session.Log("Copying file");
                File.Copy(umEngine.DownloadedFile, CustomActions.UserManualPath, true);
            } catch (Exception ex) {
                session.Log("Open_Media_Library_User_Manual Error: {0}", ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ScanNetworkForSqlServers(Session session) {
            IList<string> servers = new List<string>();
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow row in dt.Rows)
                servers.Add(string.Format("{0} ({1})", row["ServerName"], row["InstanceName"]));

                return ActionResult.Success;
        }
    }
}
