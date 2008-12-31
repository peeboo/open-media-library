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
    }
}
