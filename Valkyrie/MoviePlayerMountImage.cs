using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using System.Threading;
using OMLEngine;

namespace Valkyrie
{
    public class MountImagePlayer : IPlayMovie
    {
        public bool IsExtender()
        {
            return false;
        }

        public MountImagePlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (MountTitle(_title))
            {
                string mount_location = GetMountLocation() + ":\\video_ts";
                if (mount_location != null)
                {
                    _title.FileLocation = mount_location;
                    IPlayMovie player = new DVDPlayer(_title);
                    return player.PlayMovie();
                }
                else
                {
                    AddInHost.Current.MediaCenterEnvironment.Dialog("Mounting images not implemented yet. File " + _title.FileLocation, "Error", DialogButtons.Cancel, 0, true);
                }
            }
            return false;
        }

        private string GetMountLocation()
        {
            string mount_location = null; // cm.GetValue("VirtualDiscDrive");
            if (mount_location != null && mount_location.Length > 0)
                return mount_location;

            return null;
        }

        private bool MountTitle(MovieItem title)
        {
            Process cmd = new Process();

            string mount_util_path = OMLEngine.Properties.Settings.Default.DaemonTools;
            string VirtualDiscDrive = GetMountLocation();
            int VirtualDiscDriveNumber = OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber;

            Utilities.UnmountVirtualDrive(VirtualDiscDriveNumber);
            Thread.Sleep(100);


            if (Utilities.HasDaemonTools())
            {
                cmd.StartInfo.FileName = "\"" + mount_util_path + "\"";
                cmd.StartInfo.Arguments = @"-mount " + VirtualDiscDriveNumber + "," + "\"" + title.FileLocation + "\"";
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.UseShellExecute = false;

                cmd.Start();

                string volLabel = string.Empty;
                int _tries = 0;
                DriveInfo dInfo = Utilities.DriveInfoForDrive(VirtualDiscDrive);
                /* We need to give daemontools a bit of time to complete
                 * mounting the image, so we wait 100 milliseconds and check it
                 * if its ready then we go, if not then we wait again but we
                 * only wait for a total of 100 * 100 milliseconds (100m. x 100 tries)
                */
                do
                {
                    try
                    {
                        _tries++;
                        dInfo = Utilities.DriveInfoForDrive(VirtualDiscDrive);
                        volLabel = dInfo.VolumeLabel;
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(100);
                    }
                } while (volLabel.Length == 0 && _tries < 100);

                if (volLabel.Length == 0)
                    return false;
                else
                    return true;
            } else

            return false;
        }

        MovieItem _title;
    }


}