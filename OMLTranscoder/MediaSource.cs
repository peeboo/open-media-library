/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class MediaSource
    {
        int numAudioChannels = 0;
        int audioId = -1;
        int audioType = 0;
        float framerate = 0f;
        int height = 0;
        int width = 0;
        string path = string.Empty;
        float ratio = 0f;
        int subtitleId = -1;
        string subtitles = string.Empty;
        long timestampOffset = 0L;
        int videoType = 0;
        string volume = string.Empty;

        public string Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public int VideoType
        {
            get { return videoType; }
            set { videoType = value; }
        }

        public long TimestampOffset
        {
            get { return timestampOffset; }
            set { timestampOffset = value; }
        }

        public string Subtitles
        {
            get { return subtitles; }
            set { subtitles = value; }
        }

        public int SubtitleID
        {
            get { return subtitleId; }
            set { subtitleId = value; }
        }

        public float Ratio
        {
            get { return ratio; }
            set { ratio = value; }
        }

        public string MediaPath
        {
            get { return path; }
            set { path = value; }
        }

        public int NumberOfAudioChannels
        {
            get { return numAudioChannels; }
            set { numAudioChannels = value; }
        }

        public int AudioID
        {
            get { return audioId; }
            set { audioId = value; }
        }

        public int AudioType
        {
            get { return audioType; }
            set { audioType = value; }
        }

        public float Framerate
        {
            get { return framerate; }
            set { framerate = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public MediaSource()
        {
        }
    }
}
