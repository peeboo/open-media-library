using System;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace OMLEngine
{
    public static class Utilities
    {
        public static string[] RequiredMethods = new string[2] {
            "GetTitles",
            "TotalRowsAdded",
        };

        public static void ImportData(ref DataSet dataSet)
        {
            List<Title> titles;
            Type importerClassType = getImporterClassType("");
            if (importerClassType.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(importerClassType);

                    MethodInfo mi = importerClassType.GetMethod("TotalRowsAdded");
                    int totalTitles = (int)mi.Invoke(obj, null);
                    if (totalTitles > 0) {
                        mi = importerClassType.GetMethod("GetTitles");
                        titles = (List<Title>)mi.Invoke(obj, null);

                        if (titles != null)
                        {
                            foreach (Title title in titles)
                            {
                                Trace.WriteLine("Would add title" + title.Name);
                            }
                        }
                    }
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
            return typeof(int);
        }

        public static bool RawSetup()
        {
            if (!FileSystemWalker.RootDirExists())
                FileSystemWalker.createRootDirectory();

            if (!FileSystemWalker.ImageDirExists())
                FileSystemWalker.createImageDirectory();

            if (!FileSystemWalker.PluginsDirExists())
                FileSystemWalker.createPluginsDirectory();

            if (!FileSystemWalker.LogDirExists())
                FileSystemWalker.createLogDirectory();

            return true;
        }
    }
}
