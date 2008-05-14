using System;
using System.Xml;

namespace OMLEngine
{
    public class WindowsPlayListManager
    {
        SortedArrayList PlayListItems;

        public WindowsPlayListManager()
        {
            PlayListItems = new SortedArrayList();
        }

        public WindowsPlayListManager(string filePath)
        {
            PlayListItems = new SortedArrayList();
        }

        ~WindowsPlayListManager()
        {
        }

        public void AddItem(PlayListItem item)
        {
            item.SortOrder = PlayListItems.Count;
            PlayListItems.Add(item);
        }

        public PlayListItem RemoveItem(PlayListItem item)
        {
            return null;
        }

        public void WriteWPLFile(string filePath)
        {
            // pre-sort the items.
            PlayListItems.Sort();

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

            // TotalDuration tag
            XmlNode totalDurationNode = xDoc.CreateNode(XmlNodeType.Element, "meta", "");
            XmlAttribute tdAttr1 = xDoc.CreateAttribute("name");
            tdAttr1.InnerText = "TotalDuration";
            totalDurationNode.Attributes.Append(tdAttr1);
            XmlAttribute tdAttr2 = xDoc.CreateAttribute("content");
            tdAttr2.InnerText = "";
            totalDurationNode.Attributes.Append(tdAttr2);
            headNode.AppendChild(totalDurationNode);

            // ItemCount tag
            XmlNode icNode = xDoc.CreateNode(XmlNodeType.Element, "meta", "");
            XmlAttribute icAttr1 = xDoc.CreateAttribute("name");
            icAttr1.InnerText = "ItemCount";
            icNode.Attributes.Append(icAttr1);
            XmlAttribute icAttr2 = xDoc.CreateAttribute("content");
            icAttr2.InnerText = "";
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
            foreach (PlayListItem item in PlayListItems)
            {
                XmlNode mediaNode = xDoc.CreateNode(XmlNodeType.Element, "media", "");
                XmlAttribute srcAttr = xDoc.CreateAttribute("src");
                srcAttr.InnerText = item.title.FileLocation;
                mediaNode.Attributes.Append(srcAttr);
                bodySeqNode.AppendChild(mediaNode);
            }

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

        public void ReadWPLFIle(string filePath)
        {
        }
    }

    public class PlayListItem : IComparable
    {
        private Title _title;
        private int _sortOrder;

        public PlayListItem(Title t)
        {
            _title = t;
        }

        public Title title
        {
            get { return _title; }
            set { _title = value; }
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
