using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace OMLEngine
{
    public class DVDDecrypterController
    {
        IList<DriveInfo> availableDrives;
        DriveInfo sourceDrive;
        DirectoryInfo destinationFolder;
        string movieName;

        public string MovieName
        {
            get { return movieName; }
            set { movieName = value; }
        }

        public DVDDecrypterController()
        {
            availableDrives = new List<DriveInfo>();

            foreach (DriveInfo dInfo in DriveInfo.GetDrives())
            {
                if (dInfo.DriveType == DriveType.CDRom)
                    availableDrives.Add(dInfo);
            }
        }

        public IList<DriveInfo> OpticalDrives
        {
            get { return availableDrives; }
        }

        public void SetSourceDrive(DriveInfo dInfo)
        {
            if (dInfo != null && dInfo.DriveType == DriveType.CDRom)
            {
                sourceDrive = dInfo;
                if (dInfo.VolumeLabel.Length > 0)
                    movieName = dInfo.VolumeLabel;
            }
        }

        public void SetDestinationFolder(string strDestination)
        {
            if (!string.IsNullOrEmpty(strDestination))
            {
                if (Directory.Exists(strDestination))
                {
                    DirectoryInfo dir = new DirectoryInfo(strDestination);
                    if (dir != null)
                        destinationFolder = dir;
                }
                else
                {
                    string basePath = Path.GetDirectoryName(strDestination);
                    if (Directory.Exists(basePath))
                    {
                        string newFolder = strDestination.Substring(strDestination.LastIndexOf(Path.DirectorySeparatorChar) +1);
                        if (!string.IsNullOrEmpty(newFolder))
                        {
                            try
                            {
                                Directory.CreateDirectory(Path.Combine(basePath, newFolder));
                                destinationFolder = new DirectoryInfo(strDestination);
                            }
                            catch (Exception e)
                            {
                                Utilities.DebugLine("Error creating subfolder {0}: {1}", strDestination, e.Message);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void RunDiscImport()
        {
            if (sourceDrive == null)
                throw new Exception("Missing selected Source Drive");

            if (destinationFolder == null)
                throw new Exception("Missing destination location");

            Process proc = new Process();
            proc.StartInfo.FileName = "";
            proc.StartInfo.Arguments = "";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.EnableRaisingEvents = true;
            proc.ErrorDataReceived += new DataReceivedEventHandler(proc_ErrorDataReceived);
            proc.Exited += new EventHandler(proc_Exited);
            proc.OutputDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);
            proc.Start();
        }

        void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void proc_Exited(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void LocationDVDDecrypter(string driveName)
        {

        }
    }
}
