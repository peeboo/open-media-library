using System;
using OMLEngine;
using OMLSDK;
using MyMoviesPlugin;
using System.IO;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class MyMoviesPluginTest
    {
        [TearDown]
        public void TearDownTest()
        {
            FileInfo fi;
            fi = new FileInfo("\\testOML.dat");
            if (fi != null)
                fi.Delete();
        }

        [Test]
        public void TEST_BASE_CASE()
        {
            TitleCollection tc = new TitleCollection("\\testOML.dat");
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.Load("C:\\mymovies.xml");

            foreach (Title t in importer.GetTitles())
                tc.Add(t);

            tc.saveTitleCollection();
            tc = null;

            tc = new TitleCollection("\\testOML.dat");
            tc.loadTitleCollection();
            Assert.IsNotNull(tc);

            Assert.AreEqual(58, tc.Count);
        }
    }
}
