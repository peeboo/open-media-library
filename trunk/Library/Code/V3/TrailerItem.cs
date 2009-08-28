using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using System.IO;
using Microsoft.MediaCenter.UI;
using OMLEngine.Settings;

namespace Library.Code.V3
{
    public class TrailerItem : GalleryItem
    {
        public string Runtime { get; set; }
        public string Rating { get; set; }
        public string Studio { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Copyright { get; set; }
        public string Director { get; set; }
        
        public DateTime Postdate { get; set; }
        public IList<string> Actors = new List<string>();
        public IList<string> Genres = new List<string>();
        public string PosterUrl { get; set; }
        public string XLargePosterUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string Filesize { get; set; }

        public TrailerItem(IModelItem owner)
            : base(owner)
        {
            this.Invoked += delegate(object sender, EventArgs args)
            {
                this.PlayMovie();
            };

            //this.Description = title.Caption;

            //this.Invoked += delegate(object sender, EventArgs args)
            //{
            //    //am I being silly here copying this?
            //    List<TitleFilter> newFilter = new List<TitleFilter>();
            //    foreach (TitleFilter filt in filter)
            //    {
            //        newFilter.Add(filt);
            //    }
            //    newFilter.Add(new TitleFilter(title.Category.FilterType, title.Name));
            //    OMLProperties properties = new OMLProperties();
            //    properties.Add("Application", OMLApplication.Current);
            //    properties.Add("I18n", I18n.Instance);
            //    Command CommandContextPopOverlay = new Command();
            //    properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

            //    Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(newFilter, title.Description);

            //    properties.Add("Page", gallery);
            //    OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            //};
        }

        public override Image DefaultImage
        {
            get
            {
                return new Image(this.PosterUrl);
            }
        }

        /// <summary>
        /// gives our Now Playing a pretty name
        /// </summary>
        /// <param name="videoFiles"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string createTempPlaylistWrapper(string videoURL, string name)
        {
            //Writes text to a temporary file and returns path
            string strFilename = OMLEngine.FileSystemWalker.TempPlayListDirectory;
            strFilename = Path.Combine(strFilename, System.Guid.NewGuid().ToString() + ".wvx");
            using (StreamWriter sw = new StreamWriter(strFilename))
            {
                sw.Write(string.Format("<ASX version = \"3.0\">  <TITLE>{0}</TITLE>    ", name));
                sw.Write("<ENTRY>   <TITLE>{0}</TITLE>   <REF HREF = \"{1}\" /> </ENTRY> ", name, videoURL);
                sw.Write(" </ASX> ");
            }
            return strFilename;
        }

        /// <summary>
        /// Plays the movie.
        /// </summary>
        public void PlayMovie() {
            Disk disk = new Disk("trailer", TrailerUrl, VideoFormat.URL);
            MediaSource source = new MediaSource(disk);
            IPlayMovie moviePlayer = MoviePlayerFactory.CreateMoviePlayer(source);
            moviePlayer.PlayMovie();
        }

        static public void Transport_PropertyChanged(IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                Microsoft.MediaCenter.MediaTransport t = (Microsoft.MediaCenter.MediaTransport)sender;
                OMLApplication.DebugLine("[AppleTrailers] Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    OMLApplication.Current.NowPlayingStatus = t.PlayState;
                    OMLApplication.DebugLine("[AppleTrailers] MoviePlayerFactory.Transport_PropertyChanged: movie {0} state {1}", OMLApplication.Current.NowPlayingMovieName, t.PlayState.ToString());

                    if (t.PlayState == Microsoft.MediaCenter.PlayState.Finished || t.PlayState == Microsoft.MediaCenter.PlayState.Stopped)
                    {
                        OMLApplication.DebugLine("[AppleTrailers] Playstate is stopped, moving to previous page");
                        OMLApplication.Current.Session.BackPage();
                    }
                }
            });
        }
    }

    public class TrailerGenreItem : GalleryItem
    {

        public TrailerGenreItem(IModelItem owner)
            : base(owner)
        {
            this.trailers = new List<TrailerItem>();

            this.Invoked += delegate(object sender, EventArgs args)
            {
                //this.PlayMedia();
            };
        }

        public override Image DefaultImage
        {
            get
            {
                return null;
            }
        }

        private List<TrailerItem> trailers;
        public List<TrailerItem> Trailers
        {
            get
            {
                return this.trailers;
            }
        }

        public override string Metadata
        {
            get
            {
                return string.Format("{0} trailers", this.trailers.Count.ToString());
            }
        }
    }
}
