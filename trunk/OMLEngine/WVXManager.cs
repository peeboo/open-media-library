using System;
using System.Xml;
using System.Collections;
using System.IO;

namespace OMLEngine
{
    /// <summary>
    /// 
    /// </summary>
    public class WVXManager
    {
        private ArrayList _PlayListItems;

        /// <summary>
        /// 
        /// </summary>
        public ArrayList PlayListItems
        {
            get
            {
                return _PlayListItems;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WVXManager()
        {
            _PlayListItems = new SortedArrayList();
        }

        public WVXManager(string filePath)
        {
            _PlayListItems = new SortedArrayList();
            ReadWVXFile(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        ~WVXManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(PlayListItem item)
        {
            item.SortOrder = _PlayListItems.Count;
            _PlayListItems.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public PlayListItem RemoveItem(PlayListItem item)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void WriteWVXFile(string filePath)
        {
            _PlayListItems.Sort();
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(string.Format("<ASX version = \"3.0\">  <TITLE>{0}</TITLE>    ", ((PlayListItem)PlayListItems[0]).Name));
                
                foreach (PlayListItem item in PlayListItems)
                {
                    sw.Write("<ENTRY>   <TITLE>{0}</TITLE>   <REF HREF = \"file://{1}\" /> </ENTRY> ", item.Name, item.FileLocation);
                }
                sw.Write(" </ASX> ");
            }
        }

        public void ReadWVXFile(string filePath)
        {
            Utilities.DebugLine("[WVXManager] Reading Playlist file");
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(filePath);

                XmlNodeList mediaNodes = xDoc.SelectNodes("//ASX/ENTRY");
                Utilities.DebugLine("[WVXManager] Found " + mediaNodes.Count + " media nodes");
                foreach (XmlNode mediaNode in mediaNodes)
                {
                    string itemName = mediaNode.SelectSingleNode("TITLE").InnerText;
                    string itemPath=mediaNode.SelectSingleNode("REF").Attributes["HREF"].Value;
                            Utilities.DebugLine("[WindowsPlayListManager] Found media item: " + itemPath);
                            _PlayListItems.Add(new PlayListItem(itemPath, itemName));
                }
            }
            catch (Exception e)
            {
                Utilities.DebugLine("[WVXManager] Error: " + e.Message);
            }
        }
    }
}
