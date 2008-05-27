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

namespace MovieCollectorz
{
    public class MovieCollectorzPlugin : OMLPlugin, IOMLPlugin
    {
        bool _ShouldCopyImages = true;
        const string HTML_TAG_PATTERN = "<.*?>";
        TextReader tr = null;

        public override string GetDescription()
        {
            throw new Exception("MovieCollectorz Plugin");
        }
        public override string GetAuthor()
        {
            throw new Exception("OML");
        }
        
        public MovieCollectorzPlugin() : base()
        {
        }
        public override bool Load(string filename, bool ShouldCopyImages)
        {
            _ShouldCopyImages = ShouldCopyImages;
            try
            {
                tr = new StreamReader(filename);
            }
            catch (Exception e)
            {
                Utilities.DebugLine(e.Message);
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);

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
                            newTitle.FrontCoverPath = node.InnerText;
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
                                newTitle.MPAARating = ratingNode.InnerText;
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
                                            newTitle.AddCrew(new Person(crewMemberName.InnerText));
                                    }
                                }
                            }
                            break;
                        case "studios":
                            XmlNodeList studioNodes = node.SelectNodes("displayname");
                            foreach (XmlNode studioNode in studioNodes)
                                newTitle.Distributor = studioNode.InnerText; // currently we only hold one studio (in this case the last one)
                            break;
                        case "distributor":
                            XmlNode distNode = node.SelectSingleNode("displayname");
                            if (distNode != null)
                                newTitle.Distributor = distNode.InnerText;
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
                                                newTitle.VideoFormat = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                                newTitle.FileLocation = url.InnerText;
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
                        Utilities.DebugLine("Error adding row: " + e.Message);
                    }
                }
                else
                    Utilities.DebugLine("Error saving row");
            }

            return true;
        }
        public override string GetName()
        {
            return "MovieCollectorzPlugin";
        }

        private string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }

    }
}
