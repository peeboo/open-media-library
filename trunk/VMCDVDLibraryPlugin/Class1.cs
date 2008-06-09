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

namespace VMCDVDLibraryPlugin
{
    public class DVDLibraryImporter : OMLPlugin, IOMLPlugin
    {
        private static double VERSION = 0.1;
        public DVDLibraryImporter()
            : base()
        {
            Utilities.DebugLine("[DVDLibraryImporter] created");
        }

        public override string GetName()
        {
            return "VMCDVDLibraryPlugin";
        }
        public override string GetAuthor()
        {
            return "OML Development Team";
        }
        public override string GetDescription()
        {
            return "VMC DVD Library importer for Open Media Library v" + VERSION;
        }
        public override bool Load(string filename, bool ShouldCopyImages)
        {
            GetMovies(filename);
            return true;
        }

        public void GetMovies( string startFolder )
        {
            try
            {
                List<string> dirList = new List<string>();
                GetSubFolders(startFolder, dirList);
                dirList.Add(startFolder);

                foreach (string currentFolder in dirList)
                {
                    Title t = GetDVDMetaData(currentFolder);

                    if (t != null)
                    {
                        AddTitle(t);
                    }
                } // loop through the sub folders
            }
            catch (Exception ex)
            {
            }
        }


        private Title GetDVDMetaData(string folderName)
        {
            try
            {
                string xmlFile = "";
                if (Directory.Exists(folderName + "\\VIDEO_TS") || File.Exists(folderName + "\\VIDEO_TS.IFO") || File.Exists(folderName + "\\VTS_01_1.VOB"))
                {
                    string[] xmlFiles = Directory.GetFiles(folderName, "*dvdid*xml");
                    if (xmlFiles.Length > 0)
                    {
                        //xmlFile = Path.GetFileName(xmlFiles[0]); // get the first one
                        xmlFile = xmlFiles[0];
                    }
                    else
                    {
                        xmlFiles = Directory.GetFiles(folderName, "*xml");
                        if (xmlFiles.Length > 0)
                            //xmlFile = Path.GetFileName(xmlFiles[0]); // get the first one
                            xmlFile = xmlFiles[0]; // get the first one
                        else
                            xmlFile = "";
                    }

                    // xmlFile contains the dvdid - then we need to lookup in the cache folder based on this id
                    if (xmlFile != "" && File.Exists(xmlFile))
                    {
                        string dvdid = GetDVDID(xmlFile);
                        string xmlDetailsFile = GetDVDCacheFileName(dvdid);
                        return ReadMetaData(xmlDetailsFile);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        private Title ReadMetaData(string dvdCacheXMLFile)
        {
            Title t = new Title();
            try
            {
                if (File.Exists(dvdCacheXMLFile))
                {
                    XmlReaderSettings xmlSettings = new XmlReaderSettings();
                    xmlSettings.IgnoreWhitespace = false;
                    using (XmlReader reader = XmlReader.Create(dvdCacheXMLFile, xmlSettings))
                    {
                        bool bFound = reader.ReadToFollowing("MDR-DVD");
                        if (!bFound) return null;

                        bFound = reader.ReadToFollowing("dvdTitle");
                        if (!bFound) return null;
                        t.Name = reader.ReadString();

                        bFound = reader.ReadToFollowing("studio");
                        if (bFound)
                            t.Distributor = reader.ReadString();

                        bFound = reader.ReadToFollowing("leadPerformer");
                        if (bFound)
                        {
                            string leadPerformer = reader.ReadString();
                            string[] actors = leadPerformer.Split(new char[] { ';', ',', '|' });
                            foreach (string actor in actors)
                            {
                                t.AddActor(new Person(actor));
                                t.AddActingRole(actor, "");
                            }
                        }

                        bFound = reader.ReadToFollowing("director");
                        if (bFound)
                        {
                            string directorList = reader.ReadString();
                            string[] directors = directorList.Split(new char[] { ';', ',', '|' });
                            foreach (string director in directors)
                            {
                                t.AddDirector(new Person(director));
                            }
                        }

                        bFound = reader.ReadToFollowing("MPAARating");
                        if (bFound)
                            t.MPAARating = reader.ReadString();

                        bFound = reader.ReadToFollowing("language");
                        if (bFound)
                            t.AddLanguageFormat(reader.ReadString());

                        bFound = reader.ReadToFollowing("releaseDate");
                        if (bFound)
                        {
                            string releaseDate = reader.ReadString();
                            DateTime dt;
                            if (DateTime.TryParse(releaseDate, out dt))
                            {
                                t.ReleaseDate = dt;
                            }
                        }

                        bFound = reader.ReadToFollowing("genre");
                        if (bFound)
                        {
                            string genreList = reader.ReadString();
                            string[] genres = genreList.Split(new char[] { ';', ',', '|' });
                            foreach (string genre in genres)
                            {
                                t.AddGenre(genre);
                            }
                        }

                        bFound = reader.ReadToFollowing("largeCoverParams");
                        if (bFound)
                            t.FrontCoverPath = reader.ReadString();

                        bFound = reader.ReadToFollowing("duration");
                        if (bFound)
                        {
                            int runtime;
                            if( Int32.TryParse(reader.ReadString(), out runtime))
                            {
                                t.Runtime = runtime;
                            }
                        }

                        bFound = reader.ReadToFollowing("synopsis");
                        if (bFound)
                            t.Synopsis = reader.ReadString();

                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return t;
        }

        //
        // the dvid.xml file contains the Name of the movie and the DVDID
        //
        private string GetDVDID( string dvdidXMLFile )
        {
            string dvdid = "";
            try
            {
                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.IgnoreWhitespace = true;
                using (XmlReader reader = XmlReader.Create(dvdidXMLFile, xmlSettings))
                {
                    reader.MoveToContent();
                    string currentElement = reader.Name;

                    // some dvdid.xml files use upper case, some used mixed case
                    // we cannot read an element based on name because i
                    if (currentElement.ToUpper() == "DISC")
                    {
                        reader.Read();
                        while (!reader.IsStartElement()) reader.Read(); // skip to next element
                        currentElement = reader.Name;

                        if (currentElement.ToUpper() == "NAME")
                        {
                            reader.ReadStartElement(reader.Name);
                            //m_DVDName = reader.ReadString();
                            reader.ReadString();
                            reader.Read();
                            while (!reader.IsStartElement()) reader.Read(); // skip to next element
                            currentElement = reader.Name;
                            if (currentElement.ToUpper() == "ID")
                            {
                                reader.ReadStartElement(reader.Name);
                                dvdid = reader.ReadString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return dvdid;
        }

        private string GetDVDCacheFileName(string dvdid)
        {
            string xmlFileName = "";

            // dvd id should be of the format x|y where x is 8 char long and y is 8 char long
            if (dvdid.Length == 17 && dvdid[8] == '|')
            {
                xmlFileName = dvdid.Substring(0, 8) + "-" + dvdid.Substring(9) + ".xml";
            }
            else
            {
                // can be custom id
                xmlFileName = dvdid + ".xml";
            }
            return DVDCacheFolder + "\\" + xmlFileName; ;
        }

        // gets the local machine's VMC DVD cache folder
        private string DVDCacheFolder
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\eHome\\DvdInfoCache"; }
        }

        private string GetSuggestedMovieName(string dvdFolderNameFullPath)
        {
            string folderName = Path.GetFileName(dvdFolderNameFullPath);
            folderName = folderName.Trim();
            folderName = folderName.Replace('_', ' ');
            folderName = folderName.Replace('-', ' ');
            folderName = folderName.Replace('.', ' ');
            return folderName;
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
                // ignore any permission errors
            }
        }

    }
}
