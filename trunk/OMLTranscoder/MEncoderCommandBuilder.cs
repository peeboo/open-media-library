/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using OMLEngine;

namespace OMLTranscoder
{
    public enum MEncoderLogging
    {
        None,
        InfoOnly,
        All
    }
    
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

        public string GetArguments(MEncoderLogging MEncoderLogLevel)
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
                OMLGetDVDInfo.AudioEncoding audioFormat = OMLGetDVDInfo.AudioEncoding.AC3;
                if (_source.DVDTitle.AudioTracks.Count > 0)
                    audioFormat = _source.DVDTitle.AudioTracks[0].Format;
                if (_source.AudioStream != null)
                    audioFormat = _source.AudioStream.Format;
                strBuilder.AppendFormat(@" -oac {0} -lavcopts acodec=mp2", audioFormat != OMLGetDVDInfo.AudioEncoding.DTS ? "copy" : "lavc");

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
                // We are not playing a dvd, we are playing a movie file (avi etc)
                strBuilder.AppendFormat(@"""{0}""", _source.MediaPath);

                // audio format
                // TODO Mikem2te - lavc gives better sound compatabiliy at the expense of performance. Ideally
                // Detect audio stream and decide if it needs recoding.
                //strBuilder.Append(@" -oac copy");
                strBuilder.Append(@" -oac lavc");

                
                // Try to find the technical details of the movie file.
                // Assume one feature and one video stream for now
                DIFeature df = null;
                DIVideoStream dvs= null;
                try
                {
                    df = _source.Disk.DiskFeatures[0];
                    dvs = df.VideoStreams[0];

                    Utilities.DebugLine(string.Format("[Transcode] Movie details. Resolution {0}x{1} @ {2} fps.", dvs.Resolution.Width, dvs.Resolution.Height, dvs.FrameRate));
                    
                    // Add the frame rate
                    if (dvs.FrameRateString != "")
                    {
                        strBuilder.Append(@" -ofps " + dvs.FrameRateString);
                    }

                    // Video scaling
                    string AspectRatio = dvs.AspectRatio;
                    strBuilder.AppendFormat(BuildVF_String(dvs.Resolution.Width, dvs.Resolution.Height, ref AspectRatio));
                    
                    //video codec
                    strBuilder.AppendFormat(@" -ovc lavc");

                    // Build the lavcopts
                    strBuilder.Append(@" -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0");
                    //strBuilder.Append(@" -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_minrate=4900:vrc_maxrate=4900:vbitrate=4900:keyint=15:vstrict=0");
                
                    if (dvs.AspectRatio != "")
                    {
                        strBuilder.AppendFormat(@":aspect=" + AspectRatio);
                    }

                    strBuilder.AppendFormat(" -mpegopts format=mpeg"); //:muxrate=9800");
                    if (dvs.AspectRatio != "")
                    {
                        strBuilder.AppendFormat(@":vaspect=" + AspectRatio);
                    }
                }
                catch
                {
                    //video format
                    strBuilder.AppendFormat(@" -ovc lavc");

                    // Build the lavcopts
                    strBuilder.Append(@" -lavcopts vcodec=mpeg2video:vrc_buf_size=1835:vrc_maxrate=9800:vbitrate=4900:keyint=15:vstrict=0");
                }
            }

            // these are the same for dvds and non-dvds
            strBuilder.Append(@" -of mpeg");
            
            switch (MEncoderLogLevel)
            {
                case MEncoderLogging.All:
                    break;
                case MEncoderLogging.InfoOnly:
                    strBuilder.Append(@" -quiet");
                    break;
                case MEncoderLogging.None:
                    strBuilder.Append(@" -really-quiet");
                    break;
            }

            //output
            strBuilder.AppendFormat(@" -o ""{0}""", outputFile);

            string completedArguments = strBuilder.ToString();
            Utilities.DebugLine("[MEncoderCommandBuilder] Arguments: {0}", completedArguments);
            return completedArguments;
        }

        static int Floor16(double i)
        {
            return (int)i / 16 * 16;
        }
        static int Round16(double i)
        {
            return (int)(Math.Round(i / 16.0) * 16.0);
        }

        static string BuildVF_String(int Width, int Height, ref string AspectRatio)
        {
            // Absolute maximum resolution for the transcoded file
            int maxwidth = 720;
            int maxheight = 576;

            List<string> param = new List<string>();

            double CalculatedAspectRatio = (double)Width / (double)Height;

            // Look for troublesome files for the xbox. Xbox can have 
            // trouble with wide aspect ratio or large resolution
            if ((CalculatedAspectRatio > 1.8) || (Width > maxwidth) || (Height > maxheight))
            {
                int cswidth;
                int csheight;

                if ((Width > maxwidth) || (Height > maxheight))
                {
                    // Movie is to big, scale it
                    // Find scale factor
                    double sfwidth = (double)maxwidth / (double)Width;
                    double sfheight = (double)maxheight / (double)Height;
                    double scalefactor = Math.Min(sfwidth, sfheight);

                    // Find the resolution to scale to. These are rounded down to the nearest 16 pixel
                    // boundary to be kind to the encoding and the extender. May cause slight AR inacuracy
                    cswidth = Floor16((double)Width * scalefactor);
                    csheight = Floor16((double)Height * scalefactor);

                    //Console.WriteLine("Calculated scale " + cropwidth.ToString() + ", " + cropheight.ToString());
                    if ((cswidth != Width) || (csheight != Height))
                    {
                        param.Add(string.Format(@"scale={0}:{1}", cswidth, csheight));
                        Utilities.DebugLine(string.Format("[Transcode] Scaling Movie to {0}x{1}.", cswidth, csheight));
                    }
                }
                else
                {
                    // File is within size limits but is too wide an aspect ratio.
                    // Crop movie on a 16 pixel boundary
                    cswidth = Floor16(Width);
                    csheight = Floor16(Height);
                    //Console.WriteLine("Calculated crop " + cropwidth.ToString() + ", " + cropheight.ToString());
                    if ((cswidth != Width) || (csheight != Height))
                    {
                        param.Add(string.Format(@"crop={0}:{1}:0:0", cswidth, csheight));
                        Utilities.DebugLine(string.Format("[Transcode] Cropping Movie to {0}x{1}.", cswidth, csheight));
                    }
                }

                if (CalculatedAspectRatio > 1.7)
                {
                    // Aspect ratio is too wide
                    double newaspectratio = 16.0 / 9.0;
                    AspectRatio = "16/9";

                    // Calculate new output size (add borders)
                    int expandwidth = cswidth;
                    int expandheight = Round16((int)((double)cswidth / newaspectratio));

                    // Calculate the vertical offset on 16 pixel boundary
                    int voffset = Round16((expandheight - csheight) / 2);
                    //Console.WriteLine("Calculated output size " + expandwidth.ToString() + ", " + expandheight.ToString());

                    if (expandheight > csheight)
                    {
                        param.Add(string.Format(@"expand={0}:{1}:0:{2}", expandwidth, (csheight - expandheight), voffset));
                        Utilities.DebugLine(string.Format("[Transcode] Expanding Movie to {0}x{1}.", expandwidth, expandheight));
                    }
                }
            }

            if (param.Count > 0)
            {
                return @" -vf " + string.Join(",", param.ToArray());
            }
            else
            {
                return "";
            }
        }

    }
}
