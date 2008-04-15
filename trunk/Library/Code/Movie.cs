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
using System.Reflection;

namespace Library
{
    public class Movie
    {
        static object syncObj = new object();
        static MethodInfo fromStreamMethodInfo = null;

        public static int TITLE = 14;
        public static int FRONT_COVER = 2;
        public static int REAR_COVER = 3;
        private DataSet dataSet;
        const string HTML_TAG_PATTERN = "<.*?>";
        private static DisplayItem[] myMovies = null;
        private static Title[] titles = null;
        private static Boolean initialized = false;

        public Movie()
        {
            Trace.WriteLine("Movie:Movie()");

            Type ImporterClassType = getImporterClassType();
            if (ImporterClassType.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(ImporterClassType);
                    MethodInfo mi = ImporterClassType.GetMethod("getDataSet");
                    dataSet = (DataSet)mi.Invoke(obj, null);
                }
                catch (FileNotFoundException e)
                {
                    Trace.WriteLine(e.Message);
                }
            }

            initialize();
        }

        private Type getImporterClassType()
        {
            return typeof(MoviesXmlImporter);
        }

        public void initialize()
        {
            Trace.WriteLine("Movie:initialize()");
            createGallery();
            initialized = true;
        }

        public DisplayItem[] GetMovies
        {
            get
            {
                Trace.WriteLine("Movies:GetMovies() With " + myMovies.Length + " movies");
                if (!initialized)
                {
                    initialize();
                }
                return myMovies;
            }
        }

        public DisplayItem[] createGallery()
        {
            Trace.WriteLine("Movie:createGallery()");
            if (myMovies == null)
            {
                DataTable tbl_Movie = dataSet.Tables["movie"];
                ArrayList list = new ArrayList();
                foreach (DataRow movieData in tbl_Movie.Rows)
                {
                    list.Add(CreateGalleryItem(movieData));
                }
                myMovies = (DisplayItem[])list.ToArray(typeof(DisplayItem));
            }
            return myMovies;

            /* new way
            if (myMovies == null)
            {
                if (titles.Length > 0)
                {
                    ArrayList list = new ArrayList();
                    foreach (Title title in titles)
                    {
                        list.Add(CreateGalleryItem(title));
                    }
                    myMovies = (DisplayItem[])list.ToArray(typeof(DisplayItem));
                }
                else
                    myMovies[0] = new DisplayItem();
            }
            return myMovies;
            */
        }

        private DisplayItem CreateGalleryItem(Title title)
        {
            Trace.WriteLine("Movie:CreateGalleryItem(Title)");
            DisplayItem item = new DisplayItem();
            item.Description = title.description;
            item.title = title.Name;
            item.itemId = title.itemId;
            item.image = ConvertImage(title.boxart);
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

        private DisplayItem CreateGalleryItem(DataRow movieData)
        {
            Trace.WriteLine("Movie::CreateGalleryItem()");
            DisplayItem item = new DisplayItem();
            item.Description = (string)movieData["title"];
            item.title = (string)movieData["title"];
            item.itemId = int.Parse((string)movieData["id"]);
            //item.image = LoadImage("c:\\2001ASpaceOdyssey1968237_f.jpg");
            item.image = LoadImage((string)movieData["coverfront"]);
            item.runtime = (string)movieData["runtime"];
            item.mpaaRating = getChildColumn(movieData, "mpaarating", "displayname");
            item.imdbRating = "N/A";

            /*
            
            

            if (movieData["imdbrating"] == System.DBNull.Value)
            {
                item.imdbRating = "N/A";
            }
            else
            {
                item.imdbRating = (string)movieData["imdbrating"];
            }
            */
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

        public DetailsPage CreateDetailsPage(int movieId)
        {
            Trace.WriteLine("Movie:CreateDetailsPage()");
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
            //page.LocalMedia = new System.IO.FileInfo("C:\\users\\dxs\\documents\\Downloads\\Good Eats - Season 6\\Good Eats - S06E16 - Beet It.avi");

            return page;
        }

        private MovieMetadata ExtractMetadata(DataRow movieData, int movieId)
        {
            Trace.WriteLine("Movie:ExtractMetadata()");
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

        private IList getActors(DataRow dataRow)
        {
            Trace.WriteLine("Movie:getActors()");
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
            Trace.WriteLine("Movie:getCrew()");
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
            Trace.WriteLine("Movie:getUrl()");
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

        private String getChildColumn(DataRow dataRow, String relatedTable, String column)
        {
            Trace.WriteLine("Movie:getChildColumn() Column: " + column + " RelatedTable: " + relatedTable);
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

        private string StripHTML(string inputString)
        {
            Trace.WriteLine("Movie:StripHTML()");
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }

        private string StripRating(string inputString)
        {
            Trace.WriteLine("Movie:StripRating()");
            return Regex.Replace(inputString, "()", string.Empty);
        }

        public DataRow GetMovieData(int itemId)
        {
            Trace.WriteLine("Movie:getMovieData()");
            DataTable tbl_Movie = dataSet.Tables["movie"];
            return Movie.GetSingleDataRow(tbl_Movie, "id", itemId);
        }

        public static DataRow GetSingleDataRow(DataTable table, string column, object value)
        {
            Trace.WriteLine("Movie:GetSingleDataRow()");
            string query = String.Format("{0} = '{1}'", column, value);
            DataRow[] matches = table.Select(query);

            if (matches.Length != 1)
                throw new Exception("Unable to find a matching for " + query);

            return matches[0];
        }

        private static Image LoadImage(string imageName)
        {
            Trace.WriteLine("Movie:LoadImage()");
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

        private static Image ConvertImage(System.Drawing.Image d_image)
        {
            Stream strm = new MemoryStream();
            d_image.Save(strm, System.Drawing.Imaging.ImageFormat.MemoryBmp);
            return ImageFromStream(strm);
        }

        private static Image ImageFromStream(Stream stream)
        {
            if (fromStreamMethodInfo == null)
            {
                lock (syncObj)
                {
                    MethodInfo[] mis = typeof(Image).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

                    foreach (MethodInfo mi in mis)
                    {
                        ParameterInfo[] pis = mi.GetParameters();
                        if (mi.Name == "FromStream" && pis.Length == 2)
                        {
                            if (pis[0].ParameterType == typeof(String) &&
                                pis[1].ParameterType == typeof(Stream))
                            {
                                fromStreamMethodInfo = mi;
                            }
                        }
                    }
                }
            }

            return (Image)fromStreamMethodInfo.Invoke(null, new object[] { null, stream });
        }
    }
}