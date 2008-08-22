using System;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    public class BluRayPlayer : IPlayMovie
    {
        MovieItem _title;

        public BluRayPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (MediaData.IsBluRay(_title.SelectedDisk.Path))
            {
                string play_string = MediaData.GetPlayStringForPath(_title.SelectedDisk.Path);
//                play_string = "BLURAY://" + play_string;
//                play_string.Replace('\\', '/');
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, play_string, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        Utilities.DebugLine("BluRayPlayer.PlayMovie: movie {0} Playing", _title.Name);
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
    }
}
