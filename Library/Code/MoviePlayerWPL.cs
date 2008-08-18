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

            OMLApplication.DebugLine("[MoviePlayerWPL] Loading for playlist: " + _mItem.SelectedDisk.Path);
            _wplm = new WindowsPlayListManager(_mItem.SelectedDisk.Path);
            _currentItem = 0;
        }

        public bool PlayMovie()
        {
            Utilities.DebugLine("[MoviePlayerWPL] Total Items: "+_wplm.PlayListItems.Count+" Current Item: " + _currentItem);

            if (_wplm.PlayListItems.Count > _currentItem)
            {
                Utilities.DebugLine("[MoviePlayerWPL] Loading Video Player: " + _currentItem);
                try
                {
                    PlayListItem item = (PlayListItem)_wplm.PlayListItems[_currentItem];
                    if (item != null)
                    {
                        _mItem.SelectedDisk.Path = item.FileLocation;
                        _mItem.TitleObject.SelectedDisk.Format = VideoFormat.WMV;
                        Utilities.DebugLine("[MoviePlayerWPL] Playing now: " + _mItem.SelectedDisk.Path);
                        IPlayMovie player = MoviePlayerFactory.CreateMoviePlayer(_mItem);
                        if (player != null)
                            player.PlayMovie();

                        _currentItem++;
                    }
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[MoviePlayerWPL] Error: " + e.Message);
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
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                //Utilities.DebugLine("MoviePlayerWPL.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        //Utilities.DebugLine("MoviePlayerWPL.Transport_PropertyChanged: movie {0} Finished", OMLApplication.Current.NowPlayingMovieName);
                        OMLApplication.Current.NowPlayingStatus = PlayState.Finished;
                        if (t.Position.Seconds == 0)
                            PlayMovie();
                    }
                }
            });
        }
    }
}
