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

namespace MovieCollectorzPlugin
{
    public class MovieCollectorzPlugin : OMLPlugin, IOMLPlugin
    {
        const string HTML_TAG_PATTERN = "<.*?>";

        protected override string GetAuthor()
        {
            throw new Exception("OML");
        }

        public override string SetupDescription()
        {
            return GetName() + @" will search for and import [" + DefaultFileToImport() + @"] files.";
        }

        public override bool IsSingleFileImporter()
        {
            return true;
        }

        public override string DefaultFileToImport()
        {
            return @"movies.xml";
        }

        public MovieCollectorzPlugin() : base()
        {
        }

        public override void ProcessFile(string file)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);

            XmlNodeList nodeList = xDoc.SelectNodes("//movielist/movie");
            foreach (XmlNode movieNode in nodeList)
            {
                Title newTitle = new Title();
                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "id":
                            newTitle.MetadataSourceID = node.InnerText;
                            break;
                        case "coverfront":
                            SetFrontCoverImage(ref newTitle, node.InnerText);
                            break;
                        // this case just represents if the disc is a DVD or blu ray move.
                        // can use this to put a little ICON on the screen to notify the user
                        //
                        //case "format":
                        //    XmlNode formatNode = node.SelectSingleNode("displayname");
                        //    if (formatNode != null)
                        //    {
                        //        switch (formatNode.InnerText.ToUpper())
                        //        {
                        //            case "DVD":
                        //                newTitle.VideoFormat = VideoFormat.DVD;
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //    }
                        //    break;
                        case "language":
                            XmlNodeList langNodes = node.SelectNodes("displayname");
                            foreach (XmlNode languageNode in langNodes)
                            {
                                newTitle.AddLanguageFormat(languageNode.InnerText);
                            }
                            break;
                        case "title":
                            newTitle.Name = node.InnerText;
                            break;
                        case "plot":
                            newTitle.Synopsis = StripHTML(node.InnerText);
                            break;
                        case "releasedate":
                            XmlNode rdYear = node.SelectSingleNode("year");
                            XmlNode rdMonth = node.SelectSingleNode("month");
                            XmlNode rdDay = node.SelectSingleNode("day");

                            if (rdYear != null && rdMonth != null && rdDay != null)
                            {
                                DateTime rd = new DateTime(Int32.Parse(rdYear.InnerText),
                                                           Int32.Parse(rdMonth.InnerText),
                                                           Int32.Parse(rdDay.InnerText));
                                if (rd != null)
                                    newTitle.ReleaseDate = rd;
                            }
                            break;
                        case "mpaarating":
                            XmlNode ratingNode = node.SelectSingleNode("displayname");
                            if (ratingNode != null)
                                newTitle.ParentalRating = ratingNode.InnerText;
                            break;
                        case "genres":
                            XmlNode genreNode = node.SelectSingleNode("genre");
                            if (genreNode != null)
                            {
                                XmlNode disNameNode = genreNode.SelectSingleNode("displayname");
                                if (disNameNode != null)
                                    newTitle.AddGenre(disNameNode.InnerText);
                            }
                            break;
                        case "subtitles":
                            XmlNode subtitleNode = node.SelectSingleNode("subtitle");
                            if (subtitleNode != null)
                            {
                                XmlNode disNameNode = subtitleNode.SelectSingleNode("displayname");
                                if (disNameNode != null)
                                    newTitle.AddSubtitle(disNameNode.InnerText);
                            }
                            break;
                        case "runtimeminutes":
                            newTitle.Runtime = Int32.Parse(node.InnerText);
                            break;
                        case "cast":
                            XmlNodeList persons = node.SelectNodes("person");
                            foreach (XmlNode person in persons)
                            {
                                XmlNode disNameNode = person.SelectSingleNode("displayname");
                                if (disNameNode != null)
                                    newTitle.AddActor( new Person(disNameNode.InnerText));
                            }
                            break;
                        case "crew":
                            XmlNodeList crewMembers = node.SelectNodes("crewmember");
                            foreach (XmlNode crewMember in crewMembers)
                            {
                                XmlNode roleId = crewMember.SelectSingleNode("roleid");
                                if (roleId != null)
                                {
                                    if (roleId.InnerText.ToUpper().CompareTo("DFDIRECTOR") == 0)
                                    {
                                        XmlNode person = crewMember.SelectSingleNode("person");
                                        if (person != null)
                                        {
                                            XmlNode displayName = person.SelectSingleNode("displayname");
                                            if (displayName != null)
                                                newTitle.AddDirector(new Person(displayName.InnerText));
                                        }
                                    }
                                    else
                                    {
                                        XmlNode crewMemberName = crewMember.SelectSingleNode("displayname");
                                        if (crewMemberName != null)
                                            newTitle.AddNonActingRole(crewMemberName.InnerText, "crew");
                                    }
                                }
                            }
                            break;
                        case "studios":
                            XmlNodeList studioNodes = node.SelectNodes("displayname");
                            foreach (XmlNode studioNode in studioNodes)
                                newTitle.Studio = studioNode.InnerText; // currently we only hold one studio (in this case the last one)
                            break;
                        case "distributor":
                            XmlNode distNode = node.SelectSingleNode("displayname");
                            if (distNode != null)
                                newTitle.Studio = distNode.InnerText;
                            break;
                        case "country":
                            XmlNode countryNode = node.SelectSingleNode("displayname");
                            if (countryNode != null)
                                newTitle.CountryOfOrigin = countryNode.InnerText;
                            break;
                        case "upc":
                            newTitle.UPC = node.InnerText;
                            break;

                        case "links":
                            XmlNodeList links = node.SelectNodes("link");
                            foreach (XmlNode link in links)
                            {
                                XmlNode urlType = link.SelectSingleNode("urltype");
                                if (urlType != null)
                                {
                                    if (urlType.InnerText.ToUpper().CompareTo("MOVIE") == 0)
                                    {
                                        XmlNode url = link.SelectSingleNode("url");
                                        if (url != null)
                                        {
                                            FileInfo fi = new FileInfo(url.InnerText);
                                            string ext = fi.Extension.Substring(1);
                                            
                                            if (IsSupportedFormat(ext))
                                            {
                                                Disk disk = new Disk();
                                                disk.Format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                                disk.Path = url.InnerText;
                                                disk.Name = "Disk 1";
                                                newTitle.Disks.Add(disk);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }

                if (ValidateTitle(newTitle))
                {
                    try
                    {
                        AddTitle(newTitle);
                    }
                    catch (Exception e)
                    {
                        Utilities.DebugLine("[MovieCollectorzPlugin] Error adding row: " + e.Message);
                    }
                }
                else
                    Utilities.DebugLine("[MovieCollectorzPlugin] Error saving row");
            }
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
            return "Movie Collectorz";
        }
        protected override string GetName()
        {
            return "MovieCollectorzPlugin";
        }
        protected override string GetDescription()
        {
            return @"Movie Collectorz xml file importer v" + Version;
        }

        private string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }

    }
}
