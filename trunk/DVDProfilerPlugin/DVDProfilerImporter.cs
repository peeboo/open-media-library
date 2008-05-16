using System;
using OMLSDK;
using OMLEngine;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace DVDProfilerPlugin
{
    public class DVDProfilerImporter : OMLPlugin, IOMLPlugin
    {
        TextReader tr = null;
        private static double VERSION = 0.1;

        public DVDProfilerImporter()
            : base()
        {
        }

        public override bool Load(string filename)
        {
            try { tr = new StreamReader(filename); }
            catch (Exception e) { Trace.WriteLine(e.Message); }

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

        public override string GetName()
        {
            return "DVDProfilerPlugin";
        }
        public override string GetAuthor()
        {
            return "OML Development Team";
        }
        public override string GetDescription()
        {
            return "DVDProfiler xml file importer for Open Media Library v" + VERSION;
        }

        public string CopyImage(string from_location, string to_location)
        {
            FileInfo fi = new FileInfo(from_location);
            File.Copy(from_location, to_location, true);
            return fi.Name;
        }

        private void process_node_switch(Title newTitle, XmlNode node)
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
                    if (isDVD != null && isDVD.InnerText.ToUpper().CompareTo("TRUE") == 0)
                    {
                        newTitle.VideoFormat = VideoFormat.DVD;
                        break;
                    }
                    if (isHDDVD != null && isHDDVD.InnerText.ToUpper().CompareTo("TRUE") == 0)
                    {
                        newTitle.VideoFormat = VideoFormat.HDDVD;
                        break;
                    }
                    if (isBluRay != null && isBluRay.InnerText.ToUpper().CompareTo("TRUE") == 0)
                    {
                        newTitle.VideoFormat = VideoFormat.BLURAY;
                        break;
                    }
                    break;
                case "UPC":
                    newTitle.UPC = node.InnerText;
                    break;
                case "CountryOfOrigin":
                    newTitle.CountryOfOrigin = node.InnerText;
                    break;
                case "Released":
                    string release_date = node.InnerText;
                    string[] parts = release_date.Split('-');
                    if (parts.Length == 3)
                    {
                        int year = Int32.Parse(parts[0]);
                        int month = Int32.Parse(parts[1]);
                        int day = Int32.Parse(parts[2]);

                        if (year != null && month != null && day != null)
                            newTitle.ReleaseDate = new DateTime(year, month, day);
                    }
                    break;
                case "RunningTime":
                    newTitle.Runtime = Int32.Parse(node.InnerText);
                    break;
                case "Rating":
                    newTitle.MPAARating = node.InnerText;
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
                        newTitle.Distributor = studioNode.InnerText;
                    }
                    break;
                case "Audio":
                    XmlNodeList languages = node.SelectNodes("AudioContent");
                    foreach (XmlNode langNode in languages)
                    {
                        newTitle.AddLanguageFormat(langNode.InnerText);
                    }

                    XmlNodeList soundFormats = node.SelectNodes("AudioFormat");
                    foreach (XmlNode soundFormatNode in soundFormats)
                    {
                        newTitle.AddSoundFormat(soundFormatNode.InnerText);
                    }
                    break;
                case "Actors":
                    XmlNodeList actors = node.SelectNodes("Actor");
                    foreach (XmlNode actorNode in actors)
                    {
                        string first_name = string.Empty;
                        string last_name = string.Empty;

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
                                default:
                                    break;
                            }
                        }
                        if (first_name.Length > 0 && last_name.Length > 0)
                            newTitle.AddActor(new Person(first_name + " " + last_name));
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
