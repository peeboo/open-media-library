using System;

using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

using OMLEngine;
using OMLTranscoder;

namespace Library
{
    public class TranscodePlayer : IPlayMovie
    {
        string path_to_play = string.Empty;
        MediaSource _source;
        Transcode transcode;

        public TranscodePlayer(MediaSource source)
        {
            _source = source;
            transcode = new Transcode(_source);
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
//                      stopTranscode();
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
                    status = transcode.BeginTranscodeJob();
                    path_to_play = this._source.GetTranscodingFileName(FileSystemWalker.TranscodeBufferDirectory);
                },
                delegate
                {
                    switch (status)
                    {
                        case 0:
                            // TheSeeker: Application.DeferredInvoke is not needed, since this delegate is executed on the main UI thread
                            Application.DeferredInvoke(updateTranscodeStatus, "Started, buffering...");
                            // TheSeeker: this is blocking the main UI thread, maybe not the best synchronization method
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
            // TODO: note this will always be false, since Application.DeferredInvokeOnWorkerThread returns immediatly, and the delegates will executed
            // asynchron on a background thread
            // TheSeeker: I think this needs to be redesigned a bit..., I will start writing test cases and such
            return retVal;
        }
    }
}