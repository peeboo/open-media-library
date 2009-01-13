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

        // TODO: detect or make an settings/option to set NTSC or PAL
        static bool IsNTSC { get { return true; } }

        public string GetArguments()
        {
            string outputFile = _source.GetTranscodingFileName();
            if (outputFile == null)
                throw new Exception(string.Format("OutputFile not set for {0}", _source));

            StringBuilder strBuilder = new StringBuilder();

            // from TGB: 
            // -oac copy -ovc lavc -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0:autoaspect=1, harddup -of mpeg -mpegopts format=dvd:tsaf
            // input location

            // from Vlader's page (http://iandixon.co.uk/cs/wikis/mediacenter/dvd-library-on-extenders-using-vader-s-transcoder.aspx):
            // -dvd-device "{0}" dvd://{2} -alang en -slang en -oac copy -ovc lavc 
            // NTSC: -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=5000:keyint=18:vstrict=0:aspect=16/9 
            //       -vf scale=720:480,harddup
            //       -ofps 30000/1001
            // PAL:  -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0:aspect=16/9
            //       -vf scale=720:576,harddup
            //       -ofps 25
            // -of mpeg -mpegopts format=dvd:tsaf -o "{1}" -quiet

            if (IsDVD)
            {
                strBuilder.Append(@" dvd://");
                if (_source.Title != null)
                    strBuilder.Append(_source.Title.Value.ToString());
                else
                    strBuilder.Append(_source.DVDDiskInfo.GetMainTitle().TitleNumber);
                strBuilder.AppendFormat(@" -dvd-device ""{0}""", _source.VIDEO_TS_Parent);

                // chapter start (optional)
                if (_source.StartChapter != null)
                {
                    if (_source.EndChapter != null)
                        strBuilder.AppendFormat(" -chapter {0}-{1}", _source.StartChapter, _source.EndChapter);
                    else
                        strBuilder.AppendFormat(" -chapter {0}", _source.StartChapter);
                }

                // audio format
                strBuilder.Append(@" -oac copy -lavcopts acodec=mp2");

                if (AudioEncoderFormat == MEncoder.AudioFormat.NoAudio)
                    strBuilder.Append(@" -nosound");

                //subtitles
                if (_source.Subtitle != null && _source.Subtitle.SubtitleID != null)
                    strBuilder.AppendFormat(@" -font c:\windows\fonts\arial.ttf -sid {0}", _source.Subtitle.SubtitleID.Value - 1);
                else
                {
                    if (_source.AudioStream != null && _source.AudioStream.AudioID != null)
                        strBuilder.AppendFormat(@" -aid {0}", _source.AudioStream.AudioID);
                }

                strBuilder.Append(" -ovc lavc");
                // old options: -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0:autoaspect=1,harddup
                if (IsNTSC)
                {
                    strBuilder.Append(" -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=5000:keyint=18:vstrict=0:aspect=16/9");
                    strBuilder.Append(" -vf scale=720:480,harddup");
                    strBuilder.Append(" -ofps 30000/1001");
                }
                else {
                    strBuilder.Append(" -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0:aspect=16/9");
                    strBuilder.Append(" -vf scale=720:576,harddup");
                    strBuilder.Append(" -ofps 25");
                }
                strBuilder.Append(" -mpegopts format=dvd:tsaf");
            }
            else
            {
                strBuilder.AppendFormat(@"""{0}""", _source.MediaPath);

                // audio format
                strBuilder.Append(@" -oac copy");

                //video format
                strBuilder.AppendFormat(@" -ovc lavc");
                strBuilder.Append(@" -mpegopts format=mpeg2:tsaf:vbitrate=4900");
            }

            // these are the same for dvds and non-dvds
            strBuilder.Append(@" -of mpeg");

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
