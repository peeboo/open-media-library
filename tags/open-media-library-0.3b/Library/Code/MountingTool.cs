using System;
using System.Diagnostics;

/* This entire library is either completely taken
 * from or heavily influenced by code from the MediaPortal
 * project as found via Krugle */

namespace Library
{
    public class MountingTool
    {
        public enum Tool
        {
            None,
            DaemonTools,
            VirtualCloneDrive
        };

        private string _Path;
        private string _Drive;
        private int _DriveNo;
        private string _MountedIsoFile = string.Empty;

        // this can't be a static constructor since these settings can change during the session
        public MountingTool()
        {
            OMLApplication.DebugLine("[MountingTool] MountingTool()");
            _Path = OMLEngine.Properties.Settings.Default.MountingToolPath;
            _Drive = OMLEngine.Properties.Settings.Default.VirtualDiscDrive;
            _DriveNo = OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber;
            OMLApplication.DebugLine("[MountingTool] All MountingTool params loaded");
        }

        public bool Enabled
        {
            get { return (_Path.Length > 0) ? true : false; }
        }

        public bool Mount(string IsoFile, out string VirtualDrive)
        {
            OMLApplication.DebugLine("[MountingTool] Mount called for file {0}", IsoFile);
            VirtualDrive = string.Empty;
            if (IsoFile == null) return false;
            if (IsoFile == string.Empty) return false;
            if (!Enabled) return false;
            if (!System.IO.File.Exists(_Path)) return false;

            UnMount();

            string strParams;
            if (OMLEngine.Properties.Settings.Default.MountingToolPath.ToLower().Contains("virtual"))
            {
                strParams = string.Format("/l={0} \"{1}\"", _DriveNo, IsoFile);
            }
            else
            {
                strParams = string.Format("-mount {0}, \"{1}\"", _DriveNo, IsoFile);
            }
            Process p = StartProcess(_Path, strParams, true, true);

            if (!p.WaitForExit(15000))
            {
                OMLApplication.DebugLine("[MountingTool] Waited for 15000 milliseconds and the mounter didn't exit");
            }                        

            string path = _Drive + ":\\";
            int timeout = 0;
            while (timeout < 15000)
            {
                if (System.IO.Directory.Exists(path) &&
                    System.IO.Directory.GetDirectories(path).Length != 0)
                    break;

                System.Threading.Thread.Sleep(250);
                timeout += 250;
            }

            // let's wait one extra second
            System.Threading.Thread.Sleep(1000);

            VirtualDrive = _Drive;
            _MountedIsoFile = IsoFile;
            return true;
        }

        public void UnMount()
        {
            OMLApplication.DebugLine("[MountingTool] UnMount called");
            if (!Enabled) return;
            if (!System.IO.File.Exists(_Path)) return;

            string strParams;
            if (OMLEngine.Properties.Settings.Default.MountingToolPath.ToLower().Contains("virtual"))
            {
                // its virtual clonedrive
                strParams = "/u";
            }
            else
            {
                // its daemon tools
                strParams = string.Format("-unmount {0}", _DriveNo);
            }
            Process p = StartProcess(_Path, strParams, true, true);
            int timeout = 0;
            while (!p.HasExited && (timeout < 10000))
            {
                OMLApplication.DebugLine("[MountingTool] Unmount waiting for 100 milliseconds");
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
            OMLApplication.DebugLine("[MountingTool] IsMounted called for file {0}", IsoFile);
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
            OMLApplication.DebugLine("[MountingTool] StartProcess beginning with {0}, {1}, {2}",
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
                string ErrorString = String.Format("[MountingTool] Error starting process!\n  filename: {0}  arguments: {1}\n  WorkingDirectory: {2}\n  stack: {3} {4} {5}",
                    proc.StartInfo.FileName,
                    proc.StartInfo.Arguments,
                    proc.StartInfo.WorkingDirectory,
                    e.Message,
                    e.Source,
                    e.StackTrace);
                OMLApplication.DebugLine("[MountingTool] " + ErrorString);
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
