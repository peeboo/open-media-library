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
using OMLEngine.Settings;

namespace Library
{
    public class ExternalPlayerItem
    {
        public string Path;
        public ExternalPlayer.KnownPlayers PlayerType;
    }

    /// <summary>
    /// Allows the user to specify a player outside of VMC for playing their video files
    /// </summary>
    public class ExternalPlayer : IPlayMovie
    {
        public enum KnownPlayers
        {
            None,
            TotalMediaTheater,
            PowerDVD8,
            WinDVD9,
            Other,
        }

        private static Dictionary<VideoFormat, ExternalPlayerItem> _typeToExternalPlayer = null;
        private static object _lockObject = new object();
        private string _mediaPath;
        private VideoFormat _videoFormat;

        private static Dictionary<VideoFormat, ExternalPlayerItem> TypeToExternalPlayer
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
                            if (OMLSettings.ExternalPlayerMapping != null)
                            {
                                _typeToExternalPlayer =
                                    new Dictionary<VideoFormat, ExternalPlayerItem>(OMLSettings.ExternalPlayerMapping.Count);

                                foreach (string pair in OMLSettings.ExternalPlayerMapping)
                                {
                                    string[] parts = pair.Split('|');

                                    if (parts.Length != 3)
                                    {
                                        OMLApplication.DebugLine("[ExternalPlayer] External player string in incorrect format " + pair);
                                        continue;
                                    }

                                    VideoFormat format;
                                    KnownPlayers player;

                                    string type = parts[0];
                                    string playerString = parts[1];
                                    string path = parts[2];

                                    try
                                    {
                                        format = (VideoFormat) Enum.Parse(typeof(VideoFormat), type, true);

                                        int playerInt;

                                        if (!int.TryParse(playerString, out playerInt))
                                            continue;

                                        player = (KnownPlayers)playerInt;
                                    }
                                    catch
                                    {
                                        OMLApplication.DebugLine("[ExternalPlayer] Video type is not supported " + type);
                                        continue;
                                    }

                                    _typeToExternalPlayer.Add(format, new ExternalPlayerItem() { Path = path, PlayerType = player });
                                }
                            }
                            else
                                _typeToExternalPlayer = new Dictionary<VideoFormat, ExternalPlayerItem>(0);   
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
        /// Returns the path to the IFO file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetIfoPath(string path)
        {
            if (!Directory.Exists(path))
                return path;

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                path = Path.Combine(path, "VIDEO_TS");
            }

            if (File.Exists(Path.Combine(path, "video_ts.ifo")))
            {
                path = Path.Combine(path, "video_ts.ifo");
            }

            return path;
        }

        /// <summary>
        /// Returns the largest m2ts file for playing in the folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetLargestM2TSPath(string path)
        {
            if (!Directory.Exists(path))
                return path;

            if (!Directory.Exists(Path.Combine(path, "BDMV\\STREAM")))           
                return path;

            path = Path.Combine(path, "BDMV\\STREAM");

            long fileSize = 0;

            // scan for the largest
            foreach (string file in Directory.GetFiles(path, "*.m2ts"))
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Length > fileSize)
                {
                    path = file;
                    fileSize = fi.Length;
                }
            }

            return path;
        }

        /// <summary>
        /// Launches the external player
        /// </summary>
        /// <returns></returns>
        public bool PlayMovie()
        {
            ExternalPlayerItem player = GetExternalForFormat(_videoFormat);

            string path = player.Path;

            // make sure the external player really cares about this
            if (string.IsNullOrEmpty(path))
                return false;

            // validate the player exists
            if (!File.Exists(path))
                return false;

            // setup the special rules per player type
            bool useMaximizer = false;

            switch (player.PlayerType)
            {
                case KnownPlayers.PowerDVD8:
                    useMaximizer = true;

                    if (_videoFormat == VideoFormat.DVD)
                    {
                        _mediaPath = GetIfoPath(_mediaPath);
                    }
                    else if (_videoFormat == VideoFormat.BLURAY)
                    {
                        _mediaPath = GetLargestM2TSPath(_mediaPath);
                    }                   

                    break;                

                case KnownPlayers.TotalMediaTheater:
                    useMaximizer = false;

                    if (_videoFormat == VideoFormat.BLURAY &&
                            !File.Exists(Path.Combine(_mediaPath, "index.bdmv")) &&
                            File.Exists(Path.Combine(_mediaPath, "BDMV\\index.bdmv")))
                    {
                        _mediaPath = Path.Combine(_mediaPath, "BDMV");
                    }
                    else if (_videoFormat == VideoFormat.HDDVD &&
                                Directory.Exists(_mediaPath) &&
                                !_mediaPath.EndsWith("\\"))
                    {
                        _mediaPath += "\\";
                    }
                    break;

                case KnownPlayers.Other:
                    useMaximizer = true;
                    break;

                case KnownPlayers.WinDVD9:
                    useMaximizer = true;
                    break;                    
            }                        

            OMLApplication.ExecuteSafe(delegate
            {
                // if the maximizer app can be found use that
                string maximizerPath = Path.Combine(OMLEngine.FileSystemWalker.RootDirectory, "Maximizer.exe");

                if (useMaximizer && File.Exists(maximizerPath))
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
            bool keyExists = TypeToExternalPlayer.ContainsKey(format) && File.Exists(TypeToExternalPlayer[format].Path);

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
        public static ExternalPlayerItem GetExternalForFormat(VideoFormat format)
        {
            ExternalPlayerItem playerPath;

            if (!TypeToExternalPlayer.TryGetValue(format, out playerPath))
                if (!TypeToExternalPlayer.TryGetValue(VideoFormat.ALL, out playerPath))
                    return null;

            return playerPath;
        }
    }
}
