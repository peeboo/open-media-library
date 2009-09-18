using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using OMLEngine;

namespace OMLSDK
{

    public class MetadataSearchManagement
    {
        private List<MetaDataPluginDescriptor> _metadataPlugins = new List<MetaDataPluginDescriptor>();
        
        public List<string> FanArt
        {
            get;
            set;
        }

        public MetadataSearchManagement(List<MetaDataPluginDescriptor> metadataPlugins)
        {
            _metadataPlugins = metadataPlugins;
        }

        /// <summary>
        /// Performs a metadata search using the data and plugin passed in.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="titletype"></param>
        /// <param name="titleNameSearch"></param>
        /// <param name="EpisodeName"></param>
        /// <param name="SeasonNo"></param>
        /// <param name="EpisodeNo"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool MetadataSearch(MetaDataPluginDescriptor metadata, TitleTypes titletype, string titleNameSearch, string EpisodeName, int? SeasonNo, int? EpisodeNo, out Title title)
        {
            title = null;

            if ((((titletype & TitleTypes.TVShow) != 0) ||
                ((titletype & TitleTypes.Season) != 0) ||
                ((titletype & TitleTypes.Episode) != 0)) &&
                ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsTVSearch) != 0))
            {
                // Perform a tv show search. This will return true if it has found an exact show match, false
                // if it finds multiple shows matching the show name.
                bool SearchTVShowOnly = false;

                if (((titletype & TitleTypes.Season) != 0) ||
                    ((titletype & TitleTypes.TVShow) != 0))
                {
                    SearchTVShowOnly = true;
                }
                if (metadata.PluginDLL.SearchForTVSeries(titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, 1, SearchTVShowOnly))
                {
                    // Requires a search drilldown
                    metadata.PluginDLL.SearchForTVDrillDown(1, EpisodeName, SeasonNo, EpisodeNo, 1);
                }
                title = metadata.PluginDLL.GetBestMatch();
                if (title != null)
                {
                    Utilities.DebugLine("[OMLSDK] Found episode " + title.Name + " using plugin " + metadata.DataProviderName);
                }
            }
            else
            {
                if ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsMovieSearch) != 0)
                {
                    metadata.PluginDLL.SearchForMovie(titleNameSearch, 1);
                    title = metadata.PluginDLL.GetBestMatch();
                    if (title != null)
                    {
                        Utilities.DebugLine("[OMLSDK] Found movie " + title.Name + " using plugin " + metadata.DataProviderName);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Looksup the preferred metadata on the passed in search criteria
        /// </summary>
        /// <param name="titletype"></param>
        /// <param name="coverArtOnly"></param>
        /// <param name="titleNameSearch"></param>
        /// <param name="EpisodeName"></param>
        /// <param name="SeasonNo"></param>
        /// <param name="EpisodeNo"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool MetadataSearchUsingPreferred(TitleTypes titletype, /*bool coverArtOnly,*/ string titleNameSearch, string EpisodeName, int? SeasonNo, int? EpisodeNo, out Title title)
        {
            title = null;

            string preferredplugin = OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies;
            
            if ((((titletype & TitleTypes.TVShow) != 0) ||
                ((titletype & TitleTypes.Season) != 0) ||
                ((titletype & TitleTypes.Episode) != 0)) && 
                (!string.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPluginTV)))
            {
                // If we are looking for an episode and there is a preferred setting for this, use it
                preferredplugin = OMLEngine.Settings.OMLSettings.DefaultMetadataPluginTV;
            }

            if (string.IsNullOrEmpty(preferredplugin)) return false;

            if (FanArt == null)
            {
                FanArt = new List<string>();
            }
            else
            {
                FanArt.Clear();
            }

            try
            {
                if (titleNameSearch != null)
                {

                    // Import metadata based on field mappings and configured default plugin
                    Dictionary<string, List<string>> mappings = OMLEngine.Settings.SettingsManager.MetaDataMap_PropertiesByPlugin();

                    // Loop through configured mappings
                    Type tTitle = typeof(Title);
                    MetaDataPluginDescriptor metadata;

                    bool loadedfanart = false;

                    foreach (KeyValuePair<string, List<string>> map in mappings)
                    {
                        try
                        {
                            if (map.Key == preferredplugin) continue;
                            metadata = _metadataPlugins.First(p => p.DataProviderName == map.Key);

                            MetadataSearch(metadata, titletype, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, out title);

                            if (title != null)
                            {
                                foreach (string property in map.Value)
                                {
                                    switch (property)
                                    {
                                        case "FanArt":
                                            if ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsBackDrops) != 0)
                                            {
                                                loadedfanart = true;
                                                FanArt.AddRange(metadata.PluginDLL.GetBackDropUrlsForTitle().ToList());
                                            }
                                            break;
                                        case "Genres":
                                            foreach (string genre in title.Genres.ToArray<string>())
                                                title.AddGenre(genre);
                                            break;
                                        default:
                                            Utilities.DebugLine("[OMLDatabaseEditor] Using value for " + property + " from plugin " + map.Key);
                                            System.Reflection.PropertyInfo prop = tTitle.GetProperty(property);
                                            prop.SetValue(title, prop.GetValue(title, null), null);
                                            break;
                                    }
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            Utilities.DebugLine("[OMLSDK] Processing date from {0} Caused an Exception {1}", map.Key, ex);

                        }
                    }
                    // Use default plugin for remaining fields
                    metadata = _metadataPlugins.First(p => p.DataProviderName == preferredplugin);

                    MetadataSearch(metadata, titletype, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, out title);

                    title.MetadataSourceName = metadata.DataProviderName;

                    if (title != null)
                    {
                        if (!loadedfanart)
                        {
                            if ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsBackDrops) != 0)
                            {
                                List<string> images = metadata.PluginDLL.GetBackDropUrlsForTitle();
                                if (images != null)
                                {
                                    FanArt.AddRange(metadata.PluginDLL.GetBackDropUrlsForTitle().ToList());
                                }
                            }
                        }
                    }

                    ApplyGenreMappings(title, true);

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLSDK] Exception {0}", ex);
                return false;
            }
        }

        /// <summary>
        /// Looksup the preferred metadata on the passed in title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool MetadataSearchUsingPreferred(Title title)
        {
            Title SearchResult;
            int? SeasonNo = null;
            int? EpisodeNo = null;
            bool retval = false;

            if (((title.TitleType & TitleTypes.Episode) != 0) ||
            ((title.TitleType & TitleTypes.Season) != 0) ||
            ((title.TitleType & TitleTypes.TVShow) != 0))
            {
                // TV Search
                if (title.SeasonNumber != null) SeasonNo = title.SeasonNumber.Value;
                if (title.EpisodeNumber != null) EpisodeNo = title.EpisodeNumber.Value;
                string Showname = null;

                // Try to find show name be looking up the folder structure.
                Title parenttitle = title;
                while ((title.TitleType & TitleTypes.Root) == 0)
                {
                    // Get parent
                    parenttitle = parenttitle.ParentTitle;
                    if ((parenttitle.TitleType & TitleTypes.TVShow) != 0)
                    {
                        Showname = parenttitle.Name;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(Showname))
                {
                    // Cannot find a show name in the folder structure
                    //return StartMetadataImport(title, plugin, coverArtOnly, title.Name, "", SeasonNo, EpisodeNo);
                    retval =  MetadataSearchUsingPreferred(title.TitleType, title.Name, "", SeasonNo, EpisodeNo, out SearchResult);
                }
                else
                {
                    //return StartMetadataImport(title, plugin, coverArtOnly, Showname, title.Name, SeasonNo, EpisodeNo);
                    retval =  MetadataSearchUsingPreferred(title.TitleType, Showname, title.Name, SeasonNo, EpisodeNo, out SearchResult);
                }
            }
            else
            {
                // Movie Search
                //return StartMetadataImport(title, plugin, coverArtOnly, title.Name, "", SeasonNo, EpisodeNo);
                retval =  MetadataSearchUsingPreferred(title.TitleType, title.Name, "", SeasonNo, EpisodeNo, out SearchResult);
            }


            if (retval)
            {
                // Successful lookup, process

                if (((title.TitleType & TitleTypes.Season) != 0) ||
                    ((title.TitleType & TitleTypes.TVShow) != 0) ||
                    ((title.TitleType & TitleTypes.Episode) != 0))
                {
                    // Use the preferred overwrite settings for TV
                    title.CopyMetadata(SearchResult, OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefTV,
                        OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNamePrefTV,
                        OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefTV);
                }
                else
                {
                    // Use the preferred overwrite settings for Movies
                    title.CopyMetadata(SearchResult, OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefMovies,
                        OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNamePrefMovies,
                        OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefMovies);
                }

                //LoadFanart(mds.FanArt, title); -- Do this in the calling program (may need to provide user feedback of progress
            }

            return retval;
        }


        private void ApplyGenreMappings(Title title, bool removeUnknownGenres)
        {
            List<String> genreList = new List<String>();

            int genreCount = TitleCollectionManager.GetAllGenreMetaDatas().Count();

            if (genreCount > 0)
            {
                genreList.AddRange(from gm in TitleCollectionManager.GetAllGenreMetaDatas()
                                   select gm.Name);

                Dictionary<string, string> genreIssuesList = new Dictionary<string, string>();
                Dictionary<string, string> genreChanges = new Dictionary<string, string>();

                foreach (string genre in title.Genres)
                {
                    string newGenre = genre.Trim();

                    if (!genreList.Contains(newGenre))
                    {
                        // Genre doesn't exists
                        if (OMLEngine.Settings.SettingsManager.GenreMap_GetMapping(newGenre) != null)
                        {
                            // Mapping already exists for genre
                            genreChanges[genre] = OMLEngine.Settings.SettingsManager.GenreMap_GetMapping(newGenre);
                        }
                        else
                        {
                            // Mapping doesn't exists for genre, try trimming off 'file' from the end just incase
                            if (newGenre.EndsWith("Film", true, CultureInfo.InvariantCulture))
                                newGenre = newGenre.Replace(" Film", "");

                            if (genreList.Contains(newGenre))
                                genreIssuesList[genre] = newGenre;
                            else
                            {
                                string match = genreList.FirstOrDefault(s => s.Split(' ').Intersect(newGenre.Split(' ')).Count() != 0);
                                genreIssuesList[genre] = match;
                            }
                        }
                    }
                }
                foreach (string genre in genreChanges.Keys)
                {
                    title.RemoveGenre(genre);

                    // Mapping contains empty string when user wants a specific genre ignored.
                    if (!String.IsNullOrEmpty(genreChanges[genre]) && !title.Genres.Contains(genreChanges[genre]))
                        title.AddGenre(genreChanges[genre]);
                }
                if (genreIssuesList.Keys.Count > 0)
                {
                    if (!removeUnknownGenres)
                    {
                        foreach (string genre in genreIssuesList.Keys)
                        {
                            if (title.Genres.Contains(genre))
                                title.RemoveGenre(genre);
                        }
                    }
                }
            }
        }
    }
}
