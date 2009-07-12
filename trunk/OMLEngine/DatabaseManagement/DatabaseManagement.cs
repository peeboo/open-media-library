using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace OMLEngine.DatabaseManagement
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
                (dbstatus == DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired) ||
                (dbstatus == DatabaseInformation.SQLState.OMLDBVersionNotFound))
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
            SqlDataReader reader;

            // Test 1. Create database connection to OML and open it
            // -----------------------------------------------------
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.OMLDatabaseConnectionString);

            if (sqlConn == null)
            {
                return DatabaseInformation.SQLState.LoginFailure;
            }


            // Test 2. Get the schema version number for later comparison
            // ----------------------------------------------------------
            // Schema versioning disabled for now
            int ReqMajor;
            int ReqMinor;
            int CurrentMajor;
            int CurrentMinor;

            DatabaseInformation.SQLState sqlstate = DatabaseInformation.SQLState.UnknownState;

            GetRequiredSchemaVersion(out ReqMajor, out ReqMinor);

            if (GetSchemaVersion(out CurrentMajor, out CurrentMinor))
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
            }


            sqlConn.Close();
            sqlConn.Dispose();

            return sqlstate;
        }


        /// <summary>
        /// A problem has been found, performs a series of tests on the master database to identify problem
        /// </summary>
        /// <returns></returns>
        private DatabaseInformation.SQLState DatabaseDiagnostics()
        {
            object data;

            // Test 1. Open database connection to Master and open it
            // ------------------------------------------------------
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn == null)
            {  
                return DatabaseInformation.SQLState.LoginFailure;
            }

            // Test 2. Check if the database exists on the server
            string sql = "select count(*) from sysdatabases where name = '" + DatabaseInformation.DatabaseName + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) != 1)
            {
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.OMLDBNotFound;
            }


            // Test 3. Check user account exists
            // ---------------------------------
            sql = "select count(*) from syslogins where name = '" + DatabaseInformation.OMLUserAcct.ToLower() + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) != 1)
            {
                // Try to create user
                CreateOMLUser(sqlConn);
                return DatabaseInformation.SQLState.OMLUserNotFound;
            }


            // Test 4. Check user account exists in oml database
            // -------------------------------------------------
            sqlConn.ChangeDatabase(DatabaseInformation.DatabaseName);

            sql = "select count(*) from sysusers where name = '" + DatabaseInformation.OMLUserAcct.ToLower() + "'";
            if (!ExecuteScalar(sqlConn, sql, out data))
            {
                sqlConn.Close();
                sqlConn.Dispose();
                return DatabaseInformation.SQLState.UnknownState;
            }

            if (Convert.ToInt32(data) != 1)
            {
                // Try to create user 
                CreateOMLUser(sqlConn);
                return DatabaseInformation.SQLState.OMLUserNotFound;
            }

            sqlConn.Close();
            sqlConn.Dispose();
            return DatabaseInformation.SQLState.OK;
        }


        #region Database Maintenance
        public bool BackupDatabase(string path)
        {
            bool retval = false;

            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                // Launch backup job
                string sql = "BACKUP DATABASE [" + DatabaseInformation.DatabaseName + "] TO DISK = '" + path + "' WITH INIT";
                if (ExecuteNonQuery(sqlConn, sql, 1200))
                {
                    retval = true;
                }
            }
            sqlConn.Close();
            return retval;
        }


        public bool OptimiseDatabase()
        {
            bool retval = true;

            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.OMLDatabaseConnectionString);
            if (sqlConn != null)
            {
                // Get a list of tables in database
                string sql = "select name from sysobjects where category = 0";
                SqlDataReader reader;

                if (!ExecuteReader(sqlConn, sql, out reader))
                {
                    return false;
                }

                while (reader.Read())
                {
                    // Launch reindex job
                    sql = "DBCC DBREINDEX (" + reader[0] + ", '', 70)";
                    if (!ExecuteNonQuery(sqlConn, sql))
                    {
                        reader.Close();
                        sqlConn.Close();
                        sqlConn.Dispose();
                        return false;
                    }
                }
                reader.Close();
            }
            sqlConn.Close();
            sqlConn.Dispose();
            return retval;
        }
        #endregion


        #region User and Database creation functions
        /// <summary>
        /// Creates the OML user. This should allready have been done as part
        /// of the installer.
        /// </summary>
        private void CreateOMLUser(SqlConnection sqlConn)
        {
            ExecuteNonQuery(sqlConn, "CREATE LOGIN [" + DatabaseInformation.OMLUserAcct + "] " +
                    "WITH PASSWORD=N'" + DatabaseInformation.OMLUserPassword + "', " +
                    " DEFAULT_DATABASE=[" + DatabaseInformation.DatabaseName + "], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF");
                
            sqlConn.ChangeDatabase(DatabaseInformation.DatabaseName);
            ExecuteNonQuery(sqlConn, "DROP USER [" + DatabaseInformation.OMLUserAcct + "]");
            ExecuteNonQuery(sqlConn, "DROP ROLE [" + DatabaseInformation.OMLUserAcct + "]");
            ExecuteNonQuery(sqlConn, "CREATE USER [" + DatabaseInformation.OMLUserAcct + "] FOR LOGIN [" + DatabaseInformation.OMLUserAcct + "] WITH DEFAULT_SCHEMA=[dbo]");
            ExecuteNonQuery(sqlConn, "EXEC sp_addrolemember [db_owner], [" + DatabaseInformation.OMLUserAcct + "]");
            sqlConn.ChangeDatabase("master");
        }


        public void CreateOMLUser()
        {
            bool retval = false;

            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                CreateOMLUser(sqlConn);
            }
            sqlConn.Close();
            //return retval;
        }


        public bool CreateOMLDatabase()
        {
            bool retval = false;

            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                // Launch backup job
                string sql = @"CREATE DATABASE [OML] ON  PRIMARY " +
                    @"( NAME = N'OML', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) " +
                    @" LOG ON  " +
                    @"( NAME = N'OML_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.OML\MSSQL\DATA\OML_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) ";

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
        #endregion


        #region Schema Versioning
        public void GetRequiredSchemaVersion(out int Major, out int Minor)
        {
            Major = 1;
            Minor = 1;
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
                GetSchemaVersion(sqlConn, out Major, out Minor);
            }
            return true;
        }


        public bool UpgradeSchemaVersion(string ScriptsPath)
        { 
            int ReqMajor;
            int ReqMinor;
            int CurrentMajor;
            int CurrentMinor;

            DatabaseInformation.SQLState sqlstate = DatabaseInformation.SQLState.UnknownState;

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
                }
                sqlConn.Close();
            }
        }
        #endregion
    }
}



