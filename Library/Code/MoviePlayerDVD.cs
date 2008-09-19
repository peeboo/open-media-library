using System;
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
        MediaSource _source;

        public DVDPlayer(MediaSource source)
        {
            _source = source;
        }

        public bool PlayMovie(string playString)
        {
            if (MediaData.IsDVD(_source.MediaPath))
            {
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Dvd, playString, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        OMLApplication.Current.NowPlayingMovieName = _source.Name;
                        OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool PlayMovie()
        {
            if (MediaData.IsDVD(_source.MediaPath))
            {
                return PlayMovie("DVD://" + MediaData.GetPlayStringForPath(_source.MediaPath));
            }
            return false;
        }

        public bool PlayMovie(int titleNumber, int chapterNumber, DateTime startTime)
        {
            return MediaData.IsDVD(_source.MediaPath) == false;
        }

        public static string GeneratePlayString(string path, int titleNumber, int chapterNumber)
        {
            string playString = string.Empty;
            playString = string.Format("DVD://{0}", MediaData.GetPlayStringForPath(path));
            playString = playString.Replace('\\', '/');

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
            playString = string.Format("DVD://{0}", MediaData.GetPlayStringForPath(path));
            playString = playString.Replace('\\', '/');

            if (titleNumber < 1)
                titleNumber = 1;

            playString += string.Format("?{0}/{1}:{2}:{3}",
                                        titleNumber, startTime.Hour,
                                        startTime.Minute, startTime.Second);

            return playString;
        }
    }

}