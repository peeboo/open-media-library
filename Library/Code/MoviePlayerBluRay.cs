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
        MediaSource _source;

        public BluRayPlayer(MediaSource source)
        {
            _source = source;
        }

        public bool PlayMovie()
        {
            if (MediaData.IsBluRay(_source.MediaPath))
            {
                string play_string = MediaData.GetPlayStringForPath(_source.MediaPath);
//                play_string = "BLURAY://" + play_string;
//                play_string.Replace('\\', '/');
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, play_string, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        Utilities.DebugLine("BluRayPlayer.PlayMovie: movie {0} Playing", _source);
                        OMLApplication.Current.NowPlayingMovieName = _source.Name;
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
