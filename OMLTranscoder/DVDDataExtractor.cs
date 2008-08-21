/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.IO;
using OMLEngine;

namespace OMLTranscoder
{
    public class DVDDataExtractor
    {
        private string physical_media_path;

        public DVDDataExtractor(Title title)
        {
            physical_media_path = title.SelectedDisk.Path;
            if (!File.Exists(physical_media_path) || !Directory.Exists(physical_media_path))
            {
                Utilities.DebugLine("[DVDDataExtractor] Physical path appears to be invalid: " + physical_media_path);
            }

            MEncoderCommandBuilder cmdBuilder = new MEncoderCommandBuilder();
        }
    }
}