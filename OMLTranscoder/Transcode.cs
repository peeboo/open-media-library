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
        public Process CurrentTranscodeProcess { get; private set; }
        MediaSource mediaSource;

        ~Transcode()
        {
            if (CurrentTranscodeProcess != null && !CurrentTranscodeProcess.HasExited)
            {
                OMLEngine.Utilities.DebugLine("Transcode process not finished: kill {0}", CurrentTranscodeProcess.Id);
                CurrentTranscodeProcess.Kill();
            }
        }

        public int BeginTranscodeJob(MediaSource ms, out string outputFilename)
        {
            mediaSource = ms;
            outputFilename = string.Empty;

            MEncoderCommandBuilder cmdBuilder = new MEncoderCommandBuilder()
            {
                AudioFormat = MEncoder.AudioFormat.LAVC,
                VideoFormat = MEncoder.VideoFormat.LAVC,
                OutputPath = FileSystemWalker.TranscodeBufferDirectory,
                InputLocation = ms.MediaPath,
            };

            Utilities.DebugLine("[Transcode] Output file will be: {0}", outputFilename);
            Utilities.DebugLine("[Transcode] Starting transcode job");
            CurrentTranscodeProcess = new Process();
            CurrentTranscodeProcess.StartInfo.FileName = cmdBuilder.GetCommand();
            CurrentTranscodeProcess.StartInfo.Arguments = cmdBuilder.GetArguments();
            CurrentTranscodeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            CurrentTranscodeProcess.StartInfo.ErrorDialog = false;
            CurrentTranscodeProcess.EnableRaisingEvents = true;
            CurrentTranscodeProcess.Exited += new EventHandler(this.HandleTranscodeExited);
            CurrentTranscodeProcess.Start();
            Utilities.DebugLine("[Transcode] Transcode job started, returning with buffer location");
            if (CurrentTranscodeProcess.HasExited)
                return CurrentTranscodeProcess.ExitCode;

            return 0;
        }

        private void HandleTranscodeExited(object sender, EventArgs e)
        {
            Utilities.DebugLine("[Transcode] Transcode Job Exited, Exit Code {0}", CurrentTranscodeProcess.ExitCode);
            if (CurrentTranscodeProcess.ExitCode != 0)
            {
                Utilities.DebugLine("[Transcode] An error occured");
            }
        }
    }
}
