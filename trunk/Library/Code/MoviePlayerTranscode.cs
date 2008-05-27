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
            string path_to_buffer = "\\" + _title.itemId;
            return PlayTranscodedMedia(ref path_to_buffer);
            /*
            // mount the file, then figure out which other player we want to create
            AddInHost.Current.MediaCenterEnvironment.Dialog("Transcoding not implemented yet. File " + _title.FileLocation, "Error", DialogButtons.Cancel, 0, true);
            return false;
            */
        }
        MovieItem _title;

        public bool PlayTranscodedMedia(ref string path_to_buffer)
        {
            Type ITranscode360Type = null;
            MethodInfo MIIsMediaTranscodeComplete = null;
            MethodInfo MIIsMediaTranscoding = null;
            MethodInfo MIIsMediaTranscodingWithParams = null;
            MethodInfo MITranscode = null;
            MethodInfo MITranscodeWithParams = null;
            MethodInfo MIStopTranscoding = null;

            if (Utilities.IsTranscode360LibraryAvailable())
            {
                ITranscode360Type =
                    Utilities.LoadTranscode360Assembly(
                        @"c:\\program files\\transcode360\\Transcode360.Interface.dll");

                if (ITranscode360Type != null)
                {
                    Hashtable properties = new Hashtable();
                    properties.Add("name", "");
                    TcpClientChannel channel = new TcpClientChannel(properties, null);
                    ChannelServices.RegisterChannel(channel);

                    object server = Activator.GetObject(ITranscode360Type,
                        "tcp://localhost:1401/RemotingServices/Transcode360");

                    assignMethodInfoObjects(ITranscode360Type,
                                            ref MIIsMediaTranscodeComplete,
                                            ref MIIsMediaTranscoding,
                                            ref MIIsMediaTranscodingWithParams,
                                            ref MITranscode,
                                            ref MITranscodeWithParams,
                                            ref MIStopTranscoding);


                    if (MIIsMediaTranscodeComplete != null &&
                        MIIsMediaTranscoding != null &&
                        MIIsMediaTranscodingWithParams != null &&
                        MITranscode != null &&
                        MITranscodeWithParams != null &&
                        MIStopTranscoding != null)
                    {
                        object[] paramArray = new object[1];
                        paramArray[0] = _title.FileLocation;
                        MITranscode.Invoke(server, paramArray);
                    }
                }
            }
            return false;
        }

        private void assignMethodInfoObjects(Type ITranscode360Type,
                                             ref MethodInfo MIIsMediaTranscodeComplete,
                                             ref MethodInfo MIIsMediaTranscoding,
                                             ref MethodInfo MIIsMediaTranscodingWithParams,
                                             ref MethodInfo MITranscode,
                                             ref MethodInfo MITranscodeWithParams,
                                             ref MethodInfo MIStopTranscoding)
        {
                    MethodInfo[] methodInfos = ITranscode360Type.GetMethods();

                    foreach (MethodInfo mi in methodInfos)
                    {
                        switch (mi.Name)
                        {
                            case "IsMediaTranscodeComplete":
                                MIIsMediaTranscodeComplete = mi;
                                break;
                            case "IsMediaTranscoding":
                                switch (mi.GetParameters().Length)
                                {
                                    case 1:
                                        MIIsMediaTranscoding = mi;
                                        break;
                                    case 3:
                                        MIIsMediaTranscodingWithParams = mi;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "Transcode":
                                switch (mi.GetParameters().Length)
                                {
                                    case 3:
                                        MITranscode = mi;
                                        break;
                                    case 6:
                                        MITranscodeWithParams = mi;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "StopTranscoding":
                                MIStopTranscoding = mi;
                                break;
                            default:
                                break;
                        }
                    }
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