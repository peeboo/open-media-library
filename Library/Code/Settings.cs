//#define CAROUSEL

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            Utilities.DebugLine("[Settings] Loading MountingTools Settings");
            SetupMountingTools();
            Utilities.DebugLine("[Settings] Loading UI Settings");
            SetupMovieSettings();
            Utilities.DebugLine("[Settings] Loading Language Settings");
            SetupUILanguage();
            Utilities.DebugLine("[Settings] Loading Trailers Settings");
            SetupTrailers();
            Utilities.DebugLine("[Settings] Loading Filter Settings");
            SetupFilters();
            Utilities.DebugLine("[Settings] Loading Transcoding Settings");
            SetupTranscoding();
            Utilities.DebugLine("[Settings] Setting up external player Settings");
            SetupExternalPlayers();
        }       

        private void SaveMountingTools()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.MountingToolPath = _mountingToolPath.Value;

                try
                {
                    MountingTool.Tool tool;
                    string ChosenMountingSelection = _ImageMountingSelection.Chosen as string;
                    tool = (MountingTool.Tool)Enum.Parse(typeof(MountingTool.Tool), ChosenMountingSelection);
                    _omlSettings.MountingToolSelection = (int)tool;
                }
                catch (Exception ex)
                {
                    Utilities.DebugLine("[Settings] Error saving Mounting selection: {0}", ex);
                }

                _omlSettings.VirtualDiscDrive = _virtualDrive.Chosen as string;
            });
        }

        private void SaveExternalPlayers()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                StringCollection mappings = new StringCollection();

                try
                {
                    if (!string.IsNullOrEmpty(_blueRayPlayerPath.Value) &&
                        _blueRayPlayerPath.Value.Trim().Length != 0)
                    {
                        mappings.Add(VideoFormat.BLURAY.ToString() + "|" + _blueRayPlayerPath.Value.Trim());
                        mappings.Add(VideoFormat.HDDVD.ToString() + "|" + _blueRayPlayerPath.Value.Trim());
                    }

                    _omlSettings.ExternalPlayerMapping = mappings;
                    ExternalPlayer.RefreshExternalPlayerList();
                }
                catch (Exception ex)
                {
                    Utilities.DebugLine("[Settings] Error saving external player settings", ex);
                }
            });
        }

        private void SaveFilterSettings()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.ShowFilterActors = (bool)_showFilterActors.Chosen;
                _omlSettings.ShowFilterCountry = (bool)_showFilterCountry.Chosen;
                _omlSettings.ShowFilterDateAdded = (bool)_showFilterDateAdded.Chosen;
                _omlSettings.ShowFilterDirectors = (bool)_showFilterDirectors.Chosen;
                _omlSettings.ShowFilterFormat = (bool)_showFilterFormat.Chosen;
                _omlSettings.ShowFilterGenres = (bool)_showFilterGenres.Chosen;
                _omlSettings.ShowFilterParentalRating = (bool)_showFilterParentalRating.Chosen;
                _omlSettings.ShowFilterRuntime = (bool)_showFilterRuntime.Chosen;
                _omlSettings.ShowFilterTags = (bool)_showFilterTags.Chosen;
                _omlSettings.ShowFilterUserRating = (bool)_showFilterUserRating.Chosen;
                _omlSettings.ShowFilterYear = (bool)_showFilterYear.Chosen;
                _omlSettings.ShowFilterTrailers = (bool)_showFilterTrailers.Chosen;
            });
        }
        private void SaveMovieSettings()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.MovieSort = _movieSort.Chosen as string;
                _omlSettings.StartPage = _startPage.Chosen as string;
                _omlSettings.GalleryCoverArtRows = System.Convert.ToInt32(_coverArtRows.Chosen as string);
                _omlSettings.CoverArtSpacingVertical = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
                _omlSettings.CoverArtSpacingHorizontal = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
                _omlSettings.MovieView = _movieView.Chosen as string;
                _omlSettings.ShowMovieDetails = (bool)_showMovieDetails.Chosen;
                _omlSettings.DimUnselectedCovers = (bool)_dimUnselectedCovers.Chosen;
                _omlSettings.UseOriginalCoverArt = (bool)_useOriginalCoverArt.Chosen;
                _omlSettings.MovieDetailsTransitionType = _movieDetailsTransitionType.Chosen as string;
            });
        }
        private void SaveUILanguage()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.UILanguage = CultureIdFromDisplayName(_uiLanguage.Chosen as string);
            });
        }
        private void SaveTrailers()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.TrailersDefinition = _trailersDefinition.Chosen as string;
            });
        }
        private void SaveTranscoding()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                _omlSettings.TranscodeAVIFiles = (bool)_transcodeAVIFiles.Chosen;
                _omlSettings.FlipFourCCCode = (bool)_flipFourCCCode.Chosen;
            });
        }
        public void SaveSettings()
        {
            SaveMountingTools();
            SaveMovieSettings();
            SaveUILanguage();
            SaveTrailers();
            SaveFilterSettings();
            SaveTranscoding();
            SaveExternalPlayers();

            OMLApplication.ExecuteSafe(delegate
            {
                OMLApplication.Current.Startup(null);
            });
        }

        private void SetupMountingTools()
        {
            List<string> _MountingToolSelection = new List<string>();
            foreach (string toolName in Enum.GetNames(typeof(MountingTool.Tool)))
            {
                _MountingToolSelection.Add(toolName);
            }
            _ImageMountingSelection.Options = _MountingToolSelection;

            _mountingToolPath.Value = _omlSettings.MountingToolPath;

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
#if LAYOUT_V2
            viewItems.Add("Folder View");
#endif
#if CAROUSEL
            viewItems.Add("Carousel");
#endif
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

            List<string> transitionTypes = new List<string>(3);
            transitionTypes.Add("None");
            transitionTypes.Add("Zoom");
            transitionTypes.Add("Zoom and Spin");
            _movieDetailsTransitionType.Options = transitionTypes;
            _movieDetailsTransitionType.Chosen = _omlSettings.MovieDetailsTransitionType;
            

            //_mountingToolPath.Value = _omlSettings.MountingTool;

            List<string> trailerDefinitionOptions = new List<string>();
            trailerDefinitionOptions.Add("Std");
            trailerDefinitionOptions.Add("Hi");

            _trailersDefinition.Options = trailerDefinitionOptions;
            _trailersDefinition.Chosen = _omlSettings.TrailersDefinition.ToString();
        }

        private void SetupFilters()
        {
             _showFilterActors.Chosen = _omlSettings.ShowFilterActors;
             _showFilterCountry.Chosen = _omlSettings.ShowFilterCountry;
             _showFilterDateAdded.Chosen = _omlSettings.ShowFilterDateAdded;
             _showFilterDirectors.Chosen = _omlSettings.ShowFilterDirectors;
             _showFilterFormat.Chosen = _omlSettings.ShowFilterFormat;
             _showFilterGenres.Chosen = _omlSettings.ShowFilterGenres;
             _showFilterParentalRating.Chosen = _omlSettings.ShowFilterParentalRating;
             _showFilterRuntime.Chosen = _omlSettings.ShowFilterRuntime;
             _showFilterTags.Chosen = _omlSettings.ShowFilterTags;
             _showFilterUserRating.Chosen = _omlSettings.ShowFilterUserRating;
             _showFilterYear.Chosen = _omlSettings.ShowFilterYear;
             _showFilterTrailers.Chosen = _omlSettings.ShowFilterTrailers;
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

        private void SetupTrailers()
        {
            List<string> trailerFormats = new List<string>();
            trailerFormats.Add("Hi");
            trailerFormats.Add("Std");
            _trailersDefinition.Options = trailerFormats;
            _trailersDefinition.Chosen = _omlSettings.TrailersDefinition;
        }

        private void SetupTranscoding()
        {
            _transcodeAVIFiles.Chosen = _omlSettings.TranscodeAVIFiles;
            _flipFourCCCode.Chosen = _omlSettings.FlipFourCCCode;
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

        private void SetupExternalPlayers()
        {
            _blueRayPlayerPath.Value =  ExternalPlayer.GetExternalPlayerPath(VideoFormat.BLURAY);
        }

        #region properties
        public BooleanChoice TranscodeAVIFiles
        {
            get { return _transcodeAVIFiles; }
        }
        public BooleanChoice FlipFourCCCode
        {
            get { return _flipFourCCCode; }
        }
        public BooleanChoice ShowFilterGenres
        {
            get { return _showFilterGenres; }
        }
        public BooleanChoice ShowFilterDirectors
        {
            get { return _showFilterDirectors; }
        }
        public BooleanChoice ShowFilterActors
        {
            get { return _showFilterActors; }
        }
        public BooleanChoice ShowFilterRuntime
        {
            get { return _showFilterRuntime; }
        }
        public BooleanChoice ShowFilterCountry
        {
            get { return _showFilterCountry; }
        }
        public BooleanChoice ShowFilterParentalRating
        {
            get { return _showFilterParentalRating; }
        }
        public BooleanChoice ShowFilterTags
        {
            get { return _showFilterTags; }
        }
        public BooleanChoice ShowFilterUserRating
        {
            get { return _showFilterUserRating; }
        }
        public BooleanChoice ShowFilterYear
        {
            get { return _showFilterYear; }
        }
        public BooleanChoice ShowFilterDateAdded
        {
            get { return _showFilterDateAdded; }
        }
        public BooleanChoice ShowFilterFormat
        {
            get { return _showFilterFormat; }
        }
        public BooleanChoice ShowFilterTrailers
        {
            get { return _showFilterTrailers; }
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

        public Choice TrailersDefinition
        {
            get { return _trailersDefinition; }
        }

        public Choice MovieDetailsTransitionType
        {
            get { return _movieDetailsTransitionType; }
        }
      
        #endregion
        public EditableText MountingToolPath
        {
            get
            {
                if (_mountingToolPath == null)
                {
                    _mountingToolPath = new EditableText();
                }
                return _mountingToolPath;
            }
            set
            {
                _mountingToolPath = value;
                FirePropertyChanged("MountingToolPath");
            }
        }

        public EditableText BlueRayPlayerPath
        {
            get
            {
                if (_blueRayPlayerPath == null)
                {
                    _blueRayPlayerPath = new EditableText();
                }
                return _blueRayPlayerPath;
            }
            set
            {
                _blueRayPlayerPath = value;
                FirePropertyChanged("BluRayPlayerPath");
            }
        }

        public Choice UILanguage
        {
            get { return _uiLanguage;  }
        }
        public Choice FiltersToShow
        {
            get { return _filtersToShow; }
        }
        public Choice ImageMountingSelection
        {
            get { return _ImageMountingSelection; }
            set
            {
                _ImageMountingSelection = value;
                FirePropertyChanged("ImageMountingSelection");
            }
        }

        EditableText _mountingToolPath = new EditableText();
        EditableText _blueRayPlayerPath = new EditableText();
        OMLSettings _omlSettings = new OMLSettings();
        Choice _virtualDrive = new Choice();
        Choice _movieView = new Choice();
        Choice _movieSort = new Choice();
        Choice _coverArtRows = new Choice();
        Choice _coverArtSpacing = new Choice();
        Choice _movieDetailsTransitionType = new Choice();
        BooleanChoice _showMovieDetails = new BooleanChoice();
        BooleanChoice _dimUnselectedCovers = new BooleanChoice();
        BooleanChoice _useOriginalCoverArt = new BooleanChoice();
        Choice _startPage = new Choice();
        Choice _uiLanguage = new Choice();
        Choice _ImageMountingSelection = new Choice();
        Choice _trailersDefinition = new Choice();
        Choice _filtersToShow = new Choice();
        BooleanChoice _showFilterGenres = new BooleanChoice();
        BooleanChoice _transcodeAVIFiles = new BooleanChoice();
        BooleanChoice _flipFourCCCode = new BooleanChoice();
        BooleanChoice _showFilterDirectors = new BooleanChoice();
        BooleanChoice _showFilterActors = new BooleanChoice();
        BooleanChoice _showFilterFormat = new BooleanChoice();
        BooleanChoice _showFilterTrailers = new BooleanChoice();
        BooleanChoice _showFilterDateAdded = new BooleanChoice();
        BooleanChoice _showFilterYear = new BooleanChoice();
        BooleanChoice _showFilterUserRating = new BooleanChoice();
        BooleanChoice _showFilterTags = new BooleanChoice();
        BooleanChoice _showFilterParentalRating = new BooleanChoice();
        BooleanChoice _showFilterCountry = new BooleanChoice();
        BooleanChoice _showFilterRuntime = new BooleanChoice();        
    }
}