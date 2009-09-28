using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;

namespace OMLCustomWiXAction {
    public class SqlUtils {
        private static StringBuilder prOutput = null;
        private static StringBuilder prError = null;
        private static string ScriptsPath = string.Empty;
        private static string sapassword = string.Empty;
        private static string instancename = string.Empty;
        private static string servername = string.Empty;

        public static bool CheckSqlExists(Session session) {
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
                    //OMLEngine.Utilities.DebugLine("[PostInstallerWizard] Emumerating SQL Server Instance : " + service.ToString());
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
                    //OMLEngine.Utilities.DebugLine("[PostInstallerWizard] No OML SQL Servers Not Found");
                    return false;
                }

                foreach (ManagementObject service in resultsOML) {
                    //OMLEngine.Utilities.DebugLine("[PostInstallerWizard] Found OML SQL Server Instance : " + service.ToString());
                }

                return true;
            } catch (ManagementException) {
                return false;
            }
        }

        public static bool RunSqlSetup(Session session) {
            string ExecutablePath = "";
            Process pr = new Process();

            if (File.Exists(Path.GetDirectoryName(ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x86_ENU.exe")) {
                pr.StartInfo.FileName = Path.GetDirectoryName(ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x86_ENU.exe";
                pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + Path.GetDirectoryName(ExecutablePath) + "\\SQLInstaller\\SQLConfigNoTools_x32.ini\"";
            } else {

                if (File.Exists(Path.GetDirectoryName(ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x64_ENU.exe")) {
                    pr.StartInfo.FileName = Path.GetDirectoryName(ExecutablePath) + "\\SQLInstaller\\SQLEXPR_x64_ENU.exe";
                    pr.StartInfo.Arguments = "/CONFIGURATIONFILE=\"" + Path.GetDirectoryName(ExecutablePath) + "\\SQLInstaller\\SQLConfigNoTools_x64.ini\"";
                } else {
                    //MessageBox.Show("Cannot find the SQL installers!", "Error");
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
                //MessageBox.Show("SQL installer reported error code " + ExitCode.ToString(), "Error");
                return false;
            }
        }

        private static void NetOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            if (!string.IsNullOrEmpty(outLine.Data))
                prOutput.Append(Environment.NewLine + " " + outLine.Data);
        }

        private static void NetErrorDataHandler(object sendingProcess, DataReceivedEventArgs errLine) {
            if (!String.IsNullOrEmpty(errLine.Data))
                prError.Append(Environment.NewLine + "  " + errLine.Data);
        }

        public static void CheckAndUpgradeSchema(Session session) {
            DatabaseManagement dbm = new DatabaseManagement();

            DatabaseInformation.SQLState state = dbm.CheckDatabase();

            if (state == DatabaseInformation.SQLState.OMLDBNotFound) {
                //MessageBox.Show("Detected SQL Server but cannot find the database. Click OK to create the database.", "Databse problem", MessageBoxButtons.OK);
                // OML Instance but OML database does not exist
                dbm.ConfigureSQL(ScriptsPath);
                dbm.UpgradeSchemaVersion(ScriptsPath);

                // Retest the connection
                state = dbm.CheckDatabase();
            }

            if (state == DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired) {
                //MessageBox.Show("Detected the OML Database but it requires updating. Click OK to update the database.", "Databse problem", MessageBoxButtons.OK);
                dbm.UpgradeSchemaVersion(ScriptsPath);

                // Retest the connection
                state = dbm.CheckDatabase();
            }

            if (state == DatabaseInformation.SQLState.OK) {
                //MessageBox.Show("The database appears all fine.", "Database status", MessageBoxButtons.OK);
                return;
            }
        }

        public static void ConfigureSQL(Session session) {
            DatabaseManagement dbm = new DatabaseManagement();
            dbm.ConfigureSQL(ScriptsPath);
        }

        public static void WriteSettings(Session session) {
            DatabaseInformation.xmlSettings.DatabaseName = "oml";
            DatabaseInformation.xmlSettings.OMLUserAcct = "oml";
            DatabaseInformation.xmlSettings.OMLUserPassword = "oml";
            DatabaseInformation.xmlSettings.SAPassword = sapassword;
            DatabaseInformation.xmlSettings.SQLInstanceName = instancename;
            DatabaseInformation.xmlSettings.SQLServerName = servername;
            DatabaseInformation.xmlSettings.SaveSettings();
        }
    }
}
