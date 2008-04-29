using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace OMLEngine
{
    public class FileSystemWalker
    {
        public static string RootDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OpenMediaLibrary";

        public static string ImageDirectory =
            RootDirectory + "\\Images";

        public static string PluginDirectory =
            RootDirectory + "\\plugins";
        public static string LogDirectory =
            RootDirectory + "\\logs";

        public static bool RootDirExists()
        {
            if (Directory.Exists(RootDirectory))
                return true;

            return false;
        }
        public static bool ImageDirExists()
        {
            if (Directory.Exists(ImageDirectory))
                return true;

            return false;
        }
        public static bool PluginsDirExists()
        {
            if (Directory.Exists(PluginDirectory))
                return true;

            return false;
        }
        public static bool LogDirExists()
        {
            if (Directory.Exists(LogDirectory))
                return true;

            return false;
        }
        public static void createRootDirectory()
        {
            Directory.CreateDirectory(RootDirectory);
        }
        public static void createImageDirectory()
        {
            Directory.CreateDirectory(ImageDirectory);
        }
        public static void createPluginsDirectory()
        {
            Directory.CreateDirectory(PluginDirectory);
        }
        public static void createLogDirectory()
        {
            Directory.CreateDirectory(LogDirectory);
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
