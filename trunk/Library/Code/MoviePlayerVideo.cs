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
    /// VideoPlayer class for playing standard videos (AVIs, etc)
    /// </summary>
    public class VideoPlayer : IPlayMovie
    {
        public VideoPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, _title.FileLocation, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    OMLApplication.Current.NowPlaying = "Playing: " + _title.Name;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += new PropertyChangedEventHandler(Transport_PropertyChanged);
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
            Utilities.DebugLine("VideoPlayer.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", _title.Name, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
            if (property == "PlayState")
            {
                if (t.PlayState == PlayState.Paused)
                    OMLApplication.Current.NowPlaying = "Paused: " + _title.Name;
                else if (t.PlayState == PlayState.Playing)
                    OMLApplication.Current.NowPlaying = "Playing: " + _title.Name;
                else if (t.PlayState == PlayState.Finished)
                    OMLApplication.Current.NowPlaying = "Finished: " + _title.Name;
                else if (t.PlayState == PlayState.Stopped)
                    OMLApplication.Current.NowPlaying = "Stopped: " + _title.Name;

            }
        }


        MovieItem _title;
    }
}