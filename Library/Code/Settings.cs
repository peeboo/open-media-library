using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Threading;
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
            SetupUILanguage();
        }

        public void SaveSettings()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.DaemonTools = _daemonToolsPath.Value;
                _omlSettings.MovieSort = _movieSort.Chosen as string;
                _omlSettings.StartPage = _startPage.Chosen as string;
                _omlSettings.GalleryCoverArtRows = System.Convert.ToInt32(_coverArtRows.Chosen as string);
                _omlSettings.CoverArtSpacingVertical = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
                _omlSettings.CoverArtSpacingHorizontal = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
                _omlSettings.MovieView = _movieView.Chosen as string;
                _omlSettings.ShowMovieDetails = (bool)_showMovieDetails.Chosen;
                _omlSettings.DimUnselectedCovers = (bool)_dimUnselectedCovers.Chosen;
                _omlSettings.UseOriginalCoverArt = (bool)_useOriginalCoverArt.Chosen;
                _omlSettings.VirtualDiscDrive = _virtualDrive.Chosen as string;
                _omlSettings.UILanguage = CultureIdFromDisplayName(_uiLanguage.Chosen as string);
                OMLApplication.Current.Startup(null);
            });
        }

        private void SetupDaemonTools()
        {
            _daemonToolsPath.Value = _omlSettings.DaemonTools;

            List<string> items = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
                items.Add(new string(c, 1));

            _virtualDrive.Options = items;
            _virtualDrive.Chosen = _omlSettings.VirtualDiscDrive;
        }

        private void SetupMovieSettings()
        {
            List<string> viewItems = new List<string>();
            viewItems.Add("Cover Art");
            viewItems.Add("List");
            viewItems.Add("Folder View");
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

            List<string> starPageItems = new List<string>();
            starPageItems.Add(Filter.Home);
            starPageItems.Add(Filter.Genres);
            starPageItems.Add(Filter.Tags);
            starPageItems.Add(Filter.UserRating);
            starPageItems.Add(Filter.ParentRating);


            _startPage.Options = starPageItems;
            _startPage.Chosen = _omlSettings.StartPage;


            List<string> rowItems = new List<string>();
            for (int ndx = 1; ndx <= 10; ++ndx)
                rowItems.Add(ndx.ToString());

            _coverArtRows.Options = rowItems;
            _coverArtRows.Chosen = _omlSettings.GalleryCoverArtRows.ToString();

            List<string> spaceItems = new List<string>();
            for (int ndx = 0; ndx <= 20; ndx += 2)
                spaceItems.Add(ndx.ToString());

            _coverArtSpacing.Options = spaceItems;
            _coverArtSpacing.Chosen = _omlSettings.CoverArtSpacingVertical.ToString();

            _showMovieDetails.Chosen = _omlSettings.ShowMovieDetails;
            _dimUnselectedCovers.Chosen = _omlSettings.DimUnselectedCovers;
            _useOriginalCoverArt.Chosen = _omlSettings.UseOriginalCoverArt;
        }

        private void SetupUILanguage()
        {
            string selected = null;
            List<string> list = new List<string>();
            string configuredLangId = _omlSettings.UILanguage;

            foreach (var availableCulture in I18n.AvailableCultures)
            {
                string name = availableCulture.TextInfo.ToTitleCase(availableCulture.NativeName);
                if (string.CompareOrdinal(availableCulture.Name, configuredLangId) == 0)
                {
                    selected = name;
                }
                list.Add(name);
            }

            list.Sort((a, b) => string.Compare(a, b, true, Thread.CurrentThread.CurrentCulture));

            list.Insert(0, "Use system language");
            if (string.IsNullOrEmpty(selected)) selected = list[0];

            _uiLanguage.Options = list;
            _uiLanguage.Chosen = selected;
        }

        private static String CultureIdFromDisplayName(string displayName)
        {
            foreach (var availableCulture in I18n.AvailableCultures)
            {
                if (string.CompareOrdinal(availableCulture.TextInfo.ToTitleCase(availableCulture.NativeName), displayName) == 0)
                {
                    return availableCulture.Name;
                }
            }
            return null;
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
            get { return _virtualDrive; }
        }

        public Choice MovieView
        {
            get { return _movieView; }

        }

        public Choice MovieSort
        {
            get { return _movieSort; }
        }

        public Choice StartPage
        {
            get { return _startPage; }
        }

        public Choice CoverArtRows
        {
            get { return _coverArtRows; }
        }

        public Choice CoverArtSpacing
        {
            get { return _coverArtSpacing; }
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
            set { _daemonToolsPath = value; }
        }

        public Choice UILanguage
        {
            get { return _uiLanguage;  }
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
        Choice _startPage = new Choice();
        Choice _uiLanguage = new Choice();

        BooleanChoice _showGenres = new BooleanChoice();
        BooleanChoice _showActors = new BooleanChoice();
        BooleanChoice _showDirectors = new BooleanChoice();
        BooleanChoice _showDateAdded = new BooleanChoice();
        BooleanChoice _showYear = new BooleanChoice();
        BooleanChoice _showRuntime = new BooleanChoice();
        BooleanChoice _showFormat = new BooleanChoice();
        BooleanChoice _showUserRating = new BooleanChoice();
        BooleanChoice _showTags = new BooleanChoice();
        BooleanChoice _showParentalRating = new BooleanChoice();
        BooleanChoice _showCountry = new BooleanChoice();

    }
}