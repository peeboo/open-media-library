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
        public static int TITLE = 14;
        public static int FRONT_COVER = 2;
        public static int REAR_COVER = 3;
        private DataSet dataSet;
        const string HTML_TAG_PATTERN = "<.*?>";
        private static DisplayItem[] myArr = null;
        private static Boolean initialized = false;

        public Movie()
        {
            string fileName = "C:\\Users\\Public\\Open Media Library\\movies.xml";
            TextReader rawReader = null;
            try
            {
                rawReader = new StreamReader(fileName);
            }
            catch (DirectoryNotFoundException e)
            {
                // directory does not exist, therefore Z was not
                // installed correctly
                throw new InvalidOperationException("The application data has not been correctly installed.", e);
            }
            XmlReader xmlReader = new XmlTextReader(rawReader);

            dataSet = new DataSet("Movie");
            dataSet.ReadXml(xmlReader);
            
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
                {
                    initialize();
                }
                return myArr;
            }
        }

        public DisplayItem[] createGallery()
        {
            if (myArr == null)
            {
                DataTable tbl_Movie = dataSet.Tables["movie"];
                ArrayList list = new ArrayList();
                foreach (DataRow movieData in tbl_Movie.Rows)
                {
                    list.Add(CreateGalleryItem(movieData));
                }
                myArr = (DisplayItem[])list.ToArray(typeof(DisplayItem));
            }
            return myArr;
        }

        /// <summary>
        /// Construct a gallery item for a row of data.
        /// </summary>
        private DisplayItem CreateGalleryItem(DataRow movieData)
        {
            DisplayItem item = new DisplayItem();
            //item.Description = (string)movieData["title"];
            item.title = (string)movieData["title"];
            item.itemId = int.Parse((string)movieData["id"]);
            item.image = LoadImage((string)movieData["coverfront"]);
            item.runtime = (string)movieData["runtime"];
            item.mpaaRating = getChildColumn(movieData, "movie_mpaarating", "displayname");

            if (movieData["imdbrating"] == System.DBNull.Value)
            {
                item.imdbRating = "N/A";
            }
            else
            {
                item.imdbRating = (string)movieData["imdbrating"];
            }

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


        /// <summary>
        /// Create a details page for a movie.
        /// NOTE: This is public to enable debug markup access.
        /// </summary>
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
            page.Background = LoadImage(metadata.ImagePath);
            page.Rating = metadata.Rating;
            page.Length = metadata.Length;
            page.ReleaseDate = metadata.ReleaseDate;
            page.Actors = metadata.Actors;
            page.Directors = metadata.Directors;
            page.Producers = metadata.Producers;
            page.Writers = metadata.Writers;
            page.ImdbRating = metadata.ImdbRating;
            page.LocalMedia = new System.IO.FileInfo(metadata.Url);

            return page;
        }

        /// <summary>
        /// Take the raw movie data row and create a nicely typed struct.
        /// </summary>
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
            metadata.Rating = getChildColumn(movieData, "movie_mpaarating", "displayname");
            
            if (movieData["imdbrating"] == System.DBNull.Value)
            {
                metadata.ImdbRating = "N/A";
            }
            else
            {
                metadata.ImdbRating = (string)movieData["imdbrating"];
            }

            //
            // Genre
            //


            //
            // Country
            //
            //

            return metadata;
        }

        private IList getActors(DataRow dataRow)
        {
            IList Actors = new List <string>();
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

        /// <summary>
        /// Gets a child column from a specified row
        /// getChildColumn(movieData, "movie_releasedate", "date");
        /// </summary>
        /// <param name="dataRow">current row</param>
        /// <param name="relatedTable">child table</param>
        /// <param name="column">the column in the child table</param>
        private String getChildColumn(DataRow dataRow, String relatedTable, String column)
        {
            DataRow[] rows = dataRow.GetChildRows(relatedTable);
            return (string)rows[0][column];
        }

        private string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }

        private string StripRating(string inputString)
        {
            return Regex.Replace(inputString, "()", string.Empty);
        }

        /// <summary>
        /// Get a movie data row given a unique id.
        /// </summary>
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

        /// <summary>
        /// Helper method to load an image from our data directory.
        /// </summary>
        private static Image LoadImage(string imageName)
        {
            try
            {
                return new Image("file://" + imageName);
            }
            catch (Exception)
            {
                Debug.WriteLine("Error loading image: " + imageName);
            }

            return null;
        }
    }
}