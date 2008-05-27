using System;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace OMLEngine
{
    public class OMLConfigManager
    {
        private static string OML_CONFIG_FILE = FileSystemWalker.RootDirectory + @"\\oml.config";
        private static XmlDocument xDoc = null;
        private static XmlNode rootNode = null;

        public OMLConfigManager()
        {
            xDoc = new XmlDocument();
            try
            {
                FileInfo fi = new FileInfo(OML_CONFIG_FILE);
                if (fi.Exists)
                {
                    xDoc.Load(OML_CONFIG_FILE);
                    rootNode = xDoc.SelectSingleNode("OpenMediaLibrary");
                }
                else
                {
                    XmlNode node = xDoc.CreateNode(XmlNodeType.Element, "OpenMediaLibrary", "");
                    xDoc.AppendChild(node);
                    rootNode = node;
                }
            }
            catch (Exception e)
            {
                Utilities.DebugLine("Error on init: " + e.Message);
            }
        }

        ~OMLConfigManager()
        {
            xDoc.Save(OML_CONFIG_FILE);
        }

        public void LoadConfig()
        {
            xDoc.Load(OML_CONFIG_FILE);
        }

        public void SaveConfig()
        {
            xDoc.Save(OML_CONFIG_FILE);
        }

        public string GetValue(string keyName)
        {
            try
            {
                XmlNode node = rootNode.SelectSingleNode(keyName);
                if (node != null)
                    return node.InnerText;

            }
            catch (Exception e)
            {
                Utilities.DebugLine("Error: " + e.Message);
            }
            return null;
        }

        public void SetValue(string keyName, string value)
        {
            try
            {
                if (rootNode != null)
                {
                    XmlNode node = rootNode.SelectSingleNode(keyName);
                    if (node != null)
                    {
                        node.InnerText = value;
                    }
                    else
                    {
                        XmlNode newNode = xDoc.CreateNode(XmlNodeType.Element, keyName, "");
                        if (newNode != null)
                        {
                            newNode.InnerText = value;
                            rootNode.AppendChild(newNode);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.DebugLine("Error: " + e.Message);
            }
        }
    }
}
