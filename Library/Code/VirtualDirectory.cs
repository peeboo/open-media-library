using System;
using System.Collections.Generic;
using System.IO;

namespace Library
{
    public class VirtualDirectory
    {
        #region variables
        private List<VirtualDirectory> childDirectories;
        private DirectoryInfo dirInfo;
        #endregion

        #region properties
        public bool HasChildDirectories
        {
            get { return childDirectories.Count > 0; }
        }

        public bool HasChildFiles
        {
            get { return (Directory.GetFiles(dirInfo.FullName)).Length > 0; }
        }

        public int TotalChildDirectories
        {
            get { return childDirectories.Count; }
        }

        public int TotalChildFiles
        {
            get { return (Directory.GetFiles(dirInfo.FullName)).Length; }
        }

        public string Name
        {
            get { return dirInfo.Name; }
        }

        public DateTime LastWriteTime
        {
            get { return dirInfo.LastWriteTime; }
        }

        #endregion

        public VirtualDirectory()
        {
            childDirectories = new List<VirtualDirectory>();
        }

        public VirtualDirectory(string path)
        {
            childDirectories = new List<VirtualDirectory>();

            if (Directory.Exists(path))
            {
                dirInfo = new DirectoryInfo(path);

                // add each child directory
                foreach (string childDir in Directory.GetDirectories(path))
                    AddChildDirectory(childDir);
            }
        }

        public VirtualDirectory(string[] paths)
        {
            childDirectories = new List<VirtualDirectory>();

            foreach (string path in paths)
            {
                this.AddChildDirectory(path);
            }
        }

        public void AddChildDirectory(string path)
        {
            VirtualDirectory vd = new VirtualDirectory(path);
            childDirectories.Add(vd);
        }

        public void RemoveChildDirectory(VirtualDirectory dir)
        {
            childDirectories.Remove(dir);
        }

        public string[] GetFiles()
        {
            return Directory.GetFiles(dirInfo.FullName);
        }

        public IList<VirtualDirectory> GetDirectories()
        {
            return childDirectories;
        }
    }
}
