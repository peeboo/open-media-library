using System;
using System.IO;
using System.Xml;
using System.Data;

namespace Library
{
    public class MoviesXmlImporter
    {
        public DataSet dataSet;

        public MoviesXmlImporter()
        {
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

            dataSet = new DataSet("Movie");
            dataSet.ReadXml(xmlReader);
        }

        ~MoviesXmlImporter()
        {
        }


    }
}
