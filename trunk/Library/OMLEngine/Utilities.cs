using System;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Collections;

namespace OMLEngine
{
    public static class Utilities
    {
        public static string[] RequiredMethods = new string[4] {
            "getDataSet",
            "getName",
            "getCrew",
            "getActors",
        };

        public static void ImportData(ref DataSet dataSet)
        {
            Type importerClassType = getImporterClassType("");
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
        public static bool ValidateImporter(string importerClassName)
        {
            Type importerClassType = getImporterClassType(importerClassName);
            if (importerClassType.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(importerClassType);
                    foreach (string required_method in RequiredMethods)
                    {
                        MethodInfo mi = importerClassType.GetMethod(required_method, BindingFlags.Instance);
                        if (mi == null)
                            return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Failed to validate Importer: " +
                                    importerClassName +
                                    " with error: " +
                                    e.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static Type getImporterClassType(string importerClassName)
        {
            return typeof(MoviesXmlImporter);
        }
    }
}
