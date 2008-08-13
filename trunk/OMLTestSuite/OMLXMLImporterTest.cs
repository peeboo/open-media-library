using System;
using System.Collections.Generic;
using OMLSDK;
using OMLEngine;
using System.IO;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class OMLXMLImporterTest
    {
        [Test]
        public void TEST_CONVERT_MYMOVIES_XML_TO_OML_XML()
        {
            OMLXMLPlugin.OMLXMLPlugin plugin = new OMLXMLPlugin.OMLXMLPlugin();
            plugin.ConvertMyMoviesXMLToOMLXML(@"..\..\..\Sample Files\mymovies-single.xml");
            Assert.That(File.Exists(@"..\..\..\Sample Files\oml.xml"));

            if (File.Exists(@"..\..\..\Sample Files\oml.xml"))
                File.Delete(@"..\..\..\Sample Files\oml.xml");
        }

        [Test]
        public void TEST_AUTO_LOCATE_CONVERT_AND_LOAD_A_MYMOVIES_XML_INTO_AN_OML_XML()
        {
            OMLXMLPlugin.OMLXMLPlugin plugin = new OMLXMLPlugin.OMLXMLPlugin();
            plugin.DoWork(new string[] { @"..\..\..\Sample Files\MMTOMLTEST\" });

            Assert.AreEqual(plugin.TotalTitlesAdded, 1);

            if (File.Exists(@"..\..\..\Sample Files\oml.xml"))
                File.Delete(@"..\..\..\Sample Files\oml.xml");
        }
    }
}
