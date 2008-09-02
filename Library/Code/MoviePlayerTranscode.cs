using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using Transcode360.Interface;
using Microsoft.Win32;

namespace Library
{
    public class TranscodePlayer : IPlayMovie
    {
        string path_to_buffer = string.Empty;
        //long timeStamp = 0L;
        MovieItem _title;

        public TranscodePlayer(MovieItem title)
        {
            _title = title;
        }

        public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                Utilities.DebugLine("MoviePlayerTranscode.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        Utilities.DebugLine("MoviePlayerTranscode.Transport_PropertyChanged: movie {0} Finished", OMLApplication.Current.NowPlayingMovieName);
                        OMLApplication.Current.NowPlayingStatus = PlayState.Finished;
                        //                    stopTranscode();
                    }
                }
            });
        }

        void PlayDirect(string logMsg, TcpClientChannel channel)
        {
            Utilities.DebugLine("[TranscodePlayer] " + logMsg);
            Utilities.DebugLine("Playing transcode buffer: " + path_to_buffer);
            if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, path_to_buffer, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += this.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
            }
            Utilities.DebugLine("[TranscodePlayer] Unregistering channel");
            ChannelServices.UnregisterChannel(channel);
        }

        public bool PlayMovie()
        {
            long timeStamp = 0L;
            string path_to_buffer = string.Empty;
            RegistryKey transcoderKey;
            ITranscode360 transcode;
            TcpClientChannel channel;

            try
            {
                if (File.Exists(_title.SelectedDisk.Path) | Directory.Exists(_title.SelectedDisk.Path))
                {
                    try
                    {
                        Utilities.DebugLine("[TrancodePlayer] Creating Transcode360 channel");
                        Hashtable properties = new Hashtable();
                        properties.Add("name", "");
                        channel = new TcpClientChannel(properties, null);
                        ChannelServices.RegisterChannel(channel, false);
                    }
                    catch (Exception e)
                    {
                        AddInHost.Current.MediaCenterEnvironment.Dialog("Transcode360 is either not installed or not running",
                            "Transcode Error", DialogButtons.Ok, 5, true);
                        Utilities.DebugLine("[TranscodePlayer] Failed to create Transcode360 channel: " + e.Message);
                        return false;
                    }

                    transcoderKey = Registry.LocalMachine.OpenSubKey(@"Software\Transcode360");
                    string serverAddress = (string)(transcoderKey.GetValue(@"ServerAddress"));
                    string portNumber = (string)transcoderKey.GetValue(@"InterfacePort");
                    int timeout = int.Parse((string)transcoderKey.GetValue(@"ConnectionTimeout"));
                    transcoderKey.Close();

                    transcode = (ITranscode360)Activator.GetObject(typeof(ITranscode360),
                                                                   "tcp://" + serverAddress +
                                                                   ":" + portNumber + "/RemotingServices/Transcode360");

                    if (transcode.IsMediaTranscoding(_title.SelectedDisk.Path))
                    {
                        Utilities.DebugLine("[TranscodePlayer] This item was already being transcoded, stopping transcode job");
                        transcode.StopTranscoding(_title.SelectedDisk.Path);
                    }

                    // if the item is currently being transcoded.. just play it
                    string bufferPath;
                    if (transcode.IsMediaTranscoding(_title.SelectedDisk.Path, timeStamp, out bufferPath))
                    {
                        this.path_to_buffer = bufferPath;
                        PlayDirect("Transcoding " + _title.SelectedDisk.Path, channel);
                    }
                    else // its not transcoding, start it up and play it
                    {
                        Utilities.DebugLine("[TranscodePlayer] Starting transcode job for " + _title.SelectedDisk.Path);
                        AddInHost.Current.MediaCenterEnvironment.Dialog("Beginning Transcode360 job", "Start Transcode",
                            DialogButtons.Ok, 1, false);

                        if (transcode.Transcode(_title.SelectedDisk.Path, out bufferPath, timeStamp))
                        {
                            this.path_to_buffer = bufferPath;
                            PlayDirect("Transcode job started for " + _title.SelectedDisk.Path, channel);
                        }
                        else
                        {
                            AddInHost.Current.MediaCenterEnvironment.Dialog("Failed to begin Transcode360 process", "Transcode Failure",
                                DialogButtons.Ok, 5, true);
                            Utilities.DebugLine("[TranscodePlayer] Failed to transcode " + _title.SelectedDisk.Path);
                            Utilities.DebugLine("[TranscodePlayer] Unregistering channel");
                            ChannelServices.UnregisterChannel(channel);
                            return false;
                        }
                    }
                }
                else
                {
                    AddInHost.Current.MediaCenterEnvironment.Dialog("Transcode360 does not appear to be installed",
                        "Failed to locate Transcode360", DialogButtons.Ok, 5, true);
                    Utilities.DebugLine("[TranscodePlayer] Unable to location media to transcode: " + _title.SelectedDisk.Path);
                    return false;
                }
            }
            catch (Exception e)
            {
                AddInHost.Current.MediaCenterEnvironment.Dialog("An error ocurred while trying to play Transcode360",
                    "Transcode360 Error", DialogButtons.Ok, 5, true);
                Utilities.DebugLine("[TranscodePlayer] Error with Transcode360: " + e.Message);
                return false;
            }
            return true;
        }
    }
}