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

namespace Library
{
    public class TranscodePlayer : IPlayMovie
    {
        object _server = null;
        string path_to_buffer = string.Empty;
        Type ITranscode360Type = null;
        MethodInfo MIIsMediaTranscodeComplete = null;
        MethodInfo MIIsMediaTranscoding = null;
        MethodInfo MIIsMediaTranscodingWithParams = null;
        MethodInfo MITranscode = null;
        MethodInfo MITranscodeWithParams = null;
        MethodInfo MIStopTranscoding = null;

        public TranscodePlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            if (PlayTranscodedMedia(_title, ref path_to_buffer))
            {
                OMLApplication.DebugLine("PathToBuffer: " + path_to_buffer);
                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, path_to_buffer, false))
                {
                    if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                    {
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
            /*
            // mount the file, then figure out which other player we want to create
            AddInHost.Current.MediaCenterEnvironment.Dialog("Transcoding not implemented yet. File " + _title.FileLocation, "Error", DialogButtons.Cancel, 0, true);
            return false;
            */
        }
        MovieItem _title;

        public bool PlayTranscodedMedia(MovieItem title, ref string path_to_buffer)
        {
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

                        _server = Activator.GetObject(ITranscode360Type,
                            "tcp://localhost:1401/RemotingServices/Transcode360");

                        assignMethodInfoObjects();


                        if (MIIsMediaTranscodeComplete != null &&
                            MIIsMediaTranscoding != null &&
                            MIIsMediaTranscodingWithParams != null &&
                            MITranscode != null &&
                            MITranscodeWithParams != null &&
                            MIStopTranscoding != null)
                        {
                            if (isAlreadyTranscoding())
                                setCurrentBufferPath();
                            else
                                startTranscode();
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        OMLApplication.DebugLine("Error calling transcode360: " + e.Message);
                        return false;
                    }
                }
                else
                {
                    OMLApplication.DebugLine("Didn't locate ITranscode360 interface");
                    return false;
                }
            }
            else
            {
                OMLApplication.DebugLine("Transcode360 isn't available");
                return false;
            }
        }

        private bool isAlreadyTranscoding()
        {
            OMLApplication.DebugLine("Checking if file is already being transcoded");
            bool retVal = false;
            try
            {
                retVal = (bool)MIIsMediaTranscoding.Invoke(_server, new object[] {
                _title.FileLocation
            });
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Error calling transcode360 (MIIsMediaTranscoding): " + e.Message);
                foreach (ParameterInfo pInfo in MIIsMediaTranscoding.GetParameters())
                {
                    OMLApplication.DebugLine("parameter: " + pInfo.Name + " Type: " + pInfo.ParameterType.ToString());
                }
            }

            return retVal;
        }

        private void setCurrentBufferPath()
        {
            OMLApplication.DebugLine("Setting the current path for a currently transcoding/ed file");
            object[] paramArray = new object[] { _title.FileLocation };

            try
            {
                MIIsMediaTranscoding.Invoke(_server, paramArray);
                path_to_buffer = (string)paramArray[0];
                OMLApplication.DebugLine("Setting transcoded path to: " + path_to_buffer);
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Error calling transcode360 (MIIsMediaTranscoding): " + e.Message);
                foreach (ParameterInfo pInfo in MIIsMediaTranscoding.GetParameters())
                {
                    OMLApplication.DebugLine("parameter: " + pInfo.Name + " Type: " + pInfo.ParameterType.ToString());
                }
            }
        }

        private void startTranscode()
        {
            OMLApplication.DebugLine("Starting a transcode job");
            object[] paramArray = new object[3];
            paramArray[0] = _title.FileLocation;
            paramArray[1] = path_to_buffer;
            paramArray[2] = 0;
            try
            {
                MITranscode.Invoke(_server, paramArray);

                path_to_buffer = (string)paramArray[1];
                OMLApplication.DebugLine("Setting transcode path to: " + path_to_buffer);
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Error calling transcode360 (MITranscode): " + e.Message);
                foreach (ParameterInfo pInfo in MITranscode.GetParameters())
                {
                    OMLApplication.DebugLine("parameter: " + pInfo.Name + " Type: " + pInfo.ParameterType.ToString());
                }
            }
        }

        private void assignMethodInfoObjects()
        {
            OMLApplication.DebugLine("Locating and assigning Remote library methods");
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
    }
}