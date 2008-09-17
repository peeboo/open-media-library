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

        private void updateTranscodeStatus(object obj)
        {
            OMLApplication.Current.TranscodeStatus = (string)obj;
        }

        public bool PlayMovie()
        {
            OMLApplication.Current.IsStartingTranscodingJob = true;
            Utilities.DebugLine("Starting job");
            bool retVal = false;
            int status = -1;
            OMLApplication.Current.TranscodeStatus = @"Starting Transcode Job";
            Application.DeferredInvokeOnWorkerThread(
                delegate
                {
                    status = transcode.BeginTranscodeJob(ms, out path_to_play);
                },
                delegate
                {
                    switch (status)
                    {
                        case 0:
                            Application.DeferredInvoke(updateTranscodeStatus, "Started, buffering...");
                            System.Threading.Thread.Sleep(7 * 1000);
                            OMLApplication.Current.IsStartingTranscodingJob = false;
                            System.Threading.Thread.Sleep(1000);
                            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, path_to_play, false))
                            {
                                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                                {
                                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += this.Transport_PropertyChanged;
                                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                                    retVal = true;
                                }
                            }
                            break;
                        case 1:
                            Application.DeferredInvoke(updateTranscodeStatus, "An Error Occured");
                            Utilities.DebugLine("[TranscodePlayer] And error occured");
                            break;
                        default:
                            AddInHost.Current.MediaCenterEnvironment.Dialog(string.Format("Error Code: {0}", status), "Transcode Error", DialogButtons.Ok, 0, true);
                            Utilities.DebugLine("[TranscodePlayer] Error while attempting to transcode {0}", status);
                            break;
                    }
                }, null);

            OMLApplication.Current.IsStartingTranscodingJob = false;
            return retVal;
        }
    }
}