using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using System.Xml;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Library
{
    public class Movie
    {
        private bool useTitles = true;
        private DataSet dataSet;
        private static TitleCollection titleCollection;

        const string HTML_TAG_PATTERN = "<.*?>";
        private static DisplayItem[] myMovies = null;
        private static Boolean initialized = false;

        public Movie()
        {
            Trace.WriteLine("Movie:Movie()");
            titleCollection = new TitleCollection();
            Utilities.ImportData(ref dataSet);
            initialize();
        }
        public void initialize()
        {
            createGallery();
            initialized = true;
        }
        public DisplayItem[] GetMovies
        {
            get
            {
                if (!initialized)
                    initialize();

                return myMovies;
            }
        }
        public DisplayItem[] createGallery()
        {
            if (myMovies == null)
            {
                ArrayList list = new ArrayList();
                if (useTitles)
                {
                    foreach (Title title in titleCollection)
                    {
                        list.Add(CreateGalleryItem(title));
                    }
                }
                else
                {
                    Trace.WriteLine("using movieData");
                    DataTable tbl_Movie = dataSet.Tables["movie"];
                    foreach (DataRow movieData in tbl_Movie.Rows)
                    {
                        list.Add(CreateGalleryItem(movieData));
                    }
                }
                titleCollection.saveTitleCollection();
                myMovies = (DisplayItem[])list.ToArray(typeof(DisplayItem));
            }
            return myMovies;
        }
        private DisplayItem CreateGalleryItem(Title title)
        {
            Trace.WriteLine("Movie:CreateGalleryItem(): Title");
            DisplayItem item = new DisplayItem();
            item.Description = title.description;
            item.title = title.Name;
            item.itemId = title.itemId;
            Trace.WriteLine("here");
            item.image = Utilities.LoadImage(title.boxart_path);
            Trace.WriteLine("and here");
            item.runtime = title.runtime;
            item.mpaaRating = title.mpaa_rating;
            item.imdbRating = title.imdb_rating;

            item.Invoked += delegate(object sender, EventArgs args)
            {
                DisplayItem galleryItem = (DisplayItem)sender;

                // Navigate to a details page for this item.
                DetailsPage page = CreateDetailsPage(galleryItem.itemId);
                Application.Current.GoToDetails(page);
            };

            return item;
        }
        private IList GetActors(Title title)
        {
            IList Actors = new List<string>();
            foreach (string actor_name in title.actors)
            {
                Actors.Add(actor_name);
            }
            return Actors;
        }
        private IList GetCrew(Title title)
        {
            IList crew = new List<string>();
            foreach (string crew_member in title.crew)
            {
                crew.Add(crew_member);
            }
            return crew;
        }
        private string getUrl(DataRow dataRow)
        {
            IList crew = new List<string>();
            DataRow[] castTable = dataRow.GetChildRows("movie_links");
            DataRow castR = castTable[0];

            DataRow[] urlRow = castR.GetChildRows("links_link");

            // Get the entries that match our movie.
            foreach (DataRow url in urlRow)
            {
                if (url["urltype"].Equals("Movie"))
                {
                    return (string) url["url"];                    
                }
            }
            return null;
        }

        #region deprecated methods
        private MovieMetadata ExtractMetadata(DataRow movieData, int movieId)
        {
            MovieMetadata metadata = new MovieMetadata();
            metadata.Id = movieId;

            metadata.Title = (string)movieData["title"];
            metadata.Summary = StripHTML((string)movieData["plot"]);
            metadata.ImagePath = (string)movieData["coverfront"];
            metadata.Length = (string)movieData["runtime"];
            metadata.ReleaseDate = getChildColumn(movieData, "movie_releasedate", "date");
            metadata.Actors = getActors(movieData);
            metadata.Url = getUrl(movieData);
            metadata.Directors = getCrew(movieData, "dfDirector");
            metadata.Producers = getCrew(movieData, "dfProducer");
            //metadata.Rating = getChildColumn(movieData, "mpaarating", "displayname");
            /*
            if (movieData["imdbrating"] == System.DBNull.Value)
            {
                metadata.ImdbRating = "N/A";
            }
            else
            {
                metadata.ImdbRating = (string)movieData["imdbrating"];
            }
            */
            //
            // Genre
            //


            //
            // Country
            //
            //

            return metadata;
        }
        private string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }
        private string StripRating(string inputString)
        {
            return Regex.Replace(inputString, "()", string.Empty);
        }
        public DetailsPage CreateDetailsPage(int movieId)
        {
            DetailsPage page = new DetailsPage();

            //
            // Get the full metadata from this row.
            //
            DataRow movieData = GetMovieData(movieId);
            MovieMetadata metadata = ExtractMetadata(movieData, movieId);


            //
            // Fill in the page's easy properties.
            //
            page.Title = metadata.Title;
            page.Summary = metadata.Summary;
            page.Background = Utilities.LoadImage(metadata.ImagePath);
            page.Rating = metadata.Rating;
            page.Length = metadata.Length;
            page.ReleaseDate = metadata.ReleaseDate;
            page.Actors = metadata.Actors;
            page.Directors = metadata.Directors;
            page.Producers = metadata.Producers;
            page.Writers = metadata.Writers;
            page.ImdbRating = metadata.ImdbRating;
            //page.LocalMedia = new System.IO.FileInfo("C:\\users\\dxs\\documents\\Downloads\\Good Eats - Season 6\\Good Eats - S06E16 - Beet It.avi");

            return page;
        }
        private DisplayItem CreateGalleryItem(DataRow movieData)
        {
            Trace.WriteLine("Movie::CreateGalleryItem()");
            DisplayItem item = new DisplayItem();
            Title title = new Title();
            item.Description = (string)movieData["title"];
            title.description = item.Description;
            item.title = (string)movieData["title"];
            title.Name = item.title;
            item.itemId = int.Parse((string)movieData["id"]);
            title.itemId = item.itemId;
            item.image = Utilities.LoadImage((string)movieData["coverfront"]);
            title.boxart_path = (string)movieData["coverfront"];
            item.runtime = (string)movieData["runtime"];
            title.runtime = item.runtime;
            item.mpaaRating = getChildColumn(movieData, "mpaarating", "displayname");
            title.mpaa_rating = item.mpaaRating;
            item.imdbRating = "N/A";
            title.imdb_rating = item.imdbRating;

            titleCollection.AddTitle(title);
            //
            // Hook up an event for when the gallery item is invoked.
            //
            item.Invoked += delegate(object sender, EventArgs args)
            {
                DisplayItem galleryItem = (DisplayItem)sender;

                // Navigate to a details page for this item.
                DetailsPage page = CreateDetailsPage(galleryItem.itemId);
                Application.Current.GoToDetails(page);
            };

            return item;
        }
        private IList getActors(DataRow dataRow)
        {
            IList Actors = new List<string>();
            DataRow[] castTable = dataRow.GetChildRows("movie_cast");
            DataRow castR = castTable[0];

            DataRow[] starRow = castR.GetChildRows("cast_star");

            // Get the entries that match our movie.
            foreach (DataRow star in starRow)
            {
                DataRow[] personRow = star.GetChildRows("star_person");
                Actors.Add((string)personRow[0]["displayname"]);
            }
            return Actors;
        }
        private IList getCrew(DataRow dataRow, String fieldId)
        {
            IList crew = new List<string>();
            DataRow[] castTable = dataRow.GetChildRows("movie_crew");
            DataRow castR = castTable[0];

            DataRow[] starRow = castR.GetChildRows("crew_crewmember");

            // Get the entries that match our movie.
            foreach (DataRow star in starRow)
            {
                if (star["roleid"].Equals(fieldId))
                {
                    DataRow[] personRow = star.GetChildRows("crewmember_person");
                    crew.Add((string)personRow[0]["displayname"]);
                }
            }
            return crew;
        }
        public DataRow GetMovieData(int itemId)
        {
            DataTable tbl_Movie = dataSet.Tables["movie"];
            return Movie.GetSingleDataRow(tbl_Movie, "id", itemId);
        }
        public static DataRow GetSingleDataRow(DataTable table, string column, object value)
        {
            string query = String.Format("{0} = '{1}'", column, value);
            DataRow[] matches = table.Select(query);

            if (matches.Length != 1)
                throw new Exception("Unable to find a matching for " + query);

            return matches[0];
        }
        private String getChildColumn(DataRow dataRow, String relatedTable, String column)
        {
            DataRow[] rows = dataRow.GetChildRows(relatedTable);

            string data_point = string.Empty;

            if (rows.Length > 0)
            {
                try
                {
                    data_point = (string)rows[0][column];
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
            return data_point;
        }
        #endregion
    }
}