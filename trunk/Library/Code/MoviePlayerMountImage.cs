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
            AddInHost.Current.MediaCenterEnvironment.Dialog("Mounting images not implemented yet. File " + _title.FileLocation, "Error", DialogButtons.Cancel, 0, true);

            // mount the file, then figure out which other player we want to create
            return false;
        }
        MovieItem _title;
    }


}