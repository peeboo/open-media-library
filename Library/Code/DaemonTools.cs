using System;
using System.Diagnostics;

/* This entire library is either completely taken
 * from or heavily influenced by code from the MediaPortal
 * project as found via Krugle */

namespace Library
{
    public class DaemonTools
    {
        static string _Path;
        static string _Drive;
        static int _DriveNo;
        static string _MountedIsoFile = string.Empty;

        static DaemonTools()
        {
            OMLApplication.DebugLine("[DaemonTools] DaemonTools()");
            _Path = OMLEngine.Properties.Settings.Default.DaemonTools;
            _Drive = OMLEngine.Properties.Settings.Default.VirtualDiscDrive;
            _DriveNo = OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber;
            OMLApplication.DebugLine("[DaemonTools] All Daemontools params loaded");
        }

        public bool Enabled
        {
            get { return (_Path.Length > 0) ? true : false; }
        }

        public bool Mount(string IsoFile, out string VirtualDrive)
        {
            OMLApplication.DebugLine("[DaemonTools] Mount called for file {0}", IsoFile);
            VirtualDrive = string.Empty;
            if (IsoFile == null) return false;
            if (IsoFile == string.Empty) return false;
            if (!Enabled) return false;
            if (!System.IO.File.Exists(_Path)) return false;

            UnMount();

            string strParams = String.Format("-mount {0}, \"{1}\"", _DriveNo, IsoFile);
            Process p = StartProcess(_Path, strParams, true, true);
            int timeout = 0;
            while (!p.HasExited && (timeout < 10000))
            {
                OMLApplication.DebugLine("[DaemonTools] Mount waiting for 100 milliseconds");
                System.Threading.Thread.Sleep(100);
                timeout += 100;
            }
            VirtualDrive = _Drive;
            _MountedIsoFile = IsoFile;
            return true;
        }

        public void UnMount()
        {
            OMLApplication.DebugLine("[DaemonTools] UnMount called");
            if (!Enabled) return;
            if (!System.IO.File.Exists(_Path)) return;

            string strParams = string.Format("-unmount {0}", _DriveNo);
            Process p = StartProcess(_Path, strParams, true, true);
            int timeout = 0;
            while (!p.HasExited && (timeout < 10000))
            {
                OMLApplication.DebugLine("[DaemonTools] Unmount waiting for 100 milliseconds");
                System.Threading.Thread.Sleep(100);
                timeout += 100;
            }

            _MountedIsoFile = string.Empty;
        }

        public string GetVirtualDrive()
        {
            if (_MountedIsoFile != string.Empty) return _Drive;
            return string.Empty;
        }

        public bool IsMounted(string IsoFile)
        {
            OMLApplication.DebugLine("[DaemonTools] IsMounted called for file {0}", IsoFile);
            if (IsoFile == null) return false;
            if (IsoFile == string.Empty) return false;
            if (_MountedIsoFile.Equals(IsoFile))
            {
                if (System.IO.Directory.Exists(_Drive + @"\"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        Process StartProcess(ProcessStartInfo procStartInfo, bool bWaitForExit)
        {
            OMLApplication.DebugLine("[DaemonTools] StartProcess beginning with {0}, {1}, {2}",
                                      procStartInfo.FileName,
                                      procStartInfo.Arguments,
                                      procStartInfo.WorkingDirectory);
            Process proc = new Process();
            proc.StartInfo = procStartInfo;

            try
            {
                proc.Start();
                if (bWaitForExit)
                {
                    proc.WaitForExit();
                }
            }
            catch (Exception e)
            {
                string ErrorString = String.Format("Error starting process!\n  filename: {0}  arguments: {1}\n  WorkingDirectory: {2}\n  stack: {3} {4} {5}",
                    proc.StartInfo.FileName,
                    proc.StartInfo.Arguments,
                    proc.StartInfo.WorkingDirectory,
                    e.Message,
                    e.Source,
                    e.StackTrace);
                OMLApplication.DebugLine("[DaemonTools] " + ErrorString);
            }
            return proc;
        }

        Process StartProcess(string strProgram, string strParams, bool bWaitForExit, bool bMinimized)
        {
            if (strProgram == null) return null;
            if (strProgram.Length == 0) return null;

            string strWorkingDir = System.IO.Path.GetFullPath(strProgram);
            string strFileName = System.IO.Path.GetFileName(strProgram);
            strWorkingDir = strWorkingDir.Substring(0, strWorkingDir.Length - (strFileName.Length + 1));

            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = strFileName;
            procInfo.WorkingDirectory = strWorkingDir;
            procInfo.Arguments = strParams;

            if (bMinimized)
            {
                procInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                procInfo.CreateNoWindow = true;
            }
            return StartProcess(procInfo, bWaitForExit);
        }
    }
}
