/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.IO;
using OMLEngine;

namespace OMLTranscoder
{
    public class MEncoderCommandBuilder
    {
        private string outputFile;
        private MEncoder.AudioFormat audioFormat;
        private MEncoder.VideoFormat videoFormat;

        public MEncoderCommandBuilder()
        {
            outputFile = string.Empty;
        }

        public void SetVideoOutputFormat(MEncoder.VideoFormat format)
        {
            videoFormat = format;
        }

        public void SetAudioOutputFormat(MEncoder.AudioFormat format)
        {
            audioFormat = format;
        }

        public void SetSubtitleOutputLanguage()
        {
        }

        public void SetOutputFile(string output)
        {
            outputFile = output;
        }

        public string GetCommand()
        {
            string cmdString = string.Empty;
            cmdString = Path.Combine(FileSystemWalker.RootDirectory, @"mencoder.exe");

            //audio
            if (audioFormat == MEncoder.AudioFormat.NoAudio)
                cmdString += @" -nosound";
            else
                cmdString += @" -oac " + (string)Enum.GetName(typeof(MEncoder.AudioFormat), audioFormat);

            //video
            cmdString += @" -ovc " + (string)Enum.GetName(typeof(MEncoder.VideoFormat), videoFormat);

            //subtitles

            //chapters

            //output
            cmdString += @" -o " + outputFile;

            return cmdString;
        }

        public void PresetSubtitleDetailsComamnd()
        {

        }
    }
}
