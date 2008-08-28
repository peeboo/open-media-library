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

        [Test]
        public void TEST_MULTIPLE_DISCS_FAIL_TO_IMPORT()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessFile(@"..\..\..\Sample Files\mymovies-multiple-avi-files-bug.xml");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
        }

        [Test]
        public void TEST_WHEN_NO_DISCS_ARE_DEFINED__LOOK_IN_THE_SAME_DIRECTORY_AS_THE_MYMOVIES_XML_FILE_FOR_ANY_VALID_FILES_TO_ADD_AS_DISCS()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessFile(@"..\..\..\Sample Files\MyMoviesTestFiles\MissingDiscsSection\mymovies.xml");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
            Title title = titles[0];

            Assert.AreEqual(1, title.Disks.Count);
            Assert.AreEqual(@"Bear.wmv", Path.GetFileName(title.Disks[0].Path));
        }

        [Test]
        public void TEST_WHEN_NO_DISCS_ARE_DEFINED__LOOK_IN_THE_SAME_DIRECTORY_AS_THE_MYMOVIES_XML_FILE_FOR_ANY_VALID_FILES_TO_ADD_AS_DISCS__MULTIPLE_FILES()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessFile(@"..\..\..\Sample Files\MyMoviesTestFiles\MissingDiscsSection-MultipleFiles\mymovies.xml");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
            Title title = titles[0];

            Assert.AreEqual(3, title.Disks.Count);
            Assert.AreEqual(@"Bear.wmv", Path.GetFileName(title.Disks[0].Path));
            Assert.AreEqual(@"Butterfly.wmv", Path.GetFileName(title.Disks[1].Path));
            Assert.AreEqual(@"Lake.wmv", Path.GetFileName(title.Disks[2].Path));
        }
    }
}
