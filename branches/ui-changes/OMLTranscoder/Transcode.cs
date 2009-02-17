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

        // MEncoder Logging
        private MEncoderLogging _mencoderloglevel;
        private static TextWriter _mencoderlogstream;
        private string _mencoderlogname; 
        
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
            
            _mencoderlogname = source.GetTranscodingFileName() + ".log";
            _mencoderloglevel = MEncoderLogging.InfoOnly;
        }

        ~Transcode()
        {
            if (_stream != null)
                _stream.Close();

            try
            {
                // It appears the file is allready closed by the time the destuctor is closed
                if (_mencoderlogstream != null)
                {
                    _mencoderlogstream.Flush();
                    _mencoderlogstream.Close();
                }
            }
            catch
            {
            }

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
            Process.StartInfo.Arguments = cmdBuilder.GetArguments(_mencoderloglevel);
            Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.StartInfo.ErrorDialog = false;

            if (_mencoderloglevel != MEncoderLogging.None)
            {
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.RedirectStandardError = true;
                Process.StartInfo.RedirectStandardOutput = true;
                Process.ErrorDataReceived += new DataReceivedEventHandler(this.MenCoderLogErr);
                Process.OutputDataReceived += new DataReceivedEventHandler(this.MenCoderLogOut);
            }

            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(this.HandleTranscodeExited);
            Process.Start();

            if (_mencoderloglevel != MEncoderLogging.None)
            {
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();
            }

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

            if (_mencoderlogstream != null)
                _mencoderlogstream.Close();
            _mencoderlogstream = null;

            Utilities.DebugLine("[Transcode] Transcoding Job Exited, Exit Code {0}", Process.ExitCode);

            if (Exited != null)
                Exited(sender, e);
        }

        void MenCoderLogOut(object sender, DataReceivedEventArgs e)
        {
            WriteMenCoderLogLine("[Mencoder] Output : " + e.Data);
        }

        void MenCoderLogErr(object sender, DataReceivedEventArgs e)
        {
            WriteMenCoderLogLine("[Mencoder] Error : " + e.Data);
        }

        void WriteMenCoderLogLine(string msg)
        {
            if (_mencoderlogstream == null)
            {
                try
                {
                    string file = Path.Combine(OMLEngine.FileSystemWalker.LogDirectory, _mencoderlogname);
                    if (Directory.Exists(OMLEngine.FileSystemWalker.LogDirectory) == false)
                        Directory.CreateDirectory(OMLEngine.FileSystemWalker.LogDirectory);

                    _mencoderlogstream = new StreamWriter(file, false); // FileStream(file, FileMode.Create);
                }
                catch
                {
                }
            }
            try
            {
                string prefix = string.Format("{0} [{1}#{2}], ", DateTime.Now, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
                _mencoderlogstream.WriteLine(prefix + msg);
                _mencoderlogstream.Flush();
            }
            catch
            {
            }
        }
    }
}
