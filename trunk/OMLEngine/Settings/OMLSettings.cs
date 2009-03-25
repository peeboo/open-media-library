using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Settings
{
    public static class OMLSettings
    {
        private const string InstanceName = "main";

        #region ViewOptions
        
        /*public static MovieView MovieView
        {
            get
            {
                string view = SettingsManager.GetSettingByName("MovieView", InstanceName);

                if (view == null || !Enum.IsDefined(typeof(MovieView), view))
                    return MovieView.CoverArt;

                return (MovieView)Enum.Parse(typeof(MovieView), view);
            }
            set
            {
                SettingsManager.SaveSettingByName("MovieView", value.ToString(), InstanceName);
            }
        }*/

        public static string MovieView
        {
            get { return SettingsManager.GetSettingByName("MovieView", InstanceName) ?? "Cover Art"; }
            set { SettingsManager.SaveSettingByName("MovieView", value, InstanceName); }
        }

        public static string DetailsView
        {
            get { return SettingsManager.GetSettingByName("DetailsView", InstanceName) ?? "Background Boxes"; }
            set { SettingsManager.SaveSettingByName("DetailsView", value, InstanceName); }
        }

        public static string MovieSort
        {
            get { return SettingsManager.GetSettingByName("MovieSort", InstanceName) ?? "Name Ascending"; }
            set { SettingsManager.SaveSettingByName("MovieSort", value, InstanceName); }
        }

        public static string StartPage
        {
            get { return SettingsManager.GetSettingByName("StartPage", InstanceName) ?? "OML Home"; }
            set { SettingsManager.SaveSettingByName("StartPage", value, InstanceName); }
        }

        public static string StartPageSubFilter
        {
            get { return SettingsManager.GetSettingByName("StartPageSubFilter", InstanceName) ?? string.Empty; }
            set { SettingsManager.SaveSettingByName("StartPageSubFilter", value, InstanceName); }
        }

        #endregion        

        #region Customize UI

        public static bool UseOriginalCoverArt
        {
            get { return SettingsManager.GetSettingByNameBool("UseOriginalCoverArt", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("UseOriginalCoverArt", value.ToString(), InstanceName); }
        }

        public static bool ShowMovieDetails
        {
            get { return SettingsManager.GetSettingByNameBool("ShowMovieDetails", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowMovieDetails", value.ToString(), InstanceName); }
        }

        public static bool DimUnselectedCovers
        {
            get { return SettingsManager.GetSettingByNameBool("DimUnselectedCovers", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("DimUnselectedCovers", value.ToString(), InstanceName); }
        }

        public static bool UseOnScreenAlphaJumper
        {
            get { return SettingsManager.GetSettingByNameBool("UseOnScreenAlphaJumper", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("UseOnScreenAlphaJumper", value.ToString(), InstanceName); }
        }

        public static bool ShowWatchedIcon
        {
            get { return SettingsManager.GetSettingByNameBool("ShowWatchedIcon", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowWatchedIcon", value.ToString(), InstanceName); }
        }

        public static int GalleryCoverArtRows
        {
            get { return SettingsManager.GetSettingByNameInt("GalleryCoverArtRows", InstanceName) ?? 3; }
            set { SettingsManager.SaveSettingByName("GalleryCoverArtRows", value.ToString(), InstanceName); }
        }

        public static int GalleryListRows
        {
            get { return SettingsManager.GetSettingByNameInt("GalleryListRows", InstanceName) ?? 7; }
            set { SettingsManager.SaveSettingByName("GalleryListRows", value.ToString(), InstanceName); }
        }

        public static int CoverArtSpacingHorizontal
        {
            get { return SettingsManager.GetSettingByNameInt("CoverArtSpacingHorizontal", InstanceName) ?? 0; }
            set { SettingsManager.SaveSettingByName("CoverArtSpacingHorizontal", value.ToString(), InstanceName); }
        }

        public static int CoverArtSpacingVertical
        {
            get { return SettingsManager.GetSettingByNameInt("CoverArtSpacingVertical", InstanceName) ?? 0; }
            set { SettingsManager.SaveSettingByName("CoverArtSpacingVertical", value.ToString(), InstanceName); }
        }

        public static float MainPageBackDropAlphaValue
        {
            get { return SettingsManager.GetSettingByNameFloat("MainPageBackDropAlphaValue", InstanceName) ?? 0.3f; }
            set { SettingsManager.SaveSettingByName("MainPageBackDropAlphaValue", value.ToString(), InstanceName); }
        }

        public static int MainPageBackDropIntervalValue
        {
            get { return SettingsManager.GetSettingByNameInt("MainPageBackDropIntervalValue", InstanceName) ?? 5; }
            set { SettingsManager.SaveSettingByName("MainPageBackDropIntervalValue", value.ToString(), InstanceName); }
        }

        public static float DetailsPageBackDropAlphaValue
        {
            get { return SettingsManager.GetSettingByNameFloat("DetailsPageBackDropAlphaValue", InstanceName) ?? 0.3f; }
            set { SettingsManager.SaveSettingByName("DetailsPageBackDropAlphaValue", value.ToString(), InstanceName); }
        }

        #endregion

        #region Filters

        public static IList<string> MainFiltersToShow
        {
            get
            {
                return SettingsManager.GetSettingByNameListString("MainFiltersToShow", InstanceName) ?? new List<string>(){
                                                                                                                "Genres",
                                                                                                                "Directors",
                                                                                                                "Actors",
                                                                                                                "Runtime",
                                                                                                                "Country",
                                                                                                                "Parental Rating",
                                                                                                                "Tags",
                                                                                                                "User Rating",
                                                                                                                "Year",
                                                                                                                "Date Added",
                                                                                                                "Format",
                                                                                                                "Trailers" };
            }
            set { SettingsManager.SaveSettingByName("MainFiltersToShow", value, InstanceName); }
        }

        public static bool ShowFilterGenres
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterGenres", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterGenres", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterDirectors
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterDirectors", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterDirectors", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterUnwatched
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterUnwatched", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterUnwatched", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterActors
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterActors", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowFilterActors", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterRuntime
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterRuntime", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterRuntime", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterCountry
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterCountry", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowFilterCountry", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterParentalRating
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterParentalRating", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowFilterParentalRating", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterTags
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterTags", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowFilterTags", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterUserRating
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterUserRating", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterUserRating", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterYear
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterYear", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterYear", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterDateAdded
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterDateAdded", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterDateAdded", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterFormat
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterFormat", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowFilterFormat", value.ToString(), InstanceName); }
        }

        public static bool ShowFilterTrailers
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFilterTrailers", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("ShowFilterTrailers", value.ToString(), InstanceName); }
        }

        #endregion

        #region Image Mounting

        public static int MountingToolSelection
        {
            get { return SettingsManager.GetSettingByNameInt("MountingToolSelection", InstanceName) ?? 0; }
            set { SettingsManager.SaveSettingByName("MountingToolSelection", value.ToString(), InstanceName); }
        }

        public static string MountingToolPath
        {
            get { return SettingsManager.GetSettingByName("MountingToolPath", InstanceName) ?? string.Empty; }
            set { SettingsManager.SaveSettingByName("MountingToolPath", value, InstanceName); }
        }

        public static string VirtualDiscDrive
        {
            get { return SettingsManager.GetSettingByName("VirtualDiscDrive", InstanceName) ?? "M"; }
            set { SettingsManager.SaveSettingByName("VirtualDiscDrive", value, InstanceName); }
        }

        public static int VirtualDiscDriveNumber
        {
            get { return SettingsManager.GetSettingByNameInt("VirtualDiscDriveNumber", InstanceName) ?? 0; }
            set { SettingsManager.SaveSettingByName("VirtualDiscDriveNumber", value.ToString(), InstanceName); }
        }

        #endregion

        #region Trailers

        public static string TrailersDefinition
        {
            get { return SettingsManager.GetSettingByName("TrailersDefinition", InstanceName) ?? "Hi"; }
            set { SettingsManager.SaveSettingByName("TrailersDefinition", value, InstanceName); }
        }

        #endregion

        #region External Players

        public static IList<string> ExternalPlayerMapping
        {
            get { return SettingsManager.GetSettingByNameListString("ExternalPlayerMapping", InstanceName) ?? new List<string>(0); }
            set { SettingsManager.SaveSettingByName("ExternalPlayerMapping", value, InstanceName); }
        }

        #endregion

        #region Extender / Transcover

        public static bool PreserveAudioOnTranscode
        {
            get { return SettingsManager.GetSettingByNameBool("PreserveAudioOnTranscode", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("PreserveAudioOnTranscode", value.ToString(), InstanceName); }
        }

        public static bool TranscodeAVIFiles
        {
            get { return SettingsManager.GetSettingByNameBool("TranscodeAVIFiles", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("TranscodeAVIFiles", value.ToString(), InstanceName); }
        }

        public static bool TranscodeMKVFiles
        {
            get { return SettingsManager.GetSettingByNameBool("TranscodeMKVFiles", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("TranscodeMKVFiles", value.ToString(), InstanceName); }
        }

        public static bool TranscodeOGMFiles
        {
            get { return SettingsManager.GetSettingByNameBool("TranscodeOGMFiles", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("TranscodeOGMFiles", value.ToString(), InstanceName); }
        }

        public static bool FlipFourCCCode
        {
            get { return SettingsManager.GetSettingByNameBool("FlipFourCCCode", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("FlipFourCCCode", value.ToString(), InstanceName); }
        }

        public static int TranscodeBufferDelay
        {
            get { return SettingsManager.GetSettingByNameInt("TranscodeBufferDelay", InstanceName) ?? 7; }
            set { SettingsManager.SaveSettingByName("TranscodeBufferDelay", value.ToString(), InstanceName); }
        }

        public static bool DebugTranscoding
        {
            get { return SettingsManager.GetSettingByNameBool("DebugTranscoding", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("DebugTranscoding", value.ToString(), InstanceName); }
        }

        public static bool Extender_UseAsx
        {
            get { return SettingsManager.GetSettingByNameBool("Extender_UseAsx", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("Extender_UseAsx", value.ToString(), InstanceName); }
        }

        public static bool Extender_MergeVOB
        {
            get { return SettingsManager.GetSettingByNameBool("Extender_MergeVOB", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("Extender_MergeVOB", value.ToString(), InstanceName); }
        }

        #endregion

        public static string ImpersonationUsername
        {
            get { return SettingsManager.GetSettingByName("ImpersonationUsername", InstanceName) ?? string.Empty; }
            set { SettingsManager.SaveSettingByName("ImpersonationUsername", value, InstanceName); }
        }

        public static string ImpersonationPassword
        {
            get { return SettingsManager.GetSettingByName("ImpersonationPassword", InstanceName) ?? string.Empty; }
            set { SettingsManager.SaveSettingByName("ImpersonationPassword", value, InstanceName); }
        }      

        public static bool CopyImages
        {
            get { return SettingsManager.GetSettingByNameBool("CopyImages", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("CopyImages", value.ToString(), InstanceName); }
        }

        /*public static bool ShowFileLocation
        {
            get { return SettingsManager.GetSettingByNameBool("ShowFileLocation", InstanceName); }
            set { SettingsManager.SaveSettingByName("ShowFileLocation", value.ToString(), InstanceName); }
        }*/

        public static string UILanguage
        {
            get { return SettingsManager.GetSettingByName("UILanguage", InstanceName) ?? string.Empty; }
            set { SettingsManager.SaveSettingByName("UILanguage", value, InstanceName); }
        }             
                   
        public static string AppleTrailerFidelity
        {
            get { return SettingsManager.GetSettingByName("AppleTrailerFidelity", InstanceName) ?? "Hi"; }
            set { SettingsManager.SaveSettingByName("AppleTrailerFidelity", value, InstanceName); }
        }                             

        /*
        public string ActorView
        {
            get { return SettingsManager.GetSettingByName("ActorView", InstanceName); }
            set { SettingsManager.SaveSettingByName("ActorView", value, InstanceName); }
        }

        public string DirectorView
        {
            get { return SettingsManager.GetSettingByName("DirectorView", InstanceName); }
            set { SettingsManager.SaveSettingByName("DirectorView", value, InstanceName); }
        }

        public string DirectorView
        {
            get { return SettingsManager.GetSettingByName("DirectorView", InstanceName); }
            set { SettingsManager.SaveSettingByName("DirectorView", value, InstanceName); }
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
        }*/
       
        
    /*
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
        }*/
        /*
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
        }*/

        

        /*public int CoverArtWidth
        {
            get { return SettingsManager.GetSettingByNameInt("CoverArtWidth", InstanceName); }
            set { SettingsManager.SaveSettingByName("CoverArtWidth", value.ToString(), InstanceName); }
        }

        public int CoverArtHeight
        {
            get { return SettingsManager.GetSettingByNameInt("CoverArtHeight", InstanceName); }
            set { SettingsManager.SaveSettingByName("CoverArtHeight", value.ToString(), InstanceName); }
        }

        public int CoverArtHeight
        {
            get { return SettingsManager.GetSettingByNameInt("CoverArtHeight", InstanceName); }
            set { SettingsManager.SaveSettingByName("CoverArtHeight", value.ToString(), InstanceName); }
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
        
        public string NameAscendingSort
        {
            get { return Properties.Settings.Default.NameAscendingSort; }
            set
            {
                Properties.Settings.Default.NameAscendingSort = value;
                Properties.Settings.Default.Save();
            }
        }
        */
                                             

        /*
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
         */       
        
    }
}
