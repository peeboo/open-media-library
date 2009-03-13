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
        public void ShowSampleLayout()
        {
            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/IntroPage", null);
        }

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
            Utilities.DebugLine("[Settings] Setting up impersonation Settings");
            SetupImpersonationSettings();
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
                    string chosenBluRayPlayer = _externalPlayerSelectionBluRay.Chosen as string;
                    ExternalPlayer.KnownPlayers bluRayPlayer = (ExternalPlayer.KnownPlayers)Enum.Parse(typeof(ExternalPlayer.KnownPlayers), chosenBluRayPlayer);

                    if (bluRayPlayer != ExternalPlayer.KnownPlayers.None)
                    {
                        if (!string.IsNullOrEmpty(_externalPlayerPathBluRay.Value) &&
                            _externalPlayerPathBluRay.Value.Trim().Length != 0)
                        {
                            mappings.Add(VideoFormat.BLURAY.ToString() + "|" + ((int)bluRayPlayer).ToString() + "|" + _externalPlayerPathBluRay.Value.Trim());
                        }
                    }

                    string chosenHDDVDPlayer = _externalPlayerSelectionHDDVD.Chosen as string;
                    ExternalPlayer.KnownPlayers hddvdPlayer = (ExternalPlayer.KnownPlayers)Enum.Parse(typeof(ExternalPlayer.KnownPlayers), chosenHDDVDPlayer);

                    if (hddvdPlayer != ExternalPlayer.KnownPlayers.None)
                    {
                        if (!string.IsNullOrEmpty(_externalPlayerPathHDDVD.Value) &&
                            _externalPlayerPathHDDVD.Value.Trim().Length != 0)
                        {
                            mappings.Add(VideoFormat.HDDVD.ToString() + "|" + ((int)hddvdPlayer).ToString() + "|" + _externalPlayerPathHDDVD.Value.Trim());
                        }
                    }

                    string chosenAllPlayer = _externalPlayerSelectionAll.Chosen as string;
                    ExternalPlayer.KnownPlayers allPlayer = (ExternalPlayer.KnownPlayers)Enum.Parse(typeof(ExternalPlayer.KnownPlayers), chosenAllPlayer);

                    if (allPlayer != ExternalPlayer.KnownPlayers.None)
                    {
                        if (!string.IsNullOrEmpty(_externalPlayerPathAll.Value) &&
                            _externalPlayerPathAll.Value.Trim().Length != 0)
                        {
                            mappings.Add(VideoFormat.ALL.ToString() + "|" + ((int)allPlayer).ToString() + "|" + _externalPlayerPathAll.Value.Trim());
                        }
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
                _omlSettings.ShowFilterUnwatched = (bool)_showFilterUnwatched.Chosen;
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
                _omlSettings.StartPageSubFilter = _startPageSubFilter.Chosen as string;
                _omlSettings.GalleryCoverArtRows = System.Convert.ToInt32(_coverArtRows.Chosen as string);
                _omlSettings.CoverArtSpacingVertical = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
                _omlSettings.CoverArtSpacingHorizontal = System.Convert.ToInt32(_coverArtSpacing.Chosen as string);
                _omlSettings.MovieView = _movieView.Chosen as string;
                _omlSettings.DetailsView = _detailsView.Chosen as string;
                _omlSettings.ShowMovieDetails = (bool)_showMovieDetails.Chosen;
                _omlSettings.DimUnselectedCovers = (bool)_dimUnselectedCovers.Chosen;
                _omlSettings.UseOriginalCoverArt = (bool)_useOriginalCoverArt.Chosen;
                _omlSettings.UseOnScreenAlphaJumper = (bool)_useOnScreenAlpha.Chosen;
                _omlSettings.ShowWatchedIcon = (bool)_showWatchedIcon.Chosen;
                _omlSettings.MainPageBackDropAlphaValue = (float)_mainPageBackDropAlpha.Chosen;
                _omlSettings.MainPageBackDropIntervalValue = (int)_mainPageBackDropInterval.Chosen;
                _omlSettings.DetailsPageBackDropAlphaValue = (float)_detailsPageBackDropAlpha.Chosen;
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
                _omlSettings.TranscodeMKVFiles = (bool)_transcodeMKVFiles.Chosen;
                _omlSettings.TranscodeOGMFiles = (bool)_transcodeOGMFiles.Chosen;
                _omlSettings.PreserveAudioOnTranscode = (bool)_preserveAudioOnTranscode.Chosen;
                _omlSettings.DebugTranscoding = (bool)_debugTranscoding.Chosen;
                _omlSettings.FlipFourCCCode = (bool)_flipFourCCCode.Chosen;
                int transcodeBufferDelay = 7;
                try
                {
                    Int32.TryParse(_transcodeBufferDelay.Value, out transcodeBufferDelay);
                }
                catch { }
                if (transcodeBufferDelay != 7)
                    _omlSettings.TranscodeBufferDelay = transcodeBufferDelay;
            });
        }
        private void SaveImpersonationSettings()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                OMLEngine.Properties.Settings.Default.ImpersonationUsername = _impersonationUsername.Value;
                OMLEngine.Properties.Settings.Default.ImpersonationPassword = _impersonationPassword.Value;
                OMLEngine.Properties.Settings.Default.Save();
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
            SaveImpersonationSettings();
            SaveExternalPlayers();

            //OMLApplication.ExecuteSafe(delegate
            //{
            //    OMLApplication.Current.Startup(null);
            //});
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

            _ImageMountingSelection.Chosen = ((MountingTool.Tool)_omlSettings.MountingToolSelection).ToString();

            List<string> items = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
                items.Add(new string(c, 1));

            _virtualDrive.Options = items;
            _virtualDrive.Chosen = _omlSettings.VirtualDiscDrive;
        }
        

        public void BuildSubFilterOptions()
        {            
            TitleFilterType filterType = Filter.FilterStringToTitleType(_startPage.Chosen.ToString());

            if (filterType == TitleFilterType.Unwatched ||
                filterType == TitleFilterType.All)
            {
                _startPageSubFilter.Options = new List<string>() { "" };
                return;
            }

            List<string> subFilters = new List<string>();

            subFilters.Add("");            

            // todo : solomon : make this less costly - every new movie gallery
            // instance gets a full list of movies
            MovieGallery gallery = new MovieGallery();           

            // get all subfilters for option
            Filter filter = new Filter(gallery, filterType, null);

            IList<GalleryItem> items = filter.GetGalleryItems();

            foreach (GalleryItem item in items)
            {
                subFilters.Add(item.Name);
            }

            _startPageSubFilter = new Choice();

            _startPageSubFilter.Options = subFilters;
            _startPageSubFilter.Chosen = _startPageSubFilter.Options[0];                       

            FirePropertyChanged("StartPageSubFilter");
        }

        private void SetupMovieSettings()
        {
            List<string> viewItems = new List<string>();
            viewItems.Add(GalleryView.CoverArt);
            viewItems.Add(GalleryView.CoverArtWithAlpha);
            viewItems.Add(GalleryView.List);
            viewItems.Add(GalleryView.ListWithCovers);
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

            List<string> detailViews = new List<string>();
            detailViews.Add("Original");
            detailViews.Add("Background Boxes");

            _detailsView.Options = detailViews;
            _detailsView.Chosen = _omlSettings.DetailsView;

            List<string> starPageItems = new List<string>();
            starPageItems.Add(Filter.Home);
            starPageItems.Add(Filter.Unwatched);
            starPageItems.Add(Filter.Genres);
            starPageItems.Add(Filter.Tags);
            starPageItems.Add(Filter.UserRating);
            starPageItems.Add(Filter.ParentRating);


            _startPage.Options = starPageItems;
            _startPage.Chosen = _omlSettings.StartPage;

            BuildSubFilterOptions();

            if (!string.IsNullOrEmpty(_omlSettings.StartPageSubFilter) &&
                _startPageSubFilter.Options != null &&
                _startPageSubFilter.Options.Contains(_omlSettings.StartPageSubFilter))
            {
                _startPageSubFilter.Chosen = _omlSettings.StartPageSubFilter;
            }

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
            _useOnScreenAlpha.Chosen = _omlSettings.UseOnScreenAlphaJumper;
            _showWatchedIcon.Chosen = _omlSettings.ShowWatchedIcon;

            List<string> transitionTypes = new List<string>(3);
            transitionTypes.Add("None");
            transitionTypes.Add("Zoom");
            transitionTypes.Add("Zoom and Spin");

            //_mountingToolPath.Value = _omlSettings.MountingTool;

            List<string> trailerDefinitionOptions = new List<string>();
            trailerDefinitionOptions.Add("Std");
            trailerDefinitionOptions.Add("Hi");

            _trailersDefinition.Options = trailerDefinitionOptions;
            _trailersDefinition.Chosen = _omlSettings.TrailersDefinition.ToString();

            SetupMainPageBackDropAlpha();
            SetupDetailsPageBackDropAlpha();
            SetupMainPageBackDropInterval();
        }

        private void SetupFilters()
        {
            _showFilterUnwatched.Chosen = _omlSettings.ShowFilterUnwatched;
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

        private void SetupMainPageBackDropAlpha()
        {
            List<float> alphaOptions = new List<float>();
            alphaOptions.Add(0.1F);
            alphaOptions.Add(0.2F);
            alphaOptions.Add(0.3F);
            alphaOptions.Add(0.4F);
            alphaOptions.Add(0.5F);
            alphaOptions.Add(0.6F);
            alphaOptions.Add(0.7F);
            alphaOptions.Add(0.8F);
            alphaOptions.Add(0.9F);
            alphaOptions.Add(1.0F);

            _mainPageBackDropAlpha.Options = alphaOptions;
            _mainPageBackDropAlpha.Chosen = _omlSettings.MainPageBackDropAlphaValue;
        }

        private void SetupMainPageBackDropInterval()
        {
            List<int> intervalOptions = new List<int>();
            intervalOptions.Add(5);
            intervalOptions.Add(10);
            intervalOptions.Add(15);
            intervalOptions.Add(20);
            _mainPageBackDropInterval.Options = intervalOptions;
            _mainPageBackDropInterval.Chosen = _omlSettings.MainPageBackDropIntervalValue;
        }

        private void SetupDetailsPageBackDropAlpha()
        {
            List<float> alphaOptions = new List<float>();
            alphaOptions.Add(0.1F);
            alphaOptions.Add(0.2F);
            alphaOptions.Add(0.3F);
            alphaOptions.Add(0.4F);
            alphaOptions.Add(0.5F);
            alphaOptions.Add(0.6F);
            alphaOptions.Add(0.7F);
            alphaOptions.Add(0.8F);
            alphaOptions.Add(0.9F);
            alphaOptions.Add(1.0F);

            _detailsPageBackDropAlpha.Options = alphaOptions;
            _detailsPageBackDropAlpha.Chosen = _omlSettings.DetailsPageBackDropAlphaValue;
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
            _transcodeBufferDelay.Value = _omlSettings.TranscodeBufferDelay.ToString();
            _transcodeAVIFiles.Chosen = _omlSettings.TranscodeAVIFiles;
            _transcodeMKVFiles.Chosen = _omlSettings.TranscodeMKVFiles;
            _transcodeOGMFiles.Chosen = _omlSettings.TranscodeOGMFiles;
            _preserveAudioOnTranscode.Chosen = _omlSettings.PreserveAudioOnTranscode;
            _debugTranscoding.Chosen = _omlSettings.DebugTranscoding;
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
            ExternalPlayerItem bluRayPlayer = ExternalPlayer.GetExternalForFormat(VideoFormat.BLURAY);
            ExternalPlayerItem allPlayers = ExternalPlayer.GetExternalForFormat(VideoFormat.ALL);
            ExternalPlayerItem hddvdPlayer = ExternalPlayer.GetExternalForFormat(VideoFormat.HDDVD);

            List<string> externalPlayerChoices = new List<string>();
            foreach (string player in Enum.GetNames(typeof(ExternalPlayer.KnownPlayers)))
            {
                externalPlayerChoices.Add(player);
            }

            _externalPlayerSelectionAll.Options = externalPlayerChoices;
            _externalPlayerSelectionBluRay.Options = externalPlayerChoices;
            _externalPlayerSelectionHDDVD.Options = externalPlayerChoices;

            _externalPlayerSelectionBluRay.Chosen = (bluRayPlayer == null)
                                                 ? ExternalPlayer.KnownPlayers.None.ToString()
                                                 : bluRayPlayer.PlayerType.ToString();

            _externalPlayerSelectionAll.Chosen = (allPlayers == null)
                                                 ? ExternalPlayer.KnownPlayers.None.ToString()
                                                 : allPlayers.PlayerType.ToString();

            _externalPlayerSelectionHDDVD.Chosen = (hddvdPlayer == null)
                                                 ? ExternalPlayer.KnownPlayers.None.ToString()
                                                 : hddvdPlayer.PlayerType.ToString();

            List<string> localFixedDrivesOptions = new List<string>();
            foreach (DriveInfo dInfo in GetFileSystemDrives())
            {
                localFixedDrivesOptions.Add(dInfo.Name);
            }

            _localFixedDrivesBluRay.Options = localFixedDrivesOptions;
            _localFixedDrivesHDDVD.Options = localFixedDrivesOptions;
            _localFixedDrivesAll.Options = localFixedDrivesOptions;
            _LocalFixedDrives.Options = localFixedDrivesOptions;

            _externalPlayerPathBluRay.Value = (bluRayPlayer != null) ? bluRayPlayer.Path : string.Empty;
            _externalPlayerPathHDDVD.Value = (hddvdPlayer != null) ? hddvdPlayer.Path : string.Empty;
            _externalPlayerPathAll.Value = (allPlayers != null) ? allPlayers.Path : string.Empty;
        }
        private void SetupImpersonationSettings()
        {
            _impersonationUsername.Value = OMLEngine.Properties.Settings.Default.ImpersonationUsername;
            _impersonationPassword.Value = OMLEngine.Properties.Settings.Default.ImpersonationPassword;
        }

        #region properties
        public BooleanChoice PreserveAudioOnTranscode
        {
            get { return _preserveAudioOnTranscode; }
        }
        public BooleanChoice TranscodeAVIFiles
        {
            get { return _transcodeAVIFiles; }
        }
        public BooleanChoice TranscodeMKVFiles
        {
            get { return _transcodeMKVFiles; }
        }
        public BooleanChoice TranscodeOGMFiles
        {
            get { return _transcodeOGMFiles; }
        }
        public BooleanChoice DebugTranscoding
        {
            get { return _debugTranscoding; }
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

        public BooleanChoice ShowFilterUnwatched
        {
            get { return _showFilterUnwatched; }
        }

        public BooleanChoice UseOnScreenAlpha
        {
            get { return _useOnScreenAlpha; }
        }

        public BooleanChoice ShowWatchedIcon
        {
            get { return _showWatchedIcon; }
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

        public Choice DetailsView
        {
            get { return _detailsView; }
        }

        public Choice MovieSort
        {
            get { return _movieSort; }
        }

        public Choice StartPage
        {
            get { return _startPage; }
        }

        public Choice StartPageSubFilter
        {
            get { return _startPageSubFilter; }
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

        public Choice LocalFixedDrives
        {
            get { return _LocalFixedDrives; }
        }

        public Choice LocalFixedDrivesBluRay
        {
            get { return _localFixedDrivesBluRay; }
        }

        public Choice LocalFixedDrivesHDDVD
        {
            get { return _localFixedDrivesHDDVD; }
        }

        public Choice LocalFixedDrivesAll
        {
            get { return _localFixedDrivesAll; }
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
        
        public EditableText ExternalPlayerPathBluRay
        {
            get
            {
                if (_externalPlayerPathBluRay == null)
                {
                    _externalPlayerPathBluRay = new EditableText();
                }
                return _externalPlayerPathBluRay;
            }
            set
            {
                _externalPlayerPathBluRay = value;
                FirePropertyChanged("ExternalPlayerPathBluRay");
            }
        }

        public EditableText ExternalPlayerPathHDDVD
        {
            get
            {
                if (_externalPlayerPathHDDVD == null)
                {
                    _externalPlayerPathHDDVD = new EditableText();
                }
                return _externalPlayerPathHDDVD;
            }
            set
            {
                _externalPlayerPathHDDVD = value;
                FirePropertyChanged("ExternalPlayerPathHDDVD");
            }
        }

        public EditableText ExternalPlayerPathAll
        {
            get
            {
                if (_externalPlayerPathAll == null)
                {
                    _externalPlayerPathAll = new EditableText();
                }
                return _externalPlayerPathAll;
            }
            set
            {
                _externalPlayerPathAll = value;
                FirePropertyChanged("ExternalPlayerPathAll");
            }
        }

        public EditableText TranscodingBufferDelay
        {
            get
            {
                if (_transcodeBufferDelay == null)
                {
                    _transcodeBufferDelay = new EditableText();
                }
                return _transcodeBufferDelay;
            }
            set
            {
                _transcodeBufferDelay = value;
                FirePropertyChanged("TranscodingBufferDelay");
            }
        }

        public EditableText ImpersonationUsername
        {
            get
            {
                if (_impersonationUsername == null)
                    _impersonationUsername = new EditableText();

                return _impersonationUsername;
            }
            set
            {
                _impersonationUsername = value;
                FirePropertyChanged("ImpersonationUsername");
            }
        }

        public EditableText ImpersonationPassword
        {
            get
            {
                if (_impersonationPassword == null)
                    _impersonationPassword = new EditableText();

                return _impersonationPassword;
            }
            set
            {
                _impersonationPassword = value;
                FirePropertyChanged("ImpersonationPassword");
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

        public Choice ExternalPlayerSelectionAll
        {
            get { return _externalPlayerSelectionAll; }
            set
            {
                _externalPlayerSelectionAll = value;
                FirePropertyChanged("ExternalPlayerSelectionAll");
            }
        }

        public Choice ExternalPlayerSelectionBluRay
        {
            get { return _externalPlayerSelectionBluRay; }
            set
            {
                _externalPlayerSelectionBluRay = value;
                FirePropertyChanged("ExternalPlayerSelectionBluRay");
            }
        }

        public Choice ExternalPlayerSelectionHDDVD
        {
            get { return _externalPlayerSelectionHDDVD; }
            set
            {
                _externalPlayerSelectionHDDVD = value;
                FirePropertyChanged("ExternalPlayerSelectionHDDVD");
            }
        }


        public Choice MainPageBackDropInterval
        {
            get { return _mainPageBackDropInterval; }
            set
            {
                _mainPageBackDropInterval = value;
                FirePropertyChanged("MainPageBackDropInterval");
            }
        }
        public Choice MainPageBackDropAlpha
        {
            get { return _mainPageBackDropAlpha; }
            set
            {
                _mainPageBackDropAlpha = value;
                FirePropertyChanged("MainPageBackDropAlpha");
            }
        }
        public Choice DetailsPageBackDropAlpha
        {
            get { return _detailsPageBackDropAlpha; }
            set
            {
                _detailsPageBackDropAlpha = value;
                FirePropertyChanged("DetailsPageBackDropAlpha");
            }
        }

        private const string DefaultDaemonToolsPath = @"Program Files\DAEMON Tools Lite\daemon.exe";
        private const string DefaultVirtualCloneDrivePath = @"Program Files\Elaborate Bytes\VirtualCloneDrive\VCDMount.exe";        

        public void LocateSelectedMounter()
        {           
            string driveLetterToScan = LocalFixedDrives.Chosen as String;
            DriveInfo dInfo = new DriveInfo(driveLetterToScan);

            string startPath = null;

            switch ((MountingTool.Tool)Enum.Parse(typeof(MountingTool.Tool), _ImageMountingSelection.Chosen.ToString()))
            {
                case MountingTool.Tool.None:
                    // don't do anything if there's no mounting tool selected
                    return;

                case MountingTool.Tool.DaemonTools:
                    startPath = DefaultDaemonToolsPath;
                    break;

                case  MountingTool.Tool.VirtualCloneDrive:
                    startPath = DefaultVirtualCloneDrivePath;
                    break;

                default:
                    return;
            }

            if (File.Exists(Path.Combine(dInfo.RootDirectory.FullName, startPath)))
            {
                MountingToolPath.Value = Path.Combine(dInfo.RootDirectory.FullName, startPath);
            }
            else
            {
                // let's scan all the folders for it
                OMLApplication.Current.IsBusy = true;
                Application.DeferredInvokeOnWorkerThread(delegate
                {
                    exePath = ScanAllFoldersForExecutable(dInfo.RootDirectory.FullName, Path.GetFileName(startPath));

                }, delegate
                {
                    OMLApplication.Current.IsBusy = false;
                    if (exePath.Length > 0)
                    {
                        OMLApplication.DebugLine("[Settings] Found Image Mounter: {0}", exePath);
                        MountingToolPath.Value = exePath;
                    }
                    else
                    {
                        AddInHost.Current.MediaCenterEnvironment.Dialog(
                            string.Format("The image mounter was not" +
                                          " found on the [{0}] drive.", driveLetterToScan),
                            "Failed to Find Image Mounter",
                            Microsoft.MediaCenter.DialogButtons.Ok,
                            5, true);
                    }
                }, null);
            }
        }

        private const string DefaultTMTPath = @"Program Files\Arcsoft\umcedvdplayer.exe";
        private const string DefaultPowerDVD8Path = @"Program Files\CyberLink\PowerDVD8\PowerDVD8.exe";
        private const string DefaultWinDVD9Path = @"Program Files\Corel\DVD9\WinDVD.exe";

        public void LocateExternalPlayerExecutable(Choice selector, EditableText textBox, Choice localFixedDrive)
        {
            string driveLetterToScan = localFixedDrive.Chosen as String;
            DriveInfo dInfo = new DriveInfo(driveLetterToScan);

            string startPath = null;

            switch ((ExternalPlayer.KnownPlayers)Enum.Parse(typeof(ExternalPlayer.KnownPlayers), selector.Chosen.ToString()))
            {               
                case ExternalPlayer.KnownPlayers.WinDVD9:
                    startPath = DefaultWinDVD9Path;
                    break;

                case ExternalPlayer.KnownPlayers.PowerDVD8:
                    startPath = DefaultPowerDVD8Path;
                    break;

                case ExternalPlayer.KnownPlayers.TotalMediaTheater:
                    startPath = DefaultTMTPath;
                    break;

                // don't do anything if there's no mounting tool selected
                case ExternalPlayer.KnownPlayers.None:
                case ExternalPlayer.KnownPlayers.Other:
                default:
                    return;
            }

            if (File.Exists(Path.Combine(dInfo.RootDirectory.FullName, startPath)))
            {
                textBox.Value = Path.Combine(dInfo.RootDirectory.FullName, startPath);
            }
            else
            {
                // let's scan all the folders for it
                OMLApplication.Current.IsBusy = true;
                Application.DeferredInvokeOnWorkerThread(delegate
                {
                    exePath = ScanAllFoldersForExecutable(dInfo.RootDirectory.FullName, Path.GetFileName(startPath));

                }, delegate
                {
                    OMLApplication.Current.IsBusy = false;
                    if (exePath.Length > 0)
                    {
                        OMLApplication.DebugLine("[Settings] Found Image Mounter: {0}", exePath);
                        textBox.Value = exePath;
                    }
                    else
                    {
                        AddInHost.Current.MediaCenterEnvironment.Dialog(
                            string.Format("The external player was not" +
                                          " found on the [{0}] drive.", driveLetterToScan),
                            "Failed to Find External Player",
                            Microsoft.MediaCenter.DialogButtons.Ok,
                            5, true);
                    }
                }, null);
            }            
        }

        public static void CleanupImagesFolder()
        {
            long bytesRemoved = 0;
            OMLApplication.Current.IsBusy = true;
            Application.DeferredInvokeOnWorkerThread(delegate
            {
                bytesRemoved = TitleCollection.CleanUnusedImages();
            }, delegate
            {
                OMLApplication.Current.IsBusy = false;
                if (bytesRemoved > 0)
                {
                    AddInHost.Current.MediaCenterEnvironment.Dialog(
                        string.Format("A total of {0} bytes have been cleaned up by removing old images",
                                      bytesRemoved),
                        "Image Cleanup Complete",
                        Microsoft.MediaCenter.DialogButtons.Ok,
                        5, true);
                }
                else
                {
                    AddInHost.Current.MediaCenterEnvironment.Dialog(
                        "No images have been removed, all images appeared to be in use.",
                        "Image Cleanup Complete",
                        Microsoft.MediaCenter.DialogButtons.Ok,
                        5, true);
                }
            }, null);
        }

        public static IList<DriveInfo> GetFileSystemDrives()
        {
            IList<DriveInfo> fixedDrives = new List<DriveInfo>();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drInfo in drives)
            {
                if (drInfo.DriveType == DriveType.Fixed)
                    fixedDrives.Add(drInfo);
            }

            return fixedDrives;
        }

        private static string ScanAllFoldersForExecutable(string dir, string executable)
        {
            if (!Directory.Exists(dir))
                return string.Empty;

            string tmtPath = string.Empty;
            DirectoryInfo dInfo;
            FileSystemInfo[] items = new FileSystemInfo[0]; // this just needs to be init'd
            try
            {
                dInfo = new DirectoryInfo(dir);
                items = dInfo.GetFileSystemInfos();
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Caught exception trying to scan {0}: {1}", dir, e.Message);
            }

            foreach (FileSystemInfo item in items)
            {
                if (item is DirectoryInfo)
                {
                    DirectoryInfo dirInfo = item as DirectoryInfo;
                    OMLApplication.DebugLine("[Settings] Scanning folder [{0}] for TMT", dirInfo.FullName);
                    tmtPath = ScanAllFoldersForExecutable(dirInfo.FullName, executable);
                    if (!string.IsNullOrEmpty(tmtPath))
                        return tmtPath;
                }

                if (item is FileInfo)
                {
                    FileInfo fInfo = item as FileInfo;
                    if (fInfo.Name.Equals(executable, StringComparison.OrdinalIgnoreCase))
                        return fInfo.FullName;
                }
            }
            return string.Empty;
        }

        private string exePath = string.Empty;
        EditableText _mountingToolPath = new EditableText();
        
        EditableText _externalPlayerPathBluRay = new EditableText();
        EditableText _externalPlayerPathHDDVD = new EditableText();
        EditableText _externalPlayerPathAll = new EditableText();

        OMLSettings _omlSettings = new OMLSettings();
        Choice _virtualDrive = new Choice();
        Choice _movieView = new Choice();
        Choice _movieSort = new Choice();
        Choice _coverArtRows = new Choice();
        Choice _coverArtSpacing = new Choice();
        Choice _detailsView = new Choice();
        BooleanChoice _showMovieDetails = new BooleanChoice();
        BooleanChoice _dimUnselectedCovers = new BooleanChoice();
        BooleanChoice _useOriginalCoverArt = new BooleanChoice();
        BooleanChoice _useOnScreenAlpha = new BooleanChoice();
        BooleanChoice _showWatchedIcon = new BooleanChoice();
        Choice _startPage = new Choice();
        Choice _startPageSubFilter = new Choice();
        Choice _uiLanguage = new Choice();
        Choice _ImageMountingSelection = new Choice();       
        
        Choice _externalPlayerSelectionAll = new Choice();
        Choice _externalPlayerSelectionBluRay = new Choice();
        Choice _externalPlayerSelectionHDDVD = new Choice();

        Choice _localFixedDrivesBluRay = new Choice();
        Choice _localFixedDrivesHDDVD = new Choice();
        Choice _localFixedDrivesAll = new Choice();

        Choice _LocalFixedDrives = new Choice();

        Choice _trailersDefinition = new Choice();        
        Choice _filtersToShow = new Choice();
        EditableText _transcodeBufferDelay = new EditableText();
        EditableText _impersonationUsername = new EditableText();
        EditableText _impersonationPassword = new EditableText();
        BooleanChoice _showFilterGenres = new BooleanChoice();
        BooleanChoice _transcodeAVIFiles = new BooleanChoice();
        BooleanChoice _transcodeMKVFiles = new BooleanChoice();
        BooleanChoice _transcodeOGMFiles = new BooleanChoice();
        BooleanChoice _preserveAudioOnTranscode = new BooleanChoice();
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
        BooleanChoice _showFilterUnwatched = new BooleanChoice();
        BooleanChoice _debugTranscoding = new BooleanChoice();
        //BooleanChoice _useMaximizer = new BooleanChoice();
        Choice _mainPageBackDropAlpha = new Choice();
        Choice _mainPageBackDropInterval = new Choice();
        Choice _detailsPageBackDropAlpha = new Choice();
    }
}
