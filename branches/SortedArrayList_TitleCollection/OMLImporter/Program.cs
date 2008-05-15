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

        [STAThread]
        static void Main(string[] args)
        {
            OMLPlugin plugin = null;
            string file_to_import = string.Empty;

            PrintHeader();

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.ToUpper().CompareTo("type") == 0)
                    {
                    }
                }
            }

            if (plugin == null)
            {
                Console.WriteLine("Which Importer would you like to use:");
                Console.WriteLine("1) MyMovies");
                Console.WriteLine("2) DVD Profiler");
                Console.WriteLine("3) Movie Collectorz");
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
                    default:
                        Usage();
                        return;
                }
                Console.WriteLine();
            }

            if (plugin != null && file_to_import.Length == 0)
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
                    file_to_import = ofDiag.FileName;
                }
            }

            if (plugin != null && file_to_import.Length > 0)
            {
                FileInfo fi;
                try
                {
                    fi = new FileInfo(file_to_import);
                    if (fi.Exists)
                    {
                        Console.WriteLine("Loading file " + file_to_import + " using " + plugin.GetName() + " importer");
                        if (ImportFile(plugin, fi))
                        {
                            TitleCollection tc = new TitleCollection();
                            List<Title> titles = plugin.GetTitles();
                            foreach (Title t in titles)
                            {
                                tc.Add(t);
                            }
                            tc.saveTitleCollection();
                            Console.WriteLine("Complete");
                        }
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
            else
            {
                Usage();
            }
        }

        public static bool ImportFile(OMLPlugin plugin, FileInfo fInfo)
        {
            return plugin.Load(fInfo.FullName);
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
    }
}
