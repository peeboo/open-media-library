using System;
using OMLEngine;
using System.Collections;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace Library
{
    class MoviePlayerWPL : IPlayMovie
    {
        int _currentItem = 0;
        WindowsPlayListManager _wplm;
        MovieItem _mItem;

        public MoviePlayerWPL(MovieItem mItem)
        {
            _mItem = mItem;
            AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
            AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += Transport_PropertyChanged;

            OMLApplication.DebugLine("[MoviePlayerWPL] Loading for playlist: " + _mItem.FileLocation);
            _wplm = new WindowsPlayListManager(_mItem.FileLocation);
            _currentItem = 0;
        }

        public bool PlayMovie()
        {
            Utilities.DebugLine("Total Items: "+_wplm.PlayListItems.Count+" Current Item: " + _currentItem);

            if (_wplm.PlayListItems.Count > _currentItem)
            {
                Utilities.DebugLine("Loading Video Player: " + _currentItem);
                try
                {
                    PlayListItem item = (PlayListItem)_wplm.PlayListItems[_currentItem];
                    if (item != null)
                    {
                        _mItem.FileLocation = item.FileLocation;
                        _mItem.TitleObject.VideoFormat = VideoFormat.WMV;
                        Utilities.DebugLine("Playing now: " + _mItem.FileLocation);
                        IPlayMovie player = MoviePlayerFactory.CreateMoviePlayer(_mItem);
                        if (player != null)
                            player.PlayMovie();

                        _currentItem++;
                    }
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("Error: " + e.Message);
                    return false;
                }
            }
            else
            {
                _currentItem = 0;
            }
            return true;
        }

        public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            MediaTransport t = (MediaTransport)sender;
            //Utilities.DebugLine("MoviePlayerWPL.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
            if (property == "PlayState")
            {
                if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                {
                    //Utilities.DebugLine("MoviePlayerWPL.Transport_PropertyChanged: movie {0} Finished", OMLApplication.Current.NowPlayingMovieName);
                    OMLApplication.Current.NowPlayingStatus = "Finished";
                    if (t.Position.Seconds == 0)
                    PlayMovie();
                }
            }
        }
    }
}
