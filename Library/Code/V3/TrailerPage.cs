using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using Library.Code.V3;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;

namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// gallery page UI.
    /// </summary>
    /// 
    [Microsoft.MediaCenter.UI.MarkupVisible]
    public class TrailerPage : GalleryPage
    {
        private static string LoFiUrl = @"http://www.apple.com/trailers/home/xml/current.xml";
        private static string HiFiUrl = @"http://www.apple.com/trailers/home/xml/current.xml";
        //private static string HiFiUrl = @"http://www.apple.com/trailers/home/xml/current_720p.xml";

        public TrailerPage(string description)
            : base()
        {
            //description
            this.Description = description;
            this.trailers = new List<TrailerItem>();
            this.genres = new List<TrailerGenreItem>();
            this.trailersDateAdded = new List<TrailerItem>();
            this.Model = new Library.Code.V3.BrowseModel(this);
            this.Model.Pivots = new Choice(this, "desc", new ArrayListDataSet(this));
            base.CreateCommands();
            this.CreatePivots();
            //fire async!
            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(this.beginLoadTrailers, this.endLoadTrailers, null);

        }

        public TrailerPage(string description, TrailerGenreItem genre)
            : base()
        {
            //description
            this.Description = description;
            this.Model = new Library.Code.V3.BrowseModel(this);
            this.Model.Pivots = new Choice(this, "desc", new ArrayListDataSet(this));
            base.CreateCommands();

        }

        private void beginLoadTrailers(object o)
        {
            string fidelity = OMLEngine.Settings.OMLSettings.AppleTrailerFidelity;
            if (string.IsNullOrEmpty(fidelity) == true)
                fidelity = @"Low";

            switch (fidelity)
            {
                case "Hi":
                    this.GetTrailersForUrl(TrailerPage.HiFiUrl);
                    break;
                case "Low":
                    this.GetTrailersForUrl(TrailerPage.LoFiUrl);
                    break;
                default:
                    this.GetTrailersForUrl(TrailerPage.LoFiUrl);
                    break;
            }

        }

        private void endLoadTrailers(object o)
        {
            this.trailers = this.SortTrailersByName(this.trailers) as List<TrailerItem>;
            //stuff the virtuallist
            foreach (TrailerItem t in this.trailers)
            {
                this.titlePivot.Content.Add(t);
            }
            titlePivot.IsBusy = false;

            this.trailersDateAdded = this.SortTrailersByDate(this.trailers) as List<TrailerItem>;

            //stuff the virtuallist
            foreach (TrailerItem t in this.trailersDateAdded)
            {
                this.dateAddedPivot.Content.Add(t);
            }
            dateAddedPivot.IsBusy = false;
                
            this.genres = this.SortGenresByName(this.genres) as List<TrailerGenreItem>;
            //stuff the virtuallist
            foreach (TrailerGenreItem t in this.genres)
            {
                this.genrePivot.Content.Add(t);
            }
            genrePivot.IsBusy = false;
        }

        private List<TrailerGenreItem> genres;
        public List<TrailerGenreItem> Genres
        {
            get { return this.genres; }
        }

        private List<TrailerItem> trailers;
        public List<TrailerItem> Trailers
        {
            get { return this.trailers; }
        }

        private List<TrailerItem> trailersDateAdded;
        public List<TrailerItem> TrailersDateAdded
        {
            get { return this.trailersDateAdded; }
        }

        private void ParseDocument(XmlDocument xDoc)
        {
            //this.trailers = new List<TrailerItem>();
            XPathNavigator nav = xDoc.CreateNavigator();

            XPathExpression expr;
            expr = nav.Compile("//*/genre/name[not(.=preceding::*/genre/name)] ");
            XPathNodeIterator iterator = nav.Select(expr);

            // Iterate on the node set
            //genres = new List<TrailerGenreItem>();
            while (iterator.MoveNext())
            {
                XPathNavigator nav2 = iterator.Current.Clone();
                Application.DeferredInvoke(delegate
                {
                    TrailerGenreItem genre = new TrailerGenreItem(this);
                    genre.Description = nav2.Value;
                    genre.Invoked += delegate(object genreSender, EventArgs genreArgs)
                    {
                        OMLProperties properties = new OMLProperties();
                        properties.Add("Application", OMLApplication.Current);
                        properties.Add("I18n", I18n.Instance);
                        Library.Code.V3.TrailerPage gallery = new Library.Code.V3.TrailerPage("trailers", genre);
                        Command CommandContextPopOverlay = new Command();
                        properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);
                        //add name piv
                        List<TrailerItem> nameTrailers=this.SortTrailersByName(genre.Trailers) as List<TrailerItem>;
                        VirtualList nameGalleryList = new VirtualList(gallery, null);
                        foreach (TrailerItem t in nameTrailers)
                        {
                            nameGalleryList.Add(t);
                        }
                        Library.Code.V3.BrowsePivot namePivot = new Library.Code.V3.BrowsePivot(gallery, "title", "No titles were found.", nameGalleryList);
                        namePivot.ContentLabel = genre.Description;
                        namePivot.SetupContextMenu();
                        namePivot.SupportsJIL = true;
                        namePivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";

                        namePivot.ContentItemTemplate = "oneRowGalleryItemPoster";
                        namePivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
                        gallery.Model.Pivots.Options.Add(namePivot);
                        gallery.Model.Pivots.Chosen = namePivot;

                        List<TrailerItem> dateTrailers = this.SortTrailersByDate(genre.Trailers) as List<TrailerItem>;
                        VirtualList dateGalleryList = new VirtualList(gallery, null);
                        foreach (TrailerItem t in dateTrailers)
                        {
                            dateGalleryList.Add(t);
                        }
                        Library.Code.V3.BrowsePivot datePivot = new Library.Code.V3.BrowsePivot(gallery, "date added", "No titles were found.", dateGalleryList);
                        datePivot.ContentLabel = genre.Description;
                        datePivot.SetupContextMenu();
                        datePivot.SupportsJIL = true;
                        datePivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";

                        datePivot.ContentItemTemplate = "oneRowGalleryItemPoster";
                        datePivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
                        gallery.Model.Pivots.Options.Add(datePivot);

                        properties.Add("Page", gallery);
                        OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
                    };
                    genres.Add(genre);
                });

            }

            if (nav.MoveToChild("records", ""))
            {
                XPathNodeIterator nIter = nav.SelectChildren("movieinfo", "");
                nav.MoveToFirstChild();
                XPathNavigator localNav = nav.CreateNavigator();
                nav.MoveToParent();

                for (int i = 0; i < nIter.Count; i++)
                {
                    Application.DeferredInvoke(delegate
                {
                    TrailerItem currentTrailer = new TrailerItem(this);

                    if (localNav.MoveToChild("info", ""))
                    {
                        currentTrailer.Copyright = GetChildNodesValue(localNav, "copyright");
                        currentTrailer.Tagline = GetChildNodesValue(localNav, "description");
                        currentTrailer.Director = GetChildNodesValue(localNav, "director");
                        //currentTrailer.Postdate = GetChildNodesValue(localNav, "postdate");
                        currentTrailer.Rating = GetChildNodesValue(localNav, "rating");
                        //currentTrailer.ReleaseDate = GetChildNodesValue(localNav, "releasedate");
                        string postdate = GetChildNodesValue(localNav, "postdate");
                        if (!string.IsNullOrEmpty(postdate))
                        {
                            try
                            {
                                currentTrailer.Postdate = Convert.ToDateTime(postdate);
                            }
                            catch
                            {
                                currentTrailer.Postdate = new DateTime(1900, 1, 1);
                            }
                        }
                        string releaseDate = GetChildNodesValue(localNav, "releasedate");
                        if (!string.IsNullOrEmpty(releaseDate))
                        {
                            try
                            {
                                currentTrailer.ReleaseDate = Convert.ToDateTime(releaseDate);
                                currentTrailer.MetadataTop = string.Format("{0}, in theaters {1}", currentTrailer.Rating, currentTrailer.ReleaseDate.ToString("D", CultureInfo.CreateSpecificCulture("en-US")));
                            }
                            catch
                            {
                                currentTrailer.ReleaseDate = new DateTime(1900, 1, 1);
                            }
                        }
                        currentTrailer.Runtime = GetChildNodesValue(localNav, "runtime");
                        currentTrailer.Studio = GetChildNodesValue(localNav, "studio");
                        currentTrailer.Metadata = currentTrailer.Studio;
                        currentTrailer.Description = GetChildNodesValue(localNav, "title");
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

                    //this could probably be done better
                    foreach (string strGenre in currentTrailer.Genres)
                    {
                        foreach (TrailerGenreItem genre in this.genres)
                        {
                            if (strGenre == genre.Description)
                            {
                                genre.Trailers.Add(currentTrailer);
                                break;
                            }
                        }
                    }

                    localNav.MoveToNext();
                });
                }

            }
            //return trailers;
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

        public bool GetTrailersForUrl(string url)
        {
            try
            {
                WebClient client = new WebClient();
                Stream strm = client.OpenRead(url);
                StreamReader sr = new StreamReader(strm);
                string strXml = sr.ReadToEnd();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(strXml);
                this.ParseDocument(xDoc);
                //return trailers;
                return true;
            }
            catch { }
            //return null;
            return false;
        }

        private BrowsePivot titlePivot;
        private BrowsePivot dateAddedPivot;
        private BrowsePivot genrePivot;
        private void CreatePivots()
        {
            titlePivot = new Library.Code.V3.BrowsePivot(this, "title", "No titles were found.", new VirtualList(this, null));            
            titlePivot.ContentLabel = this.Description;
            titlePivot.SetupContextMenu();
            titlePivot.LoadingContentMessage = "Loading trailers...";
            titlePivot.SupportsJIL = false;
            titlePivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            titlePivot.ContentItemTemplate = "oneRowGalleryItemPoster";
            titlePivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            titlePivot.IsBusy = true;
            this.Model.Pivots.Options.Add(titlePivot);
            //setting this as the default chosen pivot
            this.Model.Pivots.Chosen = titlePivot;

            dateAddedPivot = new Library.Code.V3.BrowsePivot(this, "date added", "No titles were found.", new VirtualList(this, null));
            dateAddedPivot.ContentLabel = this.Description;
            dateAddedPivot.SetupContextMenu();
            dateAddedPivot.LoadingContentMessage = "Loading trailers...";
            dateAddedPivot.SupportsJIL = false;
            dateAddedPivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            dateAddedPivot.ContentItemTemplate = "oneRowGalleryItemPoster";
            dateAddedPivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            dateAddedPivot.IsBusy = true;
            this.Model.Pivots.Options.Add(dateAddedPivot);
            

            genrePivot = new Library.Code.V3.BrowsePivot(this, "genres", "No titles were found.", new VirtualList(this, null));
            genrePivot.ContentLabel = this.Description;
            genrePivot.SetupContextMenu();
            genrePivot.LoadingContentMessage = "Loading trailers...";
            genrePivot.SupportsJIL = true;
            genrePivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            genrePivot.ContentItemTemplate = "ListViewItem";
            genrePivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            genrePivot.IsBusy = true;
            this.Model.Pivots.Options.Add(genrePivot);

        }

        public IEnumerable<TrailerItem> SortTrailersByDate(IEnumerable<TrailerItem> titles)
        {
            List<TrailerItem> sortedList = new List<TrailerItem>(titles);
            sortedList.Sort(delegate(TrailerItem x, TrailerItem y) { return x.ReleaseDate.CompareTo(y.ReleaseDate); });
            return sortedList;
        }
        public IEnumerable<TrailerItem> SortTrailersByName(IEnumerable<TrailerItem> titles)
        {
            List<TrailerItem> sortedList = new List<TrailerItem>(titles);
            sortedList.Sort(delegate(TrailerItem x, TrailerItem y) { return x.Description.CompareTo(y.Description); });
            return sortedList;
        }

        public IEnumerable<TrailerGenreItem> SortGenresByName(IEnumerable<TrailerGenreItem> titles)
        {
            List<TrailerGenreItem> sortedList = new List<TrailerGenreItem>(titles);
            sortedList.Sort(delegate(TrailerGenreItem x, TrailerGenreItem y) { return x.Description.CompareTo(y.Description); });
            return sortedList;
        }
    }
}
