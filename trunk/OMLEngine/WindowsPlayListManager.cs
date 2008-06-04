using System;
using System.Xml;
using System.Collections;

namespace OMLEngine
{
    public class WindowsPlayListManager
    {
        private ArrayList _PlayListItems;

        public ArrayList PlayListItems
        {
            get
            {
                return _PlayListItems;
            }
        }

        public WindowsPlayListManager()
        {
            _PlayListItems = new SortedArrayList();
        }

        public WindowsPlayListManager(string filePath)
        {
            _PlayListItems = new SortedArrayList();
            ReadWPLFile(filePath);
        }

        ~WindowsPlayListManager()
        {
        }

        public void AddItem(PlayListItem item)
        {
            item.SortOrder = _PlayListItems.Count;
            _PlayListItems.Add(item);
        }

        public PlayListItem RemoveItem(PlayListItem item)
        {
            return null;
        }

        public void WriteWPLFile(string filePath)
        {
            // pre-sort the items.
            _PlayListItems.Sort();

            XmlDocument xDoc = new XmlDocument();

            // root node
            XmlNode rootElem = xDoc.CreateNode(XmlNodeType.Element, "smil", "");

            // head node
            XmlNode headNode = xDoc.CreateNode(XmlNodeType.Element, "head", "");

            // Generator tag
            XmlNode generatorNode = xDoc.CreateNode(XmlNodeType.Element, "meta", "");
            XmlAttribute genAttr1 = xDoc.CreateAttribute("name");
            genAttr1.InnerText = "Generator";
            generatorNode.Attributes.Append(genAttr1);
            XmlAttribute genAttr2 = xDoc.CreateAttribute("content");
            genAttr2.InnerText = "OpenMediaLibrary - OMLEngine";
            generatorNode.Attributes.Append(genAttr2);
            headNode.AppendChild(generatorNode);

            // ItemCount tag
            XmlNode icNode = xDoc.CreateNode(XmlNodeType.Element, "meta", "");
            XmlAttribute icAttr1 = xDoc.CreateAttribute("name");
            icAttr1.InnerText = "ItemCount";
            icNode.Attributes.Append(icAttr1);
            XmlAttribute icAttr2 = xDoc.CreateAttribute("content");
            icAttr2.InnerText = "0";
            icNode.Attributes.Append(icAttr2);
            headNode.AppendChild(icNode);

            // name of the playlist
            XmlNode nameNode = xDoc.CreateNode(XmlNodeType.Element, "title", "");
            XmlNode nameTextNode = xDoc.CreateTextNode("Blah");
            nameNode.AppendChild(nameTextNode);
            headNode.AppendChild(nameNode);

            // body node
            XmlNode bodyNode = xDoc.CreateNode(XmlNodeType.Element, "body", "");
            XmlNode bodySeqNode = xDoc.CreateNode(XmlNodeType.Element, "seq", "");
            int totalItems = 0;
            foreach (PlayListItem item in PlayListItems)
            {
                XmlNode mediaNode = xDoc.CreateNode(XmlNodeType.Element, "media", "");
                XmlAttribute srcAttr = xDoc.CreateAttribute("src");
                srcAttr.InnerText = item.FileLocation;
                mediaNode.Attributes.Append(srcAttr);
                bodySeqNode.AppendChild(mediaNode);
                totalItems++;
            }

            // set total items on the ItemCount node
            icAttr2.InnerText = totalItems.ToString();

            // put the bodySeqNode on the body
            bodyNode.AppendChild(bodySeqNode);

            // put headNode on the root
            rootElem.AppendChild(headNode);
            // put the bodyNode on the root
            rootElem.AppendChild(bodyNode);

            // put rootNode on the doc
            xDoc.AppendChild(rootElem);

            // write out the playlist
            xDoc.Save(filePath);
        }

        public void ReadWPLFile(string filePath)
        {
            Utilities.DebugLine("[WindowsPlayListManager] Reading Playlist file");
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(filePath);

                XmlNodeList mediaNodes = xDoc.SelectNodes("//smil/body/seq/media");
                Utilities.DebugLine("[WindowsPlayListManager] Found " + mediaNodes.Count + " media nodes");
                foreach (XmlNode mediaNode in mediaNodes)
                {
                    foreach (XmlAttribute attr in mediaNode.Attributes)
                    {
                        if (attr.Name.ToUpper().CompareTo("SRC") == 0)
                        {
                            Utilities.DebugLine("[WindowsPlayListManager] Found media item: " + attr.Value);
                            _PlayListItems.Add(new PlayListItem(attr.Value));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.DebugLine("[WindowsPlayListManager] Error: " + e.Message);
            }
        }
    }

    public class PlayListItem : IComparable
    {
        private string _fileLocation;
        private int _sortOrder;

        public PlayListItem(string FileLocation)
        {
            _fileLocation = FileLocation;
        }

        public string FileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; }
        }

        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public int CompareTo(object other_item)
        {
            return this._sortOrder.CompareTo(((PlayListItem)other_item)._sortOrder);
        }
    }
}
