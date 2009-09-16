using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OMLSDK;
using OMLEngine;
using System.Linq;

namespace OMLImporter
{
    class Program
    {
        public static double VERSION = 0.1;
        //private static TitleCollection mainTitleCollection = new TitleCollection();
        //private static Boolean isDirty = false;        
        private static List<OMLPlugin> plugins = new List<OMLPlugin>();
        private const string COPY_IMAGES_KEY = "copyimages=";
        private const string CLEAR_BEFORE_IMPORT_KEY = "clearallbeforeimport=";

        [STAThread]
        static void Main(string[] args)
        {
            PrintHeader();

            // this can be run as command line only in the following format
            // OMLImporter.exe <plugin name> <path> <copy images true|false [optional]>
            if (args != null && args.Length > 0)
            {                                
                if (!ProcessCommandLine(args))
                {                    
                    Usage();
                }
            }
            else
            {
                LoadPlugins();
                Menu();
                for (int ii = 0; ii < plugins.Count; ii++)
                {
                    plugins[ii] = null;
                }
                plugins = null;
            }
        }

        private static void LoadPlugins()
        {
            List<PluginServices.AvailablePlugin> pluginz =
                PluginServices.FindPlugins(FileSystemWalker.PluginsDirectory, typeof(IOMLPlugin).Name);
            
            // Loop through available plugins, creating instances and adding them
            foreach (PluginServices.AvailablePlugin oPlugin in pluginz)
            {
                OMLPlugin objPlugin = (OMLPlugin)PluginServices.CreateInstance(oPlugin);
                objPlugin.FileFound += new OMLPlugin.FileFoundEventHandler(FileFound);
                plugins.Add(objPlugin);
            }
        }

        public static void Menu()
        {
            OMLPlugin plugin = null;
            string file_to_import = string.Empty;
            Console.WriteLine("Loading current titles...");
            //mainTitleCollection.loadTitleCollection();
            IEnumerable<Title> allTitles = TitleCollectionManager.GetAllTitles();
            while (true)
            {
                plugin = null;
                Console.Clear();
                PrintHeader();
                Console.WriteLine("OML Importer: Current {0} titles in the database", allTitles.Count());
                Console.WriteLine("Which Importer would you like to use:");
                int ii;
                for (ii = 0; ii < plugins.Count; ii++)
                {
                    OMLPlugin pi = plugins[ii];
                    string sFmt = "{0}) {1} (v{2})";
                    Console.WriteLine(string.Format(sFmt, (ii + 1), pi.Menu, pi.Version));
                }
                //foreach (OMLPlugin pi in plugins)
                //{
                //    Console.WriteLine(string.Format("{0}) {1} (v{2})", ii++, pi.Name, pi.Version));
                //}
                ii++;
                //Console.WriteLine(String.Format("{0}) Save the New Titles", ii++));
                Console.WriteLine(String.Format("{0}) Quit", ii++));
                Console.WriteLine(String.Format("{0}) Remove all titles from the database (be carefull!!!)", ii++));
                Console.WriteLine();
                Console.Write("Choice: ");

                string response = Console.ReadLine();
                if (response.Length == 0) continue;

                //response = response.Substring(0, 1);
                Int32 iResp;
                if (!Int32.TryParse(response, out iResp)) continue;
                if (!(0 < iResp && iResp < ii)) continue;
                --iResp;
                if (iResp < plugins.Count)
                {
                    plugin = plugins[iResp];

                    DateTime startTime = DateTime.Now;
                    Console.WriteLine("Begin time: " + startTime);

                    plugin.DoWork(plugin.GetWork());                                        
                    LoadTitlesIntoDatabase(plugin, true, true);

                    Console.WriteLine("End time: " + DateTime.Now.ToString() + " Total seconds: " + (DateTime.Now - startTime).TotalSeconds);

                    Console.WriteLine("Done!");
                    Console.ReadLine();
                } 
                /*else if (iResp == (plugins.Count))
                {               
                    if (isDirty) 
                    {
                        Console.WriteLine("Adding Titles ...");                        
                        isDirty = !mainTitleCollection.saveTitleCollection();
                    }
                    Console.WriteLine("Complete!");
                } */
                else if (iResp == (plugins.Count))
                {
                    /*if (isDirty)
                    {
                        Console.WriteLine("You have not saved your changes. Do you want to save before quitting? (y/n)");
                        string answer = Console.ReadLine();
                        if (answer.ToUpper() == "Y")
                        {
                            Console.WriteLine("Adding Titles ...");
                            mainTitleCollection.saveTitleCollection();
                            isDirty = false;
                        }
                    }*/
                    Console.WriteLine("Complete!");
                    return;
                } 
                else if (iResp == (plugins.Count + 1))
                {
                    Console.WriteLine("This option will delete all titles from the database immediately! This operation CANNOT be undone!");
                    Console.WriteLine("Are you sure you want to delete all the titles from the database? (please type YES)");
                    string deleteAllAnswer = Console.ReadLine();
                    if (deleteAllAnswer == "YES")
                    {
                        Console.WriteLine("Removing all entries...(this can take awhile)");
                        //mainTitleCollection = new TitleCollection();
                        //mainTitleCollection.saveTitleCollection();
                        TitleCollectionManager.DeleteAllDBData();
                        //isDirty = false;
                        Console.WriteLine("Done!");
                    }
                    else
                    {
                        Console.WriteLine("Operation aborted. No titles have been deleted!");
                    }
                } 
                else
                {
                        Usage();
                }
            }

        }        

