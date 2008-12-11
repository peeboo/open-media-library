using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using OMLGetDVDInfo;

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

                if (!OMLApplication.Current.IsExtender &&
                    ExternalPlayer.ExternalPlayerExistForType(source.Format))
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] ExternalPlayer created: {0}", source);
                    return new ExternalPlayer(source);
                }
                else if (source.Format == VideoFormat.WPL) // if its a playlist, do that first
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] WPLMoviePlayer created: {0}", source);
                    return new MoviePlayerWPL(source);
                }
                else if (NeedsMounting(source.Format)) // if it needs to be mounted, do that next
                {
                    OMLApplication.DebugLine("[MoviePlayerFactory] MountImageMoviePlayer created: {0}", source);
                    return new MountImagePlayer(source);
                }
                else if (IsExtenderDVD_NoTranscoding(source)) // play the dvd
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
        static bool IsExtenderDVD_NoTranscoding(MediaSource source)
        {
            if (OMLApplication.Current.IsExtender == false || source.Format != VideoFormat.DVD 
                || MediaData.IsDVD(source.MediaPath) == false || ExtenderDVDPlayer.IsNTFS(source.MediaPath) == false)
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
                    return true;
                default:
                    return false;
            }
        }

        static bool NeedsTranscode(VideoFormat videoFormat)
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


