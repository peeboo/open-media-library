using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using OMLEngine;

namespace OMLFileWatcher
{
    public class OMLFileWatcher
    {
        private string fTitles = System.AppDomain.CurrentDomain.BaseDirectory + @"Titles.xml";
        private List<FileSystemWatcher> fsw;
        private List<title> Titles;

        public OMLFileWatcher()
        {
            Titles = new List<title>();
            fsw = new List<FileSystemWatcher>();
            LoadTitles();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The path to be monitored</param>
        /// <param name="filter">The type of files to watch.  For example, "*.txt" watches text files.</param>
        /// <remarks>Does not monitor subdirectories</remarks>
        public void AddWatch(string path, string filter)
        {
            AddWatch(path, filter, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The path to be monitored</param>
        /// <param name="filter">The type of files to watch.  For example, "*.txt" watches text files.</param>
        /// <param name="subdirectories">Include Subdirectories within the specified path to be monitored</param>
        public void AddWatch(string path, string filter, bool subdirectories)
        {
            Utilities.DebugLine("[OMLFileWatcher] AddWatch (Path:{0}, Filter:{1}, Sub:{2})", path, filter, subdirectories);
            FileSystemWatcher lFSW = new FileSystemWatcher(path, filter);
            lFSW.IncludeSubdirectories = subdirectories;
            lFSW.Renamed += new RenamedEventHandler(this.fsw_Renamed);
            lFSW.Deleted += new FileSystemEventHandler(this.fsw_Deleted);
            lFSW.Created += new FileSystemEventHandler(this.fsw_Created);
            lFSW.Changed += new FileSystemEventHandler(this.fsw_Changed);
            fsw.Add(lFSW);
        }

        public bool EnableRaisingEvents
        {
            get { if (fsw.Count > 0) { return fsw[0].EnableRaisingEvents; } else { return true; } }
            set
            {
                for (int ii = 0; ii < fsw.Count; ii++)
                {
                    fsw[ii].EnableRaisingEvents = value;
                }
            }
        }

        public delegate void eChanged(object sender, System.IO.FileSystemEventArgs e);
        public delegate void eCreated(object sender, System.IO.FileSystemEventArgs e);
        public delegate void eDeleted(object sender, System.IO.FileSystemEventArgs e);
        public delegate void eRenamed(object sender, System.IO.RenamedEventArgs e);

        private void fsw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            OnChanged(e);
            string filepath = Path.GetDirectoryName(e.FullPath);
            string fileext = Path.GetExtension(e.FullPath);

            title oTitle = FindTitle(e.FullPath);
            SaveTitles();
        }

        public event eChanged Changed;
        public event eCreated Created;
        public event eDeleted Deleted;
        public event eRenamed Renamed;

        private void OnChanged(FileSystemEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private void fsw_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            OnCreated(e);
            string filepath = Path.GetDirectoryName(e.FullPath);
            string fileext = Path.GetExtension(e.FullPath);

            Titles.Add(new title(e.FullPath));
            SaveTitles();
        }

        private void OnCreated(FileSystemEventArgs e)
        {
            if (Created != null)
                Created(this, e);
        }

        private void fsw_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            OnDeleted(e);
            string filepath = Path.GetDirectoryName(e.FullPath);
            string fileext = Path.GetExtension(e.FullPath);
            title oTitle = FindTitle(e.FullPath);
            if (oTitle != null)
            {
                Titles.Remove(oTitle);
            }
            SaveTitles();
        }

        private void OnDeleted(FileSystemEventArgs e)
        {
            if (Deleted != null)
                Deleted(this, e);
        }

        private void fsw_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            OnRenamed(e);
            string filepath = Path.GetDirectoryName(e.FullPath);
            string fileext = Path.GetExtension(e.FullPath);
            title oTitle = FindTitle(e.FullPath);
            if (oTitle != null)
            {
                oTitle.Filename = e.FullPath;
                if (oTitle.Title == Path.GetFileNameWithoutExtension(e.OldFullPath))
                {
                    oTitle.Title = Path.GetFileNameWithoutExtension(e.FullPath);
                }
            }
            SaveTitles();
        }

        private void OnRenamed(RenamedEventArgs e)
        {
            if (Renamed != null)
                Renamed(this, e);
        }

        private void LoadTitles()
        {
            if (System.IO.File.Exists(fTitles))
            {
                XmlDocument xDoc = new XmlDocument();
                XmlElement xTitle;
                xDoc.Load(fTitles);
                if (xDoc.DocumentElement.HasChildNodes)
                {
                    foreach (XmlNode xNode in xDoc.DocumentElement.ChildNodes)
                    {
                        xTitle = (XmlElement)xNode;
                        title oTitle = new title();
                        XmlAttribute xAttr = (XmlAttribute)xTitle.Attributes.GetNamedItem(@"filename");
                        if (xAttr != null)
                        {
                            oTitle.Filename = xAttr.Value;
                            oTitle.Title = xTitle.InnerText;
                            Titles.Add(oTitle);
                        }
                    }
                }
            }
            Utilities.DebugLine("[OMLFileWatcher] LoadTitles (#{0} titles)", Titles.Count);
        }

        private void SaveTitles()
        {
            Utilities.DebugLine("[OMLFileWatcher] SaveTitles (#{0} titles)", Titles.Count);

            XmlDocument xDoc = new XmlDocument();
            XmlElement xTitle;
            xDoc.LoadXml(@"<xml/>");
            foreach (title oTitle in Titles)
            {
                xTitle = (XmlElement)xDoc.DocumentElement.AppendChild(xDoc.CreateElement(@"title"));
                xTitle.Attributes.Append(xDoc.CreateAttribute(@"filename"));
                xTitle.SetAttribute(@"filename", oTitle.Filename);
                xTitle.InnerText = oTitle.Title;
            }
            File.WriteAllText(fTitles, xDoc.OuterXml);
        }

        private title FindTitle(String fPath)
        {
            title oTitle = null;
            List<title> query = new List<title>();
            foreach (title lTitle in Titles)
            {
                if (lTitle.Filename == fPath)
                {
                    query.Add(oTitle);
                }
            }
            if (query.Count == 1)
            {
                oTitle = query[0];
            }
            return oTitle;
        }

    }
}
