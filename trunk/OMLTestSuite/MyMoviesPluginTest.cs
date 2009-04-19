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
    public class MyMoviesPluginTest : TestBase
    {
        [TearDown]
        public void TearDownTest()
        {
            FileInfo fi;
            fi = new FileInfo(@"..\..\..\Sample Files\testOML.dat");
            if (fi != null)
                fi.Delete();
        }

        [Test]
        public void TEST_BASE_CASE()
        {
            /*TitleCollection tc = new TitleCollection(@"..\..\..\Sample Files\testOML.dat");
            MyMoviesImporter importer = new MyMoviesImporter();                        
            importer.ProcessFile(@"..\..\..\Sample Files\MyMovies.xml");

            foreach (Title t in importer.GetTitles())
                tc.Add(t);

            //tc.saveTitleCollection();
            tc = null;

            tc = new TitleCollection(@"..\..\..\Sample Files\testOML.dat");
            tc.loadTitleCollection();
            Assert.IsNotNull(tc);

            Assert.AreEqual(2, tc.Source.Count);*/
        }

        [Test]
        public void TEST_MULTIPLE_DISCS_FAIL_TO_IMPORT()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessFile(@"..\..\..\Sample Files\mymovies-multiple-avi-files-bug.xml");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);

            Title t = titles[0];
            Assert.AreEqual("f7262c3b-f1a3-4128-a026-c7abdbbd6e51", t.MetadataSourceID);
            Assert.AreEqual("Beatles Anthology, The", t.Name);
            Assert.AreEqual("1.33:1", t.AspectRatio);
            Assert.AreEqual(3, t.AudioTracks.Count);
            Assert.AreEqual("English, Dolby Digital, 5.1", t.AudioTracks[0]);
            Assert.AreEqual("English, DTS, 5.1", t.AudioTracks[1]);
            Assert.AreEqual("English, PCM, 2.0", t.AudioTracks[2]);
            Assert.AreEqual("United States", t.CountryOfOrigin);
            Assert.AreEqual(new DateTime(1995, 01, 01), t.ReleaseDate);
            Assert.AreEqual(674, t.Runtime);
            Assert.AreEqual("PG", t.ParentalRating);
            Assert.AreEqual(3, t.Genres.Count);
            Assert.AreEqual("Music", t.Genres[0]);
            Assert.AreEqual("Documentary", t.Genres[1]);
            Assert.AreEqual("Special Interest", t.Genres[2]);
            Assert.AreEqual(6, t.Subtitles.Count);
            Assert.AreEqual("English", t.Subtitles[0]);
            Assert.AreEqual("German", t.Subtitles[1]);
            Assert.AreEqual("Spanish", t.Subtitles[2]);
            Assert.AreEqual("French", t.Subtitles[3]);
            Assert.AreEqual("Italian", t.Subtitles[4]);
            Assert.AreEqual("Portuguese", t.Subtitles[5]);
            Assert.AreEqual("Apple Films", t.Studio);
            Assert.AreEqual(1, t.Disks.Count); // this is because none of the files exist but it finds a wpl file the original source location
            Assert.AreEqual("Nudity, hahaha", t.ParentalRatingReason);
            Assert.AreEqual("NTSC", t.VideoStandard);
            Assert.AreEqual(1, t.Directors.Count);
            Assert.AreEqual("Geoff Wonfor", ((Person)t.Directors[0]).full_name);
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

        [Test]
        public void TEST_WHEN_GIVEN_A_FOLDER_SCAN_EACH_FOLDER_LOOKING_FOR_MYMOVIES_XML_FILES()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessDir(@"..\..\..\Sample Files\MyMoviesTestFiles\TestNestedDirectory");

            IList<Title> titles = importer.GetTitles();
            Assert.AreEqual(8, titles.Count);
        }

        [Test]
        public void TEST_FOLDER_JPG_FILES_ARE_USED_IF_COVER_PATHS_DONT_APPEAR_TO_EXIST()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessDir(@"..\..\..\Sample Files\MyMoviesTestFiles\TestNestedDirectory\dir1");

            IList<Title> titles = importer.GetTitles();
            Assert.AreEqual(4, titles.Count);

            Assert.IsNotNull(titles[0].FrontCoverPath);
            string imagePath = titles[0].FrontCoverPath;
            Assert.IsTrue(File.Exists(imagePath));
            Assert.IsTrue(imagePath.EndsWith("folder.jpg", StringComparison.CurrentCultureIgnoreCase));
        }

        [Test]
        public void TEST_CORRECTLY_IMPORTS_DVRMS_FILES()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessFile(@"..\..\..\Sample Files\MyMoviesTestFiles\DVR-MS\mymovies.xml");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
            Title t = titles[0];
            Assert.AreEqual(1, t.Disks.Count);
            Assert.IsNotEmpty(t.Disks[0].Path);
        }

        [Test]
        public void TEST_FILE_APPEARS_TO_FAIL_COMPLETE_PARSING__SUPPLIED_BY_USER_SAXNIX()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessDir(@"..\..\..\Sample Files\MyMoviesTestFiles\ParseFailureFileFrom-saxnix");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
            Title t = titles[0];
            Assert.AreEqual(1, t.Disks.Count);
        }

        [Test]
        public void TEST_FILES_WITH_MULTIPLE_DISCS_APPEAR_TO_FAIL()
        {
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.ProcessDir(@"..\..\..\Sample Files\MyMoviesTestFiles\MultiDisk Issues");

            IList<Title> titles = importer.GetTitles();

            Assert.AreEqual(1, titles.Count);
        }
    }
}
