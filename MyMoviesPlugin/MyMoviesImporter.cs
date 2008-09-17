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

namespace MyMoviesPlugin
{
    public class MyMoviesImporter : OMLPlugin, IOMLPlugin
    {
        private static double MajorVersion = 0.9;
        private static double MinorVersion = 0.2;
        private string currentFile = string.Empty;

        public MyMoviesImporter() : base()
        {
            Utilities.DebugLine("[MyMoviesImporter] created");
        }
        public override string SetupDescription()
        {
            return GetName() + @" will search for and import [" + DefaultFileToImport() + @"] files.";
        }
        public override bool IsSingleFileImporter()
        {
            return false;
        }
        protected override bool GetFolderSelect()
        {
            return true;
        }
        public override void ProcessFile(string file)
        {            
            Utilities.DebugLine("[MyMoviesImporter] created[filename("+file+"), ShouldCopyImages("+CopyImages+")]");

            currentFile = file;
            if (File.Exists(currentFile))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(file);
                Utilities.DebugLine("[MyMoviesImporter] file loaded");

                XmlNodeList nodeList = xDoc.SelectNodes("//Titles/Title");
                if (nodeList.Count == 0)
                    nodeList = xDoc.SelectNodes("//Title");

                foreach (XmlNode movieNode in nodeList)
                {
                    Utilities.DebugLine("[MyMoviesImporter] Found base Title node");

                    Title newTitle = new Title();

                    XPathNavigator navigator = movieNode.CreateNavigator();
                    newTitle.Name = GetChildNodesValue(navigator, "LocalTitle");
                    loadDataFromNavigatorToTitle(ref navigator, ref newTitle);

                    if (ValidateTitle(newTitle, file))
                    {
                        Utilities.DebugLine("[MyMoviesImporter] Validating title");
                        try { AddTitle(newTitle); }
                        catch (Exception e) { Utilities.DebugLine("[MyMoviesImporter] Error adding row: " + e.Message); }
                    }
                    else Utilities.DebugLine("[MyMoviesImporter] Error saving row");
                }
            }
            else
            {
                AddError("File not found when trying to load file: {0}", currentFile);
            }
        }
        protected override double GetVersionMajor()
        {
            return MajorVersion;
        }
        protected override double GetVersionMinor()
        {
            return MinorVersion;
        }
        protected override bool GetCanCopyImages()
        {
            return true;
        }
        protected override string GetMenu()
        {
            return "MyMovies";
        }
        protected override string GetName()
        {
            return "MyMoviesPlugin";
        }
        protected override string GetAuthor()
        {
            return "OML Development Team";
        }
        protected override string GetDescription()
        {
            return "MyMovies xml file importer v" + Version;
        }
        public override bool Load(string dirName)
        {
            ProcessDir(dirName);
            return true;
        }
        public override void DoWork(string[] thework)
        {
            ProcessDir(thework[0]);
        }
        public override void ProcessDir(string startFolder)
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
                    Utilities.DebugLine("[MyMoviesImporter] folder " + currentFolder);
                 
                    string[] fileNames = null;
                    try
                    {
                        fileNames = Directory.GetFiles(currentFolder, "mymovies.xml");
                    }
                    catch
                    {
                        fileNames = null;
                    }

