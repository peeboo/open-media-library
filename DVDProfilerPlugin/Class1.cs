using System;
using System.Collections.Generic;
using OMLSDK;

namespace DVDProfilerPlugin
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
