using System;
using System.Collections.Generic;
using System.Data;

namespace OMLSDK
{
    public class OMLPlugin
    {
        List<Title> titles;
        private int totalRowsAdded = 0;

        public int TotalRowsAdded
        {
            get { return totalRowsAdded; }
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
        public string getName()
        {
            throw new Exception("Called base class getName()");
        }
    }
}
