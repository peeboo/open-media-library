using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TheMovieDbMetadata;
using OMLEngine;

namespace OMLTestSuite
{
    [TestFixture]
    public class TheMovieDbMetadataPluginTest
    {
        [Test]
        public void TEST_BASE_CASE()
        {
            TheMovieDbMetadata.TheMovieDbMetadata plugin = new TheMovieDbMetadata.TheMovieDbMetadata();
            plugin.Initialize(null);
            plugin.SearchForMovie("The Dark Knight");
            Title t = plugin.GetBestMatch();
            plugin.GetTitle(0);
        }
    }
}
