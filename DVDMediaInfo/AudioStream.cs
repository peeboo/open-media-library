using System;

namespace DVDMediaInfo
{
    public class AudioStream
    {
        private string format;
        private byte numChannels;
        private int languageId;
        private string languageExtension;

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public byte NumberOfChannels
        {
            get { return numChannels; }
            set { numChannels = value; }
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

        public AudioStream()
        {
        }
    }
}
