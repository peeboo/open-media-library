/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class MEncoder
    {
        public enum AudioFormat
        { 
          NoAudio, // -nosound
          PCM,     // -oac pcm
          TWOLAME, // -oac twolame (MP2)
          MP3LAME, // -oac mp3lame (MP3)
          Copy,    // -oac copy
          LAVC     // FFmpeg audio encoding (mp2, ac3)
        };

        public enum VideoFormat
        { 
          Copy,      // -ovc copy
          RAW,       // -ovc raw
          LAVC,      // -ovc lavc (libavcodec, best quality (this is usually divx)
          VideoForWindows, // -ovc video for windows
          qtvideo,   // -ovc qtvideo - svq1/3 are supported
          XviD,      // -ovc xvid
          X264       // -ovc x264 H.264 encoding
        };
    }
}
