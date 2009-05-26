using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{
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

    internal static class WatcherDataContext
    {
        private static WatcherDataDataContext db = null;
        private static object lockObject = new object();

        public static WatcherDataDataContext Instance
        {
            get
            {
                if (db == null)
                {
                    lock (lockObject)
                    {
                        if (db == null)
                        {
                            db = new WatcherDataDataContext();
                            db.Connection.ConnectionString = OMLEngine.DatabaseManagement.DatabaseInformation.OMLDatabaseConnectionString;
                        }
                    }
                }

                return db;
            }
        }

        public static WatcherDataDataContext InstanceOrNull { get { return db; } }        
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
