/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using OMLEngine;

namespace OMLTranscoder
{
    public class Transcode
    {
        Process _process;
        
        static string MENCODER_PATH = Path.Combine(FileSystemWalker.RootDirectory, @"mencoder.exe");
        static string BUFFER_DIRECTORY = FileSystemWalker.RootDirectory;

        public Process CurrentTranscodeProcess
        {
            get { return _process; }
            set { _process = value; }
        }

        public Transcode()
        {
        }

        ~Transcode()
        {
        }

        public MediaSource MediaSourceFromTitle(Title title)
        {
            MediaSource source = new MediaSource();

            return source;
        }
    }
}
