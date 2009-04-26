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
        public static string RootDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"OpenMediaLibrary"); }
        }

        public static string PublicRootDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData), @"OpenMediaLibrary");
            }
        }

        public static string TempPlayListDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"TempPlaylists"); }
        }

        public static string MainBackDropDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"MainBackDrops"); }
        }

        public static string FanArtDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"FanArt"); }
        }

        /// <summary>
        /// Location for cover art and other images to be stored
        /// </summary>
        public static string ImageDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"Images"); }
        }

        public static string ImageDownloadDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"DownloadImages"); }
        }

        public static string ExtenderCacheDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"ExtenderCache"); }
        }

        /// <summary>
        /// Location where all plugin dlls should be stored
        /// </summary>
        public static string PluginsDirectory
        {
            get { return Path.Combine(RootDirectory, @"Plugins"); }
        }

        public static bool FanArtDirectoryExists
        {
            get { return Directory.Exists(FanArtDirectory); }
        }

        public static void createFanArtDirectory()
        {
            Directory.CreateDirectory(FanArtDirectory);
        }
        /// <summary>
        /// Location where all debug and other logs are created
        /// </summary>
        public static string LogDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"Logs"); }
        }

        /// <summary>
        /// Location where all transcoded files are created
        /// </summary>
        public static string TranscodeBufferDirectory
        {
            get { return Path.Combine(PublicRootDirectory, @"TranscodeBuffers"); }
        }

        /// <summary>
        /// Location where all translation files are stored.
        /// </summary>
        public static string TranslationsDirectory
        {
            get { return Path.Combine(RootDirectory, @"Translations"); }
        }


        public static bool MainBackDropDirExists
        {
            get { return Directory.Exists(MainBackDropDirectory); }
        }
        /// <summary>
        /// Checks to ensure that the Root directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool RootDirExists
        {
            get { return Directory.Exists(RootDirectory); }
        }

        public static bool PublicRootDirExists
        {
            get { return Directory.Exists(PublicRootDirectory); }
        }

        /// <summary>
        /// Checks to ensure that the images directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool ImageDirExists
        {
            get { return Directory.Exists(ImageDirectory); }
        }

        public static bool ExtenderCacheDirectoryExists
        {
            get { return Directory.Exists(ExtenderCacheDirectory); }
        }

        public static bool ImageDownloadDirExists
        {
            get { return Directory.Exists(ImageDownloadDirectory); }
        }

        /// <summary>
        /// Checks to ensure that the plugins directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool PluginsDirExists
        {
            get { return Directory.Exists(PluginsDirectory); }
        }

        /// <summary>
        /// Checks to ensure that the logs directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool LogDirExists
        {
            get { return Directory.Exists(LogDirectory); }
        }

        /// <summary>
        /// Checks to ensure that the transcode buffer directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool TranscodeBufferDirExists
        {
            get { return Directory.Exists(TranscodeBufferDirectory); }
        }

        /// <summary>
        /// Checks to ensure that the translations directory exists
        /// </summary>
        /// <returns>True on success</returns>
        public static bool TranslationsDirExists
        {
            get { return Directory.Exists(TranslationsDirectory); }
        }

        public static bool TempPlayListDirExists
        {
            get { return Directory.Exists(TempPlayListDirectory); }
        }

        public static void createMainBackDropDirectory()
        {
            Directory.CreateDirectory(MainBackDropDirectory);
        }

        /// <summary>
        /// Creates the root directory if it doesn't already exist
        /// </summary>
        public static void createRootDirectory()
        {
            Directory.CreateDirectory(RootDirectory);
        }

        public static void createPublicRootDirectory()
        {
            Directory.CreateDirectory(PublicRootDirectory);
        }

        public static void createTempPlayListDirectory()
        {
            Directory.CreateDirectory(TempPlayListDirectory);
        }

        /// <summary>
        /// Creates the image directory if it doesn't already exist
        /// </summary>
        public static void createImageDirectory()
        {
            Directory.CreateDirectory(ImageDirectory);
        }

        public static void CreateExtenderCacheDirectory()
        {
            Directory.CreateDirectory(ExtenderCacheDirectory);
        }

        public static void createImageDownloadDirectory()
        {
            Directory.CreateDirectory(ImageDownloadDirectory);
        }

        /// <summary>
        /// Creates the plugins directory if it doesn't already exist
        /// </summary>
        public static void createPluginsDirectory()
        {
            Directory.CreateDirectory(PluginsDirectory);
        }

        /// <summary>
        /// Creates the log directory if it doesn't already exist
        /// </summary>
        public static void createLogDirectory()
        {
            Directory.CreateDirectory(LogDirectory);
        }

        /// <summary>
        /// Creates the transcode buffer directory if it doesn't already exist
        /// </summary>
        public static void createTranscodeBufferDirectory()
        {
            Directory.CreateDirectory(TranscodeBufferDirectory);
        }

        /// <summary>
        /// Changes the Current Working Directory to the Root Directory
        /// </summary>
        public static void changeToRootDirectory()
        {
            if (!RootDirExists)
            {
                createRootDirectory();
            }

            Directory.SetCurrentDirectory(RootDirectory);
        }

        public static void changeToPublicRootDirectory()
        {
            if (!PublicRootDirExists)
                createPublicRootDirectory();

            Directory.SetCurrentDirectory(PublicRootDirectory);
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
                Utilities.DebugLine("[FileSystemWalker] " + starting_dir + " doesn't exist" + e.InnerException);
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
