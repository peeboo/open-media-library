using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;        // need this for OML Title class
using OMLSDK;           // need this for the IOMLMetadataPlugin
using System.IO;
using System.Net;
using System.Xml;
using System.Globalization;

namespace TheMovieDbMetadata
{
    public class TheMovieDbResult
    {
        public Title Title { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrlThumb { get; set; }

        public TheMovieDbResult()
        {
            Title = new Title();
        }
    }

    public class TheMovieDbMetadata : IOMLMetadataPlugin
    {
        IList<string> BackDrops = null;
        private const string API_KEY = "1376bf98794bda0c2495bd500a37f689";
        private const string API_URL_SEARCH = "http://api.themoviedb.org/2.0/Movie.search";
        private const string API_URL_INFO = "http://api.themoviedb.org/2.0/Movie.getInfo";

        private List<TheMovieDbResult> results = null;

        public string PluginName { get { return "themoviedb.org"; } }

        // these 2 methods must be called in sequence
        public bool Initialize(Dictionary<string, string> parameters)
        {
            return true;
        }

        public bool SearchForMovie(string movieName)
        {
            SearchForMovies(movieName);

            return (results != null && results.Count != 0);
        }

        // these methods are to be called after the 2 methods above

        // get the best match
        public Title GetBestMatch()
        {
            return (results != null && results.Count != 0)
                ? results[0].Title
                : null;                
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
            return GetMovieDetails(results[index].Id);
        }

        public List<OMLMetadataOption> GetOptions()
        {
            return null;
        }

        public bool SetOptionValue(string option, string value)
        {
            return true;
        }

        /// <summary>
        /// Creates a result object from the movie result node - returns null if it's not a valid result
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private TheMovieDbResult GetTitleFromMovieNode(XmlTextReader reader)
        {
            this.BackDrops = null;
            bool notMovie = false;
            TheMovieDbResult result = new TheMovieDbResult();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "id":
                            result.Id = int.Parse(GetElementValue(reader));
                            break;

                        case "title":
                            result.Title.Name = GetElementValue(reader);
                            break;

                        case "type":
                            if (!GetElementValue(reader).Equals("movie", StringComparison.OrdinalIgnoreCase))
                            {
                                notMovie = true;
                            }
                            break;

                        case "short_overview":
                            result.Title.Synopsis = GetElementValue(reader);
                            break;

                        case "release":
                            result.Title.ReleaseDate = DateTime.Parse(
                                                            GetElementValue(reader),
                                                            CultureInfo.CreateSpecificCulture("en-GB"));
                            break;

                        case "poster":
                            if (IsAttributeValue(reader, "original"))
                            {
                                // only keep the first one
                                if (result.ImageUrl != null)
                                    continue;

                                result.ImageUrl = GetElementValue(reader);
                            }
                            else if (IsAttributeValue(reader, "thumb"))
                            {
                                // only keep the first one
                                if (result.ImageUrlThumb != null)
                                    continue;

                                result.ImageUrlThumb = GetElementValue(reader);
                            }                            
                            break;

                        case "backdrop":
                            if (IsAttributeValue(reader, "original"))
                            {
                                if (this.BackDrops == null)
                                    this.BackDrops = new List<string>();

                                this.BackDrops.Add(GetElementValue(reader));
                            }
                            break;

                        case "runtime":
                            result.Title.Runtime = int.Parse(GetElementValue(reader));
                            break;

                        case "homepage":
                            result.Title.OfficialWebsiteURL = GetElementValue(reader);                            
                            break;
                            
                        case "production_countries":
                            // todo : solomon : this needs to be filled in
                            break;

                        case "categories":
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element &&
                                    reader.Name == "category")
                                {
                                    string genre = GetElementValue(reader);

                                    if (result.Title.Genres != null &&
                                        result.Title.Genres.Contains(genre))
                                    {
                                        // the moviedb has duplicates genres for some reason
                                        continue;
                                    }

                                    result.Title.AddGenre(genre);
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement &&
                                    reader.Name == "categories")
                                    break;
                            }
                            break;

                        case "people":
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element &&
                                    reader.Name == "person")
                                {
                                    // add the actor
                                    switch (reader.GetAttribute(0))
                                    {
                                        case "actor":
                                            result.Title.AddActingRole(GetElementValue(reader), string.Empty);
                                            break;

                                        case "director":
                                            result.Title.AddDirector(new Person(GetElementValue(reader)));
                                            break;

                                        case "author":
                                        case "screenplay":
                                        case "story":
                                            result.Title.AddWriter(new Person(GetElementValue(reader)));
                                            break;                                        

                                        case "producer":
                                            result.Title.AddProducer(GetElementValue(reader));
                                            break;                                        

                                        case "original_music_composer":
                                            // unused
                                            break;

                                        case "director_of_photography":
                                            // unused
                                            break;

                                        case "editor":
                                            // unused
                                            break;

                                        case "casting":
                                            // unused
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement &&
                                    reader.Name == "people")
                                    break;
                            }

                            break;

                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement &&
                    reader.Name == "movie")
                    break;

                // if we're not a movie let's move on
                if (notMovie)
                    break;
            }

            return (notMovie) ? null : result;
        }

        private Title GetMovieDetails(int movieId)
        {
            UriBuilder uri = new UriBuilder(API_URL_INFO);
            uri.Query = "api_key=" + API_KEY + "&id=" + movieId.ToString();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);

            TheMovieDbResult title = null;

            // execute the request
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    XmlTextReader reader = new XmlTextReader(resStream);
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "movie")
                            {
                                title = GetTitleFromMovieNode(reader);
                                break;
                            }
                        }
                    }
                }
            }

            // load up the big image
            if (title != null)
            {
                DownloadImage(title.Title, title.ImageUrl);
            }

            return ( title != null ) ? title.Title : null;
        }

        /// <summary>
        /// Fills the local results with movies
        /// </summary>
        /// <param name="searchQuery"></param>
        private void SearchForMovies(string searchQuery)
        {
            UriBuilder uri = new UriBuilder(API_URL_SEARCH);
            uri.Query = "api_key=" + API_KEY + "&title=" + searchQuery;

            results = new List<TheMovieDbResult>();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.Uri);

            // execute the request
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    XmlTextReader reader = new XmlTextReader(resStream);
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "movie")
                            {
                                TheMovieDbResult title = GetTitleFromMovieNode(reader);

                                if (title != null)
                                    results.Add(title);
                            }
                        }
                    }
                }
            }

            // load up all the titles with images
            foreach (TheMovieDbResult title in results)
            {
                DownloadImage(title.Title, title.ImageUrlThumb);
            }
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

        public bool SupportsBackDrops()
        {
            return true;
        }

        public void DownloadBackDropsForTitle(Title t, int index)
        {
            if (results.Count >= index)
            {
                if (this.BackDrops == null)
                    return;

                string downloadTo = t.BackDropFolder;
                WebClient web = new WebClient();                
                foreach (string backDropUrl in this.BackDrops)
                {
                    if (!string.IsNullOrEmpty(backDropUrl))
                    {
                        if (!Directory.Exists(t.BackDropFolder))
                        {
                            string mainFanArtDir = @"C:\ProgramData\OpenMediaLibrary\FanArt";
                            string pdTitleDir = @"C:\ProgramData\OpenMediaLibrary\FanArt\" + t.Name.ToString();
                            if (!Directory.Exists(mainFanArtDir)) Directory.CreateDirectory(mainFanArtDir);
                            if (!Directory.Exists(pdTitleDir)) Directory.CreateDirectory(pdTitleDir);
                            downloadTo = pdTitleDir.ToString();
                        }
                        string filename = backDropUrl.Substring(backDropUrl.LastIndexOf('/') + 1);
                        filename = Path.GetFileName(filename);
                        web.DownloadFile(backDropUrl, Path.Combine(downloadTo, filename).ToString());
                    }
                }
            }
        }
    }
}
