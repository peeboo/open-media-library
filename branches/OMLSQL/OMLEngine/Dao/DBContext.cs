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
                        }
                    }
                }

                return db;
            }
        }

        public static OMLDataDataContext InstanceOrNull { get { return db; } }        
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
                        }
                    }
                }

                return db;
            }
        }

        public static WatcherDataDataContext InstanceOrNull { get { return db; } }        
    }
}
