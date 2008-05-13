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
    public class TranscodePlayer : IPlayMovie
    {
        public TranscodePlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            // mount the file, then figure out which other player we want to create
            AddInHost.Current.MediaCenterEnvironment.Dialog("Transcoding not implemented yet. File " + _title.FileLocation, "Error", DialogButtons.Cancel, 0, true);
            return false;
        }
        MovieItem _title;

        public bool PlayTranscodedMedia(ref string path_to_buffer)
        {
            return false;
            /*
            Type ITranscode360Type = null;
            if (Utilities.IsTranscode360LibraryAvailable())
            {
                ITranscode360Type =
                    Utilities.LoadTranscode360Assembly(
                        @"c:\\program files\\transcode360\\Transcode360.Interface.dll");
                if (ITranscode360Type != null)
                {
                    try
                    {
                        Hashtable properties = new Hashtable();
                        properties.Add("name", "");
                        TcpClientChannel channel = new TcpClientChannel(properties, null);
                        ChannelServices.RegisterChannel(channel);

                        object obj = Activator.GetObject(typeof(ITranscode360Type),
                                                         "tcp://localhost:1401/RemotingServices/Transcode360");

                        // call an interface, not interested in the result as long as we don't get a 
                        // socket/remoting exception we're happy
                        if (obj.Transcode(FileLocation, out path_to_buffer, DateTime.Now.ToBinary()))
                        {
                            if (obj.IsMediaTranscoding(FileLocation))
                                return true;
                        }

                        ChannelServices.UnregisterChannel(channel);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
            }
            return false;
            */
        }


        private string DynamicPlayMedia()
        {
            string path_to_media = _title.FileLocation;
//            Trace.WriteLine("We are an extender");
            string new_path = string.Empty;
            if (PlayTranscodedMedia(ref new_path))
            {
                path_to_media = new_path;
            }
  //          Trace.WriteLine("Returning path: " + path_to_media);
            return path_to_media;
        }
    }
}