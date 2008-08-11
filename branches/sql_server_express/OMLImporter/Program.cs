using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OMLSDK;
using OMLEngine;
using System.Drawing;

namespace OMLImporter
{
    class Program
    {
        public static double VERSION = 0.1;
        private static TitleCollection mainTitleCollection = new TitleCollection();
        private static Boolean isDirty = false;
        public static bool _copyImages = true;
        private static List<OMLPlugin> plugins = new List<OMLPlugin>();

        [STAThread]
        static void Main(string[] args)
        {
            PrintHeader();
            LoadPlugins();
            Menu();
            for (int ii = 0; ii < plugins.Count; ii++)
            {
                plugins[ii] = null;
            }
            plugins = null;
        }

        private static void LoadPlugins()
        {
            List<PluginServices.AvailablePlugin> Pluginz = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            Pluginz = PluginServices.FindPlugins(path, "OMLSDK.IOMLPlugin");
            OMLPlugin objPlugin;
            // Loop through available plugins, creating instances and adding them
            foreach (PluginServices.AvailablePlugin oPlugin in Pluginz)
            {
                objPlugin = (OMLPlugin) PluginServices.CreateInstance(oPlugin);
                objPlugin.FileFound += new OMLPlugin.FileFoundEventHandler(FileFound);
                plugins.Add(objPlugin);
            }
            Pluginz = null;
        }

        public static void Menu()
        {
            OMLPlugin plugin = null;
            string file_to_import = string.Empty;
            Console.WriteLine("Loading current titles...");
            mainTitleCollection.loadTitleCollection();
            while (true)
            {
                plugin = null;
                Console.Clear();
                PrintHeader();
                Console.WriteLine("OML Importer: Current {0} titles in the database", mainTitleCollection.Count);
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
                Console.WriteLine(String.Format("{0}) Save the New Titles", ii++));
                Console.WriteLine(String.Format("{0}) Quit (No Saving)", ii++));
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
                    if (plugin.CanCopyImages) AskIfShouldCopyImages();
                    plugin.CopyImages = Program._copyImages;
                    plugin.DoWork(plugin.GetWork());
                    LoadTitlesIntoDatabase(plugin);
                    Console.WriteLine("Done!");
                    Console.ReadLine();
                } 
                else if (iResp == (plugins.Count))
                {               
                    if (isDirty) 
                    {
                        Console.WriteLine("Adding Titles ...");                        
                        isDirty = !mainTitleCollection.saveTitleCollection();
                    }
                    Console.WriteLine("Complete!");
                } 
                else if (iResp == (plugins.Count + 1))
                {
                    if (isDirty)
                    {
                        Console.WriteLine("You have not saved your changes. Do you want to save before quitting? (y/n)");
                        string answer = Console.ReadLine();
                        if (answer.ToUpper() == "Y")
                        {
                            Console.WriteLine("Adding Titles ...");
                            mainTitleCollection.saveTitleCollection();
                            isDirty = false;
                        }
                    }
                    Console.WriteLine("Complete!");
                    return;
                } 
                else if (iResp == (plugins.Count + 2))
                {
                    Console.WriteLine("This option will delete all titles from the database immediately! This operation CANNOT be undone!");
                    Console.WriteLine("Are you sure you want to delete all the titles from the database? (please type YES)");
                    string deleteAllAnswer = Console.ReadLine();
                    if (deleteAllAnswer == "YES")
                    {
                        Console.WriteLine("Removing all entries...");
                        mainTitleCollection = new TitleCollection();
                        mainTitleCollection.saveTitleCollection();
                        isDirty = false;
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

        private static void FileFound(object sender, OMLSDK.PlugInFileEventArgs e)
        {
            OMLPlugin plugin = (OMLPlugin) sender;
            Console.WriteLine("Loading file " + e.FileName + " using " + plugin.Name + " importer");
        }

        public static void AskIfShouldCopyImages()
        {
            Console.WriteLine("Should we copy images to the OML images directory? (y/n)");
            string response = Console.ReadLine();
            response = response.Substring(0, 1);
            switch (response.ToUpper())
            {
                case "N":
                    Program._copyImages = false;
                    break;
                case "Y":
                    Program._copyImages = true;
                    break;
                default:
                    break;
            }
        }

        public static bool ImportFile(OMLPlugin plugin, string fileName)
        {
            plugin.CopyImages = Program._copyImages;
            return plugin.Load(fileName);
        }

        public static void Usage()
        {
            Console.WriteLine("Usage: OMLImporter.exe");
            Console.WriteLine("Usage: OMLImporter.exe type=<importer_class> file=<full_path_to_file>");
        }

        public static void PrintHeader()
        {
            Console.WriteLine("OML Data Importer v" + VERSION);
            Console.WriteLine("Licensed under GPL V3\n");
        }

        public static void LoadTitlesIntoDatabase(OMLPlugin plugin)
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
                bool YesToAll = false;

                if (Console.In.Peek() > 0)
                    Console.In.ReadToEnd(); // flush out anything still in there

                foreach (Title t in titles)
                {
                    if (mainTitleCollection.ContainsDisks(t.Disks))
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
                                mainTitleCollection.Add(t);
                                numberOfTitlesAdded++;
                                break;
                            case "N":
                                numberOfTitlesSkipped++;
                                break;
                            case "A":
                                YesToAll = true;
                                mainTitleCollection.Add(t);
                                numberOfTitlesAdded++;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        mainTitleCollection.Add(t);
                        numberOfTitlesAdded++;
                    }
                }

                plugin.GetTitles().Clear();

                if (numberOfTitlesAdded > 0) isDirty = true;
                Console.WriteLine();
                Console.WriteLine("Added " + numberOfTitlesAdded + " titles");
                Console.WriteLine("Skipped " + numberOfTitlesSkipped + " titles");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in LoadTitlesIntoDatabase: %1", e.Message);
            }
            //tc.saveTitleCollection();
            //Console.WriteLine("Complete");
        }
    }
}
