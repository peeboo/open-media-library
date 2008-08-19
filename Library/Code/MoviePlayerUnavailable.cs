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
    public class UnavailableMoviePlayer : IPlayMovie
    {
        public UnavailableMoviePlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            // show a popup or a page explaining the error
            AddInHost.Current.MediaCenterEnvironment.Dialog("Could not find file [" + _title.SelectedDisk.Path + "] or don't know how to play this file type", 
                "Error", DialogButtons.Ok, 0, true);
            return false;
        }

        MovieItem _title;
    }
}