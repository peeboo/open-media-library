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
        public static string[] RequiredMethods = new string[4] {
            "get_GetTitles",
            "get_TotalRowsAdded",
            "get_GetDescription",
            "get_GetAuthor"
        };

        public static void ImportData(Type importerClassType)
        {
            List<Title> titles;
            if (importerClassType.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(importerClassType);

                    MethodInfo mi = importerClassType.GetMethod("GetTotalRowsAdded");
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

        public static List<string> getPossiblePlugins()
        {
            List<string> plugins = new List<string>();
            if (FileSystemWalker.PluginsDirExists())
            {
                string[] files = Directory.GetFiles(FileSystemWalker.PluginDirectory);
                foreach (string possible_file in files)
                {
                    if (!possible_file.Contains("OMLSDK"))
                        plugins.Add(possible_file);
                }
            }
            return plugins;
        }
        public static List<Type> LoadAssemblies()
        {
            List<Type> validPlugins = new List<Type>();
            Assembly asm = null;
            List<string> possible_plugins = getPossiblePlugins();
            foreach (string posPlugin in possible_plugins)
            {
                asm = Assembly.LoadFile(posPlugin);
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    if (ValidatePlugin(type))
                    {
                        validPlugins.Add(type);
                    }
                }
            }
            return validPlugins;
        }

        public static bool ValidatePlugin(Type type)
        {
            if (type.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(type);
                    MethodInfo[] methods = type.GetMethods();

                    foreach (string required_meth in RequiredMethods)
                    {
                        if (!ContainsMethod(methods, required_meth))
                            return false;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Failed to validate Importer: " +
                                    type +
                                    " with error: " +
                                    e.Message);
                    return false;
                }
            }
            return true;
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

        private static bool ContainsMethod(MethodInfo[] methods, string required_method)
        {
            foreach (MethodInfo mi in methods)
            {
                if (mi.Name == required_method)
                    return true;
            }
            return false;
        }
    }
}
