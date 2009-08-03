using System;
using System.IO;

namespace PFMLib
{
    public class PFMAPI
    {

        static bool IsCurrentlyMounted(string fileName)
        {
            PfmApi api;
            if (PfmStatic.ApiFactory(out api) != 0)
                return false;

            PfmIterator mountList;
            long changeInstance = 0;
            long nextChangeInstance = changeInstance;
            if (api.MountIterate(changeInstance, out nextChangeInstance, out mountList) != 0)
                return false;

            int mountId = mountList.Next(ref nextChangeInstance);
            while (mountId != 0)
            {
                PfmMount mount;
                if (api.MountOpenId(mountId, out mount) != 0)
                {
                    if (mount.GetFileName().ToLower().CompareTo(fileName.ToLower()) == 0)
                        return true;
                }
                mountId = mountList.Next(ref nextChangeInstance);
            }
            return false;
        }

        static bool IsMountLocationInUse(string driveLetter)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.Name.ToLower().CompareTo(driveLetter.ToLower()) == 0)
                    return true;
            }
            return false;
        }

        static void Unmount(string fileName)
        {
            PfmApi api;
            if (PfmStatic.ApiFactory(out api) != 0)
                return;

            PfmMount mount;
            if (api.MountOpen(fileName, out mount) != 0)
                return;

            mount.Unmount(0);
            mount.Dispose();
        }

        static char? Mount(string fileName)
        {
            PfmApi api;
            if (PfmStatic.ApiFactory(out api) != 0)
                return null;

            PfmMount mount;
            if (api.MountOpen(fileName, out mount) != 0)
                return null;

            char driveLetter = mount.GetDriveLetter();
            return driveLetter;
        }
    }
}
