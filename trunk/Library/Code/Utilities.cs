using System;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using System.Data;
using System.Reflection;

namespace Library
{
    public static class Utilities
    {
        public static Image LoadImage(string imageName)
        {
            try
            {
                return new Image("file://" + imageName);
            }
            catch (Exception)
            {
                Trace.WriteLine("Error loading image: " + imageName);
            }
            return null;
        }

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
