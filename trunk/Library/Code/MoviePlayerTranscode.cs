using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using Transcode360.Interface;
using Microsoft.Win32;
using OMLTranscoder;

namespace Library
{
    public class TranscodePlayer : IPlayMovie
    {
        string path_to_play = string.Empty;
        MovieItem _title;
        Transcode transcode;
        MediaSource ms;

        public TranscodePlayer(MovieItem title)
        {
            _title = title;
            transcode = new Transcode();
            ms = transcode.MediaSourceFromTitle(_title.TitleObject);
        }

        public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                Utilities.DebugLine("MoviePlayerTranscode.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        Utilities.DebugLine("MoviePlayerTranscode.Transport_PropertyChanged: movie {0} Finished", OMLApplication.Current.NowPlayingMovieName);
                        OMLApplication.Current.NowPlayingStatus = PlayState.Finished;
                        //                    stopTranscode();
                    }
                }
            });
        }

        public bool PlayMovie()
        {
            Utilities.DebugLine("Starting job");
            path_to_play = transcode.BeginTranscodeJob(ms);
            //OMLApplication.Current.IsStartingTranscodingJob = false;

            Utilities.DebugLine("[TranscodePlayer] Playing transcode buffer: " + path_to_play);
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, path_to_play, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += this.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                    return true;
                }
            }
            return false;
        }
    }
}