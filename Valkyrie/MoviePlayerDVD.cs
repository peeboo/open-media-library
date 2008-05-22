using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Valkyrie
{
    /// <summary>
    /// DVDPlayer class for playing a DVD
    /// </summary>
    public class DVDPlayer : IPlayMovie
    {
        public bool IsExtender()
        {
            return false;
        }

        public DVDPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            string media = "DVD://" + _title.FileLocation;
            media.Replace('\\', '/');
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Dvd, media, false))
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