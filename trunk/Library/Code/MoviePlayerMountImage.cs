using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

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
                string mount_location = GetMountLocation();
                if (mount_location != null && mount_location.Length > 0)
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
            return string.Empty;
        }

        private bool MountTitle(MovieItem title)
        {
            if (DaemonToolsFound())
            {
            }

            return false;
        }

        private bool DaemonToolsFound()
        {
            return false;
        }

        MovieItem _title;
    }


}