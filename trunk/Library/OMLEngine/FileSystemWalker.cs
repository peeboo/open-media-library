using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace OMLEngine
{
    class FileSystemWalker
    {
        public static string RootDirectory =
            Environment.SpecialFolder.LocalApplicationData + "/OpenMediaLibrary";

        public static string ImageDirectory =
            RootDirectory + "/Images";

        public static bool RootDirExists()
        {
            if (Directory.Exists(RootDirectory))
                return true;

            return false;
        }
        public static void createRootDirectory()
        {
            Directory.CreateDirectory(RootDirectory);
        }
        public static void changeToRootDirectory()
        {
            if (!RootDirExists())
            {
                createRootDirectory();
            }

            Directory.SetCurrentDirectory(RootDirectory);
        }
        public static bool DirectoryExists(string Dir)
        {
            return Directory.Exists(Dir);
        }
        public static void CreateDirectory(string Dir)
        {
            if (!DirectoryExists(Dir))
                Directory.CreateDirectory(Dir);
        }
        public static void ChangeToDirectory(string Dir)
        {
            if (!DirectoryExists(Dir))
                CreateDirectory(Dir);

            Directory.SetCurrentDirectory(Dir);
        }
        public static IList<DriveInfo> GetDrives(DriveType requestedType)
        {
            IList<DriveInfo> requested_drives = new List<DriveInfo>();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.DriveType == requestedType)
                {
                    requested_drives.Add(drive);
                }
            }
            return requested_drives;
        }
    }
}
