using System;
using System.Collections.Generic;
using OMLSDK;
using System.Xml;
using System.Xml.XPath;

namespace MyMoviesPlugin
{
    public class MyMoviesImporter : IDataImporter
    {
        private OMLDataSet ods;

        public MyMoviesImporter()
        {
            ods = new OMLDataSet();
        }

        public OMLDataSet GetOmlDataSet()
        {
            return ods;
        }
    }
}
