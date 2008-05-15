using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Library
{
    public class MountImagePlayer : IPlayMovie
    {
        public MountImagePlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (MountTitle(_title))
            {
                string mount_location = GetMountLocation() + ":\\video_ts";
                Trace.WriteLine("Going to play movie at: " + mount_location);
                if (mount_location != null)
                {
                    _title.FileLocation = mount_location;
                    IPlayMovie player = new DVDPlayer(_title);
                    Trace.WriteLine("Playing now");
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
            OMLConfigManager cm = new OMLConfigManager();
            string mount_location = cm.GetValue("VirtualDiscDrive");
            if (mount_location != null && mount_location.Length > 0)
                return mount_location;

            return null;
        }

        private bool MountTitle(MovieItem title)
        {
            Trace.WriteLine("Mounting Title");
            Process cmd = new Process();
            OMLConfigManager cm = new OMLConfigManager();
            string mount_util_path = cm.GetValue("DaemonTools");
            Trace.WriteLine("DaemonTools: " + mount_util_path);
            string VirtualDiscDrive = GetMountLocation();
            string VirtualDiscDriveNumber = cm.GetValue("VirtualDiscDriveNumber");
            Trace.WriteLine("Drive: " + VirtualDiscDrive);

            Trace.WriteLine("Unmounting any old image");
            Utilities.UnmountVirtualDrive(VirtualDiscDriveNumber);
            Thread.Sleep(100);


            if (Utilities.HasDaemonTools())
            {
                Trace.WriteLine("HasDaemonTools: TRUE");
                cmd.StartInfo.FileName = "\"" + mount_util_path + "\"";
                cmd.StartInfo.Arguments = @"-mount " + VirtualDiscDriveNumber + "," + "\"" + title.FileLocation + "\"";
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.UseShellExecute = false;

                Trace.WriteLine("Mounting image: " + title.FileLocation);
                Trace.WriteLine("CMD: " + cmd.StartInfo.FileName + cmd.StartInfo.Arguments);
                cmd.Start();

                string volLabel = string.Empty;
                int _tries = 0;
                DriveInfo dInfo = Utilities.DriveInfoForDrive(VirtualDiscDrive);
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
                        Trace.WriteLine("Not ready, sleeping for 100 milliseconds");
                        Thread.Sleep(100);
                    }
                } while (volLabel.Length == 0 && _tries < 100);

                Trace.WriteLine("Got it: VolLabel: " + volLabel);
                if (volLabel.Length == 0)
                    return false;
                else
                    return true;
            } else
                Trace.WriteLine("HasDaemonTools: FALSE");

            return false;
        }

        MovieItem _title;
    }


}