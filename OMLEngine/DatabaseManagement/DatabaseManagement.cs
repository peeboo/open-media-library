﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

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
            /*DatabaseInformation.SchemaVersion = 0;
            string sql = "select version from version";
            if (!ExecuteReader(sql, out reader))
            {
                CloseDatabase();
                return DatabaseInformation.SQLState.UnknownState;
            }

            while (reader.Read())
            {
                DatabaseInformation.SchemaVersion = Convert.ToInt32(reader[0]);
            }

            reader.Close();*/

            sqlConn.Close();
            sqlConn.Dispose();


            // Schema versioning disabled for now
            //if (DatabaseInformation.SchemaVersion == 0) return DatabaseInformation.SQLState.OMLDBFoundNoVersion;
  
            return DatabaseInformation.SQLState.OK;
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


        public bool BackupDatabase(string path)
        {
            bool retval = false;

            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                // Launch backup job
                string sql = "BACKUP DATABASE [" + DatabaseInformation.DatabaseName + "] TO DISK = '" + path + "' WITH INIT";
                if (ExecuteNonQuery(sqlConn, sql))
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


        public void RunSQLScript(string filname)
        { 
            // Create database connection to OML and open it
            SqlConnection sqlConn = OpenDatabase(DatabaseInformation.MasterDatabaseConnectionString);

            if (sqlConn != null)
            {
                string sql = System.IO.File.ReadAllText(filname);

                string[] commands = sql.Split(
                    new string[] { "GO\r\n", "GO ", "GO\t" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in commands)
                {
                    ExecuteNonQuery(sqlConn, c);

                }
            }
            sqlConn.Close();
        }


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
            SqlCommand sqlComm = new SqlCommand(query, sqlConn);
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
    }
}



