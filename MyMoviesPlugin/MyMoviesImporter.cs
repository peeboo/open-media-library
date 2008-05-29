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
        bool _ShouldCopyImages = true;
        TextReader tr = null;
        private static double VERSION = 0.1;

        public MyMoviesImporter() : base()
        {
            Utilities.DebugLine("[MyMoviesImporter] created");
        }

        public override bool Load(string filename, bool ShouldCopyImages)
        {
            Utilities.DebugLine("[MyMoviesImporter] created[filename("+filename+"), ShouldCopyImages("+ShouldCopyImages+")]");
            _ShouldCopyImages = ShouldCopyImages;
            try { tr = new StreamReader(filename); }
            catch (Exception e) { Utilities.DebugLine("[MyMoviesImporter] " + e.Message); }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);
            Utilities.DebugLine("[MyMoviesImporter] file loaded");

            XmlNodeList nodeList = xDoc.SelectNodes("//Titles/Title");
            foreach (XmlNode movieNode in nodeList)
            {
                Utilities.DebugLine("[MyMoviesImporter] Found base Title node");
                Title newTitle = new Title();
                // first get the name
                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    if (node.Name.CompareTo("LocalTitle") == 0)
                    {
                        newTitle.Name = node.InnerText;
                        Utilities.DebugLine("[MyMoviesImporter] Title appears to be: "+newTitle.Name+" moving to load title meta data");
                        break;
                    }
                }
                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    Utilities.DebugLine("[MyMoviesImporter] Processing node: " + node.Name);
                    process_node_switch(newTitle, node);
                }
                if (ValidateTitle(newTitle))
                {
                    Utilities.DebugLine("[MyMoviesImporter] Validating title");
                    try { AddTitle(newTitle); }
                    catch (Exception e) { Utilities.DebugLine("[MyMoviesImporter] Error adding row: " + e.Message); }
                }
                else Utilities.DebugLine("[MyMoviesImporter] Error saving row");
            }
            return true;
        }

        public override string GetName()
        {
            return "MyMoviesPlugin";
        }
        public override string GetAuthor()
        {
            return "OML Development Team";
        }
        public override string GetDescription()
        {
            return "MyMovies xml file importer for Open Media Library v" + VERSION;
        }

        public string CopyImage(string from_location, string to_location)
        {
            Utilities.DebugLine("[MyMoviesImporter] Copying Image: FROM("+from_location+") TO("+to_location+")");
            FileInfo fi = new FileInfo(from_location);
            if (fi.Exists)
            {
                Utilities.DebugLine("[MyMoviesImporter] File ("+from_location+") is valid, copying");
                File.Copy(from_location, to_location, true);
                return to_location;
            }
            else
            {
                Utilities.DebugLine("[MyMoviesImporter] File ("+from_location+") is invalid, keeping original location");
                return from_location;
            }
        }

        private void process_node_switch(Title newTitle, XmlNode node)
        {
            switch (node.Name)
            {
                case "WebServiceId":
                    newTitle.MetadataSourceID = node.InnerText;
                    break;
                case "Covers":
                    XmlNode front_node = node.ChildNodes[0];
                    if (front_node != null)
                    {
                        string imagePath = front_node.InnerText;
                        FileInfo fi;
                        try {
                            fi = new FileInfo(imagePath);
                            string new_full_name = OMLEngine.FileSystemWalker.ImageDirectory +
                                                   "\\F" + newTitle.InternalItemID +
                                                   fi.Extension;
                            if (_ShouldCopyImages)
                            {
                                CopyImage(imagePath, new_full_name);
                                imagePath = new_full_name;
                            }

                            newTitle.FrontCoverPath = imagePath;
                        }
                        catch (Exception e) { Utilities.DebugLine(e.Message); }
                    }
                    XmlNode back_node = node.ChildNodes[1];
                    if (back_node != null)
                    {
                        string imagePath = back_node.InnerText;
                        FileInfo fi;
                        try
                        {
                            fi = new FileInfo(imagePath);
                            string new_full_name = OMLEngine.FileSystemWalker.ImageDirectory +
                                                   "\\B" + newTitle.InternalItemID +
                                                   fi.Extension;
                            if (_ShouldCopyImages)
                            {
                                CopyImage(imagePath, new_full_name);
                                imagePath = new_full_name;
                            }
                            newTitle.BackCoverPath = imagePath;
                        }
                        catch (Exception e) { Utilities.DebugLine(e.Message); }
                    }
                    break;
                case "Description":
                    newTitle.Synopsis = node.InnerText;
                    break;
                case "ReleaseDate":
                    string rls_date_str = node.InnerText;
                    string[] parts = rls_date_str.Split(new char[] { '/' });
                    if (parts.Length == 3)
                    {
                        int year = Int32.Parse(parts[2]);
                        int month = Int32.Parse(parts[0]);
                        int day = Int32.Parse(parts[1]);
                        DateTime rls_date = new DateTime(year, month, day);
                        newTitle.ReleaseDate = rls_date;
                    }
                    break;
                case "ParentalRating":
                    XmlNode ratingIdNode = node.SelectSingleNode("Value");
                    if (ratingIdNode != null)
                    {
                        string ratingId = ratingIdNode.InnerText;
                        if (ratingId.Length > 0)
                        {
                            int mmRatingId = Int32.Parse(ratingId);
                            switch (mmRatingId)
                            {
                                case 0:
                                    newTitle.MPAARating = "Unrated";
                                    break;
                                case 1:
                                    newTitle.MPAARating = "G";
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    newTitle.MPAARating = "PG";
                                    break;
                                case 4:
                                    newTitle.MPAARating = "PG13";
                                    break;
                                case 5:
                                    break;
                                case 6:
                                    newTitle.MPAARating = "R";
                                    break;
                            }
                        }
                    }
                    break;
                case "RunningTime":
                    newTitle.Runtime = Int32.Parse(node.InnerText);
                    break;
                case "Persons":
                    XmlNodeList persons = node.SelectNodes("Person");
                    foreach (XmlNode personNode in persons)
                    {
                        XmlNode nameNode = personNode.SelectSingleNode("Name");
                        XmlNode typeNode = personNode.SelectSingleNode("Type");

                        Person p = new Person(nameNode.InnerText);
                        switch (typeNode.InnerText)
                        {
                            case "Actor":
                                newTitle.AddActor(p);
                                break;
                            case "Director":
                                newTitle.AddDirector(p);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "Studios":
                    XmlNodeList studios = node.SelectNodes("Studio");
                    foreach (XmlNode studioNode in studios)
                    {
                        newTitle.Distributor = studioNode.InnerText;
                    }
                    break;
                case "Country":
                    newTitle.CountryOfOrigin = node.InnerText;
                    break;
                case "Discs":
                    Utilities.DebugLine("[MyMoviesImporter] Beginning a Video File node");
                    XmlNodeList discs = node.SelectNodes("Disc");
                    foreach (XmlNode disc in discs)
                    {
                        XmlNode sideA = disc.SelectSingleNode("LocationSideA");
                        if (sideA != null)
                        {
                            Utilities.DebugLine("[MyMoviesImporter] Found a file node, beginning investigation");
                            string directory = sideA.InnerText;
                            if (directory.Length > 0)
                            {
                                DirectoryInfo di;
                                try
                                {
                                    di = new DirectoryInfo(directory);
                                    if (di != null)
                                    {
                                        Utilities.DebugLine("[MyMoviesImporter] Directory is valid, searching for files");
                                        FileSystemInfo[] infos = di.GetFileSystemInfos();
                                        foreach (FileSystemInfo info in infos)
                                        {
                                            Utilities.DebugLine("[MyMoviesImporter] Found a file ("+info.FullName+")");
                                            if (info.GetType().Equals(typeof(FileInfo)))
                                            {
                                                string ext = info.Extension.Substring(1);
                                                if (IsSupportedFormat(ext))
                                                {
                                                    Utilities.DebugLine("[MyMoviesImporter] File is a valid format, adding file");
                                                    newTitle.VideoFormat =
                                                        (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                                    newTitle.FileLocation = info.FullName;
                                                    break;
                                                }
                                            }
                                            if (info.GetType().Equals(typeof(DirectoryInfo)))
                                            {
                                                Utilities.DebugLine("[MyMoviesImporter] Director found ("+info.FullName+")");
                                                if (info.Name.ToUpper().CompareTo("VIDEO_TS") == 0)
                                                {
                                                    Utilities.DebugLine("[MyMoviesImporter] Video_ts directory found, adding");
                                                    newTitle.VideoFormat = VideoFormat.DVD;
                                                    newTitle.FileLocation = info.FullName;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                { Utilities.DebugLine("Error: " + e.Message); }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
