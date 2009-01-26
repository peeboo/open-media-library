using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using OMLSDK;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;


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
        private const string HTML_TAG_PATTERN = @"<(.|\n)*?>";

        private List<NetFlixDbResult> results = null;

        public string PluginName
        {
            get { return "NetFlix"; }
        }

        public bool Initialize(Dictionary<string, string> parameters)
        {
            //Utilities.DebugLine("[NetFlixDb] Initializing...");
            //Utilities.DebugLine("[NetFlixDb] DONE!");
            return true;
        }

        public string getSignedUrl(string url)
        {
            //Utilities.DebugLine("[NetFlixDb] Getting signed url: " + url + "...");
            string normalizedUrl = string.Empty;
            string normalizedReqParams = string.Empty;
            OAuth.OAuthBase oauth = new OAuth.OAuthBase();
            string signature = oauth.GenerateSignature(new Uri(url), API_KEY, SHARED_SECRET, null, null, "GET", oauth.GenerateTimeStamp(), oauth.GenerateNonce(), out normalizedUrl, out normalizedReqParams);
            //Utilities.DebugLine("[NetFlixDb] \tSignature: " + signature);

            signature = HttpUtility.UrlEncode(signature);
            normalizedReqParams = string.Join("&", new string[] { normalizedReqParams, string.Format("oauth_signature={0}", signature) });
            string finalUrl = string.Join("?", new string[] { normalizedUrl, normalizedReqParams });
            //Utilities.DebugLine("[NetFlixDb] \tSigned Url: " + finalUrl);
            return finalUrl;
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
            bool isMovie = true;
            NetFlixDbResult result = new NetFlixDbResult();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //Utilities.DebugLine("[NetFlixDb] \t\tGetting title info, reader.NodeType [" + reader.NodeType + "], name: " + reader.Name);
                    switch (reader.Name)
                    {
                        case "id":
                            try { result.Id = int.Parse(GetElementValue(reader)); }
                            catch { }
                            break;

                        case "title":
                            //Utilities.DebugLine("[NetFlixDb] \t\t---Searching for title");
                            result.Title.Name = GetElementValue(reader, "attribute", "regular");
                            //Utilities.DebugLine("[NetFlixDb] \t\t---Done title search");
                            break;
                        
                        // Synopsis and other data comes from other api links
                        case "link":
                            string attrTitle = null;
                            string attrUrl = null;
                            string attrRel = null;
                            
                            if (reader.HasAttributes)
                            {
                                //Utilities.DebugLine("[NetFlixDb] \t\tNode has attributes...");
                                while (reader.MoveToNextAttribute())
                                {
                                    switch (reader.Name)
                                    {
                                        case "href":
                                            attrUrl = reader.Value;
                                            break;
                                        case "rel":
                                            attrRel = reader.Value;
                                            break;
                                        case "title":
                                            attrTitle = reader.Value;
                                            break;
                                    }
                                }
                                // Move the reader back to the element node.
                                reader.MoveToElement();
                            }
                            
                            // Find out which item we have here
                            string attrSignedUrl = getSignedUrl(attrUrl);
                            switch (attrTitle)
                            {
                                case "synopsis":
                                    //Utilities.DebugLine("[NetFlixDb] \t\tSynopsis at: " + attrSignedUrl);
                                    try { result.Title.Synopsis = getNonHTML(getExternalData(attrUrl, attrTitle)); }
                                    catch { result.Title.Synopsis = ""; }
                                    Utilities.DebugLine("[NetFlixDb] \t\tSynopsis value: " + result.Title.Synopsis);
                                    break;
                                case "cast":
                                    Utilities.DebugLine("[NetFlixDb] \t\tCast at: " + attrSignedUrl);
                                    try 
                                    {
                                        // Cast separated by:  :::
                                        string cast = getNonHTML(getExternalData(attrUrl, attrTitle));
                                        string[] actors = Regex.Split(cast, ":::");
                                        for (int i = 0; i < actors.Length; i++)
                                        {
                                            //Utilities.DebugLine("[NetFlixDb] \t\t*** Actor found: " + actors[i]);
                                            result.Title.AddActingRole(actors[i], string.Empty);
                                        }
                                        //Utilities.DebugLine("[NetFlixDb] \t\tCast value: " + cast);
                                    }
                                    catch {  }
                                    break;
                                case "directors":
                                    Utilities.DebugLine("[NetFlixDb] \t\tDirectors at: " + attrSignedUrl);
                                    try 
                                    {
                                        // Cast separated by:  :::
                                        string directors = getNonHTML(getExternalData(attrUrl, attrTitle));
                                        string[] dirs = Regex.Split(directors, ":::");
                                        for (int i = 0; i < dirs.Length; i++)
                                        {
                                            Utilities.DebugLine("[NetFlixDb] \t\t*** Director found: " + dirs[i]);
                                            result.Title.AddDirector(new Person(dirs[i]));
                                        }
                                        Utilities.DebugLine("[NetFlixDb] \t\tDirector value: " + directors);
                                    }
                                    catch {  }
                                    break;
                                case "web page":
                                    result.Title.OfficialWebsiteURL = attrUrl;
                                    break;
                            }
                            break;

                        case "release_year":
                            try { result.Title.ReleaseDate = new DateTime(int.Parse(GetElementValue(reader)), 1, 1); }
                            catch { }
                            break;

                        case "box_art":
                            result.ImageUrl = GetElementValue(reader, "attribute", "large");
                            result.ImageUrlThumb = result.ImageUrl;
                            Utilities.DebugLine("[NetFlixDb] \t\tImage at: " + result.ImageUrl);
                            break;

                        case "runtime":
                            result.Title.Runtime = int.Parse(GetElementValue(reader));
                            break;

                        case "category":
                            string scheme = GetElementValue(reader, "attribute", "scheme", false);
                            string label = GetElementValue(reader, "attribute", "label");
                            //Utilities.DebugLine("[NetFlixDb] \t\tCat: sheme: " + scheme + ", label: " + label);
                            if (scheme == "http://api.netflix.com/categories/genres")
                            {
                                result.Title.AddGenre(label);
                            }
                            else if (scheme == "http://api.netflix.com/categories/mpaa_ratings")
                            {
                                result.Title.ParentalRatingReason = label;
                            }
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "catalog_title") break;

                // See if Television is in the genre list to determine if its a movie or not
                // if we're not a movie let's move on
                if (!isMovie) break;
            }

            return (!isMovie) ? null : result;
        }

        // Data is located on another page, get it here
        private string getExternalData(string exUrl, string field)
        {
            string foundVal = null;
            exUrl = getSignedUrl(exUrl);
            //Utilities.DebugLine("[NetFlixDb] \t\t----- Loading external data for: "+field);
            string startPath = null;
            string collectionOf = null;
            string valueType = null;
            string valueFrom = null;
            switch (field)
            {
                case "synopsis":
                    startPath = "xml/";
                    collectionOf = "synopsis";
                    valueType = "cdata";
                    break;
                case "directors":
                case "cast":
                    startPath = "xml/people/";
                    collectionOf = "person";
                    valueFrom = "name";
                    //valueType = "cdata";
                    break;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(exUrl);
            request.AllowAutoRedirect = true;
            //Utilities.DebugLine("[NetFlixDb] \t\t----- Request: " + request.Address.ToString()+", headers: "+request.Headers.ToString());
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //Utilities.DebugLine("[NetFlixDb] \t\t----- response: " + response.ToString());
                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    //Utilities.DebugLine("[NetFlixDb] \t\t----- resString: " + resStream.ToString());
                    XmlTextReader reader = new XmlTextReader(resStream);
                    //Utilities.DebugLine("[NetFlixDb] \t\t----- Reader: " + reader.ToString());
                    string pathSoFar = "";
                    while (reader.Read())
                    {
                        //Utilities.DebugLine("[NetFlixDb] \t\t----- Path read node type: " + reader.NodeType);
                        if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace) continue;

                        pathSoFar += reader.Name + "/";
                        //Utilities.DebugLine("[NetFlixDb] \t\t----- Checking path so far: " + pathSoFar+", name: "+reader.Name);
                        if (startPath.Substring(0, pathSoFar.Length) != pathSoFar) return "";
                        if (pathSoFar == startPath)
                        {
                            //Utilities.DebugLine("[NetFlixDb] \t\t----- Entered start path! " + pathSoFar+", name: "+reader.Name);
                            foundVal = null;
                            while (reader.Read())
                            {
                                if(reader.Name == collectionOf)
                                {
                                    if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace) continue;
                                    if (valueFrom != null)
                                    {
                                        //Utilities.DebugLine("[NetFlixDb] \t\t----- SUB VALUE FROM: " + valueFrom);
                                        while (reader.Read())
                                        {
                                            //Utilities.DebugLine("[NetFlixDb] \t\t----- SUB current name: " + reader.Name);
                                            if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace) continue;
                                            if (reader.Name != valueFrom) continue;
                                            if (valueType == "cdata") reader.Read();
                                            //Utilities.DebugLine("[NetFlixDb] \t\t----- SUB Get value from: " + reader.Name);
                                            string tmpName = reader.Name;
                                            if (foundVal != null) foundVal += ":::";
                                            string tmpVal = GetElementValue(reader, valueType, "");
                                            foundVal += tmpVal;
                                            //Utilities.DebugLine("[NetFlixDb] \t\t----- SUB VALUE so far: " + foundVal + " :: " + tmpName);
                                        }
                                    }
                                    else
                                    {
                                        if (valueType == "cdata") reader.Read();
                                        //Utilities.DebugLine("[NetFlixDb] \t\t----- Get value from: " + reader.Name);
                                        if (foundVal != null) foundVal += "|";
                                        foundVal += GetElementValue(reader, valueType, "");
                                        //Utilities.DebugLine("[NetFlixDb] \t\t----- VALUE so far: " + foundVal);
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }
            return foundVal;
        }
        private Title GetMovieDetails(int movieId)
        {
            // load up all the titles with images
            foreach (NetFlixDbResult title in results)
            {
                if (title.Id != movieId) continue;

                DownloadImage(title.Title, title.ImageUrlThumb);
                return title.Title;
            }

            return null;
        }

        private void SearchForMovies(string query)
        {
            results = new List<NetFlixDbResult>();

            Utilities.DebugLine("[NetFlixDb] Searching for: "+query+"...");
            string queryUrl = getSignedUrl("http://api.netflix.com/catalog/titles?term=" + query + "&max_results=10");
            Utilities.DebugLine("[NetFlixDb] \tMovie search url: "+queryUrl);


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(queryUrl);

            // execute the request
            //Utilities.DebugLine("[NetFlixDb] \tParsing results...");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // we will read data via the response stream
                using (Stream resStream = response.GetResponseStream())
                {
                    XmlTextReader reader = new XmlTextReader(resStream);

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            //Utilities.DebugLine("[NetFlixDb] \t\t-Node: " + reader.Name);
                            if (reader.Name == "catalog_title")
                            {
                                //Utilities.DebugLine("[NetFlixDb] \t\t-Geting title from node: " + reader.Name);
                                NetFlixDbResult title = GetTitleFromMovieNode(reader);
                                //Utilities.DebugLine("[NetFlixDb] \t\t-Done!");
                                if (title != null)results.Add(title);
                            }
                        }
                    }
                }
            }

            // load up all the titles with images
            foreach (NetFlixDbResult title in results)
            {
                DownloadImage(title.Title, title.ImageUrlThumb);
            }
        }

        private string GetElementValue(XmlTextReader reader)
        {
            return GetElementValue(reader, "value", null);
        }
        private string GetElementValue(XmlTextReader reader, string valueType, string valueTarget)
        {
            return GetElementValue(reader, valueType, valueTarget, true);
        }
        private string GetElementValue(XmlTextReader reader, string valueType, string valueTarget, bool postAttrAdvance)
        {
            // Check by attribute value
            if (valueType == "attribute")
            {
                //Utilities.DebugLine("[NetFlixDb] \t\treader.NodeType: " + reader.Name);
                if (reader.HasAttributes)
                {
                    //Utilities.DebugLine("[NetFlixDb] \t\tNode has attributes...");
                    while (reader.MoveToNextAttribute())
                    {
                        //Utilities.DebugLine("[NetFlixDb] \t\t-Post attribute move: " + reader.Name + ", " + reader.Value);
                        if (reader.Name == valueTarget) return reader.Value;
                    }
                    // Move the reader back to the element node.
                    if (postAttrAdvance) reader.MoveToElement();
                }
                return "";
            }

            // CHeck by CDATA
            if (valueType == "cdata")
            {
                Utilities.DebugLine("[NetFlixDb] \t\t*** Checking CDATA: " + reader.Name + ", " + reader.NodeType+", "+reader.Value);
                if (reader.NodeType == XmlNodeType.CDATA)
                {
                    string foundVal = reader.Value;
                    // Move the reader back to the element node.
                    if (postAttrAdvance) reader.MoveToElement();
                    return foundVal;
                }
            }

            // Check by value
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
            return false;
        }

        public void DownloadBackDropsForTitle(Title t, int index)
        {
        }

        private static string getNonHTML(string inputString)
        {
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }
    }
}
