using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{
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

        public static OMLDataDataContext Instance
        {
            get
            {
                if (db == null)
                {
                    lock (lockObject)
                    {
                        if (db == null)
                        {                            
                            db = new OMLDataDataContext();

                            db.DeferredLoadingEnabled = true;

                            System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();                            
                            //loadOptions.LoadWith<Title>(t => t.Disks);
                            loadOptions.LoadWith<Title>(i => i.Images);
                            db.LoadOptions = loadOptions;

                            db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
                        }
                    }
                }

                return db;
            }
        }

        public static OMLDataDataContext InstanceOrNull { get { return db; } }        
    }

    internal class LocalDataContext : IDisposable
    {
        private OMLDataDataContext db = null;

        public OMLDataDataContext Context { get { return db; } }

        public LocalDataContext()
        {
            db = new OMLDataDataContext();
            db.DeferredLoadingEnabled = true;

            System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
            //loadOptions.LoadWith<Title>(t => t.Disks);
            loadOptions.LoadWith<Title>(i => i.Images);
            db.LoadOptions = loadOptions;

            db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
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

        public static OMLDataSettingsDataContext Instance
        {
            get
            {
                if (db == null)
                {
                    lock (lockObject)
                    {
                        if (db == null)
                        {
                            db = new OMLDataSettingsDataContext();
                            db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
                        }
                    }
                }

                return db;
            }
        }

        public static OMLDataSettingsDataContext InstanceOrNull { get { return db; } }
    }
}