        private static bool ProcessCommandLine(string [] args)
        {
            OMLPlugin pluginToUse = null;
            string pluginName = args[0].Trim();
            bool copyImages = true;
            string path = null;
            bool clearBeforeImport = false;
            
            if (args.Length > 1)
            {
                path = args[1];
            }
            else
            {
                return false;
            }

            for(int i = 2 ; i < args.Length ; i++)
            {
                if (args[i].StartsWith(COPY_IMAGES_KEY, StringComparison.OrdinalIgnoreCase))
                {
                    if (!bool.TryParse(args[i].Substring(COPY_IMAGES_KEY.Length), out copyImages))
                    {
                        return false;
                    }
                }
                else if (args[i].StartsWith(CLEAR_BEFORE_IMPORT_KEY, StringComparison.OrdinalIgnoreCase))
                {
                    if (!bool.TryParse(args[i].Substring(CLEAR_BEFORE_IMPORT_KEY.Length), out clearBeforeImport))
                    {
                        return false;
                    }
                }
                else // command line argument not understood
                    return false;
            }

            // Loop through available plugins to find the new we need to create
            foreach (PluginServices.AvailablePlugin plugin in
                PluginServices.FindPlugins(FileSystemWalker.PluginsDirectory, "OMLSDK.IOMLPlugin"))
            {
                if (plugin.ClassName.Equals(pluginName, StringComparison.OrdinalIgnoreCase))
                {
                    pluginToUse = (OMLPlugin)PluginServices.CreateInstance(plugin);                    
                    pluginToUse.FileFound += new OMLPlugin.FileFoundEventHandler(FileFound);
                }
            }

            if (pluginToUse == null)
            {
                Console.WriteLine(pluginName + " was not found as a plugin. The valid plugins are:");
                Console.WriteLine("");
                foreach (PluginServices.AvailablePlugin plugin in
                    PluginServices.FindPlugins(FileSystemWalker.PluginsDirectory, typeof(IOMLPlugin).Name))
                    Console.WriteLine(plugin.ClassName);
            }
            else
            {
                // use the found plugin                
                if (pluginToUse.IsSingleFileImporter() && !File.Exists(path))
                {
                    Console.WriteLine(pluginToUse.Name + " requires an import file which it can't find (" + path + ")");
                }
                else if (!pluginToUse.IsSingleFileImporter() && !Directory.Exists(path))
                {
                    Console.WriteLine(pluginToUse.Name + " requires an import directory which can't be found (" + path + ")");
                }
                else
                {
                    if (clearBeforeImport)
                    {
                        Console.WriteLine("Clearing out old data before import ( this can take awhile )");
                        TitleCollectionManager.DeleteAllDBData();
                    }

                    Console.WriteLine("Beginning to import titles...");

                    pluginToUse.DoWork(new string[] { path });
                    LoadTitlesIntoDatabase(pluginToUse, false, true);
                }
            }

            return true;
        }

