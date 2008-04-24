using System;
using System.Collections.Generic;
using System.Text;
using OMLSDK;

namespace MovieCollectorz
{
    public class Class1 : IDataImporter
    {
        public OMLDataSet GetOmlDataSet()
        {
            OMLDataSet ods = new OMLDataSet();

            return ods;
        }
    }
}
