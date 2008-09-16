using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace AppleTrailers
{
    public class AppleTrailers
    {
        private List<AppleTrailer> trailers;

        public List<AppleTrailer> Trailers
        {
            get { return trailers; }
        }

        public AppleTrailers()
        {
            trailers = new List<AppleTrailer>();
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
                    trailers.Add(trailer);
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
            catch (Exception e)
            {
            }
            return null;
        }
    }

    public class AppleTrailer
    {
        private string appleId;
        private string fandangoId;
        private string movieTitle;
        private string runtime;
        private List<ApplePoster> posters;
        private List<ApplePreview> previews;

        public string AppleId
        {
            get { return appleId;}
            set { appleId = value;}
        }

        public string FandangoId
        {
            get { return fandangoId;}
            set { fandangoId = value;}
        }

        public string MovieTitle
        {
            get { return movieTitle;}
            set { movieTitle = value;}
        }

        public string Runtime
        {
            get { return runtime;}
            set { runtime = value;}
        }

        public void AddPoster(ApplePoster poster)
        {
            posters.Add(poster);
        }

        public void AddPreview(ApplePreview preview)
        {
            previews.Add(preview);
        }

        public AppleTrailer()
        {
            posters = new List<ApplePoster>();
            previews = new List<ApplePreview>();
        }
    }

    public class ApplePoster
    {
        string type;
        string url;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
    }

    public class ApplePreview
    {
        string type;
        string url;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Url
        {
            get { return type; }
            set { type = value; }
        }
    }
}
