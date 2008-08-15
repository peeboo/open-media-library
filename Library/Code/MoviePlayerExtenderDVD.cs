using System;
using OMLEngine;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    /// <summary>
    /// DVDPlayer class for playing a DVD
    /// </summary>
    public class ExtenderDVDPlayer : IPlayMovie
    {
        public ExtenderDVDPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            string asxFile = GetAsxFile();
            if (asxFile != null && AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, asxFile, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    Utilities.DebugLine("ExtenderDVDPlayer.PlayMovie: movie {0} Playing", _title.Name);
                    OMLApplication.Current.NowPlayingMovieName = _title.Name;
                    OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        string GetAsxFile()
        {
            foreach (string fileName in Directory.GetFiles(Path.Combine(_title.SelectedDisk.Path, ".."), "*.asx"))
            {
                //AddInHost.Current.MediaCenterEnvironment.DialogNotification(string.Format("asx file: {0}", fileName), new object[0], 4, null, null);
                Utilities.DebugLine("ExtenderDVDPlayer.GetAsxFile: found asx {0}", fileName);
                return fileName;
            }
            Utilities.DebugLine("ExtenderDVDPlayer.GetAsxFile: no asx file found {0}", _title.SelectedDisk.Path);
            return null;
        }

        MovieItem _title;
    }

}