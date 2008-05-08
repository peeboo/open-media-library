using System;
using NUnit.Framework;
using OMLEngine;
using OMLSDK;
using MyMoviesPlugin;

namespace OMLTestSuite
{
    [TestFixture]
    public class TestTitleCollection
    {
        [Test]
        public void TEST_THOUSAND_PLUS_TITLES()
        {
            TitleCollection tc = new TitleCollection();
            tc.loadTitleCollection();

            MyMoviesImporter importer = new MyMoviesImporter();
            importer.Load(@"C:\mymovies.xml");
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }
            foreach (Title title in importer.GetTitles)
            { tc.AddTitle(title); }

            tc.saveTitleCollection();
        }
    }
}
