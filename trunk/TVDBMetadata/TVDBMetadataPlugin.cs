using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;        // need this for OML Title class
using OMLSDK;           // need this for the IOMLMetadataPlugin
using System.IO;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Linq;


namespace TVDBMetadata
{
    public class TheTVDBDbResult
    {
        public Title Title { get; set; }
        public int SeasonNo { get; set; }
        public int EpisodeNo { get; set; }
        public int NameMatchConfidence { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrlThumb { get; set; }

        public TheTVDBDbResult()
        {
            Title = new Title();
        }
    }


    public class TVDBMetadataPlugin : IOMLMetadataPlugin
    {
        private const string DEFAULT_API_KEY = "FC18699D6C4514F7";
        private string API_KEY;
        private const string API_KEY_HIDDEN_TEXT = "[Enter your API key here to override the OML key]";

        private bool FanartEnabled = false;
        private List<string> BackDrops = null;

        List<string> xmlmirrors;
        List<string> bannermirrors;
        List<string> zipmirrors;

        private List<TheTVDBDbResult> results = null;


        public List<MetaDataPluginDescriptor> GetProviders
        {
            get
            {
                List<MetaDataPluginDescriptor> descriptors = new List<MetaDataPluginDescriptor>();

                MetaDataPluginDescriptor descriptor = new MetaDataPluginDescriptor();
                descriptor.DataProviderName = "thetvdb.com";
                descriptor.DataProviderMessage = "Data provided by thetvdb.com";
                descriptor.DataProviderLink = "http://www.thetvdb.com";
                descriptor.DataProviderCapabilities = MetadataPluginCapabilities.SupportsTVSearch | MetadataPluginCapabilities.SupportsBackDrops;
                descriptor.PluginDLL = null;
                descriptors.Add(descriptor);
                return descriptors;
            }
        }


        // these 2 methods must be called in sequence
        public bool Initialize(string provider, Dictionary<string, string> parameters)
        {
            // Set api info
            API_KEY = DEFAULT_API_KEY;

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                SetOptionValue(parameter.Key, parameter.Value);
            }

            xmlmirrors = new List<string>();
            bannermirrors = new List<string>();
            zipmirrors = new List<string>();
            
            // Mike - don't initialise mirrors now, do it when first used
            //GetMirrors();

            return true;
        }

        public bool SearchForMovie(string movieName, int maxResults)
        {
            return false;
        }

        // these methods are to be called after the 2 methods above

        // get the best match
        public Title GetBestMatch()
        {
            if (results != null)
            {
                if (results.Count != 0)
                {
                    return GetTitle(0);
                }
            }
            return null;
        }


        // or choose among all the titles
        public Title[] GetAvailableTitles()
        {
            Title[] titles = new Title[results.Count];
            for (int x = 0; x < results.Count; x++)
                titles[x] = results[x].Title;

            return titles;
        }

        public Title GetTitle(int index)
        {
            // Check if image has been downloaded
            if (string.Compare(results[index].Title.FrontCoverPath.Substring(0, 4), "http", true) == 0)
            {
                DownloadImage(results[index].Title, results[index].Title.FrontCoverPath);
            }

            return results[index].Title;
        }

        public List<OMLMetadataOption> GetOptions()
        {
            List<OMLMetadataOption> options = new List<OMLMetadataOption>();

            // API Key option
            OMLMetadataOption apikey = null;
            if (API_KEY == DEFAULT_API_KEY)
            {
                apikey = new OMLMetadataOption("API Key", API_KEY_HIDDEN_TEXT, null, false);
            }
            else
            {
                apikey = new OMLMetadataOption("API Key", API_KEY, null, false);
            }

            options.Add(apikey);
            
            // Fan Art enabled option
            Dictionary<string,string> YesNo = new Dictionary<string,string>();
            YesNo["Yes"] = "Yes";
            YesNo["No"] = "No";
            OMLMetadataOption fanart = null;
            if (FanartEnabled)
            {
                 fanart = new OMLMetadataOption("Fanart Enabled", "Yes", YesNo, true);
            }
            else
            {
                fanart = new OMLMetadataOption("Fanart Enabled", "No", YesNo, true);
            }
            options.Add(fanart);


            return options;
        }

        public bool SetOptionValue(string option, string value)
        {
            if (string.Compare(option, "API Key", true) == 0)
            {
                API_KEY = DEFAULT_API_KEY;

                if (!string.IsNullOrEmpty(value))
                {
                    if (value != API_KEY_HIDDEN_TEXT)
                    {
                        API_KEY = value;
                    }
                }
            }

            if (string.Compare(option, "Fanart Enabled", true) == 0)
            {
                FanartEnabled = false;
                if (!string.IsNullOrEmpty(value))
                {
                    if (string.Compare(value, "yes", true) == 0)
                    {
                        FanartEnabled = true;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Returns the text field value of a node or empty string if it doesn't have one
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string GetElementValue(XmlTextReader reader)
        {
            string returnValue = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    returnValue = reader.Value;
                    break;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    returnValue = string.Empty;
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Returns if the 0 first attribute of the current element has the given value
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsAttributeValue(XmlTextReader reader, string value)
        {
            bool found = false;
            if (reader.AttributeCount != 0 &&
                string.Equals(reader.GetAttribute(0), value, StringComparison.OrdinalIgnoreCase))
            {
                found = true;
            }

            return found;
        }

        /// <summary>
        /// Downloads the image for the results url and sets it to the internal title object
        /// </summary>
        /// <param name="result"></param>
        private void DownloadImage(Title title, string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string tempFileName = Path.GetTempFileName();
                WebClient web = new WebClient();
                try
                {
                    web.DownloadFile(imageUrl, tempFileName);
                    title.FrontCoverPath = tempFileName;
                }
                catch
                {
                    File.Delete(tempFileName);
                }
            }
        }

        public List<string> GetBackDropUrlsForTitle()
        {
            if (FanartEnabled)
            {
                return BackDrops;
            }
            else
            {
                return null;
            }
        }


        /*#region Variables to record TV search progress
        string SeriesName;
        int SeriesID;
        string EpisodeName;
        int? SeriesNo;
        int? EpisodeNo;
        #endregion*/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SeriesName"></param>
        /// <param name="EpisodeName"></param>
        /// <param name="SeriesNo"></param>
        /// <param name="EpisodeNo"></param>
        /// <returns>Boolean to indicate a drill down is required</returns>
        public bool SearchForTVSeries(string SeriesName, string EpisodeName, int? SeasonNo, int? EpisodeNo, int maxResults)
        {
            // Store the search criteria for later
            /*SeriesName = pSeriesName;
            EpisodeName = pEpisodeName; 
            SeriesNo = pSeriesNo;
            EpisodeNo = pEpisodeNo;*/

            if (xmlmirrors.Count == 0)
            {
                // Looks like the tvdb api initialisation failed. Try again
                if (!GetMirrors())
                {
                    return true;
                }
            }

            // First try to find the TV Show on tvdb
            SearchForTVShow(SeriesName);

            if (results.Count <= 0) return false;

            if (results.Count > 1)
            {
                // Found more than one possible show. Present the list to the user
                // but first load up all the titles with images
                foreach (TheTVDBDbResult title in results)
                {
                    title.Title.FrontCoverPath = "http://images.thetvdb.com/banners/" + title.ImageUrl;
                    //DownloadImage(title.Title, "http://images.thetvdb.com/banners/" + title.ImageUrl);
                }
                return true;
            }
            else
            {
                // We have a single match on the series. Try to load the episodes
                SeriesID = results[0].Id;
                SearchForEpisode(EpisodeName, SeasonNo, EpisodeNo, maxResults);
                return false;
            }
        }

        public bool SearchForTVDrillDown(int id, string EpisodeName, int? SeasonNo, int? EpisodeNo, int maxResults)
        {
            // If no series ID cached, get from the id passed to function.
            if (SeriesID == 0) SeriesID = results[id].Id;
            SearchForEpisode(EpisodeName, SeasonNo, EpisodeNo, maxResults);
            return false;
        }

        /// <summary>
        /// Queries tvdb to get a list of tc shows matching 'SeriesName'
        /// </summary>
        /// <param name="SeriesName"></param>
        private void SearchForTVShow(string SeriesName)
        {
            // Reset current series ID
            SeriesID = 0;

            UriBuilder uri = new UriBuilder("http://www.thetvdb.com/api/GetSeries.php");
            uri.Query = "seriesname=" + SeriesName;

            results = new List<TheTVDBDbResult>();


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);

            // execute the request
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                string mirrorpath = "";
                int typemask = 0;

                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    XmlTextReader reader = new XmlTextReader(resStream);
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name.ToLower() == "series")
                            {
                                TheTVDBDbResult result = new TheTVDBDbResult();

                                //TheTVDBDbResult title = GetTitleFromMovieNode(reader);

                                //if (title != null)
                                //    results.Add(title);
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "seriesid":
                                                result.Id = int.Parse(GetElementValue(reader));
                                                break;
                                            //case "language":
                                            //    result. = GetElementValue(reader);
                                            //    break;
                                            case "seriesname":
                                                result.Title.Name = GetElementValue(reader);
                                                break;
                                            case "banner":
                                                result.ImageUrl = GetElementValue(reader);
                                                break;
                                            case "overview":
                                                result.Title.Synopsis = GetElementValue(reader);
                                                break;
                                            case "firstAired":
                                                result.Title.ReleaseDate = DateTime.Parse(GetElementValue(reader));
                                                break;
                                            //case "IMDB_ID":
                                            //    typemask = int.Parse(GetElementValue(reader));
                                            //    break;
                                            //case "zap2it_id":
                                            //    typemask = int.Parse(GetElementValue(reader));
                                            //    break;
                                            //case "id":
                                            //    typemask = int.Parse(GetElementValue(reader));
                                            //    break;
                                            //case "IMDB_ID":
                                            //    typemask = int.Parse(GetElementValue(reader));
                                            //    break;
                                            default:
                                                break;
                                        }
                                    }

                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "series")
                                    {
                                        results.Add(result);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of all episodes
        /// </summary>
        /// <param name="id"></param>
        int SeriesID;
        private void SearchForEpisode(string EpisodeName, int? SeasonNo, int? EpisodeNo, int maxResults)
        {
            UriBuilder uri = new UriBuilder("http://thetvdb.com/api/" + API_KEY + "/series/" + SeriesID.ToString() + "/all/");

            string actors = "";
            string genres = "";
            string network = "";
            int runtime = 0;
            string rating = "";

            results = new List<TheTVDBDbResult>();


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);

            // execute the request
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                string mirrorpath = "";
                int typemask = 0;

                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    XmlTextReader reader = new XmlTextReader(resStream);
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name.ToLower() == "series")
                            {
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "actors":
                                                actors = GetElementValue(reader).ToString();
                                                break;
                                            case "genre":
                                                genres = GetElementValue(reader);
                                                break;
                                            case "network":
                                                network = GetElementValue(reader);
                                                break;
                                            case "runtime":
                                                int.TryParse(GetElementValue(reader), out runtime);
                                                //runtime = int.Parse(GetElementValue(reader));
                                                break;
                                            case "fanart":
                                                if (BackDrops == null) BackDrops = new List<string>();
                                                BackDrops.Add("http://images.thetvdb.com/banners/" + GetElementValue(reader));
                                                break;
                                            case "poster":
                                                if (BackDrops == null) BackDrops = new List<string>();
                                                BackDrops.Add("http://images.thetvdb.com/banners/" + GetElementValue(reader));
                                                break;
                                            case "contentrating":
                                                rating = GetElementValue(reader);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "episode")
                                    {
                                        break;
                                    }
                                }
                            }

                            if (reader.Name.ToLower() == "episode")
                            {
                                TheTVDBDbResult result = new TheTVDBDbResult();

                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "id":
                                                result.Id = int.Parse(GetElementValue(reader));
                                                break;
                                            case "episodename":
                                                result.Title.Name = GetElementValue(reader);
                                                break;
                                            case "director":
                                                foreach (string directorstr in GetElementValue(reader).Split('|'))
                                                {
                                                    if (!string.IsNullOrEmpty(directorstr))
                                                    {
                                                        Person director = new Person(directorstr);
                                                        result.Title.AddDirector(director);
                                                    }
                                                }
                                                break;
                                            case "episodenumber":
                                                result.EpisodeNo = int.Parse(GetElementValue(reader));
                                                result.Title.EpisodeNumber = (short)result.EpisodeNo;
                                                break;
                                            case "firstaired":
                                                //    result.Title.ReleaseDate = DateTime.Parse(GetElementValue(reader));
                                                break;
                                            case "overview":
                                                result.Title.Synopsis = GetElementValue(reader);
                                                break;
                                            case "seasonnumber":
                                                result.SeasonNo = int.Parse(GetElementValue(reader));
                                                result.Title.SeasonNumber = (short)result.SeasonNo;
                                                break;
                                            case "seasonid":
                                                //    typemask = int.Parse(GetElementValue(reader));
                                                break;
                                            case "seriesid":
                                                //    typemask = int.Parse(GetElementValue(reader));
                                                break;
                                            case "filename":
                                                result.ImageUrl = GetElementValue(reader);
                                                break;
                                            case "gueststars":
                                                foreach (string actor in GetElementValue(reader).Split('|'))
                                                {
                                                    if (!string.IsNullOrEmpty(actor))
                                                    {
                                                        result.Title.AddActingRole(actor, "");
                                                    }
                                                }
                                                break;

                                            case "runtime":
                                                result.Title.Runtime = int.Parse(GetElementValue(reader));
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "episode")
                                    {
                                        if (result.SeasonNo != 0)
                                        {
                                            bool AddTitle = true;

                                            // If an episode or season number is specified, only add if matched
                                            if ((SeasonNo > 0) && (SeasonNo != result.SeasonNo)) AddTitle = false;
                                            if ((EpisodeNo > 0) && (EpisodeNo != result.EpisodeNo)) AddTitle = false;

                                            // If an episode name is specified, calculate the match confidence
                                            if (!string.IsNullOrEmpty(EpisodeName)) result.NameMatchConfidence = StringMatching.Compute(EpisodeName, result.Title.Name);

                                            if (AddTitle)
                                            {
                                                foreach (string actor in actors.Split('|'))
                                                {
                                                    if (!string.IsNullOrEmpty(actor))
                                                    {
                                                        result.Title.AddActingRole(actor, "");
                                                    }
                                                }
                                                foreach (string genre in genres.Split('|'))
                                                {
                                                    if (!string.IsNullOrEmpty(genre))
                                                    {
                                                        result.Title.AddGenre(genre);
                                                    }
                                                }
                                                result.Title.Studio = network;
                                                result.Title.Runtime = runtime;
                                                if (string.IsNullOrEmpty(result.Title.ParentalRating))
                                                {
                                                    result.Title.ParentalRating = rating;
                                                }
                                                results.Add(result);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //if (!string.IsNullOrEmpty(EpisodeName))
            //{
            // Sort by name confidence and limit result set
            results = (from r in results
                       orderby r.NameMatchConfidence
                       select r).Take(maxResults).ToList();
            //} 

            // load up all the titles with images
            foreach (TheTVDBDbResult title in results)
            {
                title.Title.FrontCoverPath = "http://images.thetvdb.com/banners/" + title.ImageUrl;
                //DownloadImage(title.Title, "http://images.thetvdb.com/banners/" + title.ImageUrl);
            }
        }

        private bool GetMirrors()
        {
            try
            {
                UriBuilder uri = new UriBuilder("http://www.thetvdb.com/api/" + API_KEY + "/mirrors.xml");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);

                // execute the request
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string mirrorpath = "";
                    int typemask = 0;

                    // we will read data via the response stream
                    using (Stream resStream = response.GetResponseStream())
                    {
                        XmlTextReader reader = new XmlTextReader(resStream);
                        reader.WhitespaceHandling = WhitespaceHandling.None;

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.ToLower() == "mirror")
                                {
                                    //TheTVDBDbResult title = GetTitleFromMovieNode(reader);

                                    //if (title != null)
                                    //    results.Add(title);
                                    while (reader.Read())
                                    {
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                            switch (reader.Name.ToLower())
                                            {
                                                case "mirrorpath":
                                                    mirrorpath = GetElementValue(reader);
                                                    break;
                                                case "typemask":
                                                    typemask = int.Parse(GetElementValue(reader));
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "mirror")
                                        {
                                            if ((typemask & 1) != 0)
                                            {
                                                xmlmirrors.Add(mirrorpath);
                                            }
                                            if ((typemask & 2) != 0)
                                            {
                                                bannermirrors.Add(mirrorpath);
                                            }
                                            if ((typemask & 4) != 0)
                                            {
                                                zipmirrors.Add(mirrorpath);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return false;
            }

            if (xmlmirrors.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
