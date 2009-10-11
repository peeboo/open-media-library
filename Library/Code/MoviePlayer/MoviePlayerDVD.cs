using System;
using OMLEngine;
using OMLEngine.FileSystem;
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
        MediaSource _source;
        string _mediaPath;

        public DVDPlayer(MediaSource source, string path)
        {
            _source = source;
            _mediaPath = path;
        }

        private bool PlayMovie(string playString)
        {            
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Dvd, playString, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    OMLApplication.Current.NowPlayingMovieName = _source.Name;
                    OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                    //AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged -= MoviePlayerFactory.Transport_PropertyChanged;
                    //AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            return false;            
        }

        public bool PlayMovie()
        {
            if (FileScanner.IsDVD(_mediaPath))
            {
                bool isUNC = false;
                string path = FileScanner.GetPlayStringForPath(_mediaPath);

                // the unc path requires that it start with // so remove \\ if it exists
                //http://discuss.mediacentersandbox.com/forums/thread/9307.aspx
                if (path.StartsWith("\\\\"))
                {
                    path = path.Substring(2);
                    isUNC = true;
                }

                path = path.Replace("\\", "/");

                if(OMLApplication.IsWindows7 && isUNC)
                    path = string.Format("//{0}", path);
                else
                    path = string.Format("DVD://{0}", path);

                OMLApplication.DebugLine("[MoviePlayerDVD] Actual play string being passed to PlayMovie: {0}", path);
                return PlayMovie(path);
            }
            return false;
        }

        public bool PlayMovie(int titleNumber, int chapterNumber, DateTime startTime)
        {
            return FileScanner.IsDVD(_source.MediaPath) == false;
        }

        public static string GeneratePlayString(string path, int titleNumber, int chapterNumber)
        {
            string playString = string.Empty;
            playString = string.Format("DVD://{0}", FileScanner.GetPlayStringForPath(path));
            playString = playString.Replace("\\", "/");

            if (titleNumber > 0)
            {
                if (chapterNumber > 0)
                    playString += string.Format("?{0}/{1}", titleNumber, chapterNumber);
                else
                    playString += string.Format("?{0}", titleNumber);
            }
            return playString;
        }

        public static string GeneratePlayString(string path, int titleNumber, DateTime startTime)
        {
            string playString = string.Empty;
            playString = string.Format("DVD://{0}", FileScanner.GetPlayStringForPath(path));
            playString = playString.Replace("\\", "/");

            if (titleNumber < 1)
                titleNumber = 1;

            playString += string.Format("?{0}/{1}:{2}:{3}",
                                        titleNumber, startTime.Hour,
                                        startTime.Minute, startTime.Second);

            return playString;
        }
    }

}
