using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace OMLEngine.Dao
{
    internal static class OMLDBContextLogger {
        private static FileStream Log;
        private static TextWriter writer;

        public static TextWriter Logger() {
            if (Log == null || writer == null) {
                try {
                    string file = Path.Combine(OMLEngine.FileSystemWalker.LogDirectory, "dbaccess-debug.txt");
                    if (Directory.Exists(OMLEngine.FileSystemWalker.LogDirectory) == false)
                        Directory.CreateDirectory(OMLEngine.FileSystemWalker.LogDirectory);

                    bool tooLarge = (File.Exists(file) && (new FileInfo(file)).Length > 1000000);

                    Log = new FileStream(file, File.Exists(file) && !tooLarge ? FileMode.Append : FileMode.Create);
                    writer = new StreamWriter(Log);
                } catch (Exception e) {
                    Utilities.DebugLine("Error creating db logfile: {0}", e.Message);
                    return Console.Out;
                }
            }
            return writer;
        }
    }
    /// <summary>
    /// If you noticed issues with ExecuteReader complaining it is closed, opening or otherwise barfing
    /// you will find yourself here!
    /// 
    /// Your issue is with hitting the context from multiple threads. It isn't thread safe.
    /// You can lock, you can double-check but it will do you no good-
    /// 
    /// Follow the link - http://stackoverflow.com/questions/441293/linqtosql-and-the-exception-executereader-requires-an-open-and-available-connec
    /// Your only choice is to refactor your code or create a new context for your call (which would suck the perf out of it).
    /// 
    /// Wait for PLINQ?
    /// </summary>
    internal static class DBContext
    {
        private static OMLDataDataContext db = null;
        private static object lockObject = new object();

        public static OMLDataDataContext Instance {
            get {
                lock (lockObject) {
                    if (db == null) {
                        db = new OMLDataDataContext();

                        db.DeferredLoadingEnabled = true;

                        System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                        //loadOptions.LoadWith<Title>(t => t.Disks;)
                        loadOptions.LoadWith<Title>(i => i.Images);
                        db.LoadOptions = loadOptions;
                        db.Log = OMLDBContextLogger.Logger();
                        db.ObjectTrackingEnabled = false;

                        db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
                    }
                    return db;
                }
            }
        }
        public static OMLDataDataContext InstanceOrNull { get { return db; } }        
    }

    internal class LocalDataContext : IDisposable
    {
        private OMLDataDataContext db = null;
        private static object lockObject = new object();
        public OMLDataDataContext Context { get { return db; } }

        public LocalDataContext()
        {
            lock (lockObject) {
                db = new OMLDataDataContext();
                db.DeferredLoadingEnabled = true;

                System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
                //loadOptions.LoadWith<Title>(t => t.Disks);
                loadOptions.LoadWith<Title>(i => i.Images);
                db.LoadOptions = loadOptions;
                db.Log = OMLDBContextLogger.Logger();
                db.ObjectTrackingEnabled = false;

                db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
            }
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }                
    }    

    internal static class OMLDataSettingsDBContext
    {
        private static OMLDataSettingsDataContext db = null;
        private static object lockObject = new object();

        public static OMLDataSettingsDataContext Instance {
            get {
                lock (lockObject) {
                    if (db == null) {
                        db = new OMLDataSettingsDataContext();
                        db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
                        db.Log = OMLDBContextLogger.Logger();
                        //db.ObjectTrackingEnabled = false; leave this active for the settings context
                    }
                    return db;
                }
            }
        }
        public static OMLDataSettingsDataContext InstanceOrNull { get { return db; } }
    }
}
