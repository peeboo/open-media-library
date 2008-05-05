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
    public class MyMoviesImporter : OMLPlugin
    {
        TextReader tr = null;

        public MyMoviesImporter() : base()
        {
        }

        public bool Load(string filename)
        {
            try { tr = new StreamReader(filename); }
            catch (Exception e) { Trace.WriteLine(e.Message); }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);

            XmlNodeList nodeList = xDoc.SelectNodes("//Titles/Title");
            foreach (XmlNode movieNode in nodeList)
            {
                Title newTitle = new Title();
                // first get the name
                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    if (node.Name.CompareTo("LocalTitle") == 0)
                    {
                        newTitle.Name = node.InnerText;
                        break;
                    }
                }
                foreach (XmlNode node in movieNode.ChildNodes)
                {
                    process_node_switch(newTitle, node);
                }
                if (ValidateTitle(newTitle))
                {
                    try { AddTitle(newTitle); }
                    catch (Exception e) { Trace.WriteLine("Error adding row: " + e.Message); }
                }
                else Trace.WriteLine("Error saving row");
            }
            return true;
        }

        public string GetName()
        {
            return "MyMoviesPlugin";
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
                case "WebServiceId":
                    newTitle.sourceId = node.InnerText;
                    break;
                case "Type":
                    switch (node.InnerText)
                    {
                        case "HD DVD":
                            newTitle.VideoFormat = VideoFormat.HDDVD;
                            break;
                        case "Blu-ray":
                            newTitle.VideoFormat = VideoFormat.BLURAY;
                            break;
                    }
                    break;
                case "Covers":
                    XmlNode front_node = node.ChildNodes[0];
                    if (front_node != null)
                    {
                        string current_filename = front_node.InnerText;
                        FileInfo fi;
                        try {
                            fi = new FileInfo(current_filename);
                            string new_full_name = OMLEngine.FileSystemWalker.ImageDirectory +
                                                   "\\F" + newTitle.Name +
                                                   fi.Extension;
                            CopyImage(current_filename, new_full_name);
                            newTitle.front_boxart_path = new_full_name;
                        }
                        catch (Exception e) { Trace.WriteLine(e.Message); }
                    }
                    XmlNode back_node = node.ChildNodes[1];
                    if (back_node != null)
                    {
                        string current_filename = back_node.InnerText;
                        FileInfo fi;
                        try
                        {
                            fi = new FileInfo(current_filename);
                            string new_full_name = OMLEngine.FileSystemWalker.ImageDirectory +
                                                   "\\B" + newTitle.Name +
                                                   fi.Extension;
                            CopyImage(current_filename, new_full_name);
                            newTitle.back_boxart_path = new_full_name;
                        }
                        catch (Exception e) { Trace.WriteLine(e.Message); }
                    }
                    break;
                case "Description":
                    newTitle.Synopsis = node.InnerText;
                    break;
                case "ReleaseDate":
                    break;
                case "ParentalRating":
                    break;
                case "RunningTime":
                    newTitle.Runtime = node.InnerText;
                    break;
                case "Persons":
                    break;
                case "Studios":
                    break;
            }
        }
    }
}
