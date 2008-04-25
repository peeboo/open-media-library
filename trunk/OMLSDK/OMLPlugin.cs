using System;
using System.Collections.Generic;
using System.Data;

namespace OMLSDK
{
    public class OMLPlugin
    {
        public static bool ValidateRow(OMLDataSet ds, DataRow row)
        {

            foreach (string columnname in ds.GetColumnNames())
            {
                if (row[columnname] == null)
                    return false;
            }

            return true;
        }
    }
}
