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
            this.AudioEncoderFormat = MEncoder.AudioFormat.LAVC;
            this.VideoEncoderFormat = MEncoder.VideoFormat.LAVC;

            if (_source.Format != VideoFormat.DVD)
            {
                OutputFile = Path.Combine(path, Path.GetFileName(_source.MediaPath));
            }
            else
            {
                string root = _source.VIDEO_TS_Parent;
                if (root == Path.GetPathRoot(root))
                {
                    DriveInfo inputDrive = new DriveInfo(root.Substring(0, 1));
                    try
                    {
                        if (inputDrive.IsReady == false)
                            inputDrive = null;
                        else
                            OutputFile = Path.Combine(path, inputDrive.VolumeLabel);
                    }
                    catch { }
                }
                else
                    OutputFile = Path.Combine(path, new DirectoryInfo(root).Name);
            }
        }

        public string OutputFile { get; set; }
        public bool IsDVD { get { return _source.Format == VideoFormat.DVD; } }
        public MEncoder.AudioFormat AudioEncoderFormat { get; set; }
        public MEncoder.VideoFormat VideoEncoderFormat { get; set; }
        public SubtitleStream SubtitleStream { get { return _source.Subtitle; } set { _source.Subtitle = value; } }
        public AudioStream AudioStream { get { return _source.AudioStream; } set { _source.AudioStream = value; } }

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
            if (OutputFile == null)
                throw new Exception(string.Format("OutputFile not set for {0}", _source));

            StringBuilder strBuilder = new StringBuilder();

            // input location
            if (IsDVD)
            {
                strBuilder.Append(@" dvd://");
                if (_source.Title != null)
                    strBuilder.Append(_source.Title.Value.ToString());
                else
                    strBuilder.Append(_source.DVDDiskInfo.GetMainTitle().TitleNumber);
                strBuilder.AppendFormat(@" -dvd-device ""{0}""", _source.VIDEO_TS_Parent);
            }
            else
                strBuilder.AppendFormat(@"""{0}""", _source.MediaPath);

            if (_source.StartChapter != null)
            {
                if (_source.EndChapter != null)
                    strBuilder.AppendFormat(" -chapter {0}-{1}", _source.StartChapter, _source.EndChapter);
                else
                    strBuilder.AppendFormat(" -chapter {0}", _source.StartChapter);
            }

            //audio format
            if (AudioEncoderFormat == MEncoder.AudioFormat.NoAudio)
                strBuilder.Append(@" -nosound");
            else
            {
                if (_source.AudioStream != null && _source.AudioStream.AudioID != null)
                    strBuilder.AppendFormat(@" -aid {0}", _source.AudioStream.AudioID);
            }

            strBuilder.AppendFormat(@" -oac {0}", AudioEncoderFormat.ToString().ToLower());

            //video
            strBuilder.AppendFormat(@" -ovc {0}", VideoEncoderFormat.ToString().ToLower());

            //subtitles
            if (_source.Subtitle != null && _source.Subtitle.SubtitleID != null)
                strBuilder.AppendFormat(@" -font c:\windows\fonts\arial.ttf -sid {0}", _source.Subtitle.SubtitleID.Value - 1);

            // output format
            // always set the output format to mpeg for extenders
            if (IsDVD)
                strBuilder.Append(@" -of mpeg -mpegopts format=dvd:tsaf -vf harddup");
            else
                strBuilder.Append(@" -lavcopts vcodec=msmpeg4v2:vpass=1");

            // set quiet mode on
            strBuilder.Append(@" -really-quiet");

            //output
            strBuilder.AppendFormat(@" -o ""{0}""", 
                Path.ChangeExtension(OutputFile, IsDVD ? @".mpg" : @".wmv"));

            string completedArguments = strBuilder.ToString();
            Utilities.DebugLine("[MEncoderCommandBuilder] Arguments: {0}", completedArguments);
            return completedArguments;
        }
    }
}
