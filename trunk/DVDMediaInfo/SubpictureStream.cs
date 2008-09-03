using System;

namespace DVDMediaInfo
{
    public class SubpictureStream
    {
        private string subpictureCoding;
        private int languageId;
        private string languageExtension;
        private string subpictureType;

        public string Coding
        {
            get { return subpictureCoding; }
            set { subpictureCoding = value; }
        }

        public int LanguageId
        {
            get { return languageId; }
            set { languageId = value; }
        }

        public string LanguageExtension
        {
            get { return languageExtension; }
            set { languageExtension = value; }
        }

        public string Type
        {
            get { return subpictureType; }
            set { subpictureType = value; }
        }

        public SubpictureStream()
        {
        }
    }
}
