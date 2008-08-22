using System;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    public class HDDVDPlayer : IPlayMovie
    {
        MovieItem _title;
        public HDDVDPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (MediaData.IsHDDVD(_title.SelectedDisk.Path))
            {
                string media = MediaData.GetPlayStringForPath(_title.SelectedDisk.Path);
                media = "HDDVD://" + media;
                media.Replace('\\', '/');
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, media, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        Utilities.DebugLine("HDDVDPlayer.PlayMovie: movie {0} Playing", _title.Name);
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
