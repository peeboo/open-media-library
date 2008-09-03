/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OMLEngine;

namespace OMLTranscoder
{
    public class Transcode
    {
        Process _process;
        MediaSource mediaSource;
        
        static string MENCODER_PATH = Path.Combine(FileSystemWalker.RootDirectory, @"mencoder.exe");
        static string BUFFER_DIRECTORY = FileSystemWalker.TranscodeBufferDirectory;

        public Process CurrentTranscodeProcess
        {
            get { return _process; }
            set { _process = value; }
        }

        public Transcode()
        {
        }

        ~Transcode()
        {
        }

        public string BeginTranscodeJob(MediaSource ms)
        {
            mediaSource = ms;
            string outputFilename = string.Empty;

            MEncoderCommandBuilder cmdBuilder = new MEncoderCommandBuilder();
            cmdBuilder.SetAudioOutputFormat(MEncoder.AudioFormat.COPY);
            IList<AudioStream> aStreams = mediaSource.AvailableAudioStreams();
            cmdBuilder.SetAudioStream(aStreams[0]);

            IList<SubtitleStream> sStreams = mediaSource.AvailableSubtitleStreams();
            cmdBuilder.SetSubtitleStream(sStreams[0]);

            _process = new Process();
            _process.StartInfo.FileName = @"";
            _process.StartInfo.Arguments = @"";
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.StartInfo.ErrorDialog = false;
            _process.EnableRaisingEvents = true;
            _process.Exited += new EventHandler(this.HandleTranscodeExited);
            _process.Start();
            _process.WaitForExit(1);

            return outputFilename;
        }

        private void HandleTranscodeExited(object sender, EventArgs e)
        {
            if (_process.ExitCode != 0)
            {
                Utilities.DebugLine("[Transcode] An error occured");
            }
        }

        public MediaSource MediaSourceFromTitle(Title title)
        {
            MediaSource source = new MediaSource();

            return source;
        }
    }
}
