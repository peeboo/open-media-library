using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLDatabaseEditor
{
    [Serializable]
    public class EditorSettings
    {
        public string ImageRootDirectory = string.Empty;
        public string ImageEditor = string.Empty;
        public bool ImportAll = true;
        public bool CopyCoverArt = false;
    }
}
