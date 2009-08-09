using System;
using OMLEngine;
using System.Collections;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace Library
{
    class MoviePlayerWVX : IPlayMovie
    {
        int _currentItem = 0;
        WVXManager _wplm;
        MediaSource _mItem;

        public MoviePlayerWVX(MediaSource source)
        {
            _mItem = source;

            OMLApplication.DebugLine("[MoviePlayerWVX] Loading for playlist: " + _mItem.MediaPath);
            _wplm = new WVXManager(_mItem.MediaPath);
            _currentItem = 0;
        }

        public bool PlayMovie()
        {
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, _mItem.MediaPath, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    Utilities.DebugLine("VideoPlayer.PlayMovie: movie {0} Playing", _mItem);
                    OMLApplication.Current.NowPlayingMovieName = _mItem.Name;
                    OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                //Utilities.DebugLine("MoviePlayerWVX.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        //Utilities.DebugLine("MoviePlayerWVX.Transport_PropertyChanged: movie {0} Finished", OMLApplication.Current.NowPlayingMovieName);
                        OMLApplication.Current.NowPlayingStatus = PlayState.Finished;
                        if (t.Position.Seconds == 0)
                            PlayMovie();
                    }
                }
            });
        }
    }
}
