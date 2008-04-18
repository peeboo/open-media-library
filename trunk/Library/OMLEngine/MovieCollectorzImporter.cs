using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OMLEngine
{
    public class MoviesXmlImporter : IDataImporter
    {
        const string HTML_TAG_PATTERN = "<.*?>";
        public DataSet _dataSet;

        public MoviesXmlImporter()
        {
            Trace.WriteLine("using internal " + typeof(MoviesXmlImporter).Name + " class through reflection");
            string fileName = "C:\\movies.xml";
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

            _dataSet = new DataSet("Movie");
            _dataSet.ReadXml(xmlReader);
        }
        public DataSet getDataSet()
        {
            return _dataSet;
        }
        public IList getActors(DataRow dataRow)
        {
            IList actors = new List<string>();
            DataRow[] castTable = dataRow.GetChildRows("movie_cast");
            DataRow castR = castTable[0];

            DataRow[] starRow = castR.GetChildRows("cast_star");

            // Get the entries that match our movie.
            foreach (DataRow star in starRow)
            {
                DataRow[] personRow = star.GetChildRows("star_person");
                actors.Add((string)personRow[0]["displayname"]);
            }
            return actors;
        }
        public IList getCrew()
        {
            return null;
        }
        public IList getCrew(DataRow dataRow, string something)
        {
            IList crew = new List<string>();
            return crew;
        }
        ~MoviesXmlImporter()
        {
        }
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
        public DataRow GetMovieData(int itemId)
        {
            DataTable tbl_Movie = _dataSet.Tables["movie"];
            return GetSingleDataRow(tbl_Movie, "id", itemId);
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
                    return (string)url["url"];
                }
            }
            return null;
        }
    }
}
