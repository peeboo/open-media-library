using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using System.IO;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    public class MovieItem : GalleryItem
    {
        public MovieItem(Library.GalleryItem title, IModelItem owner)
            : base(owner)
        {
            this.Description = title.Caption;
            
            this.Invoked += delegate(object sender, EventArgs args)
            {
                List<TitleFilter> filter = new List<TitleFilter>();
                filter.Add(new TitleFilter(title.Category.FilterType, title.Name));
                IEnumerable<Title> items = TitleCollectionManager.GetFilteredTitles(filter);
                GalleryPage page = new GalleryPage(items, title.Name);
            };
        }
        public MovieItem(Title title, IModelItem owner)
            : base(owner)
        {
            //to test deck
            //_deck.CommandPushOverlay.Invoke();

            //to test v2
            //if (this._page != null)
            //    this._page.PageState.TransitionState = Library.Code.V3.PageTransitionState.NavigatingAwayForward;

            //Library.Code.V3.GalleryItem galleryItem = (Library.Code.V3.GalleryItem)sender;

            //// Navigate to a details page for this item.
            //MovieDetailsPage page = galleryItem.InternalMovieItem.CreateDetailsPage(galleryItem.InternalMovieItem);
            //OMLApplication.Current.GoToDetails(page);

            //this needs to be backed out- should only reference the title object
            this.InternalMovieItem = new Library.MovieItem(title, null);
            this.ItemType = 0;
            string imageName = null;
            string moviePath = null;
            DateTime releaseDate = Convert.ToDateTime(this.InternalMovieItem.ReleaseDate);// new DateTime(2000, 1, 1);
            //invalid dates
            if (releaseDate.Year != 1 && releaseDate.Year != 1900)
                this.MetadataTop = releaseDate.Year.ToString();
            else
                this.MetadataTop = "";

            this.ItemId = 1;
            string starRating = Convert.ToString(Math.Round((Convert.ToDouble(this.InternalMovieItem.UseStarRating) * 0.8), MidpointRounding.AwayFromZero));
            this.StarRating = starRating;
            string extendedMetadata = string.Empty;

            this.SortName = title.SortName;

            this.Metadata = this.InternalMovieItem.Rating.Replace("PG13", "PG-13").Replace("NC17", "NC-17");
            if (string.IsNullOrEmpty(this.InternalMovieItem.Rating))
                this.Metadata = "Not Rated";
            if (this.InternalMovieItem.Runtime != "0")
                this.Metadata += string.Format(", {0} minutes", this.InternalMovieItem.Runtime);
            this.Tagline = this.InternalMovieItem.Synopsis;


            if (!string.IsNullOrEmpty(title.FrontCoverMenuPath) && File.Exists(title.FrontCoverMenuPath))
            {
                this.DefaultImage = new Image("file://" + title.FrontCoverMenuPath);
            }
            this.Description = title.Name;

            //TODO: not sure how to read this yet...
            this.SimpleVideoFormat = this.InternalMovieItem.TitleObject.VideoFormat.ToString();


            this.Invoked += delegate(object sender, EventArgs args)
            {

                //to test v3
                Library.Code.V3.DetailsPage page = new Library.Code.V3.DetailsPage();
                //DataRow movieData = this.GetMovieData(movieId);

                page.Description = "movie details";
                page.Title = this.Description;
                page.Summary = this.InternalMovieItem.Synopsis;
                page.Background = this.DefaultImage;
                page.Details = new Library.Code.V3.ExtendedDetails();

                page.Details.CastArray = new ArrayListDataSet();

                string directors = string.Empty;
                int directorCount = 0;
                foreach (string director in this.InternalMovieItem.Directors)
                {
                    Library.Code.V3.CastCommand directorCommand = new Library.Code.V3.CastCommand();
                    directorCommand.Role = " ";
                    directorCommand.Description = director;
                    if (directorCount == 0)
                    {
                        directorCommand.CastType = "TitleAndDesc";
                        directorCommand.GroupTitle = "DIRECTOR";
                        directorCommand.ActorId = 1;//not sure how this works in oml
                    }
                    else
                        directorCommand.CastType = "Desc";
                    page.Details.CastArray.Add(directorCommand);
                    directorCount++;
                    AppendSeparatedValue(ref directors, director, "; ");
                }
                if (!string.IsNullOrEmpty(directors))
                    page.Details.Director = string.Format("Directed By: {0}", directors);

                string cast = string.Empty;
                int actorCount = 0;
                foreach (Role kvp in this.InternalMovieItem.TitleObject.ActingRoles)
                {
                    Library.Code.V3.CastCommand actorCommand = new Library.Code.V3.CastCommand();
                    actorCommand.Description = kvp.PersonName;
                    actorCommand.ActorId = 1;//not sure how this works in oml
                    actorCommand.Invoked += new EventHandler(actorCommand_Invoked);
                    actorCommand.Role = kvp.RoleName;
                    if (actorCount == 0)
                    {
                        //add the title "CAST"
                        actorCommand.CastType = "TitleAndDesc";
                        actorCommand.GroupTitle = "CAST";
                    }
                    else
                        actorCommand.CastType = "Desc";

                    page.Details.CastArray.Add(actorCommand);
                    actorCount++;
                }

                foreach (string castmember in this.InternalMovieItem.Actors)
                {
                    AppendSeparatedValue(ref cast, castmember, "; ");
                }
                if (!string.IsNullOrEmpty(cast))
                    page.Details.Cast = string.Format("Cast Info: {0}", cast);


                if (this.StarRating != "0")
                    page.Details.StarRating = new Image(string.Format("resx://Library/Library.Resources/V3_Controls_Common_Stars_{0}", starRating));

                //strip invalid dates
                if (releaseDate.Year != 1 && releaseDate.Year != 1900)
                    page.Details.YearString = releaseDate.Year.ToString();

                page.Details.Studio = this.InternalMovieItem.Distributor;

                string genres = string.Empty;
                if (this.InternalMovieItem.TitleObject.Genres.Count > 0)
                {
                    foreach (string genre in this.InternalMovieItem.TitleObject.Genres)
                    {
                        AppendSeparatedValue(ref genres, genre, ", ");
                    }
                }
                AppendSeparatedValue(ref genres, this.Metadata, ", ");
                page.Details.GenreRatingandRuntime = genres;

                //fixes double spacing issues
                page.Details.Summary = this.InternalMovieItem.Synopsis.Replace("\r\n", "\n");
                page.Commands = new ArrayListDataSet(page);
                //default play command
                Command playCmd = new Command();
                playCmd.Description = "Play";
                playCmd.Invoked += new EventHandler(playCmd_Invoked);
                page.Commands.Add(playCmd);

                if (page.Details.CastArray.Count > 0)
                {
                    foreach (Library.Code.V3.CastCommand actor in page.Details.CastArray)
                    {
                        //actor.Invoked += new EventHandler(actor_Invoked);
                        //TitleCollection titles = new TitleCollection();
                        TitleFilter personFilter = new TitleFilter(TitleFilterType.Person, actor.Description);
                        TitleCollectionManager.GetFilteredTitles(new List<TitleFilter>() { personFilter });
                    }

                    Command command = new Command();
                    command.Description = "Cast + More";

                    command.Invoked += delegate(object castSender, EventArgs castArgs)
                    {
                        Dictionary<string, object> castProperties = new Dictionary<string, object>();
                        castProperties["Page"] = page;
                        castProperties["Application"] = Library.OMLApplication.Current;
                        Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPageCastCrew", castProperties);
                    };

                    page.Commands.Add(command);
                }

                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Page"] = page;
                properties["Application"] = Library.OMLApplication.Current;

                Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPage", properties);
            };
        }

        void actorCommand_Invoked(object sender, EventArgs e)
        {
            Library.Code.V3.CastCommand actor=(Library.Code.V3.CastCommand)sender;
            List<TitleFilter> filter = new List<TitleFilter>();
            filter.Add(new TitleFilter(TitleFilterType.Person, actor.Description));
            IEnumerable<Title> items = TitleCollectionManager.GetFilteredTitles(filter);
            GalleryPage page = new GalleryPage(items, actor.Description);
        }

        void playCmd_Invoked(object sender, EventArgs e)
        {
            this.InternalMovieItem.PlayAllDisks();
        }

        private static void AppendSeparatedValue(ref string text, string value, string seperator)
        {
            if (String.IsNullOrEmpty(text))
                text = value;
            else
                text = string.Format("{0}{1}{2}", text, seperator, value);
        }
    }
}