        private static void FileFound(object sender, OMLSDK.PlugInFileEventArgs e)
        {
            OMLPlugin plugin = (OMLPlugin) sender;
            Console.WriteLine("Loading file " + e.FileName + " using " + plugin.Name + " importer");
        }       

        public static bool ImportFile(OMLPlugin plugin, string fileName)
        {
            return plugin.Load(fileName);
        }

        public static void Usage()
        {
            Console.WriteLine("Usage: OMLImporter.exe");
            Console.WriteLine("Usage: OMLImporter.exe <plugin name> <path> <CopyImages=true|false [optional <true>]> <ClearAllBeforeImport=true|false [optional <false>]>");
        }

        public static void PrintHeader()
        {
            Console.WriteLine("OML Data Importer v" + VERSION);
            Console.WriteLine("Licensed under GPL V3\n");
        }

        public static void LoadTitlesIntoDatabase(OMLPlugin plugin, bool flushInputBuffer, bool autoYesToAll)
        {
            try
            {
                Utilities.DebugLine("[OMLImporter] Titles loaded, beginning Import process");
                //TitleCollection tc = new TitleCollection();
                IList<Title> titles = plugin.GetTitles();
                Utilities.DebugLine("[OMLImporter] " + titles.Count + " titles found in input file");
                Console.WriteLine("Found " + titles.Count + " titles");

                int numberOfTitlesAdded = 0;
                int numberOfTitlesSkipped = 0;
                bool YesToAll = autoYesToAll;

                if (flushInputBuffer && Console.In.Peek() > 0)
                    Console.In.ReadToEnd(); // flush out anything still in there

                foreach (Title t in titles)
                {
                    if (TitleCollectionManager.ContainsDisks(t.Disks))
                    {
                        Console.WriteLine("Title {0} skipped because already in the collection", t.Name);
                        numberOfTitlesSkipped++;
                        continue;
                    }


                    Console.WriteLine("Adding: " + t.Name);
                    if (YesToAll == false)
                    {
                        Console.WriteLine("Would you like to add this title? (y/n/a)");
                        string response = Console.ReadLine();
                        switch (response.ToUpper())
                        {
                            case "Y":
                                TitleCollectionManager.AddTitle(t);                                
                                numberOfTitlesAdded++;
                                break;
                            case "N":
                                numberOfTitlesSkipped++;
                                break;
                            case "A":
                                YesToAll = true;
                                TitleCollectionManager.AddTitle(t);                                
                                numberOfTitlesAdded++;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        TitleCollectionManager.AddTitle(t);                        
                        numberOfTitlesAdded++;
                    }
                }

                // save all the image updates
                TitleCollectionManager.SaveTitleUpdates();

                plugin.GetTitles().Clear();

                //if (numberOfTitlesAdded > 0) isDirty = true;
                Console.WriteLine();
                Console.WriteLine("Added " + numberOfTitlesAdded + " titles");
                Console.WriteLine("Skipped " + numberOfTitlesSkipped + " titles");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in LoadTitlesIntoDatabase: {0}", e.Message);
            }
            //tc.saveTitleCollection();
            //Console.WriteLine("Complete");
        }
    }
}
