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
        private string subtitleBaseFileName;
        private DriveInfo inputDrive;
        private FileInfo inputFile;
        private MEncoder.InputType inputType;

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

        public void SetInputType(MEncoder.InputType type)
        {
            inputType = type;
        }

        public void SetInputLocation(FileInfo fInfo)
        {
            if (inputType == MEncoder.InputType.File || inputType == null)
                inputFile = fInfo;
        }

        public void SetInputLocation(DriveInfo dInfo)
        {
            if (inputType == MEncoder.InputType.Drive || inputType == null)
                inputDrive = dInfo;
        }

        public void SetInputTypeAndLocation(MEncoder.InputType type, string location)
        {
            inputType = type;
            if (inputType == MEncoder.InputType.File)
            {
                if (File.Exists(location))
                    inputFile = new FileInfo(location);
            }

            if (inputType == MEncoder.InputType.Drive)
            {

            }
        }

        public void SetOutputFile(string output)
        {
            outputFile = output;
        }

        public void SetSubtitleStream(SubtitleStream stream)
        {
            subStream = stream;
        }

        public void SetAudioStream(AudioStream stream)
        {
            audioStream = stream;
        }

        public string GetCommand()
        {
            if (audioFormat == null)
            {
                Utilities.DebugLine("[MEncoderCommandBuilder] Missing audio format selection");
                throw new Exception("Missing audio format selection");
            }

            if (inputDrive == null && inputFile == null)
            {
                Utilities.DebugLine("[MEncoderCommandBuilder] Missing input drive or filename");
                throw new Exception("Must define either an input drive or an input device");
            }

            if (videoFormat == null)
            {
                Utilities.DebugLine("[MEncoderCommandBuilder] Missing video format selection");
                throw new Exception("Missing video format selection");
            }

            string cmdString = string.Empty;
            cmdString = Path.Combine(FileSystemWalker.RootDirectory, @"mencoder.exe");

            //audio format
            if (audioFormat == MEncoder.AudioFormat.NoAudio)
                cmdString += @" -nosound";
            else
                cmdString += string.Format(@" -oac {0}", ((string)Enum.GetName(typeof(MEncoder.AudioFormat), audioFormat)).ToLower());

            //audio stream
            if ((audioStream != null) && (audioFormat != MEncoder.AudioFormat.NoAudio))
            {
                cmdString += @"";
            }

            //video
            cmdString += string.Format(@" -ovc {0}", ((string)Enum.GetName(typeof(MEncoder.VideoFormat), videoFormat)).ToLower());

            //subtitles
            if (subStream != null)
            {
                cmdString += string.Format(@" -sub {0}", subStream.SubtitleChannel);
            }

            //chapters


            //input device or file
            cmdString += @" dvd://1";

            // input location
            if (inputType == MEncoder.InputType.Drive)
                cmdString += string.Format(@" -dvd-device {0}", inputDrive.RootDirectory);

            if (inputType == MEncoder.InputType.File)
                cmdString += string.Format(@" -dvd-device {0}", inputFile.FullName);

            // output format
            cmdString += @" -of mpeg"; // always set the output format to mpeg for extenders

            //output
            cmdString += string.Format(@" -o {0}.mpg", Path.Combine(FileSystemWalker.PublicRootDirectory, outputFile));

            return cmdString;
        }
    }
}