                    if (fileNames != null)
                    {
                        foreach (string filename in fileNames)
                        {
                            if (filename.ToLower().EndsWith(@"mymovies.xml"))
                                ProcessFile(filename);

                            if (filename.ToLower().EndsWith(@"titles.xml"))
                                ProcessFile(filename);
                        }
                    }
                } // loop through the sub folders
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[MyMoviesImporter] An error occured: " + ex.Message);
            }
        }
        private void loadDataFromNavigatorToTitle(ref XPathNavigator navigator, ref Title newTitle)
        {
            newTitle.MetadataSourceID = GetChildNodesValue(navigator, "WebServiceId");

            if (navigator.MoveToChild("Covers", ""))
            {
                if (navigator.MoveToChild("Front", ""))
                {
                    string imagePath = navigator.Value;
                    if (File.Exists(imagePath))
                    {
                        SetFrontCoverImage(ref newTitle, navigator.Value);
                    }
                    else
                    {
                        string possibleImagePath = Path.Combine(
                                                        Path.GetDirectoryName(currentFile),
                                                        "folder.jpg");

                        if (File.Exists(possibleImagePath))
                        {
                            SetFrontCoverImage(ref newTitle, possibleImagePath);
                        }
                    }
                    navigator.MoveToParent();
                }

                if (navigator.MoveToChild("Back", ""))
                {
                    string imagePath = navigator.Value;
                    if (File.Exists(imagePath))
                    {
                        SetBackCoverImage(ref newTitle, imagePath);
                    }
                    navigator.MoveToParent();
                }

                navigator.MoveToParent();
            }

            newTitle.Synopsis = GetChildNodesValue(navigator, "Description");

            if (navigator.MoveToChild("ProductionYear", ""))
            {
                try
                {
                    string year = navigator.Value;
                    if (!string.IsNullOrEmpty(year))
                    {
                        DateTime rls_date = new DateTime(int.Parse(year), 1, 1);
                        if (rls_date != null)
                            newTitle.ReleaseDate = rls_date;
                    }
                    navigator.MoveToParent();
                }
                catch (Exception e)
                {
                    navigator.MoveToParent();
                    Utilities.DebugLine("[MyMoviesImporter] error reading ProductionYear: " + e.Message);
                }
            }

            if (navigator.MoveToChild("ParentalRating", ""))
            {
                if (navigator.HasChildren)
                {
                    string ratingId = GetChildNodesValue(navigator, "Value");
                    if (!string.IsNullOrEmpty(ratingId))
                    {
                        int mmRatingId;
                        if (int.TryParse(ratingId, out mmRatingId))
                            switch (mmRatingId)
                            {
                                case 0:
                                    newTitle.ParentalRating = "Unrated";
                                    break;
                                case 1:
                                    newTitle.ParentalRating = "G";
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    newTitle.ParentalRating = "PG";
                                    break;
                                case 4:
                                    newTitle.ParentalRating = "PG13";
                                    break;
                                case 5:
                                    break;
                                case 6:
                                    newTitle.ParentalRating = "R";
                                    break;
                            }
                        else
                            Utilities.DebugLine("[MyMoviesImporter] Error parsing rating: {0} not a number", ratingId);
                    }
                }
                navigator.MoveToParent();
            }

            newTitle.Runtime = Int32.Parse(GetChildNodesValue(navigator, "RunningTime"));

            if (navigator.MoveToChild("Persons", ""))
            {
                if (navigator.HasChildren)
                {
                    XPathNodeIterator nIter = navigator.SelectChildren("Person", "");
                    navigator.MoveToFirstChild();
                    XPathNavigator localNav = navigator.CreateNavigator();
                    navigator.MoveToParent();
                    for (int i = 0; i < nIter.Count; i++)
                    {
                        string name = GetChildNodesValue(localNav, "Name");
                        string role = GetChildNodesValue(localNav, "Role");
                        string type = GetChildNodesValue(localNav, "Type");

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(type))
                        {
                            switch (type)
                            {
                                case "Actor":
                                    newTitle.AddActingRole(name, role);
                                    break;
                                case "Director":
                                    Person p = new Person(name);
                                    newTitle.AddDirector(p);
                                    break;
                                default:
                                    break;
                            }
                        }
                        localNav.MoveToNext("Person", "");
                    }
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Studios", ""))
            {
                if (navigator.HasChildren)
                {
                    XPathNodeIterator nIter = navigator.SelectChildren("Studio", "");
                    navigator.MoveToFirstChild();
                    XPathNavigator localNav = navigator.CreateNavigator();
                    navigator.MoveToParent();
                    for (int i = 0; i < nIter.Count; i++)
                    {
                        newTitle.Studio = GetChildNodesValue(localNav, "Studio");
                    }
                }
                navigator.MoveToParent();
            }

            newTitle.CountryOfOrigin = GetChildNodesValue(navigator, "Country");
            newTitle.AspectRatio = GetChildNodesValue(navigator, "AspectRatio");
            newTitle.OriginalName = GetChildNodesValue(navigator, "OriginalTitle");
            newTitle.SortName = GetChildNodesValue(navigator, "SortTitle");

            if (navigator.MoveToChild("Genres", ""))
            {
                XPathNodeIterator nIter = navigator.SelectChildren("Genre", "");
                navigator.MoveToFirstChild();
                XPathNavigator localNav = navigator.CreateNavigator();
                navigator.MoveToParent();
                for (int i = 0; i < nIter.Count; i++)
                {
                    newTitle.AddGenre(localNav.Value);
                    localNav.MoveToNext("Genre", "");
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("AudioTracks", ""))
            {
                XPathNodeIterator nIter = navigator.SelectChildren("AudioTrack", "");
                navigator.MoveToFirstChild();
                XPathNavigator localNav = navigator.CreateNavigator();
                navigator.MoveToParent();
                for (int i = 0; i < nIter.Count ; i++)
                {
                    string audioLanguage = localNav.GetAttribute("Language", "");
                    string audioType = localNav.GetAttribute("Type", "");
                    string audioChannels = localNav.GetAttribute("Channels", "");

                    if (!string.IsNullOrEmpty(audioLanguage))
                    {
                        string audioTrackString = audioLanguage;
                        if (!string.IsNullOrEmpty(audioType))
                            audioTrackString += string.Format(", {0}", audioType);

                        if (!string.IsNullOrEmpty(audioChannels))
                            audioTrackString += string.Format(", {0}", audioChannels);

                        newTitle.AddLanguageFormat(audioTrackString);
                    }
                    localNav.MoveToNext("AudioTrack", "");
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Subtitles", ""))
            {
                if (navigator.GetAttribute("NotPresent", "").CompareTo("False") == 0)
                {
                    XPathNodeIterator nIter = navigator.SelectChildren("Subtitle", "");
                    navigator.MoveToFirstChild();
                    XPathNavigator localNav = navigator.CreateNavigator();
                    navigator.MoveToParent();
                    for (int i = 0; i < nIter.Count; i++)
                    {
                        string subtitleLanguage = localNav.GetAttribute("Language", "");
                        if (!string.IsNullOrEmpty(subtitleLanguage))
                            newTitle.AddSubtitle(subtitleLanguage);

                        localNav.MoveToNext("Subtitle", "");
                    }
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Discs", ""))
            {
                XPathNodeIterator nIter = navigator.SelectChildren("Disc", "");
                navigator.MoveToFirstChild();
                XPathNavigator localNav = navigator.CreateNavigator();
                navigator.MoveToParent();
                extractDisksFromXML(nIter, localNav, newTitle);
            }
        }
        private void extractDisksFromXML(XPathNodeIterator nIter, XPathNavigator localNav, Title newTitle)
        {
            Utilities.DebugLine("[MyMoviesImporter] Scanning for Discs...");
            for (int i = 0; i < nIter.Count; i++)
            {
                bool isDoubleSided = (((string)GetChildNodesValue(localNav, "DoubleSided")).CompareTo("False") == 0)
                                     ? false : true;

                string discName = (string)GetChildNodesValue(localNav, "Name");
                string sideAId = (string)GetChildNodesValue(localNav, "DiscIdSideA");
                string sideALocation = (string)GetChildNodesValue(localNav, "LocationSideA");
                string sideALocationType = (string)GetChildNodesValue(localNav, "LocationTypeSideA");
                string changerSlot = (string)GetChildNodesValue(localNav, "ChangerSlot");

                string sideBId = string.Empty;
                string sideBLocation = string.Empty;
                string sideBLocationType = string.Empty;
                if (isDoubleSided)
                {
                    sideBId = (string)GetChildNodesValue(localNav, "DiscIdSideB");
                    sideBLocation = (string)GetChildNodesValue(localNav, "LocationSideB");
                    sideBLocationType = (string)GetChildNodesValue(localNav, "LocationTypeSideB");
                }

                IList<Disk> sideAdisks = GetDisksForLocation(sideALocation, sideALocationType);

                int j = 1;
                foreach (Disk disk in sideAdisks)
                {
                    disk.Name = string.Format(@"Disk{0}", j++);
                    newTitle.Disks.Add(disk);
                }

                if (isDoubleSided)
                {
                    IList<Disk> sideBdisks = GetDisksForLocation(sideBLocation, sideBLocationType);
                    foreach (Disk disk in sideBdisks)
                    {
                        disk.Name = string.Format(@"Disk{0}", j++);
                        newTitle.Disks.Add(disk);
                    }
                }

                localNav.MoveToNext("Disc", "");
            }
        }
        private IList<Disk> GetDisksForLocation(string location, string locationType)
        {
            List<Disk> disks = new List<Disk>();
            if (!string.IsNullOrEmpty(location))
            {
                if (location.CompareTo(".") == 0)
                {
                    location = Path.GetDirectoryName(currentFile);
                }

                string fullPath = Path.GetFullPath(location);

                int iLocationType = int.Parse(locationType);
                switch (iLocationType)
                {
                    case 1:
                        // online folder
                        if (Directory.Exists(fullPath))
                        {
                            if (MediaData.IsDVD(fullPath))
                            {
                                Disk disk = new Disk();
                                disk.Format = VideoFormat.DVD;
                                disk.Path = fullPath;
                                disks.Add(disk);
                                break;
                            }

                            if (MediaData.IsBluRay(fullPath))
                            {
                                Disk disk = new Disk();
                                disk.Format = VideoFormat.BLURAY;
                                disk.Path = fullPath;
                                disks.Add(disk);
                                break;
                            }

                            if (MediaData.IsHDDVD(fullPath))
                            {
                                Disk disk = new Disk();
                                disk.Format = VideoFormat.HDDVD;
                                disk.Path = fullPath;
                                disks.Add(disk);
                                break;
                            }

                            IList<string> files = GetVideoFilesForFolder(location);
                            foreach (string fileName in files)
                            {
                                string ext = Path.GetExtension(fileName);
                                ext = ext.Substring(1);
                                ext = ext.Replace("-", string.Empty);
                                VideoFormat format;
                                try
                                {
                                    format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                    if (format != VideoFormat.UNKNOWN)
                                    {
                                        Disk disk = new Disk();
                                        disk.Format = format;
                                        disk.Path = fileName;
                                        disks.Add(disk);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddError("Error parsing extention: {0}, {1}", ext, ex);
                                }
                            }
                            break;
                        }
                        else
                        {
                            AddError("Location {0} is of type folder but the folder doesn't appear to exist", fullPath);
                        }
                        break;
                    case 2:
                        // online file
                        if (File.Exists(fullPath))
                        {
                            string ext = Path.GetExtension(fullPath);
                            ext = ext.Substring(1);
                            ext.Replace("-", string.Empty);
                            VideoFormat format;
                            try
                            {
                                format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                if (format != VideoFormat.UNKNOWN)
                                {
                                    Disk disk = new Disk();
                                    disk.Format = format;
                                    disk.Path = fullPath;
                                    disks.Add(disk);
                                }
                            }
                            catch (Exception ex)
                            {
                                AddError("Unable to parse format for ext: {0}, {1}", ext, ex);
                            }
                        }
                        else
                        {
                            AddError("Location {0} is of type file but the file doesn't appear to exist", fullPath);
                        }
                        break;
                    case 3:
                        Disk t3disk = new Disk();
                        t3disk.Format = VideoFormat.OFFLINEDVD;
                        disks.Add(t3disk);
                        // offline dvd
                        break;
                    case 4:
                        // dvd changer
                        Disk t4disk = new Disk();
                        t4disk.Format = VideoFormat.OFFLINEDVD;
                        disks.Add(t4disk);
                        break;

                    case 5:
                        // media changer of some kind, treat as 4
                        Disk t5disk = new Disk();
                        t5disk.Format = VideoFormat.OFFLINEDVD;
                        disks.Add(t5disk);
                        break;
                    default:
                        // no idea...
                        AddError("I have NO idea what type of video is available at {0}", location);
                        break;
                }
            }
            return disks;
        }
        private string GetChildNodesValue(XPathNavigator nav, string nodeName)
        {
            string value = string.Empty;
            if (nav.MoveToChild(nodeName, ""))
            {
                value = nav.Value;
                nav.MoveToParent();
            }
            return value;
        }
        public enum MyMoviesLocationType { Folder = 1, File };
        private VideoFormat GetVideoFormatForLocation(string location, string locationType)
        {
            int type = Int32.Parse(locationType);

            switch (type)
            {
                case 1:
                    //online folder
                    if (Directory.Exists(location))
                    {
                        if (MediaData.IsBluRay(location))
                            return VideoFormat.BLURAY;

                        if (MediaData.IsHDDVD(location))
                            return VideoFormat.HDDVD;

                        if (MediaData.IsDVD(location))
                            return VideoFormat.DVD;
                        // unable to determine disc, there must be a file flagged as a folder... go check for files
                    }
                    break;
                case 2:
                    // online file
                    string extension = Path.GetExtension(location);
                    extension = extension.Substring(1);
                    extension.Replace("-", string.Empty);
                    VideoFormat format;
                    try
                    {
                        format = (VideoFormat)Enum.Parse(typeof(VideoFormat), extension, true);
                    }
                    catch (Exception ex)
                    {
                        AddError("Error parsing format for extension: {0}, {1}", extension, ex);
                        format = VideoFormat.UNKNOWN;
                    }

                    return format;
                case 3:
                    // offline dvd
                    break;
                case 4:
                    // dvd changer
                    break;
                case 5:
                    // media changer of some kind, treat as 4
                default:
                    return VideoFormat.UNKNOWN;
            }

            return VideoFormat.UNKNOWN;
        }
        public bool ValidateTitle(Title title_to_validate, string file)
        {
            if (title_to_validate.Disks.Count == 0)
            {
                string directoryName = Path.GetDirectoryName(file);
                if (Directory.Exists(directoryName))
                {
                    if (MediaData.IsBluRay(directoryName))
                    {
                        title_to_validate.Disks.Add(new Disk("Disk1", directoryName, VideoFormat.BLURAY));
                        return true;
                    }

                    if (MediaData.IsHDDVD(directoryName))
                    {
                        title_to_validate.Disks.Add(new Disk("Disk1", directoryName, VideoFormat.HDDVD));
                        return true;
                    }

                    if (MediaData.IsDVD(directoryName))
                    {
                        title_to_validate.Disks.Add(new Disk("Disk1", directoryName, VideoFormat.DVD));
                        return true;
                    }

                    string[] files = Directory.GetFiles(directoryName);
                    Array.Sort(files);
                    for (int i = 0; i < files.Length; i++)
                    {

                        string ext = Path.GetExtension(files[i]).Substring(1);
                        try
                        {
                            if (new List<string>(Enum.GetNames(typeof(VideoFormat))).Contains(ext.ToUpper()))
                            {
                                VideoFormat format;
                                try
                                {
                                    format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                    Disk disk = new Disk();
                                    disk.Name = string.Format("Disk{0}", i + 1);
                                    disk.Path = Path.Combine(directoryName, files[i]);
                                    disk.Format = format;
                                    title_to_validate.Disks.Add(disk);
                                }
                                catch (Exception)
                                {
                                    AddError("Unable to determine format for extension: {0}", ext);
                                }
                            }
                        }
                        catch
                        {
                            // didnt get the extension, its not a valid file, skip it
                        }
                    }
                }
            }
            return true;
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
                    GetSubFolders(folder.FullName, folderList);
                }

            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[MyMoviesImporter] An error occured: " + ex.Message);
                // ignore any permission errors
            }
        }
        private IList<string> GetVideoFilesForFolder(string folder)
        {
            List<string> fileNames = new List<string>();

            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder);
                Array.Sort(files);
                foreach (string fName in files)
                {
                    fileNames.Add(Path.GetFullPath(fName));
                }
            }
            else
            {
                AddError("Disc entry specified folder ({0}) but that folder doesn't appear to exist", folder);
            }
            return fileNames;
        }
    }
}
