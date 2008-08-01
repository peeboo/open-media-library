using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Data;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using OMLEngine;
using OMLSDK;

namespace Library
{
    public class Settings : ModelItem
    {
        public Settings()
        {
            SetupDaemonTools();
            SetupMovieSettings();
        }

        public void SaveSettings()
        {
            _omlSettings.DaemonTools = _daemonToolsPath.Value;
            _omlSettings.MovieSort = _movieSort.Chosen as string;
            _omlSettings.MovieView = _movieView.Chosen as string;
            _omlSettings.ShowMovieDetails = (bool)_ShowMovieDetails.Chosen;
            _omlSettings.VirtualDiscDrive = _virtualDrive.Chosen as string;
            OMLApplication.Current.Startup();
        }

        private void SetupDaemonTools()
        {
            _daemonToolsPath.Value = _omlSettings.DaemonTools;

            List<string> items = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                items.Add(new string(c, 1));
            }
            _virtualDrive.Options = items;
            _virtualDrive.Chosen = _omlSettings.VirtualDiscDrive;
        }

        private void SetupMovieSettings()
        {
            List<string> viewItems = new List<string>();
            viewItems.Add("Cover Art");
            viewItems.Add("List");
            _movieView.Options = viewItems;
            _movieView.Chosen = _omlSettings.MovieView;


            List<string> items = new List<string>();
            items.Add("Name Ascending");
            items.Add("Name Descending");
            items.Add("Year Ascending");
            items.Add("Year Descending");
            items.Add("Date Added Ascending");
            items.Add("Date Added Descending");
            items.Add("Runtime Ascending");
            items.Add("Runtime Descending");
            items.Add("User Rating Ascending");
            items.Add("User Rating Descending");
            items.Add("Date Added Ascending");
            items.Add("Date Added Descending");

            _movieSort.Options = items;
            _movieSort.Chosen = _omlSettings.MovieSort;

            _ShowMovieDetails.Chosen = _omlSettings.ShowMovieDetails;
        }

        public BooleanChoice ShowMovieDetails
        {
            get { return _ShowMovieDetails; }
        }

        public Choice VirtualDrive
        {
            get
            {
                return _virtualDrive;
            }
        }

        public Choice MovieView
        {
            get 
            {
                return _movieView; 
            }

        }

        public Choice MovieSort
        {
            get 
            {
                return _movieSort; 
            }
        }

        public EditableText DaemonToolsPath
        {
            get
            {
                if (_daemonToolsPath == null)
                {
                    _daemonToolsPath = new EditableText();
                }
                return _daemonToolsPath;
            }
            set
            {
                _daemonToolsPath = value;
                

            }
        }

        EditableText _daemonToolsPath = new EditableText();
        OMLSettings _omlSettings = new OMLSettings();
        Choice _virtualDrive = new Choice();
        Choice _movieView = new Choice();
        Choice _movieSort = new Choice();
        BooleanChoice _ShowMovieDetails = new BooleanChoice();

    }
}