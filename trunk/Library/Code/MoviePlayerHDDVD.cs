using System;
using OMLEngine;
using OMLEngine.FileSystem;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    public class HDDVDPlayer : IPlayMovie
    {
        MediaSource _source;
        string _mediaPath;

        public HDDVDPlayer(MediaSource source, string mediaPath)
        {
            _source = source;
            _mediaPath = mediaPath;
        }

        public bool PlayMovie()
        {
            if (FileScanner.IsHDDVD(_mediaPath))
            {
                string media = FileScanner.GetPlayStringForPath(_mediaPath);
                media = "HDDVD://" + media;
                media.Replace('\\', '/');
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, media, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        Utilities.DebugLine("HDDVDPlayer.PlayMovie: movie {0} Playing", _source);
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
