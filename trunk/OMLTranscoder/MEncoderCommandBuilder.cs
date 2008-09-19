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
        public string OutputPath { get; set; }
        public string OutputFile { get; set; }
        public MEncoder.AudioFormat AudioFormat { get; set; }
        public MEncoder.VideoFormat VideoFormat { get; set; }
        public SubtitleStream SubtitleStream { get; set; }
        public AudioStream AudioStream { get; set; }

        //private string subtitleBaseFileName;

        private DriveInfo inputDrive;
        private FileInfo inputFile;
        private MEncoder.InputType inputType;

        public string InputLocation
        {
            get { return null; }
            set
            {
                if (File.Exists(value))
                {
                    inputType = MEncoder.InputType.File;
                    inputFile = new FileInfo(value);
                    OutputFile = Path.Combine(OutputPath, Path.GetFileName(value));
                }
                else if (Directory.Exists(value))
                {
                    inputType = MEncoder.InputType.Drive;
                    inputDrive = new DriveInfo(value.Substring(0, 1));
                    try
                    {
                        if (inputDrive.IsReady == false)
                            inputDrive = null;
                        else
                            OutputFile = Path.Combine(OutputPath, inputDrive.VolumeLabel);
                    }
                    catch
                    {
                        inputDrive = null;
                    }
                }
            }
        }

        static string _command = null;
        public string GetCommand()
        {
            if (_command == null)
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
                        _command = completedCommand;
                    }
                }
            }
            return _command;
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
            switch (inputType)
            {
                case MEncoder.InputType.File:
                    strBuilder.AppendFormat(@"""{0}""", inputFile.FullName);
                    break;
                case MEncoder.InputType.Drive:
                    strBuilder.Append(@" dvd://");
                    strBuilder.AppendFormat(@" -dvd-device ""{0}:\""", inputDrive.RootDirectory);
                    break;
                default:
                    Utilities.DebugLine("No idea what format the file is");
                    throw new Exception("Can't determine input format (file or drive)");
            }

            //audio format
            /* DISABLED UNTIL AUDIO STREAM CLASSES ARE FINALIZED
            if (AudioFormat == MEncoder.AudioFormat.NoAudio)
                strBuilder.Append(@" -nosound");
            else
            {
                if (audioStream != null)
                    strBuilder.AppendFormat(@" -alang {0}", audioStream.AudioID);
                else
                    strBuilder.Append(@" -alang en");
            }

            //audio stream
            if ((audioStream != null) && (AudioFormat != MEncoder.AudioFormat.NoAudio))
            {
                strBuilder.Append(@"");
            }
            */

            strBuilder.AppendFormat(@" -oac {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.AudioFormat), AudioFormat)).ToLower());

            //video
            strBuilder.AppendFormat(@" -ovc {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.VideoFormat), VideoFormat)).ToLower());

            //subtitles
            /* DISABLED UNTIL SUBPICTURE STREAM CLASSES ARE FINALIZED
            if (subStream != null)
                strBuilder.AppendFormat(@" -font C:\windows\fonts\arial.ttf -slang {0}", subStream.SubtitleChannel);
            */

            //chapters
            // strBuilder.AppendFormat(@" -chapter {0}-{1}", fromChapter, toChapter);

            // output format
            // always set the output format to mpeg for extenders
            switch (inputType)
            {
                case MEncoder.InputType.Drive:
                    strBuilder.Append(@" -of mpeg -mpegopts format=dvd:tsaf -vf harddup");
                    break;
                case MEncoder.InputType.File:
                    strBuilder.Append(@" -lavcopts vcodec=msmpeg4v2:vpass=1");
                    break;
                default:
                    Utilities.DebugLine("I have no idea format output format to use");
                    throw new Exception("Unable to determine what output format to use");
            }

            // set quiet mode on
            strBuilder.Append(@" -really-quiet");

            //output
            string outputExtension = (inputType == MEncoder.InputType.Drive) ? @"mpg" : @"wmv";
            strBuilder.AppendFormat(@" -o ""{0}.{1}""", Path.Combine(FileSystemWalker.PublicRootDirectory, OutputFile), outputExtension);

            string completedArguments = strBuilder.ToString();
            Utilities.DebugLine("[MEncoderCommandBuilder] Arguments: {0}", completedArguments);
            return completedArguments;
        }
    }
}
