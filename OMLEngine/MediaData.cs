/*********************************************
 * Inspired by the MediaCenter.DVD namespace *
 *********************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;

namespace OMLEngine
{
    static public class MediaData
    {
        const string BluRayFileName     = @"BDMV";
        const string DVDDirectoryName = @"VIDEO_TS";
        const string DVDFileName = @"VIDEO_TS.IFO";
        const string DVDFileName2 = @"VIDEO_TS\VIDEO_TS.IFO";
        const string HDDVDDirectoryName = @"HVDVD_TS";

        static public bool IsBluRay(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return DoesDirectoryExist(path, BluRayFileName);
        }

        static public bool IsHDDVD(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return DoesDirectoryExist(path, HDDVDDirectoryName);
        }

        static public bool IsDVD(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            bool flag = File.Exists(Path.Combine(path, DVDFileName));
            if (!flag)
                flag = File.Exists(Path.Combine(path, DVDFileName2));

            return flag;
        }

        static private bool DoesDirectoryExist(string basePath, string path)
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(basePath, path));

            try
            {
                return info.Exists;
            }
            catch
            {
                return false;
            }
        }

        static public string GetPlayStringForPath(string path)
        {
            Utilities.DebugLine("[MediaData] GetPlayStringForPath called");
            if (IsBluRay(path))
            {
                Utilities.DebugLine("[MediaData] Media detected is BluRay");
                if (Directory.Exists(Path.Combine(path, BluRayFileName)))
                {
                    Utilities.DebugLine("[MediaData] Returning BluRay path as: " +
                                        Path.Combine(path, BluRayFileName));
                    return Path.Combine(path, BluRayFileName);
                }
            }

            if (IsHDDVD(path))
            {
                Utilities.DebugLine("[MediaData] Media detected is HDDVD");
                if (Directory.Exists(Path.Combine(path, HDDVDDirectoryName)))
                {
                    Utilities.DebugLine("[MediaData] Returning HDDVD path as: " +
                                        Path.Combine(path, HDDVDDirectoryName));
                    return Path.Combine(path, HDDVDDirectoryName);
                }
            }

            if (IsDVD(path))
            {
                Utilities.DebugLine("[MediaData] Media detected is DVD");
                if (Directory.Exists(path) && path.EndsWith("VIDEO_TS", StringComparison.CurrentCultureIgnoreCase))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " + path);
                    return path;
                }

                if (Directory.Exists(Path.Combine(path, DVDDirectoryName)))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " +
                                        Path.Combine(path, DVDDirectoryName));
                    return Path.Combine(path, DVDDirectoryName);
                }
                if (File.Exists(Path.Combine(path, DVDFileName)))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " +
                                        Path.Combine(path, DVDFileName));
                    return Path.Combine(path, DVDFileName);
                }

                if (File.Exists(Path.Combine(path, DVDFileName2)))
                {
                    Utilities.DebugLine("[MediaData] Returning DVD path as: " +
                                        Path.Combine(path, DVDFileName2));
                    return Path.Combine(path, DVDFileName2);
                }
            }

            return null;
        }
    }
}
