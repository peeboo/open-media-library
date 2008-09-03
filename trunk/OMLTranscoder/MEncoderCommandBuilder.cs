/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.IO;
using System.Text;
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
        //private string subtitleBaseFileName;
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

        public void SetSubtitleOutputLanguage(string languageId)
        {
        }

        public void SetInputType(MEncoder.InputType type)
        {
            inputType = type;
        }

        public void SetInputLocation(FileInfo fInfo)
        {
            if (inputType == MEncoder.InputType.File)
                inputFile = fInfo;
        }

        public void SetInputLocation(DriveInfo dInfo)
        {
            if (inputType == MEncoder.InputType.Drive)
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
            if (inputDrive == null && inputFile == null)
            {
                Utilities.DebugLine("[MEncoderCommandBuilder] Missing input drive or filename");
                throw new Exception("Must define either an input drive or an input device");
            }

            StringBuilder strBuilder = new StringBuilder(Path.Combine(FileSystemWalker.RootDirectory, @"mencoder.exe"));

            //audio format
            if (audioFormat == MEncoder.AudioFormat.NoAudio)
                strBuilder.Append(@" -nosound");
            else
            {
                if (audioStream != null)
                    strBuilder.AppendFormat(@" -alang {0}", audioStream.AudioID);
                else
                    strBuilder.Append(@" -alang en");

                strBuilder.AppendFormat(@" -oac {0}",
                                        ((string)Enum.GetName(typeof(MEncoder.AudioFormat), audioFormat)).ToLower());
            }

            //audio stream
            if ((audioStream != null) && (audioFormat != MEncoder.AudioFormat.NoAudio))
            {
                strBuilder.Append(@"");
            }

            //video
            strBuilder.AppendFormat(@" -ovc {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.VideoFormat), videoFormat)).ToLower());

            //subtitles
            if (subStream != null)
                strBuilder.AppendFormat(@" -sub {0}", subStream.SubtitleChannel);

            //chapters


            // input location
            if (inputType == MEncoder.InputType.Drive)
                strBuilder.AppendFormat(@" -dvd-device ""{0}""", inputDrive.RootDirectory);

            if (inputType == MEncoder.InputType.File)
                strBuilder.AppendFormat(@" -dvd-device ""{0}""", inputFile.FullName);

            //input device or file
            strBuilder.Append(@" dvd://");

            // output format
            // always set the output format to mpeg for extenders
            strBuilder.Append(@" -of mpeg -mpegopts format=dvd:tsaf -vf harddup");

            // set quiet mode on
            strBuilder.Append(@" -quiet");

            //output
            strBuilder.AppendFormat(@" -o ""{0}.mpg""", Path.Combine(FileSystemWalker.PublicRootDirectory, outputFile));

            return strBuilder.ToString();
        }
    }
}
