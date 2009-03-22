﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;


namespace OMLEngine.DatabaseManagement
{
    public static class DatabaseInformation
    {
        public enum SQLState
        {
            OK,
            LoginFailure,
            OMLDBNotFound,
            OMLDBFoundInvalid,
            OMLDBFoundNoVersion,
            OMLUserNotFound,
            UnknownState
        }

        private static string _SQLServerName;
        private static string _SQLInstanceName;
        private static string _SAPassword;
        private static string _OMLUserAcct;
        private static string _OMLUserPassword;
        private static string _DatabaseName;
        
        public static void LoadSettings()
        {
            OMLEngine.Settings.XMLSettingsManager xmlsettings = new OMLEngine.Settings.XMLSettingsManager();
            _SQLServerName = xmlsettings.SQLServerName;
            _SQLInstanceName = xmlsettings.SQLInstanceName;
            _DatabaseName = xmlsettings.DatabaseName;
            _SAPassword = xmlsettings.SAPassword;
            _OMLUserAcct = xmlsettings.OMLUserAcct;
            _OMLUserPassword = xmlsettings.OMLUserPassword;
        }

        public static void SaveSettings()
        {
            OMLEngine.Settings.XMLSettingsManager xmlsettings = new OMLEngine.Settings.XMLSettingsManager();
            xmlsettings.SQLServerName = _SQLServerName;
            xmlsettings.SQLInstanceName = _SQLInstanceName;
            xmlsettings.DatabaseName = _DatabaseName;
            xmlsettings.SAPassword = _SAPassword;
            xmlsettings.OMLUserAcct = _OMLUserAcct;
            xmlsettings.OMLUserPassword = _OMLUserPassword;
            xmlsettings.SaveSettings();
        }
     
        // Database server name, database name and credentials
        public static string SQLServerName
        {
            get
            {
                return (_SQLServerName ?? "localhost");
            }
            set
            {
                _SQLServerName = value;
                SaveSettings();
            }
        }
        public static string SQLInstanceName
        {
            get
            {
                return (_SQLInstanceName ?? "oml");
            }
            set
            {
                _SQLInstanceName = value;
                SaveSettings();
            }
        }
        public static string SAPassword
        {
            get
            {
                return (_SAPassword ?? "omladmin");
            }
            set
            {
                _SAPassword = value;
                SaveSettings();
            }
        }
        public static string OMLUserAcct
        {
            get
            {
                return (_OMLUserAcct ?? "oml");
            }
            set
            {
                _OMLUserAcct = value;
                SaveSettings();
            }
        }
        public static string OMLUserPassword
        {
            get
            {
                return (_OMLUserPassword ?? "oml");
            }
            set
            {
                _OMLUserPassword = value;
                SaveSettings();
            }
        }
        public static string DatabaseName 
        { 
            get
            {
                return (_DatabaseName ?? "oml");
            }
            set
            {
                _DatabaseName = value;
                SaveSettings();
            }
        }

        // Database Error & schema version
        public static string LastSQLError { get; set; }
        public static int SchemaVersion { get; set; }

        // Connection strings
        private static string ServerInstance
        {
            get
            {
                if (string.IsNullOrEmpty(SQLInstanceName))
                {
                    return SQLServerName;
                }
                else
                {
                    return SQLServerName + @"\" + SQLInstanceName;
                }
            }
        }

        public static string MasterDatabaseConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(SQLServerName))
                {
                    return "Server=" + ServerInstance + ";UID=sa;PWD=" + SAPassword + ";Database=master;Connect Timeout=5;";
                }
                else
                {
                    return "";
                }
            }
        }

        public static string OMLDatabaseConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(SQLServerName))
                {
                    return "Server=" + ServerInstance + ";UID=" + OMLUserAcct + ";PWD=" + OMLUserPassword + ";Database=" + DatabaseName + ";Connect Timeout=5;";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}