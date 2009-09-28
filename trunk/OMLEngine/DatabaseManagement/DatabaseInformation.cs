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
            OMLDBVersionNotFound,
            OMLDBVersionUpgradeRequired,
            OMLDBVersionCodeOlderThanSchema,
            OMLUserNotFound,
            UnknownState
        }

        public static bool ConfigFileExists { get; set; }

        private static string _SQLServerName;
        private static string _SQLInstanceName;
        private static string _SAPassword;
        private static string _OMLUserAcct;
        private static string _OMLUserPassword;
        private static string _DatabaseName;
        
        static DatabaseInformation()
        {
            OMLEngine.Settings.XMLSettingsManager xmlsettings = new OMLEngine.Settings.XMLSettingsManager();
            _SQLServerName = xmlsettings.SQLServerName;
            _SQLInstanceName = xmlsettings.SQLInstanceName;
            _DatabaseName = xmlsettings.DatabaseName;
            _SAPassword = xmlsettings.SAPassword;
            _OMLUserAcct = xmlsettings.OMLUserAcct;
            _OMLUserPassword = xmlsettings.OMLUserPassword;
            ConfigFileExists = xmlsettings.ConfigFileExists;
            OMLEngine.Utilities.DebugLine("[OMLEngine] Does settings.xml exist : " + ConfigFileExists.ToString());

        }

        /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
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
        /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
     
        // Database server name, database name and credentials
        public static string SQLServerName
        {
            get
            {
                return (_SQLServerName ?? "localhost");
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            set
            {
                _SQLServerName = value;
                SaveSettings();
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
        }
        public static string SQLInstanceName
        {
            get
            {
                return (_SQLInstanceName ?? "oml");
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            set
            {
                _SQLInstanceName = value;
                SaveSettings();
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
        }
        public static string SAPassword
        {
            get
            {
                return (_SAPassword ?? "R3WztB4#9");
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            set
            {
                _SAPassword = value;
                SaveSettings();
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
        }
        public static string OMLUserAcct
        {
            get
            {
                return (_OMLUserAcct ?? "oml");
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            set
            {
                _OMLUserAcct = value;
                SaveSettings();
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
        }
        public static string OMLUserPassword
        {
            get
            {
                return (_OMLUserPassword ?? "oml");
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            set
            {
                _OMLUserPassword = value;
                SaveSettings();
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
        }
        public static string DatabaseName 
        { 
            get
            {
                return (_DatabaseName ?? "oml");
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            set
            {
                _DatabaseName = value;
                SaveSettings();
            }
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
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
                    return "Server=" + ServerInstance + ";UID=sa;PWD=" + SAPassword + ";Database=master;Connect Timeout=50;";
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
                    return "Server=" + ServerInstance + ";UID=" + OMLUserAcct + ";PWD=" + OMLUserPassword + ";Database=" + DatabaseName + ";Connect Timeout=50;";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
