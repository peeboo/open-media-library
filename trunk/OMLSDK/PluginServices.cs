using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using OMLEngine;

namespace OMLSDK
{
    public class PluginTypes
    {
        public const string ImportPlugin = "OMLSDK.IOMLPlugin";
        public const string MetadataPlugin = "OMLSDK.IOMLMetadataPlugin";
    }

    public class PluginServices
    {
        public delegate void PluginLoaded(string plugingName);
        public event PluginLoaded Loaded;

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
                catch (Exception ex){
                    // Error loading DLL, we don't need to do anything special
                    Utilities.DebugLine("[PluginService] Loading plugin {0} Caused an Exception {1}", strDLLs[intIndex], ex.Message);
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
            catch {
                return null;
            }
            return objPlugin;
        }
        
        public struct AvailablePlugin {
            
            public string AssemblyPath;
            
            public string ClassName;
        }

        // Not putting a static on this as it doesn't seem to like a static event
        public void LoadMetadataPlugins(string pluginType, List<MetaDataPluginDescriptor> pluginList)
        {
            pluginList.Clear();

            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, pluginType);
            IOMLMetadataPlugin objPlugin;
            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                foreach (PluginServices.AvailablePlugin oPlugin in plugins)
                {
                    // Create an instance to enumerate providers in the plugin
                    objPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);

                    foreach (MetaDataPluginDescriptor provider in objPlugin.GetProviders)
                    {
                        if (Loaded != null)
                            Loaded(provider.DataProviderName);

                        // Create instance of the plugin for this particular provider. This would create a unique instance per provider.
                        provider.PluginDLL = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);

                        // Initialise the plugin and select which provider it serves
                        provider.PluginDLL.Initialize(provider.DataProviderName, new Dictionary<string, string>());

                        // Configure the plugin with any settings stored in the db
                        if (provider.PluginDLL.GetOptions() != null)
                        {
                            foreach (OMLMetadataOption option in provider.PluginDLL.GetOptions())
                            {
                                string setting = OMLEngine.Settings.SettingsManager.GetSettingByName(option.Name, "PLG-" + provider.DataProviderName);
                                if (setting != null)
                                {
                                    provider.PluginDLL.SetOptionValue(option.Name, setting);
                                }
                            }
                        }
                        pluginList.Add(provider);
                    }
                }
                plugins = null;
            }
        }

    }
}
