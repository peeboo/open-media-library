﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OMLCustomWiXAction
{
    public class DatabaseManagement
    {
        /// <summary>
        /// Tests database connectivity
        /// </summary>
        /// <returns></returns>
        public DatabaseInformation.SQLState CheckDatabase()
        {
            DatabaseInformation.SQLState dbstatus = CheckOMLDatabase();

            if ((dbstatus == DatabaseInformation.SQLState.OMLDBVersionCodeOlderThanSchema) ||
                (dbstatus == DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired)) // ||
                //(dbstatus == DatabaseInformation.SQLState.OMLDBVersionNotFound))
            {
                // Return to the ui / editor to deal with
                return dbstatus;
            }
           
            if (dbstatus != DatabaseInformation.SQLState.OK)
            {
                // There has been a problem logging in to the OML Database. Investigate
                dbstatus = DatabaseDiagnostics();
            }

            if ((dbstatus == DatabaseInformation.SQLState.LoginFailure) || (dbstatus == DatabaseInformation.SQLState.OMLUserNotFound))
            {
                DatabaseInformation.LastSQLError = "A logon error has occured. An attempt to fix this issue has been made. Please close the program and try again.";
            }
            return dbstatus;
        }

        /// <summary>
        /// Performs a series of tests on the OML database to ensure there are no problems
        /// </summary>
        /// <returns></returns>
        private DatabaseInformation.SQLState CheckOMLDatabase()
        {
            CustomActions.LogToSession("[DatabaseManagement] : Entering CheckOMLDatabase()");

            // Test 1. Create database connection to OML and open it
            // -----------------------------------------------------
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.OMLDatabaseConnectionString);

            if (sqlConn == null)
            {
                CustomActions.LogToSession("[DatabaseManagement / CheckOMLDatabase()] : Failed to login using - " + DatabaseInformation.OMLDatabaseConnectionString);
                return DatabaseInformation.SQLState.LoginFailure;
            }

            // Test 2. Ensure the database is in multiuser mode - could be a result of a bad restore
            // -------------------------------------------------------------------------------------
            string sql = "ALTER DATABASE [" + DatabaseInformation.xmlSettings.DatabaseName + "] SET MULTI_USER";
            if (!ExecuteNonQuery(sqlConn, sql))
            {
                CustomActions.LogToSession("[DatabaseManagement / CheckOMLDatabase()] : Attempting to set the database to Multi user failed");
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            // Test 3. Get the schema version number for later comparison
            // ----------------------------------------------------------
            // Schema versioning disabled for now
            int ReqMajor;
            int ReqMinor;
            int CurrentMajor;
            int CurrentMinor;

            DatabaseInformation.SQLState sqlstate = DatabaseInformation.SQLState.UnknownState;

            GetRequiredSchemaVersion(out ReqMajor, out ReqMinor);

            if (GetSchemaVersion(sqlConn, out CurrentMajor, out CurrentMinor))
            {
                if (ReqMajor > CurrentMajor) sqlstate = DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired;
                if (ReqMajor < CurrentMajor) sqlstate = DatabaseInformation.SQLState.OMLDBVersionCodeOlderThanSchema;

                if (ReqMajor == CurrentMajor)
                {
                    if (ReqMinor > CurrentMinor) sqlstate = DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired;
                    if (ReqMinor < CurrentMinor) sqlstate = DatabaseInformation.SQLState.OMLDBVersionCodeOlderThanSchema;
                    if (ReqMinor == CurrentMinor) sqlstate = DatabaseInformation.SQLState.OK;
                }
            }
            else
            {
                // Cannot find schema version
                sqlstate = DatabaseInformation.SQLState.OMLDBVersionNotFound;
                CustomActions.LogToSession("[DatabaseManagement / CheckOMLDatabase()] : Unable to finr the schema version.");

            }


            sqlConn.Close();
            sqlConn.Dispose();

            CustomActions.LogToSession("[DatabaseManagement] : Leaving CheckOMLDatabase()");

            return sqlstate;
        }


        /// <summary>
        /// A problem has been found, performs a series of tests on the master database to identify problem
        /// </summary>
        /// <returns></returns>
        private DatabaseInformation.SQLState DatabaseDiagnostics()
        {
            CustomActions.LogToSession("[DatabaseManagement] : Entering DatabaseDiagnostics()");

            object data;

            // Test 1. Open database connection to Master and open it
            // ------------------------------------------------------
            CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Checking the master database");
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);
            if (sqlConn == null)
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Failed to login using - " + DatabaseInformation.MasterDatabaseConnectionString);
                return DatabaseInformation.SQLState.LoginFailure;
            }

            // Test 2. Check if the database exists on the server
            CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Checking the oml sysdatabase entry");
            string sql = "select count(*) from sysdatabases where name = '" + DatabaseInformation.xmlSettings.DatabaseName + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : sysdatabases check failed");
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) != 1)
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Could not find the database - " + DatabaseInformation.xmlSettings.DatabaseName);
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.OMLDBNotFound;
            }


            // Test 3. Check user account exists
            // ---------------------------------
            CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Checking the syslogins entry");
            sql = "select count(*) from syslogins where name = '" + DatabaseInformation.xmlSettings.OMLUserAcct.ToLower() + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : syslogins check failed");
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) != 1)
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Could not find the oml login in master database");
                // Try to create user
                CreateOMLUser(sqlConn);
                return DatabaseInformation.SQLState.OMLUserNotFound;
            }

            // Test 4. Check user account exists in oml database
            // -------------------------------------------------
            sqlConn.ChangeDatabase(DatabaseInformation.xmlSettings.DatabaseName);
            CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Checking the oml.sysusers entry");
            sql = "select count(*) from sysusers where name = '" + DatabaseInformation.xmlSettings.OMLUserAcct.ToLower() + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : oml.sysusers check failed");

                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) != 1)
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Could not find the oml user in the user database");
                // Try to create user 
                CreateOMLUser(sqlConn);
                return DatabaseInformation.SQLState.OMLUserNotFound;
            }

            // Test 5. Check for matching sid on the master.syslogins amd oml.sysusers entries
            // --------------------------------------------------------------------------------
            sqlConn.ChangeDatabase(DatabaseInformation.xmlSettings.DatabaseName);
            CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Checking the sid fields on master.sys oml.sysusers");
            sql = "select count(*) from master.sys.syslogins sl " + 
                "inner join oml.sys.sysusers su " +
                "on sl.sid = su.sid " +
                "where sl.name = '" + DatabaseInformation.xmlSettings.OMLUserAcct.ToLower() + "' " +
                "and  su.name = '" + DatabaseInformation.xmlSettings.OMLUserAcct.ToLower() + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : sid fields on master.sys oml.sysusers check failed");

                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) < 1)
            {
                CustomActions.LogToSession("[DatabaseManagement / DatabaseDiagnostics()] : Mismatch on the SID entries. Attempting to recreate accounts");
                // Try to create user 
                CreateOMLUser(sqlConn);
                return DatabaseInformation.SQLState.OMLUserNotFound;
            }


            sqlConn.Close();
            sqlConn.Dispose();

            CustomActions.LogToSession("[DatabaseManagement] : Leaving DatabaseDiagnostics()");

            return DatabaseInformation.SQLState.OK;
        }

        #region User and Database creation functions
        /// <summary>
        /// Creates the OML user. This should allready have been done as part
        /// of the installer.
        /// </summary>
        private void CreateOMLUser(SqlConnection sqlConn)
        {
            CustomActions.LogToSession("[DatabaseManagement] : Entering CreateOMLUser()");

            ExecuteNonQuery(sqlConn, "CREATE LOGIN [" + DatabaseInformation.xmlSettings.OMLUserAcct + "] " +
                    "WITH PASSWORD=N'" + DatabaseInformation.xmlSettings.OMLUserPassword + "', " +
                    " DEFAULT_DATABASE=[" + DatabaseInformation.xmlSettings.DatabaseName + "], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF");

            sqlConn.ChangeDatabase(DatabaseInformation.xmlSettings.DatabaseName);
            ExecuteNonQuery(sqlConn, "DROP USER [" + DatabaseInformation.xmlSettings.OMLUserAcct + "]");
            ExecuteNonQuery(sqlConn, "DROP ROLE [" + DatabaseInformation.xmlSettings.OMLUserAcct + "]");
            ExecuteNonQuery(sqlConn, "CREATE USER [" + DatabaseInformation.xmlSettings.OMLUserAcct + "] FOR LOGIN [" + DatabaseInformation.xmlSettings.OMLUserAcct + "] WITH DEFAULT_SCHEMA=[dbo]");
            ExecuteNonQuery(sqlConn, "EXEC sp_addrolemember [db_owner], [" + DatabaseInformation.xmlSettings.OMLUserAcct + "]");
            sqlConn.ChangeDatabase("master");

            CustomActions.LogToSession("[DatabaseManagement] : Leaving CreateOMLUser()");

        }


        public void CreateOMLUser()
        {
            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                CreateOMLUser(sqlConn);
            }
            sqlConn.Close();
        }


        public bool CreateOMLDatabase()
        {
            bool retval = false;

            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                // Launch backup job
                //string sql = @"CREATE DATABASE [OML] ON  PRIMARY " +
                //    @"( NAME = N'OML', FILENAME = N'" + Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) " +
                //    @" LOG ON  " +
                //    @"( NAME = N'OML_log', FILENAME = N'" + Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) ";
                ExecuteNonQuery(sqlConn, "EXEC sp_configure filestream_access_level, 2");
                ExecuteNonQuery(sqlConn, "RECONFIGURE");

                string sql = @"CREATE DATABASE [OML] ON  PRIMARY " +
                    @"( NAME = N'OML', FILENAME = N'" + Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) " +
                    @"FILEGROUP [OMLFILEGROUP] CONTAINS FILESTREAM  DEFAULT ( NAME = N'OMLFS_Group', FILENAME = N'" + Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML_FSData' ) " +
                    @" LOG ON  " +
                    @"( NAME = N'OML_log', FILENAME = N'" + Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) ";

                if (ExecuteNonQuery(sqlConn, sql))
                {
                    retval = true;

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET COMPATIBILITY_LEVEL = 100");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET ANSI_NULL_DEFAULT OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET ANSI_NULLS OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET ANSI_PADDING OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET ANSI_WARNINGS OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET AUTO_CLOSE OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET AUTO_CREATE_STATISTICS ON ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET AUTO_SHRINK OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET AUTO_UPDATE_STATISTICS ON ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET CURSOR_CLOSE_ON_COMMIT OFF ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET RECOVERY SIMPLE ");

                    ExecuteNonQuery(sqlConn, "ALTER DATABASE [OML] SET  MULTI_USER ");

                }
                sqlConn.Close();
            }

            return retval;
        }


        public void ConfigureSQL(string ScriptsPath)
        {
            CreateOMLDatabase();

            // Run sql configuration script
            RunSQLScript(ScriptsPath + "\\SQLConfigurationScript.sql");

            // Run schema creation script
            RunSQLScript(ScriptsPath + "\\Title Database.sql");
            CreateOMLUser();
        }

        public void CreateSchema()
        {      
            // Run schema creation script
            RunSQLScript(GetScriptsPath + "\\Title Database.sql");
        }

        public string GetScriptsPath
        {
            get
            {
                string location = System.Reflection.Assembly.GetEntryAssembly().Location;

                // Find the script path. Also include hack to find scripts if running from VS rather than c:\program files....
                string ScriptsPath = Path.GetDirectoryName(location) + "\\SQLInstaller";
                if (Directory.Exists(Path.GetDirectoryName(location) + "\\..\\..\\..\\SQL Scripts"))
                {
                    ScriptsPath = Path.GetDirectoryName(location) + "\\..\\..\\..\\SQL Scripts";
                }
                return ScriptsPath;
            }
        }
        #endregion


        #region Schema Versioning
        public void GetRequiredSchemaVersion(out int Major, out int Minor)
        {
            Major = 1;
            Minor = 5;
        }


        public bool GetSchemaVersion(SqlConnection sqlConn, out int Major, out int Minor)
        {
            Major = 0;
            Minor = 0;
            object version;
            if (sqlConn != null)
            {
                if (ExecuteScalar(sqlConn, "select dbo.GetSchemaVersion() as VersionString", out version))
                {
                    string strversion = (string)version;
                    string[] versions = strversion.Split('.');
                    if (versions.Count() == 2)
                    {
                        Major = Convert.ToInt32(versions[0]);
                        Minor = Convert.ToInt32(versions[1]);
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        public bool GetSchemaVersion(out int Major, out int Minor)
        {
            Major = 0;
            Minor = 0;
            
            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.OMLDatabaseConnectionString);

            if (sqlConn != null)
            {
                return GetSchemaVersion(sqlConn, out Major, out Minor);
            }
            return false;
        }


        public bool UpgradeSchemaVersion(string ScriptsPath)
        { 
            int ReqMajor;
            int ReqMinor;
            int CurrentMajor;
            int CurrentMinor;

            GetRequiredSchemaVersion(out ReqMajor, out ReqMinor);

            if (!GetSchemaVersion(out CurrentMajor, out CurrentMinor))
            {
                // No version in db - abort
                return false;
            }


            for (int i = CurrentMajor; i <= ReqMajor; i++)
            {
                for (int j = CurrentMinor; j <= ReqMinor; j++)
                {
                    if ((i != CurrentMajor) || (j != CurrentMinor))
                    {
                        RunSQLScript(ScriptsPath + "\\Title Database - Upgrade " + i.ToString() + "." + j.ToString() + ".sql");
                    }
                }
            }

            // Backup the database

            return true;
        }
        #endregion


        #region Open database code and sql Execution functions
        private SqlConnection OpenDatabase(string connectionstring)
        {
            SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(connectionstring);
            try
            {
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                // Unable to open database connection
                sqlConn.Dispose();
                sqlConn = null;
                DatabaseInformation.LastSQLError = ex.Message;
                return null;
            }
            return sqlConn;
        }

        private bool ExecuteNonQuery(SqlConnection sqlConn, string query)
        {
            return ExecuteNonQuery(sqlConn, query, 50);
        }

        private bool ExecuteNonQuery(SqlConnection sqlConn, string query, int timeout)
        {
            SqlCommand sqlComm = new SqlCommand(query, sqlConn);

            sqlComm.CommandTimeout = timeout;

            try
            {
                sqlComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                DatabaseInformation.LastSQLError = ex.Message;
                return false;
            }
            return true;
        }


        private bool ExecuteScalar(SqlConnection sqlConn, string query, out object data)
        {
            data = null;
            SqlCommand sqlComm = new SqlCommand(query, sqlConn);
            try
            {
                data = sqlComm.ExecuteScalar();
            }
            catch (Exception ex)
            {
                CustomActions.LogToSession("[DatabaseManagement / ExecuteScalar()] : Executing SQL -" + query);
                CustomActions.LogToSession("                      Returned error - " + ex.Message);
                CustomActions.LogToSession("                      Connection string - " + sqlConn.ConnectionString);

                DatabaseInformation.LastSQLError = ex.Message;
                return false;
            }
            return true;
        }


        private bool ExecuteReader(SqlConnection sqlConn, string query, out SqlDataReader reader)
        {
            reader = null;
            SqlCommand sqlComm = new SqlCommand(query, sqlConn);
            try
            {
                reader = sqlComm.ExecuteReader();
            }
            catch (Exception ex)
            {
                DatabaseInformation.LastSQLError = ex.Message;
                return false;
            }
            return true;
        }


        public void RunSQLScript(string filename)
        {
            if (File.Exists(filename))
            {
                // Create database connection to OML and open it
                SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

                if (sqlConn != null)
                {
                    string sql = System.IO.File.ReadAllText(filename);

                    string[] commands = sql.Split(
                        new string[] { "GO\r\n", "GO ", "GO\t" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string c in commands)
                    {
                        ExecuteNonQuery(sqlConn, c);

                    }
                    sqlConn.Close();
                }
                else
                {
                    DatabaseInformation.LastSQLError = "A logon error has occured. Please verify your login information in your settings.xml file.";
                }
            }
        }
        #endregion
    }
}



