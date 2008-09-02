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
                if (navigator.MoveToChild("LocalTitle", ""))
                {
                    newTitle.Name = navigator.Value;
                    navigator.MoveToParent();
                    loadDataFromNavigatorToTitle(ref navigator, ref newTitle);
                }
                else
                {
                    break;
                }

                if (ValidateTitle(newTitle, file))
                {
                    Utilities.DebugLine("[MyMoviesImporter] Validating title");
                    try { AddTitle(newTitle); }
                    catch (Exception e) { Utilities.DebugLine("[MyMoviesImporter] Error adding row: " + e.Message); }
                }
                else Utilities.DebugLine("[MyMoviesImporter] Error saving row");
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
            if (navigator.MoveToChild("WebServiceId", ""))
            {
                newTitle.MetadataSourceID = navigator.Value;
                navigator.MoveToParent();
            }

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
                    SetBackCoverImage(ref newTitle, navigator.Value);
                    navigator.MoveToParent();
                }

                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Description", ""))
            {
                newTitle.Synopsis = navigator.Value;
                navigator.MoveToParent();
            }

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
                    Utilities.DebugLine("[MyMoviesImporter] error reading ProductionYear: " + e.Message);
                }
            }

            if (navigator.MoveToChild("ParentalRating", ""))
            {
                string ratingId = navigator.Value;
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
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("RunningTime", ""))
            {
                newTitle.Runtime = Int32.Parse(navigator.Value);
                navigator.MoveToParent();
            }

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
                        string name = string.Empty;
                        string role = string.Empty;
                        string type = string.Empty;

                        if (localNav.MoveToChild("Name", ""))
                        {
                            name = localNav.Value;
                            localNav.MoveToParent();
                        }

                        if (localNav.MoveToChild("Role", ""))
                        {
                            role = localNav.Value;
                            localNav.MoveToParent();
                        }

                        if (localNav.MoveToChild("Type", ""))
                        {
                            type = localNav.Value;
                            localNav.MoveToParent();
                        }

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
                        newTitle.Studio = localNav.Value;
                        localNav.MoveToNext("Studio", "");
                    }
                }
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("Country", ""))
            {
                newTitle.CountryOfOrigin = navigator.Value;
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("AspectRatio", ""))
            {
                newTitle.AspectRatio = navigator.Value;
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("OriginalTitle", ""))
            {
                newTitle.OriginalName = navigator.Value;
                navigator.MoveToParent();
            }

            if (navigator.MoveToChild("SortTitle", ""))
            {
                newTitle.SortName = navigator.Value;
                navigator.MoveToParent();
            }

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
                    string audioLanguage = string.Empty;
                    string audioType = string.Empty;
                    string audioChannels = string.Empty;

                    audioLanguage = localNav.GetAttribute("Language", "");
                    audioType = localNav.GetAttribute("Type", "");
                    audioChannels = localNav.GetAttribute("Channels", "");

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
                for (int i = 0; i < nIter.Count; i++)
                {
                    bool isDoubleSided = false;
                    string discName = string.Empty;
                    string sideAId = string.Empty;
                    string sideBId = string.Empty;
                    string sideALocation = string.Empty;
                    string sideBLocation = string.Empty;
                    string sideALocationType = string.Empty;
                    string sideBLocationType = string.Empty;
                    string changerSlot = string.Empty;

                    if (localNav.MoveToChild("DoubleSided", ""))
                    {
                        isDoubleSided = (localNav.Value.CompareTo("False") == 0) ? false : true;
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("Name", ""))
                    {
                        discName = localNav.Value;
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("DiscIdSideA", ""))
                    {
                        sideAId = localNav.Value;
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("LocationSideA", ""))
                    {
                        sideALocation = localNav.Value;
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("LocationTypeSideA", ""))
                    {
                        sideALocationType = localNav.Value;
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("ChangerSlot", ""))
                    {
                        changerSlot = localNav.Value;
                        localNav.MoveToParent();
                    }

                    if (isDoubleSided)
                    {
                        if (localNav.MoveToChild("DiscIdSideB", ""))
                        {
                            sideBId = localNav.Value;
                            localNav.MoveToParent();
                        }

                        if (localNav.MoveToChild("LocationSideB", ""))
                        {
                            sideBLocation = localNav.Value;
                            localNav.MoveToParent();
                        }

                        if (localNav.MoveToChild("LocationTypeSideB", ""))
                        {
                            sideBLocationType = localNav.Value;
                            localNav.MoveToParent();
                        }
                    }

                    if (!string.IsNullOrEmpty(sideALocation))
                    {
                        if (Directory.Exists(Path.GetFullPath(sideALocation)) || File.Exists(Path.GetFullPath(sideALocation)))
                        {
                            VideoFormat format = GetVideoFormatForLocation(sideALocation, sideALocationType);
                            if (format != VideoFormat.UNKNOWN)
                            {
                                newTitle.Disks.Add(new Disk(
                                    discName,
                                    sideALocation,
                                    format)
                                );
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(sideBLocation))
                    {
                        if (Directory.Exists(Path.GetFullPath(sideBLocation)) || File.Exists(Path.GetFullPath(sideBLocation)))
                        {
                            VideoFormat format = GetVideoFormatForLocation(sideBLocation, sideBLocationType);
                            if (format != VideoFormat.UNKNOWN)
                            {
                                newTitle.Disks.Add(new Disk(
                                    discName,
                                    sideBLocation,
                                    format)
                                    );
                            }
                        }
                    }
                    localNav.MoveToNext("Disc", "");
                }
                navigator.MoveToParent();
            }

        }
        public enum MyMoviesLocationType { Folder = 1, File };
        private VideoFormat GetVideoFormatForLocation(string location, string locationType)
        {
            int type = Int32.Parse(locationType);

            switch (type)
            {
                case -1:
                    return VideoFormat.UNKNOWN;
                case 1:
                    if (Directory.Exists(location))
                    {
                        if (MediaData.IsBluRay(location))
                            return VideoFormat.BLURAY;

                        if (MediaData.IsHDDVD(location))
                            return VideoFormat.HDDVD;

                        if (MediaData.IsDVD(location))
                            return VideoFormat.DVD;
                    }
                    // folder
                    break;
                case 2:
                    string extension = Path.GetExtension(location);
                    extension = extension.Substring(1);
                    return (VideoFormat)Enum.Parse(typeof(VideoFormat), extension, true);
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
                    for (int i = 0; i < files.GetLength(0); i++)
                    {

                        string ext = Path.GetExtension(files[i]).Substring(1);
                        try
                        {
                            if (new List<string>(Enum.GetNames(typeof(VideoFormat))).Contains(ext.ToUpper()))
                            {
                                VideoFormat format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                title_to_validate.Disks.Add(new Disk(string.Format("Disk{0}", i + 1), Path.Combine(directoryName, files[i]), format));
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
    }
}
