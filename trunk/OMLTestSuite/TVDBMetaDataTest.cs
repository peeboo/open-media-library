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
            tvd.Initialize("", null);

            string show = "Red";
            string episode = "holoship";
            int ? seasonno = 0;
            int ? episodeno = 0;

            // Search for show
            if (!tvd.SearchForTVSeries(show, episode, seasonno, episodeno, OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty, false))
            {
                Console.WriteLine("We found the show. Episode list below.");

                OMLSDK.OMLSDKTitle[] titles = tvd.GetAvailableTitles();
                Console.WriteLine("Title count " + titles.Count());
                foreach (OMLSDK.OMLSDKTitle t in titles)
                {
                    Console.WriteLine(t.Name);
                }
            }
            else
            {
                Console.WriteLine("We didn't find an exact match on the show, matches below.");

                OMLSDK.OMLSDKTitle[] titles = tvd.GetAvailableTitles();
                Console.WriteLine("Title count " + titles.Count());
                foreach (OMLSDK.OMLSDKTitle t in titles)
                {
                    Console.WriteLine(t.Name);
                }
                Console.WriteLine("");

                // Search for episode
                tvd.SearchForTVDrillDown(0, episode, seasonno, episodeno, OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty);
                
                titles = tvd.GetAvailableTitles();
                Console.WriteLine("Title count " + titles.Count());

                Console.WriteLine("Episode list below.");

                titles = tvd.GetAvailableTitles();
                Console.WriteLine("Title count " + titles.Count());
                foreach (OMLSDK.OMLSDKTitle t in titles)
                {
                    Console.WriteLine(t.Name);
                }


            }
        }
    }
}
