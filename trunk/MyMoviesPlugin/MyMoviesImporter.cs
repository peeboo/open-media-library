using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            SDKUtilities.DebugLine("[MyMoviesImporter] created");
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
            SDKUtilities.DebugLine("[MyMoviesImporter] created[filename("+file+")]");

            currentFile = file;
            if (File.Exists(currentFile))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(file);
                SDKUtilities.DebugLine("[MyMoviesImporter] file loaded");

                XmlNodeList nodeList = xDoc.SelectNodes("//Titles/Title");
                if (nodeList.Count == 0)
                    nodeList = xDoc.SelectNodes("Title");

                foreach (XmlNode movieNode in nodeList)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Found base Title node");

                    OMLSDKTitle newTitle = new OMLSDKTitle();

                    XPathNavigator navigator = movieNode.CreateNavigator();
                    newTitle.Name = GetChildNodesValue(navigator, "LocalTitle");
                    loadDataFromNavigatorToTitle(ref navigator, ref newTitle);

                    if (ValidateTitle(newTitle, file))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] Validating title");
                        try { AddTitle(newTitle); }
                        catch (Exception e) { SDKUtilities.DebugLine("[MyMoviesImporter] Error adding row: " + e.Message); }
                    }
                    else SDKUtilities.DebugLine("[MyMoviesImporter] Error saving row");
                }
            }
            else
            {
                AddError("[MyMoviesImporter] File not found when trying to load file: {0}", currentFile);
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
                GetSubFolders(startFolder, ref dirList);

                // the share or link may not exist nowbb
                if (Directory.Exists(startFolder))
                    dirList.Add(startFolder);

                foreach (string currentFolder in dirList)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Checking Folder " + currentFolder);
                 
                    string[] fileNames = null;
                    try
                    {
                        fileNames = Directory.GetFiles(currentFolder, @"*.xml");
                    }
                    catch (Exception ex)
                    {
                        SDKUtilities.DebugLine("Failed to locate files for dir ({0}): {1}", currentFolder, ex.Message);
                        fileNames = null;
                    }

                    if (fileNames != null)
                    {
                        foreach (string filename in fileNames)
                        {
                            SDKUtilities.DebugLine("Checking xml file {0}", filename);
                            if (filename.ToLower().EndsWith(@"mymovies.xml"))
                            {
                                SDKUtilities.DebugLine("ProcessFile called on: {0} in folder {1}", filename, currentFolder);
                                ProcessFile(filename);
                            }

                            if (filename.ToLower().EndsWith(@"titles.xml"))
                            {
                                SDKUtilities.DebugLine("ProcessFile called on {0} in folder {1}", filename, currentFolder);
                                ProcessFile(filename);
                            }
                        }
                    }
                } // loop through the sub folders
            }
            catch (Exception ex)
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] An error occured: " + ex.Message);
            }
        }
        private void loadDataFromNavigatorToTitle(ref XPathNavigator navigator, ref OMLSDKTitle newTitle)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] Loading data for a new title");
            newTitle.MetadataSourceID = GetChildNodesValue(navigator, "WebServiceID");

            #region covers
            SDKUtilities.DebugLine("[MyMoviesImporter] Scanning for {0} node", "Covers");
            if (navigator.MoveToChild("Covers", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Covers found, processing");
                SDKUtilities.DebugLine("[MyMoviesImporter] Scanning for {0} node", "Front");
                if (navigator.MoveToChild("Front", ""))
                {
                    string imagePath = navigator.Value;
                    string finalImagePath = FindFinalImagePath(imagePath);
                    SDKUtilities.DebugLine("[MyMoviesImporter] Final image path is: {0}", finalImagePath);
                    if (File.Exists(finalImagePath))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] This file appears to be valid, we'll set it on the title");
                        newTitle.FrontCoverPath = finalImagePath;
                    }
                    navigator.MoveToParent();
                }
                SDKUtilities.DebugLine("[MyMoviesImporter] Scanning for {0} node", "Back");
                if (navigator.MoveToChild("Back", ""))
                {
                    string imagePath = navigator.Value;
                    if (File.Exists(imagePath))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] Found Back cover image");
                        newTitle.BackCoverPath = imagePath;
                    }
                    navigator.MoveToParent();
                }

                navigator.MoveToParent();
            }
            #endregion

            newTitle.Synopsis = GetChildNodesValue(navigator, "Description");

            #region production year
            if (navigator.MoveToChild("ProductionYear", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Found production year, I hope the format is something we can read");
                try
                {
                    string year = navigator.Value;
                    if (!string.IsNullOrEmpty(year))
                    {
                        DateTime rls_date = new DateTime(int.Parse(year), 1, 1);
                        if (rls_date != null)
                        {
                            SDKUtilities.DebugLine("[MyMoviesImporter] Got it, loading the production year");
                            newTitle.ReleaseDate = rls_date;
                        }
                    }
                    navigator.MoveToParent();
                }
                catch (Exception e)
                {
                    navigator.MoveToParent();
                    SDKUtilities.DebugLine("[MyMoviesImporter] error reading ProductionYear: " + e.Message);
                }
            }
            #endregion

            #region parental rating (mpaa rating)
            if (navigator.MoveToChild("ParentalRating", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Scanning for the ParentalRating");
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
                                    SDKUtilities.DebugLine("[MyMoviesImporter] This appears to be unrated");
                                    newTitle.ParentalRating = "Unrated";
                                    break;
                                case 1:
                                    SDKUtilities.DebugLine("[MyMoviesImporter] This appears to be rated G");
                                    newTitle.ParentalRating = "G";
                                    break;
                                case 2:
                                    SDKUtilities.DebugLine("[MyMoviesImporter] I have no idea what rating this is");
                                    break;
                                case 3:
                                    SDKUtilities.DebugLine("[MyMoviesImporter] This appears to be rated PG");
                                    newTitle.ParentalRating = "PG";
                                    break;
                                case 4:
                                    SDKUtilities.DebugLine("[MyMoviesImporter] This appears to be rated PG13");
                                    newTitle.ParentalRating = "PG13";
                                    break;
                                case 5:
                                    SDKUtilities.DebugLine("[MyMoviesImporter] I have NO idea what rating this is");
                                    break;
                                case 6:
                                    SDKUtilities.DebugLine("[MyMoviesImporter] This appears to be rated R");
                                    newTitle.ParentalRating = "R";
                                    break;
                            }
                        else
                            SDKUtilities.DebugLine("[MyMoviesImporter] Error parsing rating: {0} not a number", ratingId);
                    }
                    string ratingReason = GetChildNodesValue(navigator, "Description");
                    if (!string.IsNullOrEmpty(ratingReason))
                        newTitle.ParentalRatingReason = ratingReason;
                }
                navigator.MoveToParent();
            }
            #endregion

            newTitle.Runtime = Int32.Parse(GetChildNodesValue(navigator, "RunningTime"));            

            #region persons
            if (navigator.MoveToChild("Persons", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Beginning the long, painful process of scanning people");
                if (navigator.HasChildren)
                {
                    XPathNodeIterator nIter = navigator.SelectChildren("Person", "");
                    if (navigator.MoveToFirstChild())
                    {
                        XPathNavigator localNav = navigator.CreateNavigator();
                        navigator.MoveToParent();
                        for (int i = 0; i < nIter.Count; i++)
                        {
                            string name = GetChildNodesValue(localNav, "Name");
                            string role = GetChildNodesValue(localNav, "Role");
                            string type = GetChildNodesValue(localNav, "Type");

                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
                            {
                                switch (type)
                                {
                                    case "Actor":
                                        SDKUtilities.DebugLine("[MyMoviesImporter] actor {0}, {1}", name, role);

                                        newTitle.AddActingRole(name, role);
                                        break;
                                    case "Director":
                                        SDKUtilities.DebugLine("[MyMoviesImporter] director {0}", name);

                                        newTitle.AddDirector(new OMLSDKPerson(name));
                                        break;

                                    default:
                                        break;
                                }
                            }
                            localNav.MoveToNext("Person", "");
                        }
                    }
                }
                navigator.MoveToParent();
            }
            #endregion

            #region studio (s)
            if (navigator.MoveToChild("Studios", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Ahh... Studios (pssst.. we only copy the last entry from here... dont tell anyone)");
                if (navigator.HasChildren)
                {
                    XPathNodeIterator nIter = navigator.SelectChildren("Studio", "");
                    if (navigator.MoveToFirstChild())
                    {
                        XPathNavigator localNav = navigator.CreateNavigator();
                        navigator.MoveToParent();
                        for (int i = 0; i < nIter.Count; i++)
                        {
                            newTitle.Studio = localNav.Value;
                        }
                    }
                }
                navigator.MoveToParent();
            }
            #endregion

            newTitle.CountryOfOrigin = GetChildNodesValue(navigator, "Country");
            newTitle.AspectRatio = GetChildNodesValue(navigator, "AspectRatio");
            newTitle.VideoStandard = GetChildNodesValue(navigator, "VideoStandard");
            newTitle.OriginalName = GetChildNodesValue(navigator, "OriginalTitle");
            newTitle.SortName = GetChildNodesValue(navigator, "SortTitle");

            #region genres
            if (navigator.MoveToChild("Genres", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Genres... good old genres");
                XPathNodeIterator nIter = navigator.SelectChildren("Genre", "");
                if (navigator.MoveToFirstChild())
                {
                    XPathNavigator localNav = navigator.CreateNavigator();
                    navigator.MoveToParent();
                    for (int i = 0; i < nIter.Count; i++)
                    {
                        newTitle.AddGenre(localNav.Value);
                        localNav.MoveToNext("Genre", "");
                    }
                }
                navigator.MoveToParent();
            }
            #endregion

            #region audio tracks
            if (navigator.MoveToChild("AudioTracks", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] AudioTracks.. yeah, like you can even change them anyway");
                XPathNodeIterator nIter = navigator.SelectChildren("AudioTrack", "");
                if (navigator.MoveToFirstChild())
                {
                    XPathNavigator localNav = navigator.CreateNavigator();
                    navigator.MoveToParent();
                    for (int i = 0; i < nIter.Count; i++)
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

                            SDKUtilities.DebugLine("[MyMoviesImporter] Got one: {0}, {1}, {2}", audioLanguage, audioType, audioChannels);
                            newTitle.AddAudioTrack(audioTrackString);
                        }
                        localNav.MoveToNext("AudioTrack", "");
                    }
                }
                navigator.MoveToParent();
            }
            #endregion

            #region watched status (Submitted by yodine from our forums)
            if (navigator.MoveToChild("Watched", ""))
            {
                navigator.MoveToParent(); // move back, we just wanted to know this field existed.
                SDKUtilities.DebugLine("[MyMoviesImporter] Found Watched status. Trying to decode");
                string watched = GetChildNodesValue(navigator, "Watched");
                if (!string.IsNullOrEmpty(watched))
                {
                    if (Boolean.Parse(watched))
                        newTitle.WatchedCount++;
                }
            }
            #endregion

            #region subtitles
            if (navigator.MoveToChild("Subtitles", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Subtitles here we come!");
                if (navigator.GetAttribute("NotPresent", "").CompareTo("False") == 0)
                {
                    XPathNodeIterator nIter = navigator.SelectChildren("Subtitle", "");
                    if (navigator.MoveToFirstChild())
                    {
                        XPathNavigator localNav = navigator.CreateNavigator();
                        navigator.MoveToParent();
                        for (int i = 0; i < nIter.Count; i++)
                        {
                            string subtitleLanguage = localNav.GetAttribute("Language", "");
                            if (!string.IsNullOrEmpty(subtitleLanguage))
                            {
                                SDKUtilities.DebugLine("[MyMoviesImporter] Subtitle {0}", subtitleLanguage);
                                newTitle.AddSubtitle(subtitleLanguage);
                            }

                            localNav.MoveToNext("Subtitle", "");
                        }
                    }
                }
                navigator.MoveToParent();
            }
            #endregion

            #region discs
            if (navigator.MoveToChild("Discs", ""))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Discs... ok here is the good one, we'll passing this off to some other method to handle.");
                XPathNodeIterator nIter = navigator.SelectChildren("Disc", "");
                if (navigator.MoveToFirstChild())
                {
                    XPathNavigator localNav = navigator.CreateNavigator();
                    extractDisksFromXML(nIter, localNav, newTitle);
                    navigator.MoveToParent();
                }
                navigator.MoveToParent();
            }
            #endregion
        }
        private void extractDisksFromXML(XPathNodeIterator nIter, XPathNavigator localNav, OMLSDKTitle newTitle)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] Scanning for Discs...");
            SDKUtilities.DebugLine("[MyMoviesImporter] {0} entries found", nIter.Count);
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

                SDKUtilities.DebugLine("[MyMoviesImporter] Ok, I've collected some info... now we actually start looking for files");
                IList<OMLSDKDisk> sideAdisks = GetDisksForLocation(sideALocation, sideALocationType);

                int j = 1;
                SDKUtilities.DebugLine("[MyMoviesImporter] Ok, we found {0} possible files", sideAdisks.Count);
                foreach (OMLSDKDisk disk in sideAdisks)
                {
                    disk.Name = string.Format(@"Disk{0}", j++);
                    newTitle.AddDisk(disk);
                }

                if (isDoubleSided)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Whoa... this thing appears to be double-sided (old schooling it huh?)");
                    IList<OMLSDKDisk> sideBdisks = GetDisksForLocation(sideBLocation, sideBLocationType);
                    SDKUtilities.DebugLine("[MyMoviesImporter] We found {0} disks on the B side", sideBdisks.Count);
                    foreach (OMLSDKDisk disk in sideBdisks)
                    {
                        disk.Name = string.Format(@"Disk{0}", j++);
                        newTitle.AddDisk(disk);
                    }
                }

                localNav.MoveToNext("Disc", "");
            }
        }
        private IList<OMLSDKDisk> GetDisksForLocation(string location, string locationType)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] GetDisksForLocation({0}) of type {1}", location, locationType);
            List<OMLSDKDisk> disks = new List<OMLSDKDisk>();
            if (!string.IsNullOrEmpty(location))
            {
                if (location.CompareTo(".") == 0)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Your disk entry appears to be either empty or contain a '.', we're gonna look in the current directory");
                    location = Path.GetDirectoryName(currentFile);
                    SDKUtilities.DebugLine("[MyMoviesImporter] New Location: {0}", location);
                }

                string fullPath = Path.GetFullPath(location);
                SDKUtilities.DebugLine("[MyMoviesImporter] And we think the full path is: {0}", fullPath);

                int iLocationType = int.Parse(locationType);
                switch (iLocationType)
                {
                    case 1:
                        // online folder
                        SDKUtilities.DebugLine("[MyMoviesImporter] We think this is a directory");
                        if (Directory.Exists(fullPath))
                        {
                            if (SDKUtilities.IsDVD(fullPath))
                            {
                                SDKUtilities.DebugLine("[MyMoviesImporter] its dvd");
                                OMLSDKDisk disk = DiskForFormatAndLocation(OMLSDKVideoFormat.DVD, fullPath);
                                disks.Add(disk);
                                break;
                            }

                            if (SDKUtilities.IsBluRay(fullPath))
                            {
                                SDKUtilities.DebugLine("[MyMoviesImporter] its bluray");
                                OMLSDKDisk disk = DiskForFormatAndLocation(OMLSDKVideoFormat.BLURAY, fullPath);
                                disks.Add(disk);
                                break;
                            }

                            if (SDKUtilities.IsHDDVD(fullPath))
                            {
                                SDKUtilities.DebugLine("[MyMoviesImporter] its hddvd");
                                OMLSDKDisk disk = DiskForFormatAndLocation(OMLSDKVideoFormat.HDDVD, fullPath);
                                disks.Add(disk);
                                break;
                            }

                            SDKUtilities.DebugLine("[MyMoviesImporter] no idea, searching for files in {0}", location);
                            IList<string> files = GetVideoFilesForFolder(location);
                            foreach (string fileName in files)
                            {
                                string ext = Path.GetExtension(fileName);
                                ext = ext.Substring(1);
                                ext = ext.Replace("-", string.Empty);
                                OMLSDKVideoFormat format = FormatForExtension(ext);
                                if (format != OMLSDKVideoFormat.UNKNOWN)
                                {
                                    SDKUtilities.DebugLine("[MyMoviesImporter] got one, its {0} (at: {1})", fileName, format);
                                    OMLSDKDisk disk = DiskForFormatAndLocation(format, fileName);
                                    disks.Add(disk);
                                }
                            }
                            break;
                        }
                        else
                        {
                            AddError("[MyMoviesImporter] Location {0} is of type folder but the folder doesn't appear to exist", fullPath);
                        }
                        break;
                    case 2:
                        // online file
                        SDKUtilities.DebugLine("[MyMoviesImporter] We think this is a file");
                        if (File.Exists(fullPath))
                        {
                            string ext = Path.GetExtension(fullPath);
                            ext = ext.Substring(1);
                            ext.Replace("-", string.Empty);
                            OMLSDKVideoFormat format = FormatForExtension(ext);

                            if (format != OMLSDKVideoFormat.UNKNOWN)
                            {
                                SDKUtilities.DebugLine("[MyMoviesImporter] Ok, found something here, it appears to be {0} with a format if {1}", fullPath, format);
                                OMLSDKDisk disk = DiskForFormatAndLocation(format, fullPath);
                                disks.Add(disk);
                            }
                        }
                        else
                        {
                            AddError("[MyMoviesImporter] Location {0} is of type file but the file doesn't appear to exist", fullPath);
                        }
                        break;
                    case 3:
                        SDKUtilities.DebugLine("[MyMoviesImporter] Seriously... mymovies says this is an offline disk... how are we supposed to import an offline disk?\nQuick, point us to your nearest dvd shelf!");
                        OMLSDKDisk t3disk = new OMLSDKDisk();
                        t3disk.Format = OMLSDKVideoFormat.OFFLINEDVD;
                        disks.Add(t3disk);
                        // offline dvd
                        break;
                    case 4:
                        // dvd changer
                        SDKUtilities.DebugLine("[MyMoviesImporter] Do you see any dvd changers around here?  Cause I dont!");
                        OMLSDKDisk t4disk = new OMLSDKDisk();
                        t4disk.Format = OMLSDKVideoFormat.OFFLINEDVD;
                        disks.Add(t4disk);
                        break;

                    case 5:
                        // media changer of some kind, treat as 4
                        SDKUtilities.DebugLine("[MyMoviesImporter] Ah, upgraded to a Media Changer hey? Nice... TO BAD WE DONT SUPPORT THOSE!");
                        OMLSDKDisk t5disk = new OMLSDKDisk();
                        t5disk.Format = OMLSDKVideoFormat.OFFLINEDVD;
                        disks.Add(t5disk);
                        break;
                    default:
                        // no idea...
                        AddError("[MyMoviesImporter] I have NO idea what type of video is available at {0}", location);
                        break;
                }
            }
            SDKUtilities.DebugLine("[MyMoviesImporter] Ok, I got nothing... hopefully the double-check in the validate method will find something");
            return disks;
        }
        private OMLSDKDisk DiskForFormatAndLocation(OMLSDKVideoFormat format, string location)
        {
            OMLSDKDisk disk = new OMLSDKDisk();
            disk.Format = format;
            disk.Path = location;

            return disk;
        }
        private OMLSDKVideoFormat FormatForExtension(string ext)
        {
            OMLSDKVideoFormat format;
            SDKUtilities.DebugLine("[MyMoviesImporter] Attempting to determine format based on extension {0}", ext);
            try
            {
                format = (OMLSDKVideoFormat)Enum.Parse(typeof(OMLSDKVideoFormat), ext, true);
                SDKUtilities.DebugLine("[MyMoviesImporter] Format appears to be {0}", format);
                return format;
            }
            catch (Exception ex)
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Format is Unknown.. this means its useless: {0}", ex);
            }
            return OMLSDKVideoFormat.UNKNOWN;
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
        private OMLSDKVideoFormat GetVideoFormatForLocation(string location, string locationType)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] You want the format for what exactly? {0}", location);
            int type = Int32.Parse(locationType);

            switch (type)
            {
                case 1:
                    //online folder
                    if (Directory.Exists(location))
                    {
                        if (SDKUtilities.IsBluRay(location))
                        {
                            SDKUtilities.DebugLine("[MyMoviesImporter] Nice.. you appear to have a bluray drive (or file, you sneaky dog)");
                            return OMLSDKVideoFormat.BLURAY;
                        }

                        if (SDKUtilities.IsHDDVD(location))
                        {
                            SDKUtilities.DebugLine("[MyMoviesImporter] Ah.. hddvd nice (yeah last year... you know these lost the format war right?)");
                            return OMLSDKVideoFormat.HDDVD;
                        }

                        if (SDKUtilities.IsDVD(location))
                        {
                            SDKUtilities.DebugLine("[MyMoviesImporter] Here be a dvd or video_ts folder (does it really matter which?)");
                            return OMLSDKVideoFormat.DVD;
                        }
                        // unable to determine disc, there must be a file flagged as a folder... go check for files
                    }
                    break;
                case 2:
                    // online file
                    SDKUtilities.DebugLine("[MyMoviesImporter] File time!");
                    string extension = Path.GetExtension(location);
                    extension = extension.Substring(1);
                    extension.Replace("-", string.Empty);
                    OMLSDKVideoFormat format;
                    try
                    {
                        format = (OMLSDKVideoFormat)Enum.Parse(typeof(OMLSDKVideoFormat), extension, true);
                        SDKUtilities.DebugLine("[MyMoviesImporter] Ok, I looked for the format and all I got was the lame comment line {0}", format);
                    }
                    catch (Exception ex)
                    {
                        AddError("[MyMoviesImporter] Error parsing format for extension: {0}, {1}", extension, ex);
                        format = OMLSDKVideoFormat.UNKNOWN;
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
                    return OMLSDKVideoFormat.UNKNOWN;
            }

            return OMLSDKVideoFormat.UNKNOWN;
        }
        public bool ValidateTitle(OMLSDKTitle title_to_validate, string file)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] This is the part where we validate (or not) your shiny new title, wouldn't want you to drive off the lot with it untested now would we");
            if (title_to_validate.Disks.Count == 0)
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Whoa... you dont appear to have any discs... lets double check that");
                string directoryName = Path.GetDirectoryName(file);
                if (Directory.Exists(directoryName))
                {
                    if (SDKUtilities.IsBluRay(directoryName))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] its a bluray, adding a new disk");
                        title_to_validate.AddDisk(new OMLSDKDisk("Disk1", directoryName, OMLSDKVideoFormat.BLURAY));
                        return true;
                    }

                    if (SDKUtilities.IsHDDVD(directoryName))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] its an hddvd, adding a new disk");
                        title_to_validate.AddDisk(new OMLSDKDisk("Disk1", directoryName, OMLSDKVideoFormat.HDDVD));
                        return true;
                    }

                    if (SDKUtilities.IsDVD(directoryName))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] its a dvd, adding a new disk");
                        title_to_validate.AddDisk(new OMLSDKDisk("Disk1", directoryName, OMLSDKVideoFormat.DVD));
                        return true;
                    }

                    string[] files = Directory.GetFiles(directoryName);
                    SDKUtilities.DebugLine("[MyMoviesImporter] You have {0} files in the current location {1}", files.Length, directoryName);

                    /* patch from KingManon to limit files only to those of valid file types */
                    string localExt;
                    List<string> newFiles = new List<string>();
                    List<string> allowedExtensions = new List<string>(Enum.GetNames(typeof(OMLSDKVideoFormat)));
                    foreach (string singleFile in files)
                    {
                        localExt = Path.GetExtension(singleFile).Substring(1);
                        if (allowedExtensions.Contains(localExt.ToUpper()))
                            newFiles.Add(singleFile);
                    }
                    files = newFiles.ToArray();

                    Array.Sort(files);
                    for (int i = 0; i < files.Length; i++)
                    {

                        string ext = Path.GetExtension(files[i]).Substring(1);
                        try
                        {
                            if (new List<string>(Enum.GetNames(typeof(OMLSDKVideoFormat))).Contains(ext.ToUpper()))
                            {
                                OMLSDKVideoFormat format;
                                try
                                {
                                    SDKUtilities.DebugLine("[MyMoviesImporter] Checking file for format {0}", files[i]);
                                    format = (OMLSDKVideoFormat)Enum.Parse(typeof(OMLSDKVideoFormat), ext, true);
                                    OMLSDKDisk disk = new OMLSDKDisk();
                                    disk.Name = string.Format("Disk{0}", i + 1);
                                    disk.Path = Path.Combine(directoryName, files[i]);
                                    disk.Format = format;
                                    title_to_validate.AddDisk(disk);
                                }
                                catch (Exception)
                                {
                                    AddError("[MyMoviesImporter] Unable to determine format for extension: {0}", ext);
                                }
                            }
                        }
                        catch
                        {
                            SDKUtilities.DebugLine("[MyMoviesImporter] Yeah, no extention on file ({0}), skip it!", files[i]);
                            // didnt get the extension, its not a valid file, skip it
                        }
                    }
                    if (title_to_validate.Disks.Count == 0)
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] No disks found on the title, we'll skip it");
                        return false;
                    }
                }
            }
            return true;
        }
        private void GetSubFolders(string startFolder, ref List<string> folderList)
        {
            DirectoryInfo[] diArr = null;
            try
            {
                DirectoryInfo di = new DirectoryInfo(startFolder);
                diArr = di.GetDirectories();
                foreach (DirectoryInfo folder in diArr)
                {
                    SDKUtilities.DebugLine("Got a child Folder {0}", folder.Name);
                    folderList.Add(folder.FullName);
                    GetSubFolders(folder.FullName, ref folderList);
                }

            }
            catch (Exception ex)
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] An error occured getting folder info for folder ({0}): {1}", startFolder, ex.Message);
            }
        }
        private IList<string> GetVideoFilesForFolder(string folder)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] Looking for video files in a folder, {0} folder in fact.", folder);
            List<string> fileNames = new List<string>();

            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder);
                Array.Sort(files);
                foreach (string fName in files)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Found file {0}, I don't (yet) care what file type, just throw it on the stack", fName);
                    fileNames.Add(Path.GetFullPath(fName));
                }
            }
            else
            {
                AddError("[MyMoviesImporter] Disc entry specified folder ({0}) but that folder doesn't appear to exist", folder);
            }
            return fileNames;
        }
        private string FindFinalImagePath(string imagePath)
        {
            SDKUtilities.DebugLine("[MyMoviesImporter] Attempting to determine cover image based on path {0}", imagePath);
            if (File.Exists(imagePath))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] Imagepath {0} appears to be valid", imagePath);
                return imagePath;
            }

            string possibleImagePath = Path.Combine(Path.GetDirectoryName(currentFile),
                                                    imagePath);

            SDKUtilities.DebugLine("[MyMoviesImporter] Checking for {0} as a possible image", possibleImagePath);
            if (File.Exists(possibleImagePath))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] {0} appears correct, well use it", possibleImagePath);
                return possibleImagePath;
            }

            string possibleDefaultImagePath = Path.Combine(Path.GetDirectoryName(currentFile),
                                                           "folder.jpg");

            SDKUtilities.DebugLine("[MyMoviesImporter] Checking for {0} as a possible image", possibleDefaultImagePath);
            if (File.Exists(possibleDefaultImagePath))
            {
                SDKUtilities.DebugLine("[MyMoviesImporter] {0} appears correct, well use it", possibleDefaultImagePath);
                return possibleDefaultImagePath;
            }

            string[] imageFiles = Directory.GetFiles(Path.GetDirectoryName(currentFile), "*.jpg");
            SDKUtilities.DebugLine("[MyMoviesImporter] We found {0} jpg files to check", imageFiles.Length);
            if (imageFiles.Length > 0)
            {
                if (imageFiles.Length == 1)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Only 1 file was found, we'll assume it is the cover file {0}",
                                        Path.Combine(Path.GetDirectoryName(currentFile), imageFiles[0]));
                    return Path.Combine(Path.GetDirectoryName(currentFile), imageFiles[0]);
                }

                SDKUtilities.DebugLine("[MyMoviesImporter] Looping through each file to check it for possible cover art");
                foreach (string imageFilename in imageFiles)
                {
                    SDKUtilities.DebugLine("[MyMoviesImporter] Found file: {0}", imageFilename);
                    if (imageFilename.ToLower().Contains("front"))
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] This file appears to contain the word front in it, we'll use it {0}",
                                            Path.Combine(Path.GetDirectoryName(currentFile), imageFilename));
                        return Path.Combine(Path.GetDirectoryName(currentFile), imageFilename);
                    }

                    if (imageFilename.ToLower().CompareTo(currentFile.ToLower()) == 0)
                    {
                        SDKUtilities.DebugLine("[MyMoviesImporter] This file appears to have the same name as the video file, we'll use it {0}",
                                            Path.Combine(Path.GetDirectoryName(currentFile), imageFilename));
                        return Path.Combine(Path.GetDirectoryName(currentFile), imageFilename);
                    }
                }
            }
            SDKUtilities.DebugLine("[MyMoviesImporter] No image files found, I guess this title doesn't have any cover art files");
            return string.Empty;
        }
    }
}
