using System;
using Microsoft.Win32;
using System.Data;
using System.Data.SqlClient;

namespace OMLEngine
{
    public class MyMoviesDatabase
    {
        private string _ServerName;
        private SqlConnection _conn;

        public MyMoviesDatabase()
        {
            _ServerName = OMLEngine.Properties.Settings.Default.MyMoviesServerName;
            if (string.IsNullOrEmpty(_ServerName))
            {
                RegistryKey mymoviesKey = Registry.LocalMachine.CreateSubKey(@"Software\My Movies");
                if (mymoviesKey != null)
                {
                    _ServerName = (string)mymoviesKey.GetValue("Server");
                    if (!string.IsNullOrEmpty(_ServerName))
                    {
                        Utilities.DebugLine("Loaded MyMovies server name from registry as: " + _ServerName);
                        OMLEngine.Properties.Settings.Default.MyMoviesServerName = _ServerName;
                        OMLEngine.Properties.Settings.Default.Save();
                    }
                }
                else
                {
                    Utilities.DebugLine("My Movies does NOT appear to be installed on this machine");
                    return;
                }
            }
        }

        public SqlConnection GetDBH()
        {
            if (_conn != null && _conn.State == ConnectionState.Open)
                return _conn;
            else
            {
                string connString = _ConnectionString();
                if (!string.IsNullOrEmpty(connString))
                {
                    _conn = new SqlConnection(connString);
                    _conn.Open();

                    if (_conn.State == ConnectionState.Open)
                        return _conn;
                }
            }
            return null;
        }

        public bool LoadTitlesFromMyMoviesDB(TitleCollection titleCollection)
        {
            return false;
        }

        private bool _LoadTitlesFromMyMoviesDB(TitleCollection titleCollection)
        {
            return false;
        }

        private string _ConnectionString()
        {
            if (!string.IsNullOrEmpty(_ServerName))
            {
                return @"Server=" + _ServerName
                     + @"\MYMOVIES;Database=My Movies;User ID=;Password=;Trusted_Connection=False;";
            }
            else
                return null;
        }
    }
}
