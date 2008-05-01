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
        /// <summary>
        /// Static list of methods that ALL plugins must define
        /// </summary>
        public static string[] RequiredMethods = new string[4] {
            "get_GetTitles",
            "get_TotalRowsAdded",
            "get_GetDescription",
            "get_GetAuthor"
        };

        /// <summary>
        /// Loads data from plugin classes and creates a List of Title objects
        /// </summary>
        /// <param name="importerClassType">Type of importer class</param>
        /// <returns>IList of Title objects</returns>
        public static IList<Title> ImportData(Type importerClassType)
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
                        return titles;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Importer Error: " + e.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// Scans the plugins directory for all possible plugins
        /// and validates each one before returns a list of all assemblies
        /// that passed validation.
        /// </summary>
        /// <returns>A List of plugin names</returns>
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

        /// <summary>
        /// Loads all valid plugins into memory
        /// </summary>
        /// <returns>A List of the Type objects representing all valid plugins found</returns>
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

        /// <summary>
        /// Checks a given plugin Type and ensures that it contains
        /// the required methods for use.
        /// </summary>
        /// <param name="type">Type of the plugin to be validated</param>
        /// <returns>True on successful validation</returns>
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

        /// <summary>
        /// Validates or creates required directories for the proper function
        /// of the Open Media Library application
        /// </summary>
        /// <returns>True on success</returns>
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

        /// <summary>
        /// Given a list of available methods and the method to search for,
        /// scan through all the available methods looking for the requested
        /// method.
        /// </summary>
        /// <param name="methods">List of methods to scan through</param>
        /// <param name="required_method">name of method to find</param>
        /// <returns>True on success</returns>
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
