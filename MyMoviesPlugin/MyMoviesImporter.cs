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
            Utilities.DebugLine("[MyMoviesImporter] Loading data for a new title");
            newTitle.MetadataSourceID = GetChildNodesValue(navigator, "WebServiceId");

            if (navigator.MoveToChild("Covers", ""))
            {
                Utilities.DebugLine("[MyMoviesImporter] Covers found, processing");
                if (navigator.MoveToChild("Front", ""))
                {
                    string imagePath = navigator.Value;
                    if (File.Exists(imagePath))
                    {
                        Utilities.DebugLine("[MyMoviesImporter] Front Cover: {0}", navigator.Value);
                        SetFrontCoverImage(ref newTitle, navigator.Value);
                    }
                    else
                    {
                        Utilities.DebugLine("[MyMoviesImporter] Front cover not found, looking for folder.jpg file");
                        string possibleImagePath = Path.Combine(
                                                        Path.GetDirectoryName(currentFile),
                                                        "folder.jpg");

                        if (File.Exists(possibleImagePath))
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Found folder.jpg file, we'll use that");
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
                        Utilities.DebugLine("[MyMoviesImporter] Found Back cover image");
                        SetBackCoverImage(ref newTitle, imagePath);
                    }
                    navigator.MoveToParent();
                }

                navigator.MoveToParent();
            }

            newTitle.Synopsis = GetChildNodesValue(navigator, "Description");

            if (navigator.MoveToChild("ProductionYear", ""))
            {
                Utilities.DebugLine("[MyMoviesImporter] Found production year, I hope the format is something we can read");
                try
                {
                    string year = navigator.Value;
                    if (!string.IsNullOrEmpty(year))
                    {
                        DateTime rls_date = new DateTime(int.Parse(year), 1, 1);
                        if (rls_date != null)
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Got it, loading the production year");
                            newTitle.ReleaseDate = rls_date;
                        }
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
                Utilities.DebugLine("[MyMoviesImporter] Scanning for the ParentalRating");
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
                                    Utilities.DebugLine("[MyMoviesImporter] This appears to be unrated");
                                    newTitle.ParentalRating = "Unrated";
                                    break;
                                case 1:
                                    Utilities.DebugLine("[MyMoviesImporter] This appears to be rated G");
                                    newTitle.ParentalRating = "G";
                                    break;
                                case 2:
                                    Utilities.DebugLine("[MyMoviesImporter] I have no idea what rating this is");
                                    break;
                                case 3:
                                    Utilities.DebugLine("[MyMoviesImporter] This appears to be rated PG");
                                    newTitle.ParentalRating = "PG";
                                    break;
                                case 4:
                                    Utilities.DebugLine("[MyMoviesImporter] This appears to be rated PG13");
                                    newTitle.ParentalRating = "PG13";
                                    break;
                                case 5:
                                    Utilities.DebugLine("[MyMoviesImporter] I have NO idea what rating this is");
                                    break;
                                case 6:
                                    Utilities.DebugLine("[MyMoviesImporter] This appears to be rated R");
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
                Utilities.DebugLine("[MyMoviesImporter] Beginning the long, painful process of scanning people");
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
                                    Utilities.DebugLine("[MyMoviesImporter] actor {0}, {1}", name, role);
                                    newTitle.AddActingRole(name, role);
                                    break;
                                case "Director":
                                    Person p = new Person(name);
                                    Utilities.DebugLine("[MyMoviesImporter] director {0}", name);
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
                Utilities.DebugLine("[MyMoviesImporter] Ahh... Studios (pssst.. we only copy the last entry from here... dont tell anyone)");
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
                Utilities.DebugLine("[MyMoviesImporter] Genres... good old genres");
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
                Utilities.DebugLine("[MyMoviesImporter] AudioTracks.. yeah, like you can even change them anyway");
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

                        Utilities.DebugLine("[MyMoviesImporter] Got one: {0}, {1}, {2}", audioLanguage, audioType, audioChannels);
                        newTitle.AddLanguageFormat(audioTrackString);
                    }
                    localNav.MoveToNext("AudioTrack", "");
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Subtitles", ""))
            {
                Utilities.DebugLine("[MyMoviesImporter] Subtitles here we come!");
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
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Subtitle {0}", subtitleLanguage);
                            newTitle.AddSubtitle(subtitleLanguage);
                        }

                        localNav.MoveToNext("Subtitle", "");
                    }
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Discs", ""))
            {
                Utilities.DebugLine("[MyMoviesImporter] Discs... ok here is the good one, we'll passing this off to some other method to handle.");
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
            Utilities.DebugLine("[MyMoviesImporter] {0} entries found", nIter.Count);
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

                Utilities.DebugLine("[MyMoviesImporter] Ok, I've collected some info... now we actually start looking for files");
                IList<Disk> sideAdisks = GetDisksForLocation(sideALocation, sideALocationType);

                int j = 1;
                Utilities.DebugLine("[MyMoviesImporter] Ok, we found {0} possible files", sideAdisks.Count);
                foreach (Disk disk in sideAdisks)
                {
                    disk.Name = string.Format(@"Disk{0}", j++);
                    newTitle.Disks.Add(disk);
                }

                if (isDoubleSided)
                {
                    Utilities.DebugLine("[MyMoviesImporter] Whoa... this thing appears to be double-sided (old schooling it huh?)");
                    IList<Disk> sideBdisks = GetDisksForLocation(sideBLocation, sideBLocationType);
                    Utilities.DebugLine("[MyMoviesImporter] We found {0} disks on the B side", sideBdisks.Count);
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
            Utilities.DebugLine("[MyMoviesImporter] GetDisksForLocation({0}) of type {1}", location, locationType);
            List<Disk> disks = new List<Disk>();
            if (!string.IsNullOrEmpty(location))
            {
                if (location.CompareTo(".") == 0)
                {
                    Utilities.DebugLine("[MyMoviesImporter] Your disk entry appears to be either empty or contain a '.', we're gonna look in the current directory");
                    location = Path.GetDirectoryName(currentFile);
                    Utilities.DebugLine("[MyMoviesImporter] New Location: {0}", location);
                }

                string fullPath = Path.GetFullPath(location);
                Utilities.DebugLine("[MyMoviesImporter] And we think the full path is: {0}", fullPath);

                int iLocationType = int.Parse(locationType);
                switch (iLocationType)
                {
                    case 1:
                        // online folder
                        Utilities.DebugLine("[MyMoviesImporter] We think this is a directory");
                        if (Directory.Exists(fullPath))
                        {
                            if (MediaData.IsDVD(fullPath))
                            {
                                Utilities.DebugLine("[MyMoviesImporter] its dvd");
                                Disk disk = DiskForFormatAndLocation(VideoFormat.DVD, fullPath);
                                disks.Add(disk);
                                break;
                            }

                            if (MediaData.IsBluRay(fullPath))
                            {
                                Utilities.DebugLine("[MyMoviesImporter] its hddvd");
                                Disk disk = DiskForFormatAndLocation(VideoFormat.BLURAY, fullPath);
                                disks.Add(disk);
                                break;
                            }

                            if (MediaData.IsHDDVD(fullPath))
                            {
                                Utilities.DebugLine("[MyMoviesImporter] its bluray");
                                Disk disk = DiskForFormatAndLocation(VideoFormat.HDDVD, fullPath);
                                disks.Add(disk);
                                break;
                            }

                            Utilities.DebugLine("[MyMoviesImporter] no idea, searching for files in {0}", location);
                            IList<string> files = GetVideoFilesForFolder(location);
                            foreach (string fileName in files)
                            {
                                string ext = Path.GetExtension(fileName);
                                ext = ext.Substring(1);
                                ext = ext.Replace("-", string.Empty);
                                VideoFormat format = FormatForExtension(ext);
                                if (format != VideoFormat.UNKNOWN)
                                {
                                    Utilities.DebugLine("[MyMoviesImporter] got one, its {0} (at: {1})", fileName, format);
                                    Disk disk = DiskForFormatAndLocation(format, fileName);
                                    disks.Add(disk);
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
                        Utilities.DebugLine("[MyMoviesImporter] We think this is a file");
                        if (File.Exists(fullPath))
                        {
                            string ext = Path.GetExtension(fullPath);
                            ext = ext.Substring(1);
                            ext.Replace("-", string.Empty);
                            VideoFormat format = FormatForExtension(ext);

                            if (format != VideoFormat.UNKNOWN)
                            {
                                Utilities.DebugLine("[MyMoviesImporter] Ok, found something here, it appears to be {0} with a format if {1}", fullPath, format);
                                Disk disk = DiskForFormatAndLocation(format, fullPath);
                                disks.Add(disk);
                            }
                        }
                        else
                        {
                            AddError("Location {0} is of type file but the file doesn't appear to exist", fullPath);
                        }
                        break;
                    case 3:
                        Utilities.DebugLine("[MyMoviesImporter] Seriously... mymovies says this is an offline disk... how are we supposed to import an offline disk?\nQuick, point us to your nearest dvd shelf!");
                        Disk t3disk = new Disk();
                        t3disk.Format = VideoFormat.OFFLINEDVD;
                        disks.Add(t3disk);
                        // offline dvd
                        break;
                    case 4:
                        // dvd changer
                        Utilities.DebugLine("[MyMoviesImporter] Do you see any dvd changers around here?  Cause I dont!");
                        Disk t4disk = new Disk();
                        t4disk.Format = VideoFormat.OFFLINEDVD;
                        disks.Add(t4disk);
                        break;

                    case 5:
                        // media changer of some kind, treat as 4
                        Utilities.DebugLine("[MyMoviesImporter] Ah, upgraded to a Media Changer hey? Nice... TO BAD WE DONT SUPPORT THOSE!");
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
            Utilities.DebugLine("[MyMoviesImporter] Ok, I got nothing... hopefully the double-check in the validate method will find something");
            return disks;
        }


        private Disk DiskForFormatAndLocation(VideoFormat format, string location)
        {
            Disk disk = new Disk();
            disk.Format = format;
            disk.Path = location;

            return disk;
        }

        private VideoFormat FormatForExtension(string ext)
        {
            VideoFormat format;
            Utilities.DebugLine("[MyMoviesImporter] Attempting to determine format based on extension {0}", ext);
            try
            {
                format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                Utilities.DebugLine("[MyMoviesImporter] Format appears to be {0}", format);
                return format;
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[MyMoviesImporter] Format is Unknown.. this means its useless: {0}", ex);
            }
            return VideoFormat.UNKNOWN;
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
            Utilities.DebugLine("[MyMoviesImporter] You want the format for what exactly? {0}", location);
            int type = Int32.Parse(locationType);

            switch (type)
            {
                case 1:
                    //online folder
                    if (Directory.Exists(location))
                    {
                        if (MediaData.IsBluRay(location))
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Nice.. you appear to have a bluray drive (or file, you sneaky dog)");
                            return VideoFormat.BLURAY;
                        }

                        if (MediaData.IsHDDVD(location))
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Ah.. hddvd nice (yeah last year... you know these lost the format war right?)");
                            return VideoFormat.HDDVD;
                        }

                        if (MediaData.IsDVD(location))
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Here be a dvd or video_ts folder (does it really matter which?)");
                            return VideoFormat.DVD;
                        }
                        // unable to determine disc, there must be a file flagged as a folder... go check for files
                    }
                    break;
                case 2:
                    // online file
                    Utilities.DebugLine("[MyMoviesImporter] File time!");
                    string extension = Path.GetExtension(location);
                    extension = extension.Substring(1);
                    extension.Replace("-", string.Empty);
                    VideoFormat format;
                    try
                    {
                        format = (VideoFormat)Enum.Parse(typeof(VideoFormat), extension, true);
                        Utilities.DebugLine("[MyMoviesImporter] Ok, I looked for the format and all I got was the lame comment line {0}", format);
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
            Utilities.DebugLine("[MyMoviesImporter] This is the part where we validate (or not) your shiny new title, wouldn't want you to drive off the lot with it untested now would we");
            if (title_to_validate.Disks.Count == 0)
            {
                Utilities.DebugLine("[MyMoviesImporter] Whoa... you dont appear to have any discs... lets double check that");
                string directoryName = Path.GetDirectoryName(file);
                if (Directory.Exists(directoryName))
                {
                    if (MediaData.IsBluRay(directoryName))
                    {
                        Utilities.DebugLine("[MyMoviesImporter] its a bluray, adding a new disk");
                        title_to_validate.Disks.Add(new Disk("Disk1", directoryName, VideoFormat.BLURAY));
                        return true;
                    }

                    if (MediaData.IsHDDVD(directoryName))
                    {
                        Utilities.DebugLine("[MyMoviesImporter] its an hddvd, adding a new disk");
                        title_to_validate.Disks.Add(new Disk("Disk1", directoryName, VideoFormat.HDDVD));
                        return true;
                    }

                    if (MediaData.IsDVD(directoryName))
                    {
                        Utilities.DebugLine("[MyMoviesImporter] its a dvd, adding a new disk");
                        title_to_validate.Disks.Add(new Disk("Disk1", directoryName, VideoFormat.DVD));
                        return true;
                    }

                    string[] files = Directory.GetFiles(directoryName);
                    Utilities.DebugLine("[MyMoviesImporter] You have {0} files in the current location {1}", files.Length, directoryName);
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
                                    Utilities.DebugLine("[MyMoviesImporter] Checking file for format {0}", files[i]);
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
                            Utilities.DebugLine("[MyMoviesImporter] Yeah, no extention on file ({0}), skip it!", files[i]);
                            // didnt get the extension, its not a valid file, skip it
                        }
                    }
                    if (title_to_validate.Disks.Count == 0)
                    {
                        Utilities.DebugLine("[MyMoviesImporter] No disks found on the title, we'll skip it");
                        return false;
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
            Utilities.DebugLine("[MyMoviesImporter] Looking for video files in a folder, {0} folder in fact.", folder);
            List<string> fileNames = new List<string>();

            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder);
                Array.Sort(files);
                foreach (string fName in files)
                {
                    Utilities.DebugLine("[MyMoviesImporter] Found file {0}, I don't (yet) care what file type, just throw it on the stack", fName);
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