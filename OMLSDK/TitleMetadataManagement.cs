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

        public bool MetadataSearchUsingPreferred(TitleTypes titletype, bool coverArtOnly, string titleNameSearch, string EpisodeName, int? SeasonNo, int? EpisodeNo, out Title title)
        {
            title = null;

            string preferredplugin = OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin;
            
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
