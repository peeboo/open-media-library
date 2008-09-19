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
            if ((_process != null) && (!_process.HasExited))
                _process.Kill();
        }

        public int BeginTranscodeJob(MediaSource ms, out string outputFilename)
        {
            mediaSource = ms;
            outputFilename = string.Empty;
            MEncoderCommandBuilder cmdBuilder = new MEncoderCommandBuilder();
            cmdBuilder.AudioFormat = MEncoder.AudioFormat.LAVC;
            cmdBuilder.VideoFormat = MEncoder.VideoFormat.LAVC;
            if (File.Exists(ms.MediaPath))
            {
                cmdBuilder.SetInputLocation(MEncoder.InputType.File, ms.MediaPath);
                outputFilename = Path.Combine(FileSystemWalker.TranscodeBufferDirectory, Path.GetFileName(ms.MediaPath));
            }
            else if (Directory.Exists(ms.MediaPath))
            {
                cmdBuilder.SetInputLocation(MEncoder.InputType.Drive, ms.MediaPath);
                outputFilename = Path.Combine(FileSystemWalker.TranscodeBufferDirectory, Path.GetDirectoryName(ms.MediaPath));
            }

            Utilities.DebugLine("[Transcode] Output file will be: {0}", outputFilename);

            cmdBuilder.OutputFile = outputFilename;

            Utilities.DebugLine("[Transcode] Starting transcode job");
            _process = new Process();
            _process.StartInfo.FileName = cmdBuilder.GetCommand();
            _process.StartInfo.Arguments = cmdBuilder.GetArguments();
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.StartInfo.ErrorDialog = false;
            _process.EnableRaisingEvents = true;
            _process.Exited += new EventHandler(this.HandleTranscodeExited);
            _process.Start();
            Utilities.DebugLine("[Transcode] Transcode job started, returning with buffer location");
            if (_process.HasExited)
                return _process.ExitCode;

            return 0;
        }

        private void HandleTranscodeExited(object sender, EventArgs e)
        {
            Utilities.DebugLine("[Transcode] Transcode Job Exited, Exit Code {0}", _process.ExitCode);
            if (_process.ExitCode != 0)
            {
                Utilities.DebugLine("[Transcode] An error occured");
            }
        }

        public MediaSource MediaSourceFromTitle(Title title)
        {
            MediaSource source = new MediaSource();
            source.MediaPath = title.SelectedDisk.Path;
            return source;
        }
    }
}
