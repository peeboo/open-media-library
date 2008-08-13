using System;
using OMLEngine;
using OMLSDK;
using MyMoviesPlugin;
using System.IO;
using System.Collections.Generic;
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
            bool ShouldCopyImages = false;
            importer.CopyImages = ShouldCopyImages;
            importer.ProcessFile(@"..\..\..\Sample Files\MyMovies.xml");

            foreach (Title t in importer.GetTitles())
                tc.Add(t);

            tc.saveTitleCollection();
            tc = null;

            tc = new TitleCollection("\\testOML.dat");
            tc.loadTitleCollection();
            Assert.IsNotNull(tc);

            Assert.AreEqual(2, tc.Count);
        }

        public void TEST_MULTIPLE_DISCS_FAIL_TO_IMPORT()
        {
            TitleCollection tc = new TitleCollection("\\testOML.dat");
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessFile(@"..\..\..\Sample Files\mymovies-multiple-avi-files-bug.xml");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
        }
    }
}
