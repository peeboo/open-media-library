using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.IO;
using System.ServiceProcess;
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using FileDownloader;
using System.Security.AccessControl;

namespace OMLCustomWiXAction {
    public class CustomActions {
        private static string MediaInfoX64Url = @"http://open-media-library.googlecode.com/files/MediaInfox64.dll";
        private static string MediaInfoX86Url = @"http://open-media-library.googlecode.com/files/MediaInfoi386.dll";
        private static string MediaInfoLocalPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\openmedialibrary\MediaInfo.dll";

        private static string MEncoderUrl = @"http://open-media-library.googlecode.com/files/mencoder-1.0rc2-4.2.1.exe";
        private static string MEncoderPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\openmedialibrary\MEncoder.exe";

        private static string UserManualUrl = @"http://open-media-library.googlecode.com/files/Open_Media_Library_User_Manual.pdf";
        private static string UserManualHelpPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\openmedialibrary\Help";
        private static string UserManualPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\openmedialibrary\Help\Open_Media_Library_User_Manual.pdf";

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

            session.Log("MediaInfo: Selected {0} based on detected processor architecture of {1}",
                miUrl, Environment.GetEnvironmentVariable(@"PROCESSOR_ARCHITECTURE"));

            DownloadEngine miEngine = new DownloadEngine(miUrl);

            miEngine.Bytes += (i) => {
                if (i > 0) {
                    int pct = Convert.ToInt32((Convert.ToDouble(i) / Convert.ToDouble(miEngine.TotalBytes)) * 100);
                    session.Log(string.Format("MediaInfo: ({0}) {1}b of {2}b", pct, i, miEngine.TotalBytes));
                }
            };

            session.Log("MediaInfo: Beginning download");
            bool miDownloaded = false;
            try {
                miDownloaded = miEngine.Download(true);
            } catch (Exception ex) {
                session.Log("MediaInfo: Error {0}", ex.Message);
                return ActionResult.Failure;
            }

            if (!miDownloaded) {
                session.Log("MediaInfo: Failed to download");
                return ActionResult.Failure;
            }
            session.Log("MediaInfo: Downloaded File Location {0}", miEngine.DownloadedFile);
            session.Log("MediaInfo: Final destination is {0}", CustomActions.MediaInfoLocalPath);

            try {
                session.Log("MediaInfo: copying into final location");
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
            if (session["HasScannedForSql"].CompareTo("0") == 0) {
                DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
                View sqlComboBox = session.Database.OpenView("SELECT * FROM `ComboBox`");
                sqlComboBox.Execute();
                int numRows = 0;
                while (sqlComboBox.Fetch() != null) {
                    numRows++;
                }
                if (numRows == 1) {
                    session.Log("found {0} sql servers", dt.Rows.Count);
                    int itemNumber = 2;
                    foreach (DataRow row in dt.Rows) {
                        Record rec = new Record(3);
                        rec.SetString(1, "OMLProp_SqlServers");
                        rec.SetInteger(2, itemNumber);
                        string description = string.Format("{0} - {1}", row["ServerName"], row["InstanceName"]);
                        rec.SetString(3, description);

                        session.Log("Adding a new record, its number will be {0} and its value will be {1}", itemNumber, string.Format("{0} ({1})", row["ServerName"], row["InstanceName"]));
                        sqlComboBox.Modify(ViewModifyMode.InsertTemporary, rec);
                        itemNumber++;
                    }
                }
                session["HasScannedForSql"] = "1";
            }
            return ActionResult.Success;
        }
    }
}
