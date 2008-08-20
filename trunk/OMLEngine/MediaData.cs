/*********************************************
 * Inspired by the MediaCenter.DVD namespace *
 *********************************************/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;

namespace OMLEngine
{
    public class MediaData
    {
        static string BluRayFileName     = @"BDMV";
        static string DVDDirectoryName   = @"VIDEO_TS";
        static string DVDFileName        = @"VIDEO_TS.IFO";
        static string DVDFileName2       = @"VIDEO_TS\VIDEO_TS.IFO";
        static string HDDVDDirectoryName = @"HVDVD_TS";

        static bool IsBluRay(string path)
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
        static bool IsDVD(string path)
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
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
