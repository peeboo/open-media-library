using System;
using OMLEngine;
using OMLEngine.FileSystem;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    public class BluRayPlayer : IPlayMovie
    {
        MediaSource _source;
        string _mediaPath;

        public BluRayPlayer(MediaSource source, string mediaPath)
        {
            _source = source;
            _mediaPath = mediaPath;
        }

        public bool PlayMovie()
        {
            if (FileScanner.IsBluRay(_mediaPath))
            {
                string play_string = FileScanner.GetPlayStringForPath(_mediaPath);
//                play_string = "BLURAY://" + play_string;
//                play_string.Replace('\\', '/');
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, play_string, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        Utilities.DebugLine("BluRayPlayer.PlayMovie: movie {0} Playing", _source);
                        OMLApplication.Current.NowPlayingMovieName = _source.Name;
                        OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged -= MoviePlayerFactory.Transport_PropertyChanged;
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
