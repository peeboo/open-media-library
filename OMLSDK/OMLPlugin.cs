using System;
using System.Collections.Generic;
using System.Data;
using OMLEngine;

namespace OMLSDK
{
    public class OMLPlugin :IOMLPlugin
    {
        List<Title> titles;
        private int totalRowsAdded = 0;

        public virtual string GetName()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual string GetDescription()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual string GetAuthor()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual bool Load(string filename, bool ShouldCopyImages)
        {
            throw new Exception("You must implement this method in your class.");
        }
        public int GetTotalTitlesAdded()
        {
            return totalRowsAdded;
        }
        public List<Title> GetTitles()
        {
            return titles;
        }
        public OMLPlugin()
        {
            titles = new List<Title>();
        }
        public Title newTitle()
        {
            return new Title();
        }
        public void AddTitle(Title newTitle)
        {
            titles.Add(newTitle);
            totalRowsAdded++;
        }
        public bool ValidateTitle(Title title_to_validate)
        {
            return true;
        }
        public bool IsSupportedFormat(string file_extension)
        {
            string[] formats = Enum.GetNames(typeof(VideoFormat));
            foreach (string format in formats)
            {
                if (file_extension.ToUpper().CompareTo(format.ToUpper()) == 0)
                    return true;
            }
            return false;
        }
    }
}
