using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Library
{
    public class MoviesXmlImporter : IDataImporter
    {
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
            IList crew = new List<string>();
            return crew;
        }

        ~MoviesXmlImporter()
        {
        }


    }
}
