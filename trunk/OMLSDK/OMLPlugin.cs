using System;
using System.Collections.Generic;
using System.Data;
using OMLEngine;

namespace OMLSDK
{
    public class OMLPlugin
    {
        List<Title> titles;
        private int totalRowsAdded = 0;
        private string _description = @"";
        private string _author = @"";
        private string _name = "OMLPlugin";

        public int GetTotalTitlesAdded
        {
            get { return totalRowsAdded; }
        }
        public List<Title> GetTitles
        {
            get { return titles; }
        }
        public string GetName
        {
            get { return _name; }
        }
        public string GetDescription
        {
            get { return _description; }
        }
        public string GetAuthor
        {
            get { return _author; }
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
    }
}
