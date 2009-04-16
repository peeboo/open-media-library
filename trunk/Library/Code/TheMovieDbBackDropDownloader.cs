using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using OMLEngine;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Library
{
    public class TheMovieDbBackDropDownloader : BaseModelItem
    {
        private const string API_KEY = "1376bf98794bda0c2495bd500a37f689";
        private const string API_URL_SEARCH = "http://api.themoviedb.org/2.0/Movie.search";
        private const string API_URL_INFO = "http://api.themoviedb.org/2.0/Movie.getInfo";

        public TheMovieDbBackDropDownloader()
        {
        }

        public void SearchForTitle(Title t)
        {
            UriBuilder uri = new UriBuilder(API_URL_SEARCH);
            uri.Query = "api_key=" + API_KEY + "&title=" + t.OriginalName;

            WebRequest req = HttpWebRequest.Create(uri.Uri);
            WebResponse res = req.GetResponse();

            Stream stream = res.GetResponseStream();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);

            XmlNodeList possibleMovieNodes = xDoc.SelectNodes("//moviematches//movie");

            foreach (XmlNode posssibleMovieNode in possibleMovieNodes)
            {
                XPathNavigator nav = posssibleMovieNode.CreateNavigator();
                string strScore = GetChildNodesValue(nav, "score");
                if (!string.IsNullOrEmpty(strScore))
                {
                    double score = 0;
                    try
                    {
                        double.TryParse(strScore, out score);
                        if (score == 1.0)
                        {
                            // its a perfect match, take it!
                            IList<string> backDropUrls = GrabBackDropUrls(nav);
                            DownloadBackDropsForTitle(t, backDropUrls);
                        }
                        else if (score >= 8.0)
                        {
                            // ok, its not perfect but its pretty good, take the top one
                            IList<string> backDropUrls = GrabBackDropUrls(nav);
                            DownloadBackDropsForTitle(t, backDropUrls);
                        }
                    }
                    catch (Exception)
                    {
                        OMLApplication.DebugLine("[TheMovieDbBackDropDownloader] Unable to parse a double from a string: {0}", strScore);
                    }
                }
            }
            if (stream != null)
                stream.Close();

            if (res != null)
                res.Close();
        }

        public IList<string> GrabBackDropUrls(XPathNavigator nav)
        {
            List<string> urls = new List<string>();
            XPathNodeIterator nIter = nav.SelectChildren("backdrop", "");
            if (nav.MoveToFollowing("backdrop", ""))
            {
                XPathNavigator localNav = nav.CreateNavigator();
                nav.MoveToParent();
                for (int i = 0; i < nIter.Count; i++)
                {
                    if (localNav.GetAttribute("size", "").ToUpperInvariant().Equals("original".ToUpperInvariant()))
                        urls.Add(localNav.Value);

                    localNav.MoveToNext();
                }
            }
            return urls;
        }

        public void DownloadBackDropsForTitle(Title t, IList<string> urls)
        {            
            foreach (string url in urls)
            {
                WebClient web = new WebClient();
                string filename = Path.Combine(FileSystemWalker.ImageDownloadDirectory, Guid.NewGuid().ToString());
                try
                {
                    web.DownloadFile(url, filename);
                    t.AddFanArtImage(filename);
                }
                catch (Exception e)
                {
                    OMLApplication.DebugLine("[TheMovieDbBackDropDownloader] Error downloading backdrop file {0}: {1}",
                        url, e.Message);
                }
            }
        }

        private string GetChildNodesValue(XPathNavigator nav, string nodeName)
        {
            string value = string.Empty;
            if (nav.MoveToChild(nodeName, ""))
            {
                value = nav.Value;
                nav.MoveToParent();
            }
            return value;
        }
    }
}
