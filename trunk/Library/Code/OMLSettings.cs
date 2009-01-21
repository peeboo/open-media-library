using System;
using System.Collections.Specialized;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class OMLSettings
    {
        public OMLSettings()
        {
            Utilities.DebugLine("[OMLSettings] OMLSettings created");
        }

        #region properties

        #region OMLEngine Settings
        public string MountingToolPath
        {
            get { return OMLEngine.Properties.Settings.Default.MountingToolPath; }
            set
            {
                OMLEngine.Properties.Settings.Default.MountingToolPath = value;
                OMLEngine.Properties.Settings.Default.Save();
            }
        }
        public int MountingToolSelection
        {
            get { return OMLEngine.Properties.Settings.Default.MountingToolSelection;  }
            set
            {
                OMLEngine.Properties.Settings.Default.MountingToolSelection = value;
                OMLEngine.Properties.Settings.Default.Save();
            }
        }
        public string VirtualDiscDrive
        {
            get { return OMLEngine.Properties.Settings.Default.VirtualDiscDrive; }
            set
            {
                OMLEngine.Properties.Settings.Default.VirtualDiscDrive = value;
                OMLEngine.Properties.Settings.Default.Save();
            }
        }
        public int VirtualDiscDriveNumber
        {
            get { return OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber; }
            set
            {
                OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber = value;
                OMLEngine.Properties.Settings.Default.Save();
            }
        }
        public bool CopyImages
        {
            get { return OMLEngine.Properties.Settings.Default.CopyImages; }
            set
            {
                OMLEngine.Properties.Settings.Default.CopyImages = value;
                OMLEngine.Properties.Settings.Default.Save();
            }
        }
        #endregion

        #region Library Settings

        public bool PreserveAudioOnTranscode
        {
            get { return OMLTranscoder.Properties.Settings.Default.PreserveAudioOnTranscode; }
            set
            {
                OMLTranscoder.Properties.Settings.Default.PreserveAudioOnTranscode = value;
                OMLTranscoder.Properties.Settings.Default.Save();
            }
        }
        public bool TranscodeAVIFiles
        {
            get { return Properties.Settings.Default.TranscodeAVIFiles; }
            set
            {
                Properties.Settings.Default.TranscodeAVIFiles = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool TranscodeMKVFiles
        {
            get { return Properties.Settings.Default.TranscodeMKVFiles; }
            set
            {
                Properties.Settings.Default.TranscodeMKVFiles = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool TranscodeOGMFiles
        {
            get { return Properties.Settings.Default.TranscodeOGMFiles; }
            set
            {
                Properties.Settings.Default.TranscodeOGMFiles = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool FlipFourCCCode
        {
            get { return Properties.Settings.Default.FlipFourCCCode; }
            set
            {
                Properties.Settings.Default.FlipFourCCCode = value;
                Properties.Settings.Default.Save();
            }
        }

        public StringCollection ExternalPlayerMapping
        {
            get { return Properties.Settings.Default.ExternalPlayerMapping; }
            set
            {
                Properties.Settings.Default.ExternalPlayerMapping = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool UseMaximizer
        {
            get { return Properties.Settings.Default.UseMaximizer; }
            set
            {
                Properties.Settings.Default.UseMaximizer = value;
                Properties.Settings.Default.Save();
            }
        }

        public StringCollection MainFiltersToShow
        {
            get { return Properties.Settings.Default.MainFiltersToShow; }
            set
            {
                Properties.Settings.Default.MainFiltersToShow = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool ShowFileLocation
        {
            get { return Properties.Settings.Default.ShowFileLocation; }
            set
            {
                Properties.Settings.Default.ShowFileLocation = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool DimUnselectedCovers
        {
            get { return Properties.Settings.Default.DimUnselectedCovers; }
            set
            {
                Properties.Settings.Default.DimUnselectedCovers = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool ShowMovieDetails
        {
            get { return Properties.Settings.Default.ShowMovieDetails; }
            set
            {
                Properties.Settings.Default.ShowMovieDetails = value;
                Properties.Settings.Default.Save();
            }
        }
        public string MovieView
        {
            get { return Properties.Settings.Default.MovieView; }
            set
            {
                Properties.Settings.Default.MovieView = value;
                Properties.Settings.Default.Save();
            }
        }
        public string ActorView
        {
            get { return Properties.Settings.Default.ActorView; }
            set
            {
                Properties.Settings.Default.ActorView = value;
                Properties.Settings.Default.Save();
            }
        }
        public string DirectorView
        {
            get { return Properties.Settings.Default.DirectorView; }
            set
            {
                Properties.Settings.Default.DirectorView = value;
                Properties.Settings.Default.Save();
            }
        }
        public string GenreView
        {
            get { return Properties.Settings.Default.GenreView; }
            set
            {
                Properties.Settings.Default.GenreView = value;
                Properties.Settings.Default.Save();
            }
        }
        public string DateAddedView
        {
            get { return Properties.Settings.Default.DateAddedView; }
            set
            {
                Properties.Settings.Default.DateAddedView = value;
                Properties.Settings.Default.Save();
            }
        }
        public string YearView
        {
            get { return Properties.Settings.Default.YearView; }
            set
            {
                Properties.Settings.Default.YearView = value;
                Properties.Settings.Default.Save();
            }
        }
        public string StartPage
        {
            get { return Properties.Settings.Default.StartPage; }
            set
            {
                Properties.Settings.Default.StartPage = value;
                Properties.Settings.Default.Save();
            }
        }

        public string StartPageSubFilter
        {
            get { return Properties.Settings.Default.StartPageSubFilter; }
            set
            {
                Properties.Settings.Default.StartPageSubFilter = value;
                Properties.Settings.Default.Save();
            }
        }
        public string MovieSort
        {
            get { return Properties.Settings.Default.MovieSort; }
            set
            {
                Properties.Settings.Default.MovieSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public string ActorSort
        {
            get { return Properties.Settings.Default.ActorSort; }
            set
            {
                Properties.Settings.Default.ActorSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public string DirectorSort
        {
            get { return Properties.Settings.Default.DirectorSort; }
            set
            {
                Properties.Settings.Default.DirectorSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public string YearSort
        {
            get { return Properties.Settings.Default.YearSort; }
            set
            {
                Properties.Settings.Default.YearSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public string DateAddedSort
        {
            get { return Properties.Settings.Default.DateAddedSort; }
            set
            {
                Properties.Settings.Default.DateAddedSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool SortFullString
        {
            get { return Properties.Settings.Default.SortFullString; }
            set
            {
                Properties.Settings.Default.SortFullString = value;
                Properties.Settings.Default.Save();
            }
        }
        public string GenreSort
        {
            get { return Properties.Settings.Default.GenreSort; }
            set
            {
                Properties.Settings.Default.GenreSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public float DetailsLeftAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor; }
            set
            {
                Properties.Settings.Default.DetailsLeftAnchor = value;
                Properties.Settings.Default.Save();
            }
        }
        public float DetailsTopAnchor
        {
            get { return Properties.Settings.Default.DetailsTopAnchor; }
            set
            {
                Properties.Settings.Default.DetailsTopAnchor = value;
                Properties.Settings.Default.Save();
            }
        }
        public int DetailsLeftOffset
        {
            get { return Properties.Settings.Default.DetailsLeftOffset; }
            set
            {
                Properties.Settings.Default.DetailsLeftOffset = value;
                Properties.Settings.Default.Save();
            }
        }
        public int DetailsTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset; }
            set
            {
                Properties.Settings.Default.DetailsTopOffset = value;
                Properties.Settings.Default.Save();
            }
        }
        public float BottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor; }
            set
            {
                Properties.Settings.Default.BottomAnchor = value;
                Properties.Settings.Default.Save();
            }
        }
        public int BottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset; }
            set
            {
                Properties.Settings.Default.BottomOffset = value;
                Properties.Settings.Default.Save();
            }
        }
        public int GalleryCoverArtRows
        {
            get { return Properties.Settings.Default.GalleryCoverArtRows; }
            set
            {
                Properties.Settings.Default.GalleryCoverArtRows = value;
                Properties.Settings.Default.Save();
            }
        }
        public int GalleryListRows
        {
            get { return Properties.Settings.Default.GalleryListRows; }
            set
            {
                Properties.Settings.Default.GalleryListRows = value;
                Properties.Settings.Default.Save();
            }
        }
        public int CoverArtWidth
        {
            get { return Properties.Settings.Default.CoverArtWidth; }
            set
            {
                Properties.Settings.Default.CoverArtWidth = value;
                Properties.Settings.Default.Save();
            }
        }
        public int CoverArtHeight
        {
            get { return Properties.Settings.Default.CoverArtHeight; }
            set
            {
                Properties.Settings.Default.CoverArtHeight = value;
                Properties.Settings.Default.Save();
            }
        }
        public int ListItemWidth
        {
            get { return Properties.Settings.Default.ListItemWidth; }
            set
            {
                Properties.Settings.Default.ListItemWidth = value;
                Properties.Settings.Default.Save();
            }
        }
        public int ListItemHeight
        {
            get { return Properties.Settings.Default.ListItemHeight; }
            set
            {
                Properties.Settings.Default.ListItemHeight = value;
                Properties.Settings.Default.Save();
            }
        }
        public string UserRatingSort
        {
            get { return Properties.Settings.Default.UserRatingSort; }
            set
            {
                Properties.Settings.Default.UserRatingSort = value;
                Properties.Settings.Default.Save();
            }
        }
        public float RightAnchor
        {
            get { return Properties.Settings.Default.RightAnchor; }
            set
            {
                Properties.Settings.Default.RightAnchor = value;
                Properties.Settings.Default.Save();
            }
        }
        public int RightOffset
        {
            get { return Properties.Settings.Default.RightOffset; }
            set
            {
                Properties.Settings.Default.RightOffset = value;
                Properties.Settings.Default.Save();
            }
        }
        public int CoverArtSpacingHorizontal
        {
            get { return Properties.Settings.Default.CoverArtSpacingHorizontal; }
            set
            {
                Properties.Settings.Default.CoverArtSpacingHorizontal = value;
                Properties.Settings.Default.Save();
            }
        }
        public int CoverArtSpacingVertical
        {
            get { return Properties.Settings.Default.CoverArtSpacingVertical; }
            set
            {
                Properties.Settings.Default.CoverArtSpacingVertical = value;
                Properties.Settings.Default.Save();
            }
        }
        public string NameAscendingSort
        {
            get { return Properties.Settings.Default.NameAscendingSort; }
            set
            {
                Properties.Settings.Default.NameAscendingSort = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool UseOriginalCoverArt
        {
            get { return Properties.Settings.Default.UseOriginalCoverArt; }
            set
            {
                Properties.Settings.Default.UseOriginalCoverArt = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool Extender_UseAsx
        {
            get { return Properties.Settings.Default.Extender_UseAsx; }
            set
            {
                Properties.Settings.Default.Extender_UseAsx = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool Extender_MergeVOB
        {
            get { return Properties.Settings.Default.Extender_MergeVOB; }
            set
            {
                Properties.Settings.Default.Extender_MergeVOB = value;
                Properties.Settings.Default.Save();
            }
        }
        /// <summary>
        /// Gets or sets the id (CultureInfo.Name) of the selected UI language. If not set the system's UI culture should be used.
        /// </summary>
        public string UILanguage
        {
            get { return Properties.Settings.Default.UILanguage; }
            set
            {
                Properties.Settings.Default.UILanguage = value;
                Properties.Settings.Default.Save();
            }
        }
        public string TrailersDefinition
        {
            get { return Properties.Settings.Default.AppleTrailerFidelity; }
            set
            {
                Properties.Settings.Default.AppleTrailerFidelity = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterGenres
        {
            get { return Properties.Settings.Default.ShowFilterGenres; }
            set
            {
                Properties.Settings.Default.ShowFilterGenres = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterDirectors
        {
            get { return Properties.Settings.Default.ShowFilterDirectors; }
            set
            {
                Properties.Settings.Default.ShowFilterDirectors = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterUnwatched
        {
            get { return Properties.Settings.Default.ShowFilterUnwatched; }
            set
            {
                Properties.Settings.Default.ShowFilterUnwatched = value;
                Properties.Settings.Default.Save();
            }
        }


        public bool ShowFilterActors
        {
            get { return Properties.Settings.Default.ShowFilterActors; }
            set
            {
                Properties.Settings.Default.ShowFilterActors = value;
                Properties.Settings.Default.Save();
            }
        }


        public bool ShowFilterRuntime
        {
            get { return Properties.Settings.Default.ShowFilterRuntime; }
            set
            {
                Properties.Settings.Default.ShowFilterRuntime = value;
                Properties.Settings.Default.Save();
            }
        }


        public bool ShowFilterCountry
        {
            get { return Properties.Settings.Default.ShowFilterCountry; }
            set
            {
                Properties.Settings.Default.ShowFilterCountry = value;
                Properties.Settings.Default.Save();
            }
        }


        public bool ShowFilterParentalRating
        {
            get { return Properties.Settings.Default.ShowFilterParentalRating; }
            set
            {
                Properties.Settings.Default.ShowFilterParentalRating = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterTags
        {
            get { return Properties.Settings.Default.ShowFilterTags; }
            set
            {
                Properties.Settings.Default.ShowFilterTags = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterUserRating
        {
            get { return Properties.Settings.Default.ShowFilterUserRating; }
            set
            {
                Properties.Settings.Default.ShowFilterUserRating = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterYear
        {
            get { return Properties.Settings.Default.ShowFilterYear; }
            set
            {
                Properties.Settings.Default.ShowFilterYear = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterDateAdded
        {
            get { return Properties.Settings.Default.ShowFilterDateAdded; }
            set
            {
                Properties.Settings.Default.ShowFilterDateAdded = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterFormat
        {
            get { return Properties.Settings.Default.ShowFilterFormat; }
            set
            {
                Properties.Settings.Default.ShowFilterFormat = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowFilterTrailers
        {
            get { return Properties.Settings.Default.ShowFilterTrailers; }
            set
            {
                Properties.Settings.Default.ShowFilterTrailers = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool ShowWatchedIcon
        {
            get { return Properties.Settings.Default.ShowWatchedIcon; }
            set
            {
                Properties.Settings.Default.ShowWatchedIcon = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool UseOnScreenAlphaJumper
        {
            get
            {
                return Properties.Settings.Default.UseOnScreenAlphaJumper;
            }
            set
            {
                Properties.Settings.Default.UseOnScreenAlphaJumper = value;
                Properties.Settings.Default.Save();
            }
        }

        public int TranscodeBufferDelay
        {
            get
            {
                return Properties.Settings.Default.TranscodeBufferDelay;
            }
            set
            {
                Properties.Settings.Default.TranscodeBufferDelay = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool DebugTranscoding
        {
            get
            {
                return Properties.Settings.Default.DebugTranscoding;
            }
            set
            {
                Properties.Settings.Default.DebugTranscoding = value;
                Properties.Settings.Default.Save();
            }
        }

        public float MainPageBackDropAlphaValue
        {
            get { return Properties.Settings.Default.MainPageBackDropAlpha; }
            set
            {
                Properties.Settings.Default.MainPageBackDropAlpha = value;
                Properties.Settings.Default.Save();
            }
        }

        public int MainPageBackDropIntervalValue
        {
            get { return Properties.Settings.Default.MainPageBackDropRotationInSeconds; }
            set
            {
                Properties.Settings.Default.MainPageBackDropRotationInSeconds = value;
                Properties.Settings.Default.Save();
                if (OMLApplication.Current.mainBackgroundTimer != null)
                {
                    OMLApplication.Current.mainBackgroundTimer.Stop();
                    OMLApplication.Current.mainBackgroundTimer.Enabled = false;
                    OMLApplication.Current.mainBackgroundTimer.Interval =
                        Properties.Settings.Default.MainPageBackDropRotationInSeconds * 1000;
                    OMLApplication.Current.mainBackgroundTimer.Enabled = true;
                    OMLApplication.Current.mainBackgroundTimer.Start();
                }
            }
        }

        public float DetailsPageBackDropAlphaValue
        {
            get { return Properties.Settings.Default.DetailsPageBackDropAlpha; }
            set
            {
                Properties.Settings.Default.DetailsPageBackDropAlpha = value;
                Properties.Settings.Default.Save();
            }
        }


        #endregion

        #endregion
    }
}
