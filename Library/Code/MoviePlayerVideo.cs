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
    /// <summary>
    /// VideoPlayer class for playing standard videos (AVIs, etc)
    /// </summary>
    public class VideoPlayer : IPlayMovie
    {
        public VideoPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, _title.FileLocation, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        MovieItem _title;
    }
}