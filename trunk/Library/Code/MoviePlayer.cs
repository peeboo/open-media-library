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
    /// an interface required for movie players
    /// </summary>
    public interface IPlayMovie
    {
        /// <summary>
        /// Plays a movie.
        /// </summary>
        /// <returns></returns>
        bool PlayMovie();
    }

    /// <summary>
    /// A factory class to create the movie player based on file type
    /// </summary>
    public class MoviePlayerFactory
    {
        /// <summary>
        /// Creates the movie player based on the the video formatin in the Title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        static public IPlayMovie CreateMoviePlayer(AddInHost host, MovieItem title)
        {
            // for now play just online titles. add offline capabilities soon
            if (File.Exists(title.FileLocation) || Directory.Exists(title.FileLocation))
            {
                if (title.TitleObject.VideoFormat == VideoFormat.DVD)
                {
                    return new DVDPlayer(host, title);
                }
                else if (title.TitleObject.NeedToMountBeforePlaying())
                {
                    return new MountImagePlayer(host, title);
                }
                else
                {
                    return new VideoPlayer(host, title);
                }
            }
            else
            {
                return new UnavailableMoviePlayer(host, title);
            }
        }
    }

    /// <summary>
    /// DVDPlayer class for playing a DVD
    /// </summary>
    public class DVDPlayer : IPlayMovie
    {
        public DVDPlayer(AddInHost host, MovieItem title)
        {
            _title = title;
            _host = host;
        }

        public bool PlayMovie()
        {
            if (_host != null)
            {
                string media = "DVD://" + _title.FileLocation;
                media.Replace('\\', '/');
                _host.MediaCenterEnvironment.PlayMedia(MediaType.Dvd, media, false);
                if (_host.MediaCenterEnvironment.MediaExperience != null)
                {
                    _host.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        MovieItem _title;
        AddInHost _host;
    }

    /// <summary>
    /// VideoPlayer class for playing standard videos (AVIs, etc)
    /// </summary>
    public class VideoPlayer : IPlayMovie
    {
        public VideoPlayer(AddInHost host, MovieItem title)
        {
            _title = title;
            _host = host;
        }

        public bool PlayMovie()
        {
            if (_host != null)
            {
                return _host.MediaCenterEnvironment.PlayMedia(MediaType.Video, _title.FileLocation, false);
                if (_host.MediaCenterEnvironment.MediaExperience != null)
                {
                    _host.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
            }
            else
            {
                return false;
            }
        }

        AddInHost _host;
        MovieItem _title;
    }

    public class UnavailableMoviePlayer : IPlayMovie
    {
        public UnavailableMoviePlayer(AddInHost host, MovieItem title)
        {
            _title = title;
            _host = host;
        }

        public bool PlayMovie()
        {
            // show a popup or a page explaining the error
            //return false;
            if (_host != null)
            {
                _host.MediaCenterEnvironment.Dialog("Could not find file [" + _title.FileLocation + "] or don't know how to play this file type", "Error", DialogButtons.Ok, 0, true);
            }
            return false;

        }

        AddInHost _host;
        MovieItem _title;
    }

    public class MountImagePlayer : IPlayMovie
    {
        public MountImagePlayer(AddInHost host, MovieItem title)
        {
            _title = title;
            _host = host;
        }

        public bool PlayMovie()
        {
            // mount the file, then figure out which other player we want to create
            return false;
        }

        AddInHost _host;
        MovieItem _title;
    }

}
