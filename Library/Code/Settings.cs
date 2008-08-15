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
            _omlSettings.GalleryCoverArtRows = System.Convert.ToInt32(_coverArtRows.Chosen as string);
            _omlSettings.CoverArtSpacingVertical = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
            _omlSettings.CoverArtSpacingHorizontal = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
            _omlSettings.MovieView = _movieView.Chosen as string;
            _omlSettings.ShowMovieDetails = (bool)_showMovieDetails.Chosen;
            _omlSettings.DimUnselectedCovers = (bool)_dimUnselectedCovers.Chosen;
            _omlSettings.UseOriginalCoverArt = (bool)_useOriginalCoverArt.Chosen;
            _omlSettings.VirtualDiscDrive = _virtualDrive.Chosen as string;
            OMLApplication.Current.Startup(null);
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
            // viewItems.Add("Cover Flow"); /* its not ready yet */
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

            List<string> rowItems = new List<string>();
            rowItems.Add("1");
            rowItems.Add("2");
            rowItems.Add("3");
            rowItems.Add("4");
            rowItems.Add("5");
            rowItems.Add("6");
            rowItems.Add("7");
            rowItems.Add("8");
            rowItems.Add("8");
            rowItems.Add("10");

            _coverArtRows.Options = rowItems;
            _coverArtRows.Chosen = _omlSettings.GalleryCoverArtRows.ToString();

            List<string> spaceItems = new List<string>();
            spaceItems.Add("0");
            spaceItems.Add("2");
            spaceItems.Add("4");
            spaceItems.Add("6");
            spaceItems.Add("8");
            spaceItems.Add("12");
            spaceItems.Add("14");
            spaceItems.Add("16");
            spaceItems.Add("18");
            spaceItems.Add("20");

            _coverArtSpacing.Options = spaceItems;
            _coverArtSpacing.Chosen = _omlSettings.CoverArtSpacingVertical.ToString();


            _showMovieDetails.Chosen = _omlSettings.ShowMovieDetails;
            _dimUnselectedCovers.Chosen = _omlSettings.DimUnselectedCovers;
            _useOriginalCoverArt.Chosen = _omlSettings.UseOriginalCoverArt;
        }

        public BooleanChoice ShowMovieDetails
        {
            get { return _showMovieDetails; }
        }

        public BooleanChoice UseOriginalCoverArt
        {
            get { return _useOriginalCoverArt; }
        }

        public BooleanChoice DimUnselectedCovers
        {
            get { return _dimUnselectedCovers; }
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

        public Choice CoverArtRows
        {
            get
            {
                return _coverArtRows;
            }
        }

        public Choice CoverArtSpacing
        {
            get
            {
                return _coverArtSpacing;
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
        Choice _coverArtRows = new Choice();
        Choice _coverArtSpacing = new Choice();
        BooleanChoice _showMovieDetails = new BooleanChoice();
        BooleanChoice _dimUnselectedCovers = new BooleanChoice();
        BooleanChoice _useOriginalCoverArt = new BooleanChoice();

    }
}