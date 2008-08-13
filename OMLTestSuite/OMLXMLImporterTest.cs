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
            string filename = @"c:\Sample Files\mymovies-single.xml";
            OMLXMLPlugin.OMLXMLPlugin plugin = new OMLXMLPlugin.OMLXMLPlugin();

            plugin.ConvertMyMoviesXMLToOMLXML(@"..\..\..\Sample Files\");
            Assert.That(File.Exists(@"c:\mymovies-converted-oml.xml"));
        }
    }
}
