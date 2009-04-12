using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace OMLEngine.Settings
{
    public class XMLSettingsManager
    {
        public string SQLServerName { get; set; }
        public string SQLInstanceName { get; set; }
        public string DatabaseName { get; set; }
        public string SAPassword { get; set; }
        public string OMLUserAcct { get; set; }
        public string OMLUserPassword { get; set; }


        public XMLSettingsManager()
        {
            try
            {
                DataSet ds = new DataSet();

                ds.ReadXml(SettingsFile());

                DataRow dr = ds.Tables[0].Rows[0];

                SQLServerName = ExtractField(dr, "SQLServerName");
                SQLInstanceName = ExtractField(dr, "SQLInstanceName");
                DatabaseName = ExtractField(dr, "DatabaseName");
                SAPassword = ExtractField(dr, "SAPassword");
                OMLUserAcct = ExtractField(dr, "OMLUserAcct");
                OMLUserPassword = ExtractField(dr, "OMLUserPassword");

                Utilities.DebugLine("[SettingsManager] : Loaded settings xml");
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[SettingsManager] : Failed to load settings xml {0}", ex.Message);
                SQLServerName = null;
                SQLInstanceName = null;
                DatabaseName = null;
                SAPassword = null;
                OMLUserAcct = null;
                OMLUserPassword = null;

            }
        }


        public string ExtractField(DataRow dr, string field)
        {
            try
            {
                return (string)dr[field];
            }
            catch
            {
                return null;
            }
        }

        public void SaveSettings()
        {
            TextWriter xmlhandle = new StreamWriter(SettingsFile(), false); // FileStream(file, FileMode.Create);

            xmlhandle.WriteLine("<OML>");
            xmlhandle.WriteLine("  <Settings>");

            if (SQLServerName != null ) xmlhandle.WriteLine(string.Format("    <{0}>{1}</{0}>", "SQLServerName", SQLServerName));
            if (SQLInstanceName != null) xmlhandle.WriteLine(string.Format("    <{0}>{1}</{0}>", "SQLInstanceName", SQLInstanceName));
            if (DatabaseName != null) xmlhandle.WriteLine(string.Format("    <{0}>{1}</{0}>", "DatabaseName", DatabaseName));
            if (SAPassword != null) xmlhandle.WriteLine(string.Format("    <{0}>{1}</{0}>", "SAPassword", SAPassword));
            if (OMLUserAcct != null) xmlhandle.WriteLine(string.Format("    <{0}>{1}</{0}>", "OMLUserAcct", OMLUserAcct));
            if (OMLUserPassword != null) xmlhandle.WriteLine(string.Format("    <{0}>{1}</{0}>", "OMLUserPassword", OMLUserPassword));

            xmlhandle.WriteLine("  </Settings>");
            xmlhandle.WriteLine("</OML>");

            xmlhandle.Close();
        }

        public string SettingsFile()
        {
            return Path.Combine(FileSystemWalker.PublicRootDirectory, "settings.xml");
        }
    }
}
