using System;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    /// <summary>
    /// DVDPlayer class for playing a DVD
    /// </summary>
    public class DVDPlayer : IPlayMovie
    {
        public DVDPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (MediaData.IsDVD(_title.SelectedDisk.Path))
            {
                string play_string = MediaData.GetPlayStringForPath(_title.SelectedDisk.Path);
//                play_string = "DVD://" + play_string;
//                play_string.Replace('\\', '/');
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Dvd, play_string, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        Utilities.DebugLine("DVDPlayer.PlayMovie: movie {0} Playing", _title.Name);
                        OMLApplication.Current.NowPlayingMovieName = _title.Name;
                        OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                    }
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        MovieItem _title;
    }

}