using System;
using System.Diagnostics;
using System.Data;
using System.Reflection;

namespace OMLEngine
{
    public static class Utilities
    {
        public static void ImportData(ref DataSet dataSet)
        {
            Type importerClassType = getImporterClassType();
            if (importerClassType.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(importerClassType);
                    MethodInfo mi = importerClassType.GetMethod("getDataSet");
                    dataSet = (DataSet)mi.Invoke(obj, null);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Importer Error: " + e.Message);
                }
            }
        }

        public static Type getImporterClassType()
        {
            return typeof(MoviesXmlImporter);
        }
    }
}
