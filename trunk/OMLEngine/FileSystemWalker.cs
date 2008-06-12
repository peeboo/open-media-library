using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace OMLEngine
{

    /// <summary>
    /// 
    /// </summary>
    public class FileSystemWalker
    {
        /// <summary>
        /// Location of Root directory for ALL OML files
        /// </summary>
        public static string RootDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\OpenMediaLibrary";

        /// <summary>
        /// Location for cover art and other images to be stored
        /// </summary>
        public static string ImageDirectory =
            RootDirectory + "\\Images";

        /// <summary>
        /// Location where all plugin dlls should be stored
        /// </summary>
        public static string PluginDirectory =
            RootDirectory + "\\plugins";

        /// <summary>
        /// Location where all debug and other logs are created
        /// </summary>
        public static string LogDirectory =
            RootDirectory + "\\logs";

        /// <summary>
        /// Checks to ensure that the Root directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool RootDirExists()
        {
            if (Directory.Exists(RootDirectory))
                return true;

            return false;
        }

        /// <summary>
        /// Checks to ensure that the images directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool ImageDirExists()
        {
            if (Directory.Exists(ImageDirectory))
                return true;

            return false;
        }

        /// <summary>
        /// Checks to ensure that the plugins directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool PluginsDirExists()
        {
            if (Directory.Exists(PluginDirectory))
                return true;

            return false;
        }

        /// <summary>
        /// Checks to ensure that the logs directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool LogDirExists()
        {
            if (Directory.Exists(LogDirectory))
                return true;

            return false;
        }

        /// <summary>
        /// Creates the root directory if it doesn't already exist
        /// </summary>
        public static void createRootDirectory()
        {
            Directory.CreateDirectory(RootDirectory);
        }

        /// <summary>
        /// Creates the image directory if it doesn't already exist
        /// </summary>
        public static void createImageDirectory()
        {
            Directory.CreateDirectory(ImageDirectory);
        }

        /// <summary>
        /// Creates the plugins directory if it doesn't already exist
        /// </summary>
        public static void createPluginsDirectory()
        {
            Directory.CreateDirectory(PluginDirectory);
        }

        /// <summary>
        /// Creates the log directory if it doesn't already exist
        /// </summary>
        public static void createLogDirectory()
        {
            Directory.CreateDirectory(LogDirectory);
        }

        /// <summary>
        /// Changes the Current Working Directory to the Root Directory
        /// </summary>
        public static void changeToRootDirectory()
        {
            if (!RootDirExists())
            {
                createRootDirectory();
            }

            Directory.SetCurrentDirectory(RootDirectory);
        }

        /// <summary>
        /// Checks if a given directory exists
        /// </summary>
        /// <param name="Dir">string name of directory to check</param>
        /// <returns>True if it exists</returns>
        public static bool DirectoryExists(string Dir)
        {
            return Directory.Exists(Dir);
        }

        /// <summary>
        /// Creates the given directory if it doesn't already exist
        /// </summary>
        /// <param name="Dir">string name of directory to create</param>
        public static void CreateDirectory(string Dir)
        {
            if (!DirectoryExists(Dir))
                Directory.CreateDirectory(Dir);
        }

        /// <summary>
        /// Change the Current Working Directory to the given directory
        /// </summary>
        /// <param name="Dir">string name of directory to change to</param>
        public static void ChangeToDirectory(string Dir)
        {
            if (!DirectoryExists(Dir))
                CreateDirectory(Dir);

            Directory.SetCurrentDirectory(Dir);
        }

        /// <summary>
        /// List of physical drives on the current machine
        /// </summary>
        /// <param name="requestedType">DriveType being searched for</param>
        /// <returns>List of DriveInfo objects found</returns>
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


        // Actual instance class starts here
        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo _baseDir;

        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo _currentDir;
        private ArrayList _directors_to_scan;

        /// <summary>
        /// 
        /// </summary>
        public FileSystemWalker()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="starting_dir"></param>
        public FileSystemWalker(string starting_dir)
        {
            DirectoryInfo dInfo = null;
            try
            {
                if (Directory.Exists(starting_dir)) 
                {
                    dInfo = new DirectoryInfo(starting_dir);
                    if (dInfo != null)
                        _baseDir = dInfo;
                }
            }
            catch (Exception e)
            {
                Utilities.DebugLine(starting_dir + " doesn't exist");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="starting_dir"></param>
        public FileSystemWalker(DirectoryInfo starting_dir)
        {
            _baseDir = starting_dir;
        }

        /// <summary>
        /// 
        /// </summary>
        ~FileSystemWalker()
        {
        }
    }
}
