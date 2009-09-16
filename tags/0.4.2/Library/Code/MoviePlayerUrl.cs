using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using FileDownloader;
using System.Threading;

namespace Library {
    public class MoviePlayerUrl :IPlayMovie {
        MediaSource _source;

        public MoviePlayerUrl(MediaSource source) {
            _source = source;
        }

        public bool PlayMovie() {
            bool wasItgood = false;
            DownloadEngine downloader = new DownloadEngine(_source.MediaPath);
            downloader.UserAgent = OMLEngine.Trailers.AppleTrailers.QUICKTIME_USER_AGENT_STRING;
            downloader.Log += new DownloadEngine.DownloadEventsHandler(reportDownloadStatus);

            Application.DeferredInvokeOnWorkerThread(delegate {
                OMLApplication.Current.IsBusy = true;
                downloader.Download();
            },
            delegate {
                OMLApplication.Current.IsBusy = false;
                if (!string.IsNullOrEmpty(downloader.DownloadedFile)) {
                    if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, downloader.DownloadedFile, false)) {
                        Utilities.DebugLine("UrlPlayer.PlayMovie: movie {0} Playing", _source);
                        OMLApplication.Current.NowPlayingMovieName = _source.Name;
                        OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged -= MoviePlayerFactory.Transport_PropertyChanged;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                        wasItgood = true;
                    }
                }
            },
            null);

            return wasItgood;
        }

        void reportDownloadStatus(string status) {
            //OMLApplication.Current.IsBusy = true;
        }
    }
}
