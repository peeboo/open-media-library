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

        /// <summary>
        /// Gets what kind of movie this directory may contain
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static DirectoryType GetDirectoryMediaType(string folder)
        {
            if (Directory.Exists(Path.Combine(folder, VIDEO_TS_FOLDER))
                || File.Exists(Path.Combine(folder, DVD_IFO_FILE))
                || File.Exists(Path.Combine(folder, DVD_VOB_FILE)))
            {
                return DirectoryType.DVD;
            }
            else if (Directory.Exists(Path.Combine(folder, BLURAY_SUBDIR)))
            {
                return DirectoryType.BluRay;
            }
            else if (Directory.Exists(Path.Combine(folder, HDDVD_SUBIDR)))
            {
                return DirectoryType.HDDvd;
            }

            return DirectoryType.Normal;
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
        /// Returns all the paths to media under the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllMediaFromPath(string path)
        {                        
            // if the directory doesn't exist - bail out
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
