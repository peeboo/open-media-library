using System;

using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

using OMLEngine;
using OMLTranscoder;
using OMLEngineService;

namespace Library
{
    public class TranscodePlayer : IPlayMovie
    {
        string transcodedFile = string.Empty;
        MediaSource _source;
        TranscodingAPI transcoder;

        public TranscodePlayer(MediaSource source)
        {
            _source = source;
        }

        void trancoderStatusChanged(MediaSource source, TranscodingStatus status)
        {
            Utilities.DebugLine("[TranscodePlayer] Status Changed: {0}, Status: {1}", _source, status);

            Application.DeferredInvoke(delegate
            {
                switch (status)
                {
                    case TranscodingStatus.Initializing:
                        OMLApplication.Current.IsStartingTranscodingJob = true;
                        OMLApplication.Current.TranscodeStatus = @"Starting Transcoding Job";
                        break;
                    case TranscodingStatus.BufferReady:
                        OMLApplication.Current.IsStartingTranscodingJob = true;
                        OMLApplication.Current.TranscodeStatus = @"Transcoding Buffer Ready";
                        break;
                    case TranscodingStatus.Done:
                        OMLApplication.Current.IsStartingTranscodingJob = false;
                        OMLApplication.Current.TranscodeStatus = @"Transcoding Done";
                        break;
                    case TranscodingStatus.Error:
                        OMLApplication.Current.IsStartingTranscodingJob = false;
                        OMLApplication.Current.TranscodeStatus = @"Transcoding Error";
                        break;
                    case TranscodingStatus.Stopped:
                        OMLApplication.Current.IsStartingTranscodingJob = false;
                        OMLApplication.Current.TranscodeStatus = @"Transcoding Stopped";
                        break;
                }
            });

            if (OMLApplication.Current.NowPlayingMovieName == _source.Name)
            {
                Utilities.DebugLine("[TranscodePlayer] Already started playing: {0}", _source);
                return;
            }

            if (status == TranscodingStatus.Done || status == TranscodingStatus.BufferReady)
            {
                Application.DeferredInvoke(delegate
                {
                    if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, transcodedFile, false))
                    {
                        if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                        {
                            Utilities.DebugLine("TranscodePlayer.PlayMovie: movie '{0}', Playing file '{1}'", _source.Name, _source.GetTranscodingFileName());
                            OMLApplication.Current.NowPlayingMovieName = _source.Name;
                            OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                            AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += this.Transport_PropertyChanged;
                            AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                        }
                    }
                });
            }
        }

        void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                Utilities.DebugLine("MoviePlayerTranscode.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        //transcoder.Cancel();
                        Utilities.DebugLine("MoviePlayerTranscode.Transport_PropertyChanged: movie {0} Finished", OMLApplication.Current.NowPlayingMovieName);
                        OMLApplication.Current.NowPlayingStatus = PlayState.Finished;
                    }
                }
            });
        }

        public bool PlayMovie()
        {
            OMLApplication.Current.IsStartingTranscodingJob = true;
            Utilities.DebugLine("Starting job");
            transcoder = new TranscodingAPI(_source, trancoderStatusChanged);
            transcodedFile = _source.GetTranscodingFileName();
            transcoder.Transcode();
            return true;
        }
    }
}