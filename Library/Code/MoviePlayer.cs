using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;

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
        static public IPlayMovie CreateMoviePlayer(MovieItem movieItem)
        {
            // for now play just online titles. add offline capabilities later
            if (File.Exists(movieItem.FileLocation) || Directory.Exists(movieItem.FileLocation))
            {
                if (OMLApplication.Current.IsExtender && NeedsTranscode(movieItem.TitleObject) )
                {
                    Trace.WriteLine("TranscodePlayer created");
                    return new TranscodePlayer(movieItem);
                }
                else if (NeedsMounting(movieItem.TitleObject))
                {
                    Trace.WriteLine("MountImageMoviePlayer created");
                    return new MountImagePlayer(movieItem);
                }
                else if (movieItem.TitleObject.VideoFormat == VideoFormat.DVD)
                {
                    Trace.WriteLine("DVDMoviePlayer created");
                    return new DVDPlayer(movieItem);
                }
                else if (movieItem.TitleObject.VideoFormat == VideoFormat.WPL)
                {
                    Trace.WriteLine("WPLMoviePlayer created");
                    return new MoviePlayerWPL(movieItem);
                }
                else
                {
                    Trace.WriteLine("VideoPlayer created");
                    return new VideoPlayer(movieItem);
                }
            }
            else
            {
                Trace.WriteLine("UnavailableMoviePlayer created");
                return new UnavailableMoviePlayer(movieItem);
            }
        }

        // keep all the Playing logic here
        static private bool NeedsMounting( Title title )
        {
            switch (title.VideoFormat)
            {
                case VideoFormat.BIN:
                    return true;
                case VideoFormat.CUE:
                    return true;
                case VideoFormat.IMG:
                    return true;
                case VideoFormat.ISO:
                    return true;
                case VideoFormat.MDF:
                    return true;
                default:
                    return false;
            }
        }

        // keep all the Playing logic here
        static private bool NeedsTranscode( Title title)
        {
            switch (title.VideoFormat)
            {
                case VideoFormat.DVRMS:
                    return false;
                case VideoFormat.MPEG:
                    return false;
                case VideoFormat.MPG:
                    return false;
                case VideoFormat.WMV:
                    return false;
                default:
                    return true;
            }
        }
    }
}

