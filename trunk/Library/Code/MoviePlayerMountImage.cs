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
    /*public class MountImagePlayer : IPlayMovie
    {
        MediaSource _source;
        MountingTool _dt;

        public MountImagePlayer(MediaSource source)
        {
            _source = source;
            _dt = new MountingTool();
        }

        public bool PlayMovie()
        {
            // NOTE: no need to call UnMount, this is done automatically by Mount
            string mount_location = string.Empty;
            if (_dt.Mount(_source.MediaPath, out mount_location))
            {
                mount_location += @":\\";
                OMLApplication.DebugLine("Going to play movie at: " + mount_location);
                if (mount_location != null)
                {
                    _source.MediaPath = mount_location;
                    _source.Format = VideoFormat.DVD;
                    IPlayMovie player;
                    if (OMLApplication.Current.IsExtender)
                    {
                        if (MediaData.IsDVD(_source.MediaPath) == false)
                        {
                            OMLApplication.DebugLine("[MoviePlayerMountImage] Extender detected, using TranscodePlayer to play Image file");
                            player = new TranscodePlayer(_source);
                        }
                        else
                        {
                            OMLApplication.DebugLine("[MoviePlayerMountImage] Extender detected, using ExtenderDVDPlayer to play Image file");
                            player = new ExtenderDVDPlayer(_source);
                        }
                    }
                    else
                    {
                        OMLApplication.DebugLine("[MoviePlayerMountImage] Extender NOT detected, using DVDPlayer to play Image file");
                        player = new DVDPlayer(_source);
                    }
                    OMLApplication.DebugLine("[MoviePlayerMountImage] Playing now");
                    return player.PlayMovie();
                }
                else
                {
                    AddInHost.Current.MediaCenterEnvironment.Dialog("Mounting images not implemented yet. File " + _source.MediaPath, "Error", DialogButtons.Cancel, 0, true);
                }
            }
            return false;
        }

    }*/


}