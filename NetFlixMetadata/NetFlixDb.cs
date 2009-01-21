using System;
using System.Collections.Generic;
using System.Collections;
using OMLEngine;
using OMLSDK;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;

namespace NetFlixMetadata
{
    public class NetFlixDbResult
    {
        public Title Title { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrlThumb { get; set; }

        public NetFlixDbResult()
        {
            Title = new Title();
        }
    }

    public class NetFlixDb : IOMLMetadataPlugin
    {
        private const string API_KEY = @"8mfjpswhjxg7y4md35zs5ang";
        private const string SHARED_SECRET = @"Q9J4DrqSZv";

        private IList<NetFlixDbResult> results = null;

        public string PluginName
        {
            get { return "NetFlixMetadata"; }
        }

        public bool Initialize(Dictionary<string, string> parameters)
        {
            OAuth.OAuthBase oauth = new OAuth.OAuthBase();
            string nonce = oauth.GenerateNonce();
            string normalizedUrl = string.Empty;
            string normalizedReqParams = string.Empty;

            string signature = oauth.GenerateSignature(new Uri("http://api.netflix.com/catalog/titles/movies/18704531"),
                API_KEY,
                SHARED_SECRET,
                null,
                null,
                "GET",
                oauth.GenerateTimeStamp(),
                oauth.GenerateNonce(),
                out normalizedUrl,
                out normalizedReqParams);

            signature = HttpUtility.UrlEncode(signature);
            normalizedReqParams = string.Join("&", new string[] { normalizedReqParams, string.Format("oauth_signature={0}", signature) });
            string finalUrl = string.Join("?", new string[] { normalizedUrl, normalizedReqParams });

            WebRequest req = WebRequest.Create(finalUrl);
            WebResponse res = req.GetResponse();
            return true;
        }

        public bool SearchForMovie(string movieName)
        {
            SearchForMovies(movieName);

            return (results != null && results.Count != 0);
        }

        public Title GetBestMatch()
        {
            return (results != null && results.Count != 0)
                ? results[0].Title
                : null;
        }

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

        private NetFlixDbResult GetTitleFromMovieNode(XmlTextReader reader)
        {
            return null;
        }

        private Title GetMovieDetails(int movieId)
        {
            return null;
        }

        private void SearchForMovies(string searchQuery)
        {
        }

        private string GetElementValue(XmlTextReader reader)
        {
            return string.Empty;
        }

        private bool IsAttributeValue(XmlTextReader reader, string value)
        {
            return false;
        }

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
    }
}
