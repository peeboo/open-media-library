using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Diagnostics;

namespace Library
{
    public class MoviesXmlImporter
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

        public DataSet GetDataSet()
        {
            return _dataSet;
        }
        ~MoviesXmlImporter()
        {
        }


    }
}
