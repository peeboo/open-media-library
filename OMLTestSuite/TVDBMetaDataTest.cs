using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVDBMetadata;
namespace OMLTestSuite
{
    class TVDBMetaDataTest
    {

        public void Test()
        {

            TVDBMetadataPlugin tvd = new TVDBMetadataPlugin();
            tvd.Initialize(null);
            tvd.SearchForTVSeries("Red");
            OMLEngine.Title[] titles = tvd.GetAvailableTitles();
            Console.WriteLine("Title count " + titles.Count());
            tvd.SearchForTVEpisodes(3);
            titles = tvd.GetAvailableTitles();
            Console.WriteLine("Title count " + titles.Count());

        }
    }
}
