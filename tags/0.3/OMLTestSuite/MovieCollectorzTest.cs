using System;
using NUnit.Framework;
using MovieCollectorzPlugin;

namespace OMLTestSuite
{
    [TestFixture]
    public class MovieCollectorzTest
    {
        [Test]
        public void TEST_MOVIE_COLLECTORZ_MISSING_BOXART()
        {
            MovieCollectorzPlugin.MovieCollectorzPlugin plugin = new MovieCollectorzPlugin.MovieCollectorzPlugin();
            plugin.ProcessFile(@"..\..\..\Sample Files\MovieCollectorzTestFiles\MissingCoverArtandActors\Title.xml");

            Assert.AreEqual(345, plugin.GetTitles().Count);
        }
    }
}
