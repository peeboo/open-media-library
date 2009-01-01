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

        public MEncoderCommandBuilder(MediaSource ms)
        {
            _source = ms;
            //this.AudioEncoderFormat = MEncoder.AudioFormat.LAVC;
            this.AudioEncoderFormat = MEncoder.AudioFormat.Copy;
            this.VideoEncoderFormat = ms.Subtitle == null ? MEncoder.VideoFormat.Copy : MEncoder.VideoFormat.LAVC;
        }

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
            string outputFile = _source.GetTranscodingFileName();
            if (outputFile == null)
                throw new Exception(string.Format("OutputFile not set for {0}", _source));

            StringBuilder strBuilder = new StringBuilder();

            // from TGB: -oac copy -ovc lavc -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0:autoaspect=1, harddup -of mpeg -mpegopts format=dvd:tsaf
            // input location
            //if (IsDVD)
            //{
            //    strBuilder.Append(@" dvd://");
            //    if (_source.Title != null)
            //        strBuilder.Append(_source.Title.Value.ToString());
            //    else
            //        strBuilder.Append(_source.DVDDiskInfo.GetMainTitle().TitleNumber);
            //    strBuilder.AppendFormat(@" -dvd-device ""{0}""", _source.VIDEO_TS_Parent);
            //}
            //else
                strBuilder.AppendFormat(@"""{0}""", _source.MediaPath);

            //if (_source.StartChapter != null)
            //{
            //    if (_source.EndChapter != null)
            //        strBuilder.AppendFormat(" -chapter {0}-{1}", _source.StartChapter, _source.EndChapter);
            //    else
            //        strBuilder.AppendFormat(" -chapter {0}", _source.StartChapter);
            //}

                strBuilder.Append(@" -of mpeg");
            //audio format
            strBuilder.Append(@" -oac copy");
            //if (!IsDVD && Properties.Settings.Default.PreserveAudioOnTranscode)
            //    strBuilder.Append(@" -oac copy");
            //else
            //    strBuilder.AppendFormat(@" -lavcopts acodec={0}", "mp2");
//                strBuilder.AppendFormat(@" -oac {0}", AudioEncoderFormat.ToString().ToLower());

            //if (AudioEncoderFormat == MEncoder.AudioFormat.NoAudio)
            //    strBuilder.Append(@" -nosound");
            //else
            //{
            //    if (_source.AudioStream != null && _source.AudioStream.AudioID != null)
            //        strBuilder.AppendFormat(@" -aid {0}", _source.AudioStream.AudioID);
            //}

            //subtitles
            //if (_source.Subtitle != null && _source.Subtitle.SubtitleID != null)
            //    strBuilder.AppendFormat(@" -font c:\windows\fonts\arial.ttf -sid {0}", _source.Subtitle.SubtitleID.Value - 1);

            //video
            strBuilder.AppendFormat(@" -ovc lavc");
            //strBuilder.AppendFormat(@" -ovc {0}", VideoEncoderFormat.ToString().ToLower());

            // output format
            // always set the output format to mpeg for extenders
            strBuilder.Append(@" -mpegopts format=mpeg2:tsaf:vbitrate=8000");

            // set quiet mode on
            strBuilder.Append(@" -really-quiet");

            //output
            strBuilder.AppendFormat(@" -o ""{0}""", outputFile);

            string completedArguments = strBuilder.ToString();
            Utilities.DebugLine("[MEncoderCommandBuilder] Arguments: {0}", completedArguments);
            return completedArguments;
        }
    }
}
