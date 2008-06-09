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
        private static TitleCollection tc = new TitleCollection();
        private static Boolean isDirty = false;
        public static bool _copyImages = true;

        [STAThread]
        static void Main(string[] args)
        {
            PrintHeader();
            Menu();
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
        }

        public static DialogResult GetFile(ref string file_to_import, OMLPlugin plugin)
        {
            OpenFileDialog ofDiag = new OpenFileDialog();
            ofDiag.InitialDirectory = "c:\\";
            ofDiag.Filter = "Xml files (*.xml)|*.xml|DVR-MS Files (*.dvr-ms)|*.dvr-ms|All files (*.*)|*.*";
            ofDiag.FilterIndex = 1;
            ofDiag.RestoreDirectory = true;
            ofDiag.AutoUpgradeEnabled = true;
            ofDiag.CheckFileExists = true;
            ofDiag.CheckPathExists = true;
            ofDiag.Multiselect = false;
            ofDiag.Title = "Select " + plugin.GetName() + " file to import";
            DialogResult dlgRslt = ofDiag.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                Utilities.DebugLine("[OMLImporter] Valid file found ("+ofDiag.FileName+")");
                file_to_import = ofDiag.FileName;
            }
            return dlgRslt;
        }

        public static DialogResult GetFolder(ref string file_to_import, OMLPlugin plugin)
        {
            FolderBrowserDialog ofDiag = new FolderBrowserDialog();
            DialogResult res = ofDiag.ShowDialog();
            if (res == DialogResult.OK)
            {
                file_to_import = ofDiag.SelectedPath.ToUpper();
            }

            return res;
        }

        public static void Menu()
        {
            OMLPlugin plugin = null;
            string file_to_import = string.Empty;

            while (true)
            {
                Console.WriteLine("Which Importer would you like to use:");
                Console.WriteLine("1) MyMovies");
                Console.WriteLine("2) DVD Profiler");
                Console.WriteLine("3) Movie Collectorz");
                Console.WriteLine("4) DVRMS Movie Files");
                Console.WriteLine("5) VMC Built-in DVD Library");
                Console.WriteLine("6) Quit");
                Console.Write("Choice: ");

                bool showFolderSelection = false;

                string response = Console.ReadLine();
                response = response.Substring(0, 1);
                switch (Int32.Parse(response))
                {
                    case 1:
                        AskIfShouldCopyImages();
                        plugin = new MyMoviesPlugin.MyMoviesImporter();
                        break;
                    case 2:
                        AskIfShouldCopyImages();
                        plugin = new DVDProfilerPlugin.DVDProfilerImporter();
                        break;
                    case 3:
                        AskIfShouldCopyImages();
                        plugin = new MovieCollectorz.MovieCollectorzPlugin();
                        break;
                    case 4:
                        plugin = new DVRMS.DVRMSPlugin();
                        break;
                    case 5:
                        showFolderSelection = true;
                        plugin = new VMCDVDLibraryPlugin.DVDLibraryImporter();
                        break;
                    case 6:
                        if (isDirty) 
                        {
                            tc.saveTitleCollection();
                        }
                        Console.WriteLine("Complete");
                        return;
                    default:
                        Usage();
                        continue;
                }
                Console.WriteLine();

                if (showFolderSelection)
                {
                    if (GetFolder(ref file_to_import, plugin) == DialogResult.OK)
                    {
                        ProcessFile(plugin, file_to_import);
                    }
                }
                else
                {
                    if (GetFile(ref file_to_import, plugin) == DialogResult.OK)
                    {
                        ProcessFile(plugin, file_to_import);
                    }
                }
            }

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
            return plugin.Load(fileName, Program._copyImages);
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
            //TitleCollection tc = new TitleCollection();
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
                Console.WriteLine("Adding: " + t.Name);
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
                    tc.Add(t);
                    numberOfTitlesAdded++;
                }
            }

            if (numberOfTitlesAdded > 0) isDirty = true;
            Console.WriteLine();
            Console.WriteLine("Added " + numberOfTitlesAdded + " titles");
            Console.WriteLine("Skipped " + numberOfTitlesSkipped + " titles");
            //tc.saveTitleCollection();
            //Console.WriteLine("Complete");
        }

        public static void ProcessFile(OMLPlugin plugin, string file_to_import)
        {
            try
            {
                if (File.Exists(file_to_import) || Directory.Exists(file_to_import))
                {
                    Console.WriteLine("Loading file " + file_to_import + " using " + plugin.GetName() + " importer");
                    if (ImportFile(plugin, file_to_import))
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
