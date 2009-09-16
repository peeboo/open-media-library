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
        MediaSource _source;

        public UnavailableMoviePlayer(MediaSource source)
        {
            _source = source;
        }

        public bool PlayMovie()
        {
            // show a popup or a page explaining the error
            AddInHost.Current.MediaCenterEnvironment.Dialog("Could not find file [" + _source.MediaPath + "] or don't know how to play this file type", 
                "Error", DialogButtons.Ok, 0, true);
            return false;
        }
    }
}