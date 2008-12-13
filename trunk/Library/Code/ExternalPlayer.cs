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
    /// Allows the user to specify a player outside of VMC for playing their video files
    /// </summary>
    public class ExternalPlayer : IPlayMovie
    {
        private static Dictionary<VideoFormat, string> _typeToExternalPlayer = null;
        private static object _lockObject = new object();

        private MediaSource _source;

        private static Dictionary<VideoFormat, string> TypeToExternalPlayer
        {
            get
            {
                if (_typeToExternalPlayer == null)
                {
                    lock (_lockObject)
                    {
                        if (_typeToExternalPlayer == null)
                        {
                            // generate dictionary
                            if (Properties.Settings.Default.ExternalPlayerMapping != null)
                            {
                                _typeToExternalPlayer = 
                                    new Dictionary<VideoFormat, string>(Properties.Settings.Default.ExternalPlayerMapping.Count);

                                foreach (string pair in Properties.Settings.Default.ExternalPlayerMapping)
                                {
                                    int seperatorIndex = pair.IndexOf('|');
                                    if (seperatorIndex == -1 || pair.Length < seperatorIndex + 2)
                                    {
                                        OMLApplication.DebugLine("[ExternalPlayer] External player string in incorrect format " + pair);
                                        continue;
                                    }

                                    VideoFormat format;
                                    string type = pair.Substring(0, seperatorIndex);
                                    string path = pair.Substring(seperatorIndex + 1);

                                    try
                                    {
                                        format = (VideoFormat) Enum.Parse(typeof(VideoFormat), type, true);
                                    }
                                    catch
                                    {
                                        OMLApplication.DebugLine("[ExternalPlayer] Video type is not supported " + type);
                                        continue;
                                    }

                                    _typeToExternalPlayer.Add(format, path);
                                }
                            }
                            else
                                _typeToExternalPlayer = new Dictionary<VideoFormat, string>(0);   
                        }
                    }
                }

                return _typeToExternalPlayer;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        public ExternalPlayer(MediaSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Launches the external player
        /// </summary>
        /// <returns></returns>
        public bool PlayMovie()
        {            
            string path;
            string moviePath = _source.MediaPath;

            // make sure the external player really cares about this
            if (!TypeToExternalPlayer.TryGetValue(_source.Format, out path))
                return false;

            // validate the player exists
            if (!File.Exists(path))
                return false;
            
            // solomon : apparently some external players want you to point to the \BDMV path
            // this may not work for all players so i'm adding this in now and if it becomes
            // a problem we'll need to add it as config switch down the road
            if (_source.Format == VideoFormat.BLURAY &&
                !File.Exists( Path.Combine(_source.MediaPath, "index.bdmv")) &&
                File.Exists(Path.Combine(_source.MediaPath, "BDMV\\index.bdmv")))
            {    
                moviePath = Path.Combine(moviePath, "BDMV");
            }

            OMLApplication.ExecuteSafe(delegate
            {
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = "\"" + moviePath + "\"";
                process.Start();
            });
                        
            return true;
        }

        /// <summary>
        /// Will return if the file type is registered with an external player and if the external player is found
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool ExternalPlayerExistForType(VideoFormat format)
        {
            return TypeToExternalPlayer.ContainsKey(format) && File.Exists(TypeToExternalPlayer[format]);
        }

        /// <summary>
        /// Refreshes the cached dictionary of external players
        /// </summary>
        public static void RefreshExternalPlayerList()
        {
            _typeToExternalPlayer = null;
        }

        /// <summary>
        /// Returns the external player path for the given type - will return empty string if it doesn't exist
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetExternalPlayerPath(VideoFormat format)
        {
            return (TypeToExternalPlayer.ContainsKey(format))
                    ? TypeToExternalPlayer[format]
                    : string.Empty;
        }
    }
}
