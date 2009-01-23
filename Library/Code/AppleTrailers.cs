using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class AppleTrailers : ModelItem
    {
        private static string LoFiUrl = @"http://www.apple.com/trailers/home/xml/current.xml";
        private static string HiFiUrl = @"http://www.apple.com/trailers/home/xml/current_720p.xml";

        public Choice _trailers = new Choice();

        public bool InitialRotation
        {
            get
            {
                for (int i = 0; i < _trailers.Options.Count; i++)
                {
                    trailers.ChosenIndex = i;
                    System.Threading.Thread.Sleep(100);
                }
                return true;
            }
        }

        public Choice trailers
        {
            get { return _trailers; }
            set
            {
                _trailers = value;
                FirePropertyChanged("trailers");
            }
        }

        public Double AngleDegrees
        {
            get { return -(180 / 10); }
        }

        public AppleTrailers()
        {
            OMLApplication.DebugLine("AppleTrailers constructor called");
            LoadTrailers();
        }

        public void LoadTrailers()
        {
            string fidelity = Properties.Settings.Default.AppleTrailerFidelity;
            if (string.IsNullOrEmpty(fidelity) == true)
                fidelity = @"Low";

            ArrayListDataSet newTrailers;
            switch(fidelity)
            {
                case "Hi":
                    newTrailers = GetTrailersForUrl(AppleTrailers.HiFiUrl);
                    break;
                case "Low":
                    newTrailers = GetTrailersForUrl(AppleTrailers.LoFiUrl);
                    break;
                default:
                    newTrailers = GetTrailersForUrl(AppleTrailers.LoFiUrl);
                    break;
            }

            _trailers.Options = newTrailers;
        }

        private ArrayListDataSet ParseDocument(XmlDocument xDoc)
        {
            ArrayListDataSet trailers = new ArrayListDataSet();
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

                    if (localNav.MoveToChild("info", ""))
                    {
                        currentTrailer.Copyright = GetChildNodesValue(localNav, "copyright");
                        currentTrailer.description = GetChildNodesValue(localNav, "description");
                        currentTrailer.Director = GetChildNodesValue(localNav, "director");
                        currentTrailer.Postdate = GetChildNodesValue(localNav, "postdate");
                        currentTrailer.Rating = GetChildNodesValue(localNav, "rating");
                        currentTrailer.ReleaseDate = GetChildNodesValue(localNav, "releasedate");
                        currentTrailer.Runtime = GetChildNodesValue(localNav, "runtime");
                        currentTrailer.Studio = GetChildNodesValue(localNav, "studio");
                        currentTrailer.Title = GetChildNodesValue(localNav, "title");
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("cast", ""))
                    {
                        XPathNodeIterator castIter = localNav.SelectChildren("name", "");
                        if (localNav.MoveToFirstChild())
                        {
                            XPathNavigator castNav = localNav.CreateNavigator();

                            for (int j = 0; j < castIter.Count; j++)
                            {
                                currentTrailer.Actors.Add(castNav.Value);
                                castNav.MoveToNext("name", "");
                            }
                            localNav.MoveToParent();
                        }
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("genre", ""))
                    {
                        XPathNodeIterator genreIter = localNav.SelectChildren("name", "");
                        if (localNav.MoveToFirstChild())
                        {
                            XPathNavigator genreNav = localNav.CreateNavigator();

                            for (int k = 0; k < genreIter.Count; k++)
                            {
                                currentTrailer.Genres.Add(genreNav.Value);
                                genreNav.MoveToNext("name", "");
                            }
                            localNav.MoveToParent();
                        }
                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("poster", ""))
                    {
                        if (localNav.MoveToChild("location", ""))
                        {
                            currentTrailer.PosterUrl = localNav.Value;
                            localNav.MoveToParent();
                        }

                        if (localNav.MoveToChild("xlarge", ""))
                        {
                            currentTrailer.XLargePosterUrl = localNav.Value;
                            localNav.MoveToParent();
                        }

                        localNav.MoveToParent();
                    }

                    if (localNav.MoveToChild("preview", ""))
                    {
                        if (localNav.MoveToChild(XPathNodeType.Element))
                        {
                            currentTrailer.TrailerUrl = localNav.Value;
                            currentTrailer.Filesize = localNav.GetAttribute("filesize", "");
                            localNav.MoveToParent();
                        }
                        localNav.MoveToParent();
                    }

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

        public ArrayListDataSet GetTrailersForUrl(string url)
        {
            try
            {
                WebClient client = new WebClient();
                Stream strm = client.OpenRead(url);
                StreamReader sr = new StreamReader(strm);
                string strXml = sr.ReadToEnd();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(strXml);
                ArrayListDataSet trailers = ParseDocument(xDoc);
                return trailers;
            }
            catch { }
            return null;
        }
    }

    public class AppleTrailer : ModelItem
    {
        public string Title { get; set; }
        public string Runtime { get; set; }
        public string Rating { get; set; }
        public string Studio { get; set; }
        public string ReleaseDate { get; set; }
        public string Copyright { get; set; }
        public string Director { get; set; }
        public string description { get; set; }
        public string Postdate { get; set; }
        public IList<string> Actors = new List<string>();
        public IList<string> Genres = new List<string>();
        public string PosterUrl { get; set; }
        public string XLargePosterUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string Filesize { get; set; }

        public Image Poster
        {
            get
            {
                return new Image(this.PosterUrl);
            }
        }

        public void PlayMedia()
        {
            OMLApplication.DebugLine("Playing trailer: {0}", this.TrailerUrl);
            AddInHost.Current.MediaCenterEnvironment.PlayMedia(Microsoft.MediaCenter.MediaType.Video, this.TrailerUrl, false);
            AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
            AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged +=new PropertyChangedEventHandler(Transport_PropertyChanged);
        }

        static public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                OMLApplication.DebugLine("[AppleTrailers] Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    OMLApplication.Current.NowPlayingStatus = t.PlayState;
                    OMLApplication.DebugLine("[AppleTrailers] MoviePlayerFactory.Transport_PropertyChanged: movie {0} state {1}", OMLApplication.Current.NowPlayingMovieName, t.PlayState.ToString());

                    if (t.PlayState == PlayState.Finished || t.PlayState == PlayState.Stopped)
                    {
                        OMLApplication.DebugLine("[AppleTrailers] Playstate is stopped, moving to previous page");
                        OMLApplication.Current.Session.BackPage();
                    }
                }
            });
        }
    }
}
