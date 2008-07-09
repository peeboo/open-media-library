using System;
using OMLSDK;
using OMLEngine;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace DVDProfilerPlugin
{
    [CLSCompliant(true)]
    public class DVDProfilerImporter : OMLPlugin, IOMLPlugin
    {

        public DVDProfilerImporter()
            : base()
        {
        }

        public override bool Load(string filename)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);

            XmlNodeList nodeList = xDoc.SelectNodes("//Collection/DVD");
            foreach (XmlNode movieNode in nodeList)
            {
                Title newTitle = new Title();
                // first get the name
                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    if (node.Name.CompareTo("Title") == 0)
                    {
                        newTitle.Name = node.InnerText;
                        break;
                    }
                }

                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    process_node_switch(newTitle, node);
                }

                // attempt to validate the new title before we add it to the stack
                if (ValidateTitle(newTitle))
                {
                    try { AddTitle(newTitle); }
                    catch (Exception e) { Trace.WriteLine("Error adding row: " + e.Message); }
                }
                else Trace.WriteLine("Error saving row");
            }
            return true;
        }

        private static double MajorVersion = 0.9;
        private static double MinorVersion = 0.0;
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
            return "DVD Profiler";
        }
        protected override string GetName()
        {
            return "DVDProfilerPlugin";
        }
        protected override string GetAuthor()
        {
            return "OML Development Team";
        }
        protected override string GetDescription()
        {
            return "DVDProfiler xml file importer for Open Media Library v" + Version;
        }

        private static void process_node_switch(Title newTitle, XmlNode node)
        {
            switch (node.Name)
            {
                case "ID":
                    newTitle.MetadataSourceID = node.InnerText;
                    break;
                case "MediaTypes":
                    XmlNode isDVD = node.SelectSingleNode("DVD");
                    XmlNode isHDDVD = node.SelectSingleNode("HDDVD");
                    XmlNode isBluRay = node.SelectSingleNode("BluRay");
                    Disk disk = new Disk();
                    disk.Name = "Disk 1";
                    if (isDVD != null && String.Compare(isDVD.InnerText, "TRUE", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        disk.Format = VideoFormat.DVD;
                        break;
                    }
                    if (isHDDVD != null && String.Compare(isHDDVD.InnerText, "TRUE", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        disk.Format = VideoFormat.HDDVD;
                        break;
                    }
                    if (isBluRay != null && String.Compare(isBluRay.InnerText, "TRUE", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        disk.Format = VideoFormat.BLURAY;
                        break;
                    }
                    newTitle.Disks.Add(disk);
                    break;
                case "UPC":
                    newTitle.UPC = node.InnerText;
                    break;
                case "CountryOfOrigin":
                    newTitle.CountryOfOrigin = node.InnerText;
                    break;
                case "OriginalTitle":
                    newTitle.OriginalName = node.InnerText;
                    break;
                case "Released":
                    string release_date = node.InnerText;
                    string[] parts = release_date.Split('-');
                    if (parts.Length == 3)
                    {
                        int year; int month; int day;
                        Int32.TryParse(parts[0], out year);
                        Int32.TryParse(parts[1], out month);
                        Int32.TryParse(parts[2], out day);
                        if (year != 0 && month != 0 && day != 0)
                            newTitle.ReleaseDate = new DateTime(year, month, day);
                    }
                    break;
                case "RunningTime":
                    newTitle.Runtime = Int32.Parse(node.InnerText);
                    break;
                case "Rating":
                    newTitle.ParentalRating = node.InnerText;
                    break;
                case "Genres":
                    XmlNodeList genres = node.SelectNodes("Genre");
                    foreach (XmlNode genre in genres)
                    {
                        newTitle.AddGenre(genre.InnerText);
                    }
                    break;
                case "Format":
                    XmlNode aspectRatioNode = node.SelectSingleNode("FormatAspectRatio");
                    if (aspectRatioNode != null)
                        newTitle.AspectRatio = aspectRatioNode.InnerText;
                    XmlNode videoFormatNode = node.SelectSingleNode("FormatVideoStandard");
                    if (videoFormatNode != null)
                        newTitle.VideoStandard = videoFormatNode.InnerText;
                    break;
                case "Studios":
                    XmlNodeList studios = node.SelectNodes("Studio");
                    foreach (XmlNode studioNode in studios)
                    {
                        newTitle.Studio = studioNode.InnerText;
                    }
                    break;
                case "Subtitles":
                    XmlNodeList subtitles = node.SelectNodes("Subtitle");
                    foreach (XmlNode subtitleNode in subtitles)
                    {
                        newTitle.AddSubtitle(subtitleNode.InnerText);
                    }
                    break;
                case "Audio":
                    string audioTrack = "";
                    XmlNodeList languages = node.SelectNodes("AudioContent");
                    foreach (XmlNode langNode in languages)
                    {
                        audioTrack = langNode.InnerText;
                    }

                    XmlNodeList soundFormats = node.SelectNodes("AudioFormat");
                    foreach (XmlNode soundFormatNode in soundFormats)
                    {
                        audioTrack += ", " + soundFormatNode.InnerText;
                    }
                    newTitle.AddLanguageFormat(audioTrack);
                    break;
                case "Actors":
                    XmlNodeList actors = node.SelectNodes("Actor");
                    foreach (XmlNode actorNode in actors)
                    {
                        string first_name = string.Empty;
                        string last_name = string.Empty;
                        string role = string.Empty;

                        XmlAttributeCollection attrs = actorNode.Attributes;
                        foreach (XmlAttribute attr in attrs)
                        {
                            switch (attr.Name)
                            {
                                case "FirstName":
                                    first_name = attr.Value;
                                    break;
                                case "LastName":
                                    last_name = attr.Value;
                                    break;
                                case "Role":
                                    role = attr.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (first_name.Length > 0 && last_name.Length > 0)
                        {
                            newTitle.AddActor(new Person(first_name + " " + last_name));
                            newTitle.AddActingRole(first_name + last_name, role);
                        }
                    }
                    break;
                case "Credits":
                    XmlNodeList credits = node.SelectNodes("Credit");
                    foreach (XmlNode personNode in credits)
                    {
                        string first_name = string.Empty;
                        string last_name = string.Empty;
                        string type_name = string.Empty;

                        XmlAttributeCollection attrs = personNode.Attributes;
                        foreach (XmlAttribute attr in attrs)
                        {
                            switch (attr.Name)
                            {
                                case "FirstName":
                                    first_name = attr.Value;
                                    break;
                                case "LastName":
                                    last_name = attr.Value;
                                    break;
                                case "CreditType":
                                    type_name = attr.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (first_name.Length > 0 && last_name.Length > 0)
                        {
                            switch (type_name)
                            {
                                case "Direction":
                                    newTitle.AddDirector(new Person(first_name + " " + last_name));
                                    break;
                                case "Writing":
                                    newTitle.AddWriter(new Person(first_name + " " + last_name));
                                    break;
                                case "Production":
                                    newTitle.AddProducer(first_name + " " + last_name);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
