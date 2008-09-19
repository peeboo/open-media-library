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
        MediaSource _source;

        public MEncoderCommandBuilder(MediaSource ms, string path)
        {
            _source = ms;
            this.AudioFormat = MEncoder.AudioFormat.LAVC;
            this.VideoFormat = MEncoder.VideoFormat.LAVC;
            this.OutputPath = path;

            if (File.Exists(ms.MediaPath))
            {
                inputType = MEncoder.InputType.File;
                inputFile = new FileInfo(_source.MediaPath);
                OutputFile = Path.Combine(OutputPath, Path.GetFileName(_source.MediaPath));
            }
            else if (Directory.Exists(_source.MediaPath))
            {
                inputType = MEncoder.InputType.Drive;
                inputDrive = new DriveInfo(_source.MediaPath.Substring(0, 1));
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

        public string OutputPath { get; set; }
        public string OutputFile { get; set; }
        public MEncoder.AudioFormat AudioFormat { get; set; }
        public MEncoder.VideoFormat VideoFormat { get; set; }
        public SubtitleStream SubtitleStream { get { return _source.Subtitle; } }
        public AudioStream AudioStream { get { return _source.AudioStream; } }

        private DriveInfo inputDrive;
        private FileInfo inputFile;
        private MEncoder.InputType inputType;

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
                    if (_source.Title != null)
                        strBuilder.Append(_source.Title.Value.ToString());
                    strBuilder.AppendFormat(@" -dvd-device ""{0}""", inputDrive.RootDirectory);
                    break;
                default:
                    Utilities.DebugLine("No idea what format the file is");
                    throw new Exception("Can't determine input format (file or drive)");
            }

            if (_source.StartChapter != null)
            {
                if (_source.EndChapter != null)
                    strBuilder.AppendFormat(" -chapter {0}-{1}", _source.StartChapter, _source.EndChapter);
                else
                    strBuilder.AppendFormat(" -chapter {0}", _source.StartChapter);
            }

            //audio format
            if (AudioFormat == MEncoder.AudioFormat.NoAudio)
                strBuilder.Append(@" -nosound");
            else
            {
                if (_source.AudioStream != null && string.IsNullOrEmpty(_source.AudioStream.LanguageShortName) == false)
                    strBuilder.AppendFormat(@" -alang {0}", _source.AudioStream.LanguageShortName);
            }

            strBuilder.AppendFormat(@" -oac {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.AudioFormat), AudioFormat)).ToLower());

            //video
            strBuilder.AppendFormat(@" -ovc {0}",
                                    ((string)Enum.GetName(typeof(MEncoder.VideoFormat), VideoFormat)).ToLower());

            //subtitles
            if (_source.Subtitle != null && string.IsNullOrEmpty(_source.Subtitle.LanguageShortName) == false)
                strBuilder.AppendFormat(@" -font C:\windows\fonts\arial.ttf -slang {0}", _source.Subtitle.LanguageShortName);

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
            strBuilder.AppendFormat(@" -o ""{0}.{1}""", Path.Combine(OutputPath, OutputFile), outputExtension);

            string completedArguments = strBuilder.ToString();
            Utilities.DebugLine("[MEncoderCommandBuilder] Arguments: {0}", completedArguments);
            return completedArguments;
        }
    }
}
