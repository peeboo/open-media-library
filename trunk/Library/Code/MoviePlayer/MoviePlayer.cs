using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using OMLEngine.FileSystem;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using OMLGetDVDInfo;
using OMLEngine.Settings;

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
            string mediaPath = null;
            VideoFormat mediaFormat = VideoFormat.UNKNOWN;

            // for now play just online titles. add offline capabilities later
            OMLApplication.DebugLine("[MoviePlayerFactory] Determing MoviePlayer to use for: {0}", source);
            try
            {
                if ((File.Exists(source.MediaPath) || Directory.Exists(source.MediaPath)) || source.Format == VideoFormat.URL)
                {
                    // if we need to be mounted - do that now so we can get the real type
                    if (NeedsMounting(source.Format))
                    {
                        mediaFormat = MountImage(source.MediaPath, out mediaPath);
                    }



                    // if we don't need mounting or the mounting failed setup the paths
                    if (mediaFormat == VideoFormat.UNKNOWN)
                    {
                        mediaFormat = source.Format;
                        mediaPath = source.MediaPath;
                    }
                    //now we need to determine the actual player!
                    if (mediaFormat == VideoFormat.URL)
                    {
                        OMLApplication.DebugLine("[MoviePlayerFactory] UrlMoviePlayer created: {0}", source);
                        return new MoviePlayerUrl(source);
                    }
                    else if (mediaFormat == VideoFormat.DVD && FileScanner.IsDVD(mediaPath)) // play the dvd
                    {
                        //if we are an extender we need to fake being MPG
                        if (OMLApplication.Current.IsExtender)
                        {
                            OMLApplication.DebugLine("[MoviePlayerFactory] ExtenderDVDPlayer created: {0}", source);
                            return new ExtenderDVDPlayer(source);
                        }
                        OMLApplication.DebugLine("[MoviePlayerFactory] DVDMoviePlayer created: {0}", source);
                        return new DVDPlayer(source, mediaPath);
                    }
                    else if (mediaFormat == VideoFormat.BLURAY && FileScanner.IsBluRay(mediaPath))
                    {
                        if (OMLApplication.Current.IsExtender)
                            throw new NotImplementedException("BLURAY playback through extenders is not supported");
                        OMLApplication.DebugLine("[MoviePlayerFactory] BluRayPlayer created: {0}", source);
                        return new BluRayPlayer(source, mediaPath);
                    }
                    else if (mediaFormat == VideoFormat.HDDVD && FileScanner.IsHDDVD(mediaPath))
                    {
                        if (OMLApplication.Current.IsExtender)
                            throw new NotImplementedException("HDDVD playback through extenders is not supported");
                        OMLApplication.DebugLine("[MoviePlayerFactory] HDDVDPlayer created: {0}", source);
                        return new HDDVDPlayer(source, mediaPath);
                    }
                    else if (IsPlayList(mediaFormat))
                    {
                        //this is for crap like wpl or wvx playlists
                        //DON't FORGET: our individual videos in the playlist 
                        //will be coming back here to the factory if we are transcoding
                        return new MoviePlayerWVX(source);
                    }
                    else // try to play it (likely is avi/mkv/etc)
                    {
                        if (OMLApplication.Current.IsExtender && NeedsTranscode(mediaFormat))
                        {
                            OMLApplication.DebugLine("[MoviePlayerFactory] TranscodePlayer created: {0}", source);
                            return new TranscodePlayer(source);
                        }
                        OMLApplication.DebugLine("[MoviePlayerFactory] VideoPlayer created: {0}", source);
                        return new VideoPlayer(source);
                    }
                }
                else
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] UnavailableMoviePlayer created-wtf did you try to play?");
                    return new UnavailableMoviePlayer(source);
                }
            }
            catch (Exception ex)
            {
                //ouch-you trying to play that blu-ray on your extender or what?
                OMLApplication.DebugLine("[MoviePlayerFactory] UnavailableMoviePlayer created-wtf did you try to play? {0}", ex.Message);
                return new UnavailableMoviePlayer(source);
            }
        }

        private static bool IsPlayList(VideoFormat mediaFormat)
        {
            if (mediaFormat == VideoFormat.WPL || mediaFormat == VideoFormat.WVX)
                return true;
            return false;
        }

        /// <summary>
        /// Mounts an image and returns it's path and format
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mountedPath"></param>
        /// <returns></returns>
        private static VideoFormat MountImage(string path, out string mountedPath)
        {
            VideoFormat videoFormat = VideoFormat.UNKNOWN;

            MountingTool mounter = new MountingTool();

            if (mounter.Mount(path, out mountedPath))
            {
                mountedPath += ":\\";

                // now that we've mounted it let's see what it is
                videoFormat = (FileScanner.IsDVD(mountedPath))
                                   ? VideoFormat.DVD
                                   : (FileScanner.IsBluRay(mountedPath))
                                        ? VideoFormat.BLURAY
                                        : (FileScanner.IsHDDVD(mountedPath))
                                            ? VideoFormat.HDDVD
                                            : VideoFormat.UNKNOWN;

            }
            else
            {
                mountedPath = null;
            }

            return videoFormat;
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

                    //if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    //{
                    //    if (AddInHost.Current.ApplicationContext.IsForegroundApplication && AddInHost.Current.MediaCenterEnvironment.MediaExperience.IsFullScreen)
                    //    {
                    //        AddInHost.Current.ApplicationContext.ReturnToApplication();
                    //        OMLApplication.DebugLine("[MoviePlayer] Playstate is stopped, moving to previous page");
                    //        //OMLApplication.Current.Session.BackPage();
                    //    }
                    //}
                }
            });
        }


        // keep all the Playing logic here
        static bool IsExtenderDVD_NoTranscoding(MediaSource source)
        {
            if (OMLApplication.Current.IsExtender == false || source.Format != VideoFormat.DVD
                || FileScanner.IsDVD(source.MediaPath) == false || ExtenderDVDPlayer.CanPlay(source) == false)
                return false;

            // non-default audio/subtitle/chapter start: needs transcoding
            if (source.AudioStream != null || source.Subtitle != null || source.StartChapter != null)
            {
                Utilities.DebugLine("Source has custom audio/subtitle/startchapter: {0}", source);
                return false;
            }
            if (source.DVDDiskInfo == null)
            {
                Utilities.DebugLine("Source has no DVDDiskInfo: {0}", source);
                return true;
            }

            string languageCode = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            DVDTitle title = source.Title != null ? source.DVDDiskInfo.Titles[source.Title.Value] : source.DVDDiskInfo.GetMainTitle();
            if (title.AudioTracks.Count != 0 && string.Compare(title.AudioTracks[0].LanguageID, languageCode, true) != 0)
            {
                DVDSubtitle st = title.Subtitles.Count != 0 ? title.FindSubTitle(languageCode) : null;
                if (st != null)
                {
                    source.Subtitle = new SubtitleStream(st);
                    Utilities.DebugLine("Autoselect subtitle, since default audio stream is not native: {0}", source);
                    return false;
                }
            }

            return true;
        }

        static bool NeedsMounting(VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.BIN:
                case VideoFormat.CUE:
                case VideoFormat.IMG:
                case VideoFormat.ISO:
                case VideoFormat.MDF:
                case VideoFormat.NRG:
                    return true;
                default:
                    return false;
            }
        }

        static bool NeedsTranscode(VideoFormat videoFormat)
        {
            //we don't transcode shit if we are running on win7
            if (OMLApplication.IsWindows7)
                return false;
            else
            {
                //this used to determine if we should transcode avi/mkv and ogm-
                //do we really care since these won't play on an extender
                switch (videoFormat)
                {
                    case VideoFormat.DVRMS:
                        return false;
                    case VideoFormat.MPEG:
                        return false;
                    case VideoFormat.VOB:
                        return false;
                    case VideoFormat.MPG:
                        return false;
                    case VideoFormat.WMV:
                        return false;
                    case VideoFormat.WTV:
                        return false;
                    case VideoFormat.ASX:
                        return false;
                    case VideoFormat.WVX:
                        return false;
                    case VideoFormat.ASF:
                        return false;
                    case VideoFormat.H264:
                        return false;
                    case VideoFormat.MOV:
                        return false;
                    case VideoFormat.WPL:
                        return false;
                    case VideoFormat.UNKNOWN:
                        return false;
                    default:
                        return true;
                }
            }
        }
    }
}


