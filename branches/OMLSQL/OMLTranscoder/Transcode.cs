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
        FileStream _stream;

        public Transcode(MediaSource source)
        {
            Source = source;
            FileInfo file = new FileInfo(source.GetTranscodingFileName());
            if (file.Exists)
            {
                file.Delete();
                _stream = File.Open(file.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
        }

        ~Transcode()
        {
            if (_stream != null)
                _stream.Close();
            if (Process != null && !Process.HasExited)
            {
                OMLEngine.Utilities.DebugLine("[Transcode] Transcoding process not finished: kill {0}", Process.Id);
                Process.Kill();
            }
        }

        public int BeginTranscodeJob()
        {
            MEncoderCommandBuilder cmdBuilder = new MEncoderCommandBuilder(Source);

            Process = new Process();
            Process.StartInfo.FileName = cmdBuilder.GetCommand();
            Process.StartInfo.Arguments = cmdBuilder.GetArguments();
            Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.StartInfo.ErrorDialog = false;
            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(this.HandleTranscodeExited);
            Process.Start();

            Utilities.DebugLine("[Transcode] Transcoding job started" + (Process.HasExited ? ", exit-code=" + Process.ExitCode : ""));
            
            if (Process.HasExited)
                return Process.ExitCode;

            return 0;
        }

        void HandleTranscodeExited(object sender, EventArgs e)
        {
            if (_stream != null)
                _stream.Close();
            _stream = null;
            Utilities.DebugLine("[Transcode] Transcoding Job Exited, Exit Code {0}", Process.ExitCode);

            if (Exited != null)
                Exited(sender, e);
        }
    }
}
