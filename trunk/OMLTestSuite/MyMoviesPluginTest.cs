using System;
using OMLEngine;
using OMLSDK;
using MyMoviesPlugin;

namespace OMLTestSuite
{
    public class MyMoviesPluginTest
    {
        public void TEST_BASE_CASE()
        {
            TitleCollection tc = new TitleCollection();
            MyMoviesImporter importer = new MyMoviesImporter();
            importer.Load("C:\\mymovies.xml");

            foreach (Title t in importer.GetTitles)
            {
                if (t.FileLocation != null)
                    tc.AddTitle(t);
            }
            tc.saveTitleCollection();
        }
    }
}
