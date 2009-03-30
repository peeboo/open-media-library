using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace OMLEngine.FileSystem
{
    public class MountingTool
    {

        public enum Tool
        {
            None,
            DaemonTools,
            VirtualCloneDrive
        };

        private Tool _Tool;
        private string _Path;
        private string _Drive;
        private int _DriveNo;
        private string _MountedIsoFile = string.Empty;

        // this can't be a static constructor since these settings can change during the session
        public MountingTool()
        {
            Utilities.DebugLine("[MountingTool] MountingTool()");

            _Tool = (Tool)Enum.Parse(typeof(Tool), Settings.OMLSettings.MountingToolSelection);
            _Path = Settings.OMLSettings.MountingToolPath;
            _Drive = Settings.OMLSettings.VirtualDiscDrive;
            _DriveNo = Settings.OMLSettings.VirtualDiscDriveNumber;
            Utilities.DebugLine("[MountingTool] All MountingTool params loaded");
        }

        public bool Enabled
        {
            get { return (_Path.Length > 0) ? true : false; }
        }

        public bool Mount(string IsoFile, out string VirtualDrive)
        {
            Utilities.DebugLine("[MountingTool] Mount called for file {0}", IsoFile);
            VirtualDrive = string.Empty;
            if (IsoFile == null) return false;
            if (IsoFile == string.Empty) return false;
            if (!Enabled) return false;
            if (!System.IO.File.Exists(_Path)) return false;

            UnMount();

            string strParams;
            switch (_Tool)
            {
                case Tool.VirtualCloneDrive :
                    strParams = string.Format("/l={0} \"{1}\"", _DriveNo, IsoFile);
                    break;

                default :
                    strParams = string.Format("-mount {0}, \"{1}\"", _DriveNo, IsoFile);
                    break;

            }
            Process p = StartProcess(_Path, strParams, true, true);

            if (!p.WaitForExit(15000))
            {
                Utilities.DebugLine("[MountingTool] Waited for 15000 milliseconds and the mounter didn't exit");
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
            Utilities.DebugLine("[MountingTool] UnMount called");
            if (!Enabled) return;
            if (!System.IO.File.Exists(_Path)) return;

            string strParams;
            switch (_Tool)
            {
                case Tool.VirtualCloneDrive :
                    strParams = "/u";
                    break;

                default :
                    strParams = string.Format("-unmount {0}", _DriveNo);
                    break;
            }
            Process p = StartProcess(_Path, strParams, true, true);
            int timeout = 0;
            while (!p.HasExited && (timeout < 10000))
            {
                Utilities.DebugLine("[MountingTool] Unmount waiting for 100 milliseconds");
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
            Utilities.DebugLine("[MountingTool] IsMounted called for file {0}", IsoFile);
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
            Utilities.DebugLine("[MountingTool] StartProcess beginning with {0}, {1}, {2}",
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
                Utilities.DebugLine("[MountingTool] " + ErrorString);
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


        // Functions for finding the mounting tool
        private const string DefaultDaemonToolsPath = @"Program Files\DAEMON Tools Lite\daemon.exe";
        private const string DefaultVirtualCloneDrivePath = @"Program Files\Elaborate Bytes\VirtualCloneDrive\VCDMount.exe";        

        public string ScanForMountTool(Tool _tool, string _driveToScan)
        {
            string driveLetterToScan = _driveToScan != "" ? _driveToScan : "C";

            DriveInfo dInfo = new DriveInfo(driveLetterToScan);

            string startPath = null;

            switch (_tool)
            {
                case MountingTool.Tool.None:
                    // don't do anything if there's no mounting tool selected
                    return "";

                case MountingTool.Tool.DaemonTools:
                    startPath = DefaultDaemonToolsPath;
                    break;

                case MountingTool.Tool.VirtualCloneDrive:
                    startPath = DefaultVirtualCloneDrivePath;
                    break;

                default:
                    return "";
            }

            if (File.Exists(Path.Combine(dInfo.RootDirectory.FullName, startPath)))
            {
                return Path.Combine(dInfo.RootDirectory.FullName, startPath);
            }
            else
            {
                // let's scan all the folders for it
                string exePath = ScanAllFoldersForExecutable(dInfo.RootDirectory.FullName, Path.GetFileName(startPath));

                if (exePath.Length > 0)
                {
                    Utilities.DebugLine("[Settings] Found Image Mounter: {0}", exePath);
                    return exePath;
                }
                else
                {
                    return "";
                }
            }
            return "";
        }

        private static string ScanAllFoldersForExecutable(string dir, string executable)
        {
            if (!Directory.Exists(dir))
                return string.Empty;

            string tmtPath = string.Empty;
            DirectoryInfo dInfo;
            FileSystemInfo[] items = new FileSystemInfo[0]; // this just needs to be init'd
            try
            {
                dInfo = new DirectoryInfo(dir);
                items = dInfo.GetFileSystemInfos();
            }
            catch (Exception e)
            {
                Utilities.DebugLine("Caught exception trying to scan {0}: {1}", dir, e.Message);
            }

            foreach (FileSystemInfo item in items)
            {
                if (item is DirectoryInfo)
                {
                    DirectoryInfo dirInfo = item as DirectoryInfo;
                    Utilities.DebugLine("[Settings] Scanning folder [{0}] for TMT", dirInfo.FullName);
                    tmtPath = ScanAllFoldersForExecutable(dirInfo.FullName, executable);
                    if (!string.IsNullOrEmpty(tmtPath))
                        return tmtPath;
                }

                if (item is FileInfo)
                {
                    FileInfo fInfo = item as FileInfo;
                    if (fInfo.Name.Equals(executable, StringComparison.OrdinalIgnoreCase))
                        return fInfo.FullName;
                }
            }
            return string.Empty;
        }
    }
}
