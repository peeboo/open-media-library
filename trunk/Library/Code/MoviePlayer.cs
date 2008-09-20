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
        static public IPlayMovie CreateMoviePlayer(MediaSource source)
        {
            // for now play just online titles. add offline capabilities later
            OMLApplication.DebugLine("[MoviePlayerFactory] Determing MoviePlayer to use for: {0}", source);
            if (File.Exists(source.MediaPath) || Directory.Exists(source.MediaPath))
            {
                //OMLApplication.Current.IsStartingTranscodingJob = true;
                //OMLApplication.DebugLine("[MoviePlayerFactory] TranscodePlayer created: {0}", source);
                //return new TranscodePlayer(source);

                if (source.Format == VideoFormat.WPL) // if its a playlist, do that first
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] WPLMoviePlayer created: {0}", source);
                    return new MoviePlayerWPL(source);
                }
                else if (NeedsMounting(source.Format)) // if it needs to be mounted, do that next
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] MountImageMoviePlayer created: {0}", source);
                    return new MountImagePlayer(source);
                }
                else if (OMLApplication.Current.IsExtender && source.Format == VideoFormat.DVD && MediaData.IsDVD(source.MediaPath)) // play the dvd
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] ExtenderDVDPlayer created: {0}", source);
                    return new ExtenderDVDPlayer(source);
                }
                else if (OMLApplication.Current.IsExtender && source.Format == VideoFormat.BLURAY && MediaData.IsBluRay(source.MediaPath))
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] ExtenderBlurayPlayer created: {0}", source);
                    return new TranscodeBluRayPlayer(source);
                }
                else if (OMLApplication.Current.IsExtender && source.Format == VideoFormat.HDDVD && MediaData.IsHDDVD(source.MediaPath))
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] ExtenderHDDVDPlayer created: {0}", source);
                    return new TranscodeHDDVDPlayer(source);
                }
                else if (OMLApplication.Current.IsExtender && NeedsTranscode(source.Format)) // if it needs to be transcoded
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] TranscodePlayer created: {0}", source);
                    return new TranscodePlayer(source);
                }
                else if (source.Format == VideoFormat.DVD && MediaData.IsDVD(source.MediaPath)) // play the dvd
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] DVDMoviePlayer created: {0}", source);
                    return new DVDPlayer(source);
                }
                else if (source.Format == VideoFormat.BLURAY && MediaData.IsBluRay(source.MediaPath))
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] BluRayPlayer created: {0}", source);
                    return new BluRayPlayer(source);
                }
                else if (source.Format == VideoFormat.HDDVD && MediaData.IsHDDVD(source.MediaPath))
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] HDDVDPlayer created: {0}", source);
                    return new HDDVDPlayer(source);
                }
//                else if (source.Format == VideoFormat.FOLDER)
//                {
//                    OMLApplication.DebugLine("[MoviePlayerFactory] FolderPlayer created: {0}", source);
//                    return new FolderPlayer(movieItem);
//                }
                else // try to play it (likely is avi/mkv/etc)
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] VideoPlayer created: {0}", source);
                    return new VideoPlayer(source);
                }
            }
            else
            {
                OMLApplication.DebugLine("[MoviePlayerFactory] UnavailableMoviePlayer created");
                return new UnavailableMoviePlayer(source);
            }
        }


        static public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                Utilities.DebugLine("[MoviePlayerFactory] Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    OMLApplication.Current.NowPlayingStatus = t.PlayState;
                    Utilities.DebugLine("[MoviePlayerFactory] MoviePlayerFactory.Transport_PropertyChanged: movie {0} state {1}", OMLApplication.Current.NowPlayingMovieName, t.PlayState.ToString());

                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        OMLApplication.DebugLine("[MoviePlayer] Playstate is stopped, moving to previous page");
                        OMLApplication.Current.Session.BackPage();
                    }
                }
            });
        }


        // keep all the Playing logic here
        static private bool NeedsMounting(VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.BIN:
                case VideoFormat.CUE:
                case VideoFormat.IMG:
                case VideoFormat.ISO:
                case VideoFormat.MDF:
                    return true;
                default:
                    return false;
            }
        }

        // keep all the Playing logic here
        static private bool NeedsTranscode(VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.DVRMS:
                case VideoFormat.MPEG:
                case VideoFormat.MPG:
                case VideoFormat.WMV:
                case VideoFormat.WTV:
                case VideoFormat.ASX:
                    return false;
                default:
                    return true;
            }
        }
    }
}


