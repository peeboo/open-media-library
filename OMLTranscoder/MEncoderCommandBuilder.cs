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
        private SubtitleStream subStream;
        private AudioStream audioStream;

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

            //audio format
            if (audioFormat == MEncoder.AudioFormat.NoAudio)
                cmdString += @" -nosound";
            else
                cmdString += string.Format(@" -oac {0}", (string)Enum.GetName(typeof(MEncoder.AudioFormat), audioFormat));

            //audio stream
            if ((audioStream != null) && (audioFormat != MEncoder.AudioFormat.NoAudio))
            {
                cmdString += @"";
            }

            //video
            cmdString += string.Format(@" -ovc {0}", (string)Enum.GetName(typeof(MEncoder.VideoFormat), videoFormat));

            //subtitles
            if (subStream != null)
            {
                cmdString += string.Format(@" -sub {0}", subStream.SubtitleChannel);
            }

            //chapters

            // output format
            cmdString += @" -of mpeg"; // always set the output format to mpeg for extenders

            //output
            cmdString += string.Format(@" -o {0}", outputFile);

            return cmdString;
        }

        public void SetSubtitleStream(SubtitleStream stream)
        {
        }

        public void PresetSubtitleDetailsComamnd()
        {

        }
    }
}
