﻿using System;
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
        private string _mediaPath;
        private VideoFormat _videoFormat;

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
        public ExternalPlayer(string mediaPath, VideoFormat videoFormat)
        {
            _videoFormat = videoFormat;
            _mediaPath = mediaPath;
        }

        /// <summary>
        /// Launches the external player
        /// </summary>
        /// <returns></returns>
        public bool PlayMovie()
        {
            string path = GetExternalPlayerPath(_videoFormat);

            // make sure the external player really cares about this
            if (string.IsNullOrEmpty(path))
                return false;

            // validate the player exists
            if (!File.Exists(path))
                return false;
            
            // solomon : apparently some external players want you to point to the \BDMV path
            // this may not work for all players so i'm adding this in now and if it becomes
            // a problem we'll need to add it as config switch down the road
            if (_videoFormat == VideoFormat.BLURAY &&
                !File.Exists(Path.Combine(_mediaPath, "index.bdmv")) &&
                File.Exists(Path.Combine(_mediaPath, "BDMV\\index.bdmv")))
            {
                _mediaPath = Path.Combine(_mediaPath, "BDMV");
            }
            // TMT is picky and will only play a ripped HDDVD if it ends in a backslash when
            // pointing to a folder
            else if (_videoFormat == VideoFormat.HDDVD &&
                Directory.Exists(_mediaPath) &&
                !_mediaPath.EndsWith("\\"))
            {
                _mediaPath += "\\";
            }

            // if the maximizer app can be found use that
            string maximizerPath = Path.Combine(OMLEngine.FileSystemWalker.RootDirectory, "Maximizer.exe");

            OMLApplication.ExecuteSafe(delegate
            {
                if (File.Exists(maximizerPath))
                {
                    OMLApplication.DebugLine("Calling Maximizer application \"" + path + "\" \"" + _mediaPath + "\"");
                    Process process = new Process();
                    process.StartInfo.FileName = maximizerPath;
                    process.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", path, _mediaPath);
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                }
                else
                {
                    OMLApplication.DebugLine("Calling external application \"" + path + "\" \"" + _mediaPath + "\"");
                    Process process = new Process();
                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = "\"" + _mediaPath + "\"";
                    process.Start();
                }
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
            bool keyExists = TypeToExternalPlayer.ContainsKey(format) && File.Exists(TypeToExternalPlayer[format]);

            // if we didn't find the type - see if the all override was used
            if (!keyExists)
                keyExists = TypeToExternalPlayer.ContainsKey(VideoFormat.ALL);

            return keyExists;
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
            string playerPath;

            if (!TypeToExternalPlayer.TryGetValue(format, out playerPath))
                if (!TypeToExternalPlayer.TryGetValue(VideoFormat.ALL, out playerPath))
                    playerPath = string.Empty;

            return playerPath;
        }
    }
}