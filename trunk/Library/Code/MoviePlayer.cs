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
            OMLApplication.DebugLine("[MoviePlayerFactory] Determing MoviePlayer to use for: {0}, {1}",
                                     movieItem.SelectedDisk.Path,
                                     movieItem.SelectedDisk.Format);
            if (File.Exists(movieItem.SelectedDisk.Path) || Directory.Exists(movieItem.SelectedDisk.Path))
            {
                
                if (movieItem.SelectedDisk.Format == VideoFormat.WPL) // if its a playlist, do that first
                {
                    OMLApplication.DebugLine("WPLMoviePlayer created");
                    return new MoviePlayerWPL(movieItem);
                }
                else if (NeedsMounting(movieItem.SelectedDisk)) // if it needs to be mounted, do that next
                {
                    OMLApplication.DebugLine("MountImageMoviePlayer created");
                    return new MountImagePlayer(movieItem);
                }
                else if (OMLApplication.Current.IsExtender && NeedsTranscode(movieItem.SelectedDisk) ) // if it needs to be transcoded
                {
                    OMLApplication.DebugLine("TranscodePlayer created");
                    return new TranscodePlayer(movieItem);
                }
                else if (movieItem.SelectedDisk.Format == VideoFormat.DVD) // play the dvd
                {
                    OMLApplication.DebugLine("DVDMoviePlayer created");
                    return new DVDPlayer(movieItem);
                }
                else // try to play it (likely is avi/mkv/etc)
                {
                    OMLApplication.DebugLine("VideoPlayer created");
                    return new VideoPlayer(movieItem);
                }
            }
            else
            {
                OMLApplication.DebugLine("UnavailableMoviePlayer created");
                return new UnavailableMoviePlayer(movieItem);
            }
        }


        static public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            MediaTransport t = (MediaTransport)sender;
            Utilities.DebugLine("MoviePlayerFactory.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
            if (property == "PlayState")
            {
                OMLApplication.Current.NowPlayingStatus = t.PlayState;
                Utilities.DebugLine("MoviePlayerFactory.Transport_PropertyChanged: movie {0} state {1}", OMLApplication.Current.NowPlayingMovieName, t.PlayState.ToString());
            }
        }


        // keep all the Playing logic here
        static private bool NeedsMounting(Disk SelectedDisk)
        {
            switch (SelectedDisk.Format)
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
        static private bool NeedsTranscode(Disk SelectedDisk)
        {
            switch (SelectedDisk.Format)
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


