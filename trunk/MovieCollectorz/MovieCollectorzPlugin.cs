using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OMLSDK;
using OMLEngine;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;

namespace MovieCollectorz
{
    public class MovieCollectorzPlugin : OMLPlugin
    {
        TextReader tr = null;

        public MovieCollectorzPlugin() : base()
        {
        }
        public bool Load(string filename)
        {
            try
            {
                tr = new StreamReader(filename);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
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
                            newTitle.itemId = int.Parse(node.InnerText);
                            break;
                        case "coverfront":
                            newTitle.front_boxart_path = node.InnerText;
                            break;
                        case "format":
                            break;
                        case "language":
                            break;
                        case "title":
                            newTitle.Name = node.InnerText;
                            break;
                        case "plot":
                            newTitle.Synopsis = node.InnerText;
                            break;
                        case "releasedate":
                            XmlNode rdYear = node.SelectSingleNode("//year");
                            XmlNode rdMonth = node.SelectSingleNode("//month");
                            XmlNode rdDay = node.SelectSingleNode("//day");

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
                            newTitle.Runtime = node.InnerText;
                            break;
                        case "cast":
                            List<string> actors = new List<string>();
                            XmlNodeList persons = node.SelectNodes("//person");
                            foreach (XmlNode person in persons)
                            {
                                XmlNode disNameNode = person.SelectSingleNode("displayname");
                                if (disNameNode != null)
                                    newTitle.AddActor(disNameNode.InnerText);
                            }
                            break;
                        case "crew":
                            break;
                        case "studios":
                            break;
                        case "distributor":
                            XmlNode distNode = node.SelectSingleNode("displayname");
                            if (distNode != null)
                                newTitle.Distributor = distNode.InnerText;
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
                        Trace.WriteLine("Error adding row: " + e.Message);
                    }
                }
                else
                    Trace.WriteLine("Error saving row");
            }
            CompleteAdditions();

            return true;
        }
        public string GetName()
        {
            return "MovieCollectorzPlugin";
        }
    }
}
