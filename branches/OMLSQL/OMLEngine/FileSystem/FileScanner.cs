using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace OMLEngine.FileSystem
{
    public static class FileScanner
    {
        private const string VIDEO_TS_FOLDER = "VIDEO_TS";
        private const string DVD_IFO_FILE = "VIDEO_TS.IFO";
        private const string DVD_VOB_FILE = "vts_01_1.vob";
        private const string BLURAY_SUBDIR = "BDMV";
        private const string HDDVD_SUBIDR = "HVDVD_TS";
        private const string DVD_VIDEOTS_FILENAME = VIDEO_TS_FOLDER + "\\" + DVD_IFO_FILE;

        /// <summary>
        /// Gets what kind of movie this directory may contain
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static DirectoryType GetDirectoryMediaType(string folder)
        {
            if (IsDVD(folder))
                return DirectoryType.DVD;

            if (IsBluRay(folder))
                return DirectoryType.BluRay;

            if (IsHDDVD(folder))
                return DirectoryType.HDDVD;

            return DirectoryType.Normal;
        }

        /// <summary>
        /// Returns if the given path is a bluray disk
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsBluRay(string path)
        {
            return Directory.Exists(Path.Combine(path, BLURAY_SUBDIR));
        }

        /// <summary>
        /// Returns if the given path is a dvd
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDVD(string path)
        {
            return Directory.Exists(Path.Combine(path, VIDEO_TS_FOLDER))
                    || File.Exists(Path.Combine(path, DVD_IFO_FILE))
                    || File.Exists(Path.Combine(path, DVD_VOB_FILE));
        }

        /// <summary>
        /// Returns if the given path is an hddvd
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsHDDVD(string path)
        {
            return Directory.Exists(Path.Combine(path, HDDVD_SUBIDR));
        }

        /// <summary>
        /// Returns all the paths to media under the given paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllMediaFromPath(IEnumerable<string> paths)
        {
            foreach (string path in paths)
            {
                foreach (string mediaPath in GetAllMediaFromPath(path))
                    yield return mediaPath;
            }
        }

        /// <summary>
        /// Returns the play path give the given media path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPlayStringForPath(string path)
        {
            Utilities.DebugLine("[MediaData] GetPlayStringForPath called");

            if (IsDVD(path))
            {
                Utilities.DebugLine("[MediaData] Media detected is DVD");
                if (Directory.Exists(path) && path.EndsWith("VIDEO_TS", StringComparison.CurrentCultureIgnoreCase))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " + path);
                    return path;
                }

                if (Directory.Exists(Path.Combine(path, VIDEO_TS_FOLDER)))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " +
                                        Path.Combine(path, VIDEO_TS_FOLDER));
                    return Path.Combine(path, VIDEO_TS_FOLDER);
                }

                if (File.Exists(Path.Combine(path, DVD_IFO_FILE)))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " +
                                        Path.Combine(path, DVD_IFO_FILE));
                    return Path.Combine(path, DVD_IFO_FILE);
                }

                if (File.Exists(Path.Combine(path, DVD_VIDEOTS_FILENAME)))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " +
                                        Path.Combine(path, DVD_VIDEOTS_FILENAME));
                    return Path.Combine(path, DVD_VIDEOTS_FILENAME);
                }
            }

            if (IsBluRay(path))
            {
                Utilities.DebugLine("[MediaData] Media detected is BluRay");
                if (Directory.Exists(Path.Combine(path, BLURAY_SUBDIR)))
                {
                    Utilities.DebugLine("[MediaData] Returning BluRay path as: " +
                                        Path.Combine(path, BLURAY_SUBDIR));
                    return Path.Combine(path, BLURAY_SUBDIR);
                }
            }

            if (IsHDDVD(path))
            {
                Utilities.DebugLine("[MediaData] Media detected is HDDVD");
                if (Directory.Exists(Path.Combine(path, HDDVD_SUBIDR)))
                {
                    Utilities.DebugLine("[MediaData] Returning HDDVD path as: " +
                                        Path.Combine(path, HDDVD_SUBIDR));
                    return Path.Combine(path, HDDVD_SUBIDR);
                }
            }            

            return null;
        }

        /// <summary>
        /// Returns all the paths to media under the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllMediaFromPath(string path)
        {                        
            // if the directory doesn't exist - don't process anything
            if (Directory.Exists(path))
            {
                DirectoryType dirType = GetDirectoryMediaType(path);

                if (dirType != DirectoryType.Normal)
                {
                    yield return path;
                }
                else
                {
                    string[] files = null;
                    string[] subDirs = null;

                    try
                    {
                        files = Directory.GetFiles(path);
                        subDirs = Directory.GetDirectories(path);                        
                    }
                    // ignore access denied errors
                    catch (System.UnauthorizedAccessException) { }

                    if (files != null && files.Length != 0)
                    {
                        // get all the media in this path
                        foreach (string file in files)
                        {
                            if (FileTypes.SupportedVideoExtensions.Contains(Path.GetExtension(file)))
                            {
                                yield return file;
                            }
                        }
                    }

                    if (subDirs != null && subDirs.Length != 0)
                    {
                        // process all the subdirs
                        foreach (string subdir in subDirs)
                        {
                            foreach (string file in GetAllMediaFromPath(subdir))
                            {
                                yield return file;
                            }
                        }
                    }
                }
            }
        }       
    }
}
