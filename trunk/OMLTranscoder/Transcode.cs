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
        public Process Process { get; private set; }
        public MediaSource Source { get; private set; }
        public event EventHandler Exited;

        public Transcode(MediaSource source)
        {
            Source = source;
        }

        ~Transcode()
        {
            if (Process != null && !Process.HasExited)
            {
                OMLEngine.Utilities.DebugLine("Transcode process not finished: kill {0}", Process.Id);
                Process.Kill();
            }
        }

        public int BeginTranscodeJob()
        {
            MEncoderCommandBuilder cmdBuilder = new MEncoderCommandBuilder(Source);

            Utilities.DebugLine("[Transcode] Starting transcode job");
            Process = new Process();
            Process.StartInfo.FileName = cmdBuilder.GetCommand();
            Process.StartInfo.Arguments = cmdBuilder.GetArguments(FileSystemWalker.TranscodeBufferDirectory);
            Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.StartInfo.ErrorDialog = false;
            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(this.HandleTranscodeExited);
            Process.Start();
            Utilities.DebugLine("[Transcode] Transcode job started, returning with buffer location");
            if (Process.HasExited)
                return Process.ExitCode;

            return 0;
        }

        void HandleTranscodeExited(object sender, EventArgs e)
        {
            Utilities.DebugLine("[Transcode] Transcode Job Exited, Exit Code {0}", Process.ExitCode);

            if (Exited != null)
                Exited(sender, e);

            if (Process.ExitCode != 0)
                Utilities.DebugLine("[Transcode] An error occured");
        }
    }
}
