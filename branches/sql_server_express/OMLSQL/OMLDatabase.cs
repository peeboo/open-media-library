using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace OMLSQL
{
    public class OMLDatabase
    {
        public static bool CreateDatabase()
        {
            try
            {
                SqlConnection conn = new SqlConnection("Data Source=localhost\\OML;Integrated Security=True;Asynchronous Processing=True");
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"CREATE DATABASE OpenMediaLibrary";
                cmd.ExecuteNonQuery();
                conn.ChangeDatabase(@"OpenMediaLibrary");

                conn.Close();
            }
            catch (Exception e)
            {
                // an error occured creating the database
            }

            return true;
        }
    }
}
