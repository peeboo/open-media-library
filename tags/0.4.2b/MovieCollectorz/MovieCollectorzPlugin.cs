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
                XPathNavigator nav = movieNode.CreateNavigator();

                newTitle.MetadataSourceID = GetChildNodesValue(nav, "id");
                if (nav.MoveToChild("coverfront", ""))
                {
                    newTitle.FrontCoverPath = nav.Value;
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("coverback", ""))
                {
                    newTitle.BackCoverPath = nav.Value;
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("country", ""))
                {
                    if (nav.MoveToChild("displayname", ""))
                    {
                        newTitle.CountryOfOrigin = nav.Value;
                        nav.MoveToParent();
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("title", ""))
                {
                    newTitle.Name = nav.Value;
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("plot", ""))
                {
                    newTitle.Synopsis = nav.Value;
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("releasedate", ""))
                {
                    XPathNavigator localNav = nav.CreateNavigator();
                    //XmlNode rdYear = nav.SelectSingleNode("year");
                    //XmlNode rdMonth = nav.SelectSingleNode("month");
                    //XmlNode rdDay = nav.SelectSingleNode("day");

                    //if (rdYear != null && rdMonth != null && rdDay != null)
                    //{
                    //    DateTime rd = new DateTime(Int32.Parse(rdYear.InnerText),
                    //                               Int32.Parse(rdMonth.InnerText),
                    //                               Int32.Parse(rdDay.InnerText));

                    //    if (rd != null)
                    //        newTitle.ReleaseDate = rd;
                    //}
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("mpaarating", ""))
                {
                    newTitle.ParentalRating = GetChildNodesValue(nav, "displayname");
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("upc", ""))
                {
                    newTitle.UPC = nav.Value;
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("runtimeminutes", ""))
                {
                    //newTitle.Runtime = nav.ValueAsInt;
                    newTitle.Runtime = ConvertStringToInt(nav.Value);
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("genres", ""))
                {
                    XPathNodeIterator genreIter = nav.SelectChildren("genre", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < genreIter.Count; i++)
                        {
                            newTitle.AddGenre(GetChildNodesValue(localNav, "displayname"));
                            localNav.MoveToNext("genre", "");
                        }
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("cast", ""))
                {
                    XPathNodeIterator starIter = nav.SelectChildren("star", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < starIter.Count; i++)
                        {
                            string role = GetChildNodesValue(localNav, "role");
                            XPathNavigator personNav = localNav.SelectSingleNode("person");
                            if (personNav != null)
                            {
                                string name = GetChildNodesValue(personNav, "displayname");
                                if (!string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(name))
                                {
                                    newTitle.AddActingRole(name, role);
                                }
                            }
                            localNav.MoveToNext("star", "");
                        }
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("crew", ""))
                {
                    XPathNodeIterator crewMemberIter = nav.SelectChildren("crewmember", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < crewMemberIter.Count; i++)
                        {
                            string role = GetChildNodesValue(localNav, "role");
                            XPathNavigator cmNav = localNav.SelectSingleNode("person");
                            if (cmNav != null)
                            {
                                string name = GetChildNodesValue(cmNav, "displayname");
                                if (!string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(name))
                                {
                                    switch (role.ToLower())
                                    {
                                        case "director":
                                            newTitle.AddDirector(new Person(name));
                                            break;
                                        case "writer":
                                            newTitle.AddWriter(new Person(name));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            localNav.MoveToNext("crewmember", "");
                        }
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("subtitles", ""))
                {
                    XPathNodeIterator subtitleIter = nav.SelectChildren("subtitle", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < subtitleIter.Count; i++)
                        {
                            newTitle.AddSubtitle(GetChildNodesValue(localNav, "displayname"));
                            localNav.MoveToNext("subtitle", "");
                        }
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("audios", ""))
                {
                    XPathNodeIterator audioIter = nav.SelectChildren("audio", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < audioIter.Count; i++)
                        {
                            newTitle.AddAudioTrack(GetChildNodesValue(localNav, "displayname"));
                            localNav.MoveToNext("audio", "");
                        }
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("studios", ""))
                {
                    XPathNodeIterator studioIter = nav.SelectChildren("studio", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < studioIter.Count; i++)
                        {
                            newTitle.Studio = GetChildNodesValue(localNav, "displayname");
                            localNav.MoveToNext("studio", "");
                        }
                    }
                    nav.MoveToParent();
                }

                if (nav.MoveToChild("links", ""))
                {
                    XPathNodeIterator linkIter = nav.SelectChildren("link", "");
                    if (nav.MoveToFirstChild())
                    {
                        XPathNavigator localNav = nav.CreateNavigator();
                        nav.MoveToParent();

                        for (int i = 0; i < linkIter.Count; i++)
                        {
                            string type = GetChildNodesValue(localNav, "urltype");
                            if (!string.IsNullOrEmpty(type))
                            {
                                if (type.ToUpper().CompareTo("MOVIE") == 0)
                                {
                                    string path = GetChildNodesValue(localNav, "url");
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        try
                                        {
                                            FileInfo fi = new FileInfo(path);
                                            if (fi.Exists)
                                            {
                                                string ext = fi.Extension.Substring(1);
                                                if (!string.IsNullOrEmpty(ext))
                                                {
                                                    if (IsSupportedFormat(ext))
                                                    {
                                                        Disk disk = new Disk();
                                                        disk.Format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                                                        disk.Path = path;
                                                        disk.Name = GetChildNodesValue(localNav, "description");
                                                        newTitle.AddDisk(disk);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Utilities.DebugLine("MovieCollectorzPlugin: {0}", ex);
                                        }
                                    }
                                }
                            }
                            localNav.MoveToNext("link", "");
                        }
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

        private static double MajorVersion = 1.0;
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

        private int ConvertStringToInt(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            int result = 0;
            int.TryParse(Regex.Replace(str, @"\D+", ""), out result);
            return result;
        }
    }
}
