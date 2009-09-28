using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;

namespace OMLCustomWiXAction
{
    public static class DatabaseInformation
    {
        public static XMLSettingsManager xmlSettings = new XMLSettingsManager();
        public enum SQLState
        {
            OK,
            LoginFailure,
            OMLDBNotFound,
            OMLDBFoundInvalid,
            OMLDBVersionNotFound,
            OMLDBVersionUpgradeRequired,
            OMLDBVersionCodeOlderThanSchema,
            OMLUserNotFound,
            UnknownState
        }

        public static bool ConfigFileExists { get; set; }

        // Database Error & schema version
        public static string LastSQLError { get; set; }
        public static int SchemaVersion { get; set; }

        // Connection strings
        private static string ServerInstance
        {
            get
            {
                if (string.IsNullOrEmpty(xmlSettings.SQLInstanceName))
                {
                    return xmlSettings.SQLInstanceName;
                }
                else
                {
                    return xmlSettings.SQLServerName + @"\" + xmlSettings.SQLInstanceName;
                }
            }
        }

        public static string MasterDatabaseConnectionString
        {
            get
            {
                return !string.IsNullOrEmpty(xmlSettings.SQLServerName)
                    ? string.Empty
                    : "Server=" + xmlSettings.SQLInstanceName + ";UID=sa;PWD=" + xmlSettings.SAPassword + ";Database=master;Connect Timeout=50;";
            }
        }

        public static string OMLDatabaseConnectionString
        {
            get
            {
                return string.IsNullOrEmpty(xmlSettings.SQLServerName)
                    ? string.Empty
                    : "Server=" + ServerInstance + ";UID=" + xmlSettings.OMLUserAcct + ";PWD=" + xmlSettings.OMLUserPassword + ";Database=" + xmlSettings.DatabaseName + ";Connect Timeout=50;";
            }
        }
    }
}
