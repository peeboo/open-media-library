using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using OMLSDK;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;

namespace OMLXMLPlugin
{
    public class OMLXMLPlugin : OMLPlugin, IOMLPlugin
    {
        public OMLXMLPlugin()
            : base()
        {
            Utilities.DebugLine("[OMLXMLPlugin] created");
        }

        public override bool IsSingleFileImporter()
        {
            return false;
        }

        public override string SetupDescription()
        {
            return GetName() + @" will search for and import OML.XML files";
        }

        protected override bool GetFolderSelect()
        {
            return true;
        }
        private static double MajorVersion = 0.9;
        private static double MinorVersion = 0.1;
        protected override double GetVersionMajor()
        {
            return MajorVersion;
        }

        protected override double GetVersionMinor()
        {
            return MinorVersion;
        }
        protected override string GetMenu()
        {
            return "OML.XML Importer";
        }
        protected override string GetName()
        {
            return "OML.XML Importer";
        }
        protected override string GetAuthor()
        {
            return "OML Development Team";
        }
        protected override string GetDescription()
        {
            return @"OML.XML importer v" + Version;
        }
        public override bool Load(string filename)
        {
            GetMovies(filename);
            return true;
        }

        public override void DoWork(string[] thework)
        {
            GetMovies(thework[0]);
        }


        public void GetMovies(string startFolder)
        {
            try
            {
                List<string> dirList = new List<string>();
                List<string> fileList = new List<string>();
                GetSubFolders(startFolder, dirList);

                // the share or link may not exist nowbb
                if (Directory.Exists(startFolder))
                {
                    dirList.Add(startFolder);
                }

                foreach (string currentFolder in dirList)
                {
                    Utilities.DebugLine("OMLXML importer: folder " + currentFolder);
                 
                    string[] fileNames = null;
                    try
                    {
                        fileNames = Directory.GetFiles(currentFolder, "*.oml.xml");
                    }
                    catch
                    {
                        fileNames = null;
                    }

                    if (fileNames != null)
                    {
                        foreach (string omlxml in fileNames)
                        {
                            Title t = Title.CreateFromXML(omlxml);
                            if (t != null)
                            {
                                AddTitle(t);
                            }
                        }
                    }
                } // loop through the sub folders
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[DVDLibraryImporter] An error occured: " + ex.Message);
            }
        }

        private void GetSubFolders(string startFolder, List<string> folderList)
        {
            DirectoryInfo[] diArr = null;
            try
            {
                DirectoryInfo di = new DirectoryInfo(startFolder);
                diArr = di.GetDirectories();
                foreach (DirectoryInfo folder in diArr)
                {
                    folderList.Add(folder.FullName);
                    if (Directory.Exists(folder.FullName + "\\VIDEO_TS") || File.Exists(folder.FullName + "\\VIDEO_TS.IFO") || File.Exists(folder.FullName + "\\VTS_01_1.VOB"))
                    {
                        // stop here
                    }
                    else
                    {
                        GetSubFolders(folder.FullName, folderList);
                    }
                }

            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[DVDLibraryImporter] An error occured: " + ex.Message);
                // ignore any permission errors
            }
        }
    }
}
