using System;
using System.Collections.Generic;
using System.Text;
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
            string media = "DVD://" + _title.FileLocation;
            media.Replace('\\', '/');
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Dvd, media, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    Utilities.DebugLine("DVDPlayer.PlayMovie: movie {0} Playing", _title.Name);
                    OMLApplication.Current.NowPlaying = "Playing: " + _title.Name;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            MediaTransport t = (MediaTransport)sender;
            Utilities.DebugLine("DVDPlayer.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", _title.Name, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
            if (property == "PlayState")
            {
                if (t.PlayState == PlayState.Paused)
                {
                    OMLApplication.Current.NowPlaying = "Paused: " + _title.Name;
                    Utilities.DebugLine("DVDPlayer.Transport_PropertyChanged: movie {0} Paused", _title.Name);
                }
                else if (t.PlayState == PlayState.Playing)
                {
                    OMLApplication.Current.NowPlaying = "Playing: " + _title.Name;
                    Utilities.DebugLine("DVDPlayer.Transport_PropertyChanged: movie {0} Playing", _title.Name);
                }
                else if (t.PlayState == PlayState.Finished)
                {
                    Utilities.DebugLine("DVDPlayer.Transport_PropertyChanged: movie {0} Finished", _title.Name);
                    OMLApplication.Current.NowPlaying = "Finished: " + _title.Name;
                    t.PropertyChanged -= Transport_PropertyChanged;
                }
                else if (t.PlayState == PlayState.Stopped)
                {
                    OMLApplication.Current.NowPlaying = "Stopped: " + _title.Name;
                    Utilities.DebugLine("DVDPlayer.Transport_PropertyChanged: movie {0} Stopped", _title.Name);
                    t.PropertyChanged -= Transport_PropertyChanged;
                }
                else if (t.PlayState == PlayState.Undefined)
                {
                    OMLApplication.Current.NowPlaying = "Last Watched: " + _title.Name;
                    Utilities.DebugLine("DVDPlayer.Transport_PropertyChanged: movie {0} LastWatched", _title.Name);
                    t.PropertyChanged -= Transport_PropertyChanged;
                }
            }
        }

        MovieItem _title;
    }

}