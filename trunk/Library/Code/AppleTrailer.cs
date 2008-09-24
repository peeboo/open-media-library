using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Library
{
    public class AppleTrailers
    {
        public List<AppleTrailer> Trailers { get; private set; }

        public AppleTrailers()
        {
            Trailers = new List<AppleTrailer>();
        }

        public void LoadTrailers()
        {
            string[] urls = {
                @"http://www.apple.com/trailers/home/xml/widgets/indexall.xml"
            };

            foreach (string url in urls)
            {
                List<AppleTrailer> newTrailers = GetTrailersForUrl(url);
                foreach (AppleTrailer trailer in newTrailers)
                {
                    Trailers.Add(trailer);
                }
            }
        }

        private List<AppleTrailer> ParseDocument(XmlDocument xDoc)
        {
            List<AppleTrailer> trailers = new List<AppleTrailer>();
            XPathNavigator nav = xDoc.CreateNavigator();

            if (nav.MoveToChild("records", ""))
            {
                XPathNodeIterator nIter = nav.SelectChildren("movieinfo", "");
                nav.MoveToFirstChild();
                XPathNavigator localNav = nav.CreateNavigator();
                nav.MoveToParent();

                for (int i = 0; i < nIter.Count; i++)
                {
                    AppleTrailer currentTrailer = new AppleTrailer();
                    currentTrailer.AppleId = localNav.GetAttribute("id", "");
                    currentTrailer.FandangoId = localNav.GetAttribute("fandangoid", "");

                    if (localNav.MoveToChild("info", ""))
                    {
                        currentTrailer.MovieTitle = GetChildNodesValue(localNav, "title");
                        currentTrailer.Runtime = GetChildNodesValue(localNav, "runtime");
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("posters", ""))
                    {
                        XPathNodeIterator posterIter = localNav.SelectChildren("poster", "");
                        localNav.MoveToFirstChild();
                        XPathNavigator posterNav = localNav.CreateNavigator();
                        localNav.MoveToParent();

                        for (int j = 0; j < posterIter.Count; j++)
                        {
                            ApplePoster poster = new ApplePoster();
                            poster.Type = posterNav.GetAttribute("type", "");
                            poster.Url = posterNav.Value;
                            currentTrailer.AddPoster(poster);
                        }
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("previews", ""))
                    {
                        XPathNodeIterator previewIter = localNav.SelectChildren("preview", "");
                        localNav.MoveToFirstChild();
                        XPathNavigator previewNav = localNav.CreateNavigator();
                        localNav.MoveToParent();

                        for (int k = 0; k < previewIter.Count; k++)
                        {
                            ApplePreview preview = new ApplePreview();
                            preview.Type = previewNav.GetAttribute("type", "");
                            preview.Url = previewNav.Value;
                            currentTrailer.AddPreview(preview);
                        }
                        localNav.MoveToParent();
                    }
                    if (!string.IsNullOrEmpty(currentTrailer.AppleId) && !string.IsNullOrEmpty(currentTrailer.FandangoId))
                        trailers.Add(currentTrailer);

                    localNav.MoveToNext();
                }
            }
            return trailers;
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

        public List<AppleTrailer> GetTrailersForUrl(string url)
        {
            try
            {
                WebClient client = new WebClient();
                Stream strm = client.OpenRead(url);
                StreamReader sr = new StreamReader(strm);
                string strXml = sr.ReadToEnd();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(strXml);
                List<AppleTrailer> trailers = ParseDocument(xDoc);
                return trailers;
            }
            catch { }
            return null;
        }
    }

    public class AppleTrailer
    {
        List<ApplePoster> posters = new List<ApplePoster>();
        List<ApplePreview> previews = new List<ApplePreview>();

        public string AppleId { get; set; }
        public string FandangoId { get; set; }
        public string MovieTitle { get; set; }
        public string Runtime { get; set; }

        public void AddPoster(ApplePoster poster)
        {
            posters.Add(poster);
        }

        public void AddPreview(ApplePreview preview)
        {
            previews.Add(preview);
        }
    }

    public class ApplePoster
    {
        public string Type { get; set; }
        public string Url { get; set; }
    }

    public class ApplePreview
    {
        public string Type { get; set; }
        public string Url { get; set; }
    }
}
