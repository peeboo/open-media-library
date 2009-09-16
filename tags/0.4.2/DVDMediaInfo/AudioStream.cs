using System;

namespace DVDMediaInfo
{
    public class AudioStream
    {
        public string Format { get; set; }
        public byte NumberOfChannels { get; set; }
        public int LanguageId { get; set; }
        public string LanguageExtension { get; set; }
    }
}
