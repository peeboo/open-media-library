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
            string basePath = FileSystemWalker.RootDirectory;
            if (Directory.Exists(basePath))
            {
                string[] fileNames = Directory.GetFiles(basePath, "*mencoder*.exe", SearchOption.TopDirectoryOnly);
                if (fileNames.Length > 0)
                {
                    Array.Sort(fileNames);
                    string completedCommand = Path.Combine(basePath, fileNames[0]);
                    Utilities.DebugLine("[MEncoderCommandBuilder] Command: {0}", completedCommand);
                    return completedCommand;
                }
            }
            return null;
        }

        public string GetArguments()
        {
            if (inputDrive == null && inputFile == null)
            {
                Utilities.DebugLine("[MEncoderCommandBuilder] Missing input drive or filename");
                throw new Exception("Must define either an input drive or an input device");
            }

            StringBuilder strBuilder = new StringBuilder();

            // input location
            //strBuilder.AppendFormat(@" -dvd-device ""{0}""", ((inputType == MEncoder.InputType.Drive) ? inputDrive.RootDirectory.FullName : inputFile.FullName));
            if (inputType == MEncoder.InputType.File)
                strBuilder.AppendFormat(@"""{0}""", inputFile.FullName);

            if (inputType == MEncoder.InputType.Drive)
                strBuilder.AppendFormat(@" -dvd-device ""{0}""", inputDrive.Name);

            //input device or file
            if (inputType == MEncoder.InputType.Drive)
                strBuilder.Append(@" dvd://");

            //audio format
            /* DISABLED UNTIL AUDIO STREAM CLASSES ARE FINALIZED
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
            */

            strBuilder.AppendFormat(@" -oac {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.AudioFormat), audioFormat)).ToLower());

            //video
            strBuilder.AppendFormat(@" -ovc {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.VideoFormat), videoFormat)).ToLower());

            //subtitles
            /* DISABLED UNTIL SUBPICTURE STREAM CLASSES ARE FINALIZED
            if (subStream != null)
                strBuilder.AppendFormat(@" -sub {0}", subStream.SubtitleChannel);
            */
            //chapters

            // output format
            // always set the output format to mpeg for extenders
            if (inputType == MEncoder.InputType.Drive)
            {
                strBuilder.Append(@" -of mpeg -mpegopts format=dvd:tsaf -vf harddup");
                strBuilder.Append(@" -lavcopts vcodec=msmpeg4v2:vpass=1");
            }

            // set quiet mode on
            strBuilder.Append(@" -quiet");

            //output
            strBuilder.AppendFormat(@" -o ""{0}""", Path.Combine(FileSystemWalker.PublicRootDirectory, outputFile));

            string completedArguments = strBuilder.ToString();
            Utilities.DebugLine("[MEncoderCommandBuilder] Arguments: {0}", completedArguments);
            return completedArguments;
        }
    }
}
