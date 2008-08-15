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
            _dt = new DaemonTools();
        }

        public bool PlayMovie()
        {
            // NOTE: no need to call UnMount, this is done automatically by Mount
            string mount_location = string.Empty;
            if (_dt.Mount(_title.SelectedDisk.Path, out mount_location))
            {
                mount_location += @":\\";
                OMLApplication.DebugLine("Going to play movie at: " + mount_location);
                if (mount_location != null)
                {
                    _title.SelectedDisk.Path = mount_location;
                    _title.SelectedDisk.Format = VideoFormat.DVD;
                    IPlayMovie player;
                    if (OMLApplication.Current.IsExtender)
                    {
                        if (Utilities.IsTranscode360LibraryAvailable())
                        {
                            OMLApplication.DebugLine("[MoviePlayerMountImage] Extender detected, using TranscodePlayer to play Image file");
                            player = new TranscodePlayer(_title);
                        }
                        else
                        {
                            OMLApplication.DebugLine("[MoviePlayerMountImage] Extender detected, using ExtenderDVDPlayer to play Image file");
                            player = new ExtenderDVDPlayer(_title);
                        }
                    }
                    else
                    {
                        OMLApplication.DebugLine("[MoviePlayerMountImage] Extender NOT detected, using DVDPlayer to play Image file");
                        player = new DVDPlayer(_title);
                    }
                    OMLApplication.DebugLine("[MoviePlayerMountImage] Playing now");
                    return player.PlayMovie();
                }
                else
                {
                    AddInHost.Current.MediaCenterEnvironment.Dialog("Mounting images not implemented yet. File " + _title.SelectedDisk.Path, "Error", DialogButtons.Cancel, 0, true);
                }
            }
            return false;
        }

        MovieItem _title;
        DaemonTools _dt;
    }


}