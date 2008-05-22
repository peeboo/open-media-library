using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Valkyrie
{
    public class UnavailableMoviePlayer : IPlayMovie
    {
        public bool IsExtender()
        {
            return false;
        }

        public UnavailableMoviePlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            // show a popup or a page explaining the error
            //return false;
            AddInHost.Current.MediaCenterEnvironment.Dialog("Could not find file [" + _title.FileLocation + "] or don't know how to play this file type", "Error", DialogButtons.Ok, 0, true);
            return false;
        }

        MovieItem _title;
    }
}