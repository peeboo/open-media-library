using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OMLSDK;
using OMLEngine;

namespace OMLImporter
{
    class Program
    {
        public static double VERSION = 0.1;
        public static int _exit = 0;
        public static bool _copyImages = true;

        [STAThread]
        static void Main(string[] args)
        {
            OMLPlugin plugin = null;
            string file_to_import = string.Empty;

            PrintHeader();

            /* This will be used to pass params and not use the menu (automation use?)
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.ToUpper().CompareTo("type") == 0)
                    {
                    }
                }
            }
            */

            do
            {
                Menu(ref plugin);
                if (Program._exit < 1)
                {
                    AskIfShouldCopyImages();
                    GetFile(ref file_to_import, plugin);
                    if (plugin != null && file_to_import != null)
                    {
                        Utilities.DebugLine("[OMLImporter] Found plugin and file, moving to process");
                        ProcessFile(plugin, file_to_import);
                        plugin = null;
                        file_to_import = null;
                    }
                }
                Utilities.DebugLine("");
            } while (Program._exit < 1);
        }

        public static void GetFile(ref string file_to_import, OMLPlugin plugin)
        {
            OpenFileDialog ofDiag = new OpenFileDialog();
            ofDiag.InitialDirectory = "c:\\";
            ofDiag.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            ofDiag.FilterIndex = 1;
            ofDiag.RestoreDirectory = true;
            ofDiag.AutoUpgradeEnabled = true;
            ofDiag.CheckFileExists = true;
            ofDiag.CheckPathExists = true;
            ofDiag.Multiselect = false;
            ofDiag.Title = "Select " + plugin.GetName() + " file to import";

            if (ofDiag.ShowDialog() == DialogResult.OK)
            {
                Utilities.DebugLine("[OMLImporter] Valid file found ("+ofDiag.FileName+")");
                file_to_import = ofDiag.FileName;
            }
        }

        public static void Menu(ref OMLPlugin plugin)
        {
            Console.WriteLine("Which Importer would you like to use:");
            Console.WriteLine("1) MyMovies");
            Console.WriteLine("2) DVD Profiler");
            Console.WriteLine("3) Movie Collectorz");
            Console.WriteLine("4) Quit");
            Console.Write("Choice: ");

            string response = Console.ReadLine();
            response = response.Substring(0, 1);
            switch (Int32.Parse(response))
            {
                case 1:
                    plugin = new MyMoviesPlugin.MyMoviesImporter();
                    break;
                case 2:
                    plugin = new DVDProfilerPlugin.DVDProfilerImporter();
                    break;
                case 3:
                    plugin = new MovieCollectorz.MovieCollectorzPlugin();
                    break;
                case 4:
                    Program._exit = 1;
                    return;
                default:
                    Usage();
                    return;
            }
            Console.WriteLine();
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

        public static bool ImportFile(OMLPlugin plugin, FileInfo fInfo)
        {
            return plugin.Load(fInfo.FullName, Program._copyImages);
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
            Utilities.DebugLine("[OMLImporter] Titles loaded, beginning Import process");
            TitleCollection tc = new TitleCollection();
            tc.loadTitleCollection();

            List<Title> titles = plugin.GetTitles();
            Utilities.DebugLine("[OMLImporter] "+titles.Count+" titles found in input file");
            Console.WriteLine("Found " + titles.Count + " titles");

            int numberOfTitlesAdded = 0;
            int numberOfTitlesSkipped = 0;
            bool YesToAll = false;

            if (Console.In.Peek() > 0)
                Console.In.ReadToEnd(); // flush out anything still in there

            foreach (Title t in titles)
            {
                if (YesToAll == false)
                {
                    Console.WriteLine("Would you like to add this title? (y/n/a)");
                    string response = Console.ReadLine();
                    switch (response.ToUpper())
                    {
                        case "Y":
                            tc.Add(t);
                            numberOfTitlesAdded++;
                            break;
                        case "N":
                            numberOfTitlesSkipped++;
                            break;
                        case "A":
                            YesToAll = true;
                            tc.Add(t);
                            numberOfTitlesAdded++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    OMLPlugin.BuildResizedMenuImage(t);
                    System.Collections.Hashtable fileNames = tc.MoviesByFilename;

                    if (tc.MoviesByFilename.ContainsKey(t.FileLocation))
                    {
                        Console.WriteLine("Replacing: " + t.Name);
                        Title oldTitle = (Title)tc.MoviesByFilename[t.FileLocation];
                        tc.Replace(t, oldTitle);
                    }
                    else
                    {
                        Console.WriteLine("Adding: " + t.Name);
                        tc.Add(t);
                    }

                    numberOfTitlesAdded++;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Added " + numberOfTitlesAdded + " titles");
            Console.WriteLine("Skipped " + numberOfTitlesSkipped + " titles");
            tc.saveTitleCollection();
            Console.WriteLine("Complete");
        }

        public static void ProcessFile(OMLPlugin plugin, string file_to_import)
        {
            FileInfo fi;
            try
            {
                fi = new FileInfo(file_to_import);
                if (fi.Exists)
                {
                    Console.WriteLine("Loading file " + file_to_import + " using " + plugin.GetName() + " importer");
                    if (ImportFile(plugin, fi))
                        LoadTitlesIntoDatabase(plugin);
                    else
                    {
                        Console.WriteLine("Error importing file: File doesn't exist.");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to import file (" + file_to_import + "): " + e.Message);
            }
        }
    }
}
