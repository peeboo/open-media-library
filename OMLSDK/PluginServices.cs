using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

namespace OMLSDK
{
    public class PluginTypes
    {
        public const string ImportPlugin = "OMLSDK.IOMLPlugin";
        public const string MetadataPlugin = "OMLSDK.IOMLMetadataPlugin";
    }

    public class PluginServices
    {
        public static List<AvailablePlugin> FindPlugins(string strPath, string strInterface) {
            List<AvailablePlugin> Plugins = new List<AvailablePlugin>();
            string[] strDLLs;
            int intIndex;
            Assembly objDLL;
            // Go through all DLLs in the directory, attempting to load them
            strDLLs = Directory.GetFileSystemEntries(strPath, "*.dll");
            for (intIndex = 0; (intIndex 
                        <= (strDLLs.Length - 1)); intIndex++) {
                try {
                    objDLL = Assembly.LoadFrom(strDLLs[intIndex]);
                    ExamineAssembly(objDLL, strInterface, Plugins);
                }
                catch (Exception e) {
                    // Error loading DLL, we don't need to do anything special
                }
            }
            // Return all plugins found
            if ((Plugins.Count != 0))
            {
                return Plugins;
            }
            else {
                return null;
            }
        }
        
        private static void ExamineAssembly(Assembly objDLL, string strInterface, List<AvailablePlugin> Plugins) {
            Type objInterface;
            AvailablePlugin Plugin;
            // Loop through each type in the DLL
            foreach (System.Type objType in objDLL.GetTypes())
            {
                // Only look at public types
                if ((objType.IsPublic == true)) {
                    // Ignore abstract classes
                    if ((objType.Attributes & TypeAttributes.Abstract) 
                                != TypeAttributes.Abstract) {
                        // See if this type implements our interface
                        objInterface = objType.GetInterface(strInterface, true);
                        if (!(objInterface == null)) {
                            // It does
                            Plugin = new AvailablePlugin();
                            Plugin.AssemblyPath = objDLL.Location;
                            Plugin.ClassName = objType.FullName;
                            Plugins.Add(Plugin);
                        }
                    }
                }
            }
        }
        
        public static object CreateInstance(AvailablePlugin Plugin) {
            Assembly objDLL;
            object objPlugin;
            try {
                // Load dll
                objDLL = Assembly.LoadFrom(Plugin.AssemblyPath);
                // Create and return class instance
                objPlugin = objDLL.CreateInstance(Plugin.ClassName);
            }
            catch (Exception e) {
                return null;
            }
            return objPlugin;
        }
        
        public struct AvailablePlugin {
            
            public string AssemblyPath;
            
            public string ClassName;
        }

    }
}
