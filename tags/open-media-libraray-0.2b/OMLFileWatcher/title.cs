using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OMLFileWatcher
{
    public class title
    {
        private string _title;
        private string _filename;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public title()
        {
            _title = string.Empty;
            _filename = string.Empty;
        }

        public title(string filename)
        {
            _title = Path.GetFileNameWithoutExtension(filename);
            _filename = filename;
        }
    }
}
