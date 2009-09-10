using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.ServiceModel;
using System.ServiceProcess;
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;

using OMLEngine;

namespace OMLCustomWiXAction {
    public class CustomActions {
        private static StringBuilder prOutput = null;
        private static StringBuilder prError = null;
        private string ScriptsPath = string.Empty;
        private string servername = string.Empty;
        private string sapassword = string.Empty;
        private string instancename = string.Empty;

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
                session.Log("Error starting OMLEngineService: {0}", e.Message);
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
                session.Log("An error occured starting the OMLFW Service: {0}", e.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ValidateDatabase(Session session) {
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
                session.Log("Failed to activate NetTcpPortSharing: {0}", e.Message);
                return ActionResult.Failure;
            }
        }
        #region private methods
        private bool CheckSQLExists(Session session) {
            const string instance = "MSSQL$OML";
            //const string instance = "MSSQLSERVER";

            try {
                // Enumerate all SQL instances on system
                ManagementObjectSearcher getAllSQLInstances =
                    new ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement10",
                    "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 " +
                    " and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')");

                ManagementObjectCollection resultsAll = getAllSQLInstances.Get();

                foreach (ManagementObject service in resultsAll) {
                    session.Log("[PostInstallerWizard] Emumerating SQL Server Instance : " + service.ToString());
                }


                // Enumerate OML Names instance

                ManagementObjectSearcher getOMLInstance =
                   new ManagementObjectSearcher("root\\Microsoft\\SqlServer\\ComputerManagement10",
                   "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 " +
                   " and ServiceName = '" + instance + "'" +
                   " and (PropertyName = 'SKUNAME' or PropertyName = 'SPLEVEL')");

                ManagementObjectCollection resultsOML = getOMLInstance.Get();

                // If nothing is returned, SQL isn't installed.
                if (resultsOML.Count == 0) {
                    session.Log("[PostInstallerWizard] No OML SQL Servers Not Found");
                    return false;
                }

                foreach (ManagementObject service in resultsOML) {
                    session.Log("[PostInstallerWizard] Found OML SQL Server Instance : " + service.ToString());
                }

                return true;
            } catch (ManagementException e) {
                return false;
            }
        }

        private bool RunSQLSetup(Session session) {
            Process pr = new Process();

            if (File.Exists(session.GetSourcePath("\\SQLInstaller\\SQLEXPR_x86_ENU.exe"))) {
                pr.StartInfo.FileName = session.GetSourcePath("\\SQLInstaller\\") + "SQLEXPR_x86_ENU.exe";
                pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + session.GetSourcePath("\\SQLInstaller\\") + "SQLConfigNoTools_x32.ini";
            } else {

                if (File.Exists(session.GetSourcePath("\\SQLInstaller\\") + "SQLEXPR_x64_ENU.exe")) {
                    pr.StartInfo.FileName = session.GetSourcePath("\\SQLInstaller\\") + "SQLEXPR_x64_ENU.exe";
                    pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + session.GetSourcePath("\\SQLInstaller\\SQLConfigNoTools_x64.ini");
                } else {
                    session.Log("Cannot find the SQL installers!", "Error");
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
            else {
                session.Log("SQL installer reported error code " + ExitCode.ToString(), "Error");
                return false;
            }

        }

        private static void NetOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data)) {
                // Add the text to the collected output.
                prOutput.Append(Environment.NewLine + "  " + outLine.Data);
            }
        }

        private static void NetErrorDataHandler(object sendingProcess, DataReceivedEventArgs errLine) {
            // Write the error text to the file if there is something
            // to write and an error file has been specified.

            if (!String.IsNullOrEmpty(errLine.Data)) {
                prError.Append(Environment.NewLine + "  " + errLine.Data);
            }
        }

        private void CheckAndUpgradeSchema(Session session) {
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();

            OMLEngine.DatabaseManagement.DatabaseInformation.SQLState state = dbm.CheckDatabase();

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBNotFound) {
                session.Log("Detected SQL Server but cannot find the database. Click OK to create the database.");
                // OML Instance but OML database does not exist
                dbm.ConfigureSQL(ScriptsPath);
                dbm.UpgradeSchemaVersion(ScriptsPath);

                // Retest the connection
                state = dbm.CheckDatabase();
            }

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired) {
                session.Log("Detected the OML Database but it requires updating. Click OK to update the database.");
                dbm.UpgradeSchemaVersion(ScriptsPath);

                // Retest the connection
                state = dbm.CheckDatabase();
            }

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OK) {
                session.Log("The database appears all fine.");
                return;
            }
        }

        private void ConfigureSQL() {
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
            dbm.ConfigureSQL(ScriptsPath);
        }

        private void WriteSettings() {
            OMLEngine.DatabaseManagement.DatabaseInformation.DatabaseName = "oml";
            OMLEngine.DatabaseManagement.DatabaseInformation.OMLUserAcct = "oml";
            OMLEngine.DatabaseManagement.DatabaseInformation.OMLUserPassword = "oml";
            OMLEngine.DatabaseManagement.DatabaseInformation.SAPassword = sapassword;
            OMLEngine.DatabaseManagement.DatabaseInformation.SQLInstanceName = instancename;
            OMLEngine.DatabaseManagement.DatabaseInformation.SQLServerName = servername;
            OMLEngine.DatabaseManagement.DatabaseInformation.SaveSettings();
        }
        #endregion private methods
    }
}
