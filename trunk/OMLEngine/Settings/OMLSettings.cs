using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine.FileSystem;

namespace OMLEngine.Settings
{
    public static class OMLSettings
    {
        private const string InstanceName = "main";

        #region ViewOptions

        public static bool ShowUnwatchedIcon
        {
            get { return SettingsManager.GetSettingByNameBool("ShowUnwatchedIcon", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowUnwatchedIcon", value.ToString(), InstanceName); }
        }
        
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
                return  new List<string>(){     "Unwatched",
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
                                                "Trailers"};
            }            
        }

        /// <summary>
        /// Custom users filters that can exist on the main view
        /// </summary>
        public static IList<UserFilter> UserFilters
        {
            get
            {
                IList<string> userFilterStrings = SettingsManager.GetSettingByNameListString("UserFilters", InstanceName) ?? new List<string>() { };

                List<UserFilter> userFilters = new List<UserFilter>(userFilterStrings.Count);

                foreach (string filter in userFilterStrings)
                {
                    userFilters.Add(new UserFilter(filter));
                }

                return userFilters.AsReadOnly();
            }
            set
            {
                List<string> serializedFilters = new List<string>();

                foreach (UserFilter filter in value)
                {
                    string serializedFilter = filter.ToString();

                    if (! string.IsNullOrEmpty(serializedFilter))
                        serializedFilters.Add(filter.ToString());
                }

                SettingsManager.SaveSettingByName("UserFilters", serializedFilters, InstanceName);
            }
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

        //public static string MountingToolSelection
        public static MountingTool.Tool MountingToolSelection
        {
            get {
                string toolstring = SettingsManager.GetSettingByName("MountingToolSelection", InstanceName) ?? string.Empty;
                if (!string.IsNullOrEmpty(toolstring))
                {
                    MountingTool.Tool tool = (MountingTool.Tool)Enum.Parse(typeof(MountingTool.Tool), toolstring);
                    return tool;
                }
                else
                {
                    return MountingTool.Tool.None;
                }
            }
            set { 
                SettingsManager.SaveSettingByName("MountingToolSelection", value.ToString(), InstanceName); 
            }
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

        #region Auto Disk Scanning

        public static bool AutoScanDiskOnAdding
        {
            // Only applies to DBEditor
            get { return SettingsManager.GetSettingByNameBool("AutoScanDiskOnAdding", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("AutoScanDiskOnAdding", value.ToString(), InstanceName); }
        }

        public static bool ScanDiskRollInfoToTitle
        {
            // On scanning disk file to get res, aspect ratio etc, roll these up into the title record
            get { return SettingsManager.GetSettingByNameBool("ScanDiskRollInfoToTitle", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ScanDiskRollInfoToTitle", value.ToString(), InstanceName); }
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


        #region DBEditor

        public static string DBEditorSkin
        {
            get { return SettingsManager.GetSettingByName("DBEditorSkin", InstanceName) ?? "Blue"; }
            set { SettingsManager.SaveSettingByName("DBEditorSkin", value, InstanceName); }
        }

        public static bool TreatFoldersAsTitles
        {
            // File scanner import setting
            get { return SettingsManager.GetSettingByNameBool("TreatFoldersAsTitles", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("TreatFoldersAsTitles", value.ToString(), InstanceName); }
        }

        public static bool AddParentFoldersToTitleName
        {
            // File scanner import setting
            get { return SettingsManager.GetSettingByNameBool("AddParentFoldersToTitleName", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("AddParentFoldersToTitleName", value.ToString(), InstanceName); }
        }

        public static bool UseMPAAList
        {
            get { return SettingsManager.GetSettingByNameBool("UseMPAAList", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("UseMPAAList", value.ToString(), InstanceName); }
        }

        public static bool UseGenreList
        {
            get { return SettingsManager.GetSettingByNameBool("UseGenreList", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("UseGenreList", value.ToString(), InstanceName); }
        }

        public static string DefaultMetadataPlugin
        {
            get { return SettingsManager.GetSettingByName("DefaultMetadataPlugin", InstanceName) ?? ""; }
            set { SettingsManager.SaveSettingByName("DefaultMetadataPlugin", value, InstanceName); }
        }

        public static bool TitledFanArtFolder
        {
            get { return SettingsManager.GetSettingByNameBool("TitledFanArtFolder", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("TitledFanArtFolder", value.ToString(), InstanceName); }
        }

        public static string TitledFanArtPath
        {
            get { return SettingsManager.GetSettingByName("TitledFanArtPath", InstanceName) ?? ""; }
            set { SettingsManager.SaveSettingByName("TitledFanArtPath", value, InstanceName); }
        }

        public static bool ShowSubFolderTitles
        {
            get { return SettingsManager.GetSettingByNameBool("ShowSubFolderTitles", InstanceName) ?? false; }
            set { SettingsManager.SaveSettingByName("ShowSubFolderTitles", value.ToString(), InstanceName); }
        }

        public static string DBETitleSortField
        {
            get { return SettingsManager.GetSettingByName("DBETitleSortField", InstanceName) ?? ""; }
            set { SettingsManager.SaveSettingByName("DBETitleSortField", value, InstanceName); }
        }
        public static bool DBETitleSortAsc
        {
            get { return SettingsManager.GetSettingByNameBool("DBETitleSortAsc", InstanceName) ?? true; }
            set { SettingsManager.SaveSettingByName("DBETitleSortAsc", value.ToString(), InstanceName); }
        }
        #endregion

        public static string MPAARatings
        {
            get { return SettingsManager.GetSettingByName("MPAARatings", InstanceName) ?? "NR|G|PG-13|PG|R|NC-17"; }
            set { SettingsManager.SaveSettingByName("MPAARatings", value, InstanceName); }
        }

        
        public static string Tags
        {
            get { return SettingsManager.GetSettingByName("Tags", InstanceName) ?? ""; }
            set { SettingsManager.SaveSettingByName("Tags", value, InstanceName); }
        }
         
        //public static string AppleTrailerFidelity
        //{
        //    get { return SettingsManager.GetSettingByName("AppleTrailerFidelity", InstanceName) ?? "Hi"; }
        //    set { SettingsManager.SaveSettingByName("AppleTrailerFidelity", value, InstanceName); }
        //} 
 
    }
}
