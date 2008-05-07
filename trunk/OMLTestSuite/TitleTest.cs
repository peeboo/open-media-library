using System;
using OMLEngine;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class TitleTest
    {
        [Test]
        public void TEST_BASE_CASE()
        {
            Title title = new Title();
            title.Name = "My Movie";
            title.sourceId = "ABC";
            title.sourceName = "MyMovies";
            title.Synopsis = "Blah, blah, blah";
            title.VideoFormat = VideoFormat.CUE;

            Assert.AreEqual(true, title.NeedToTranscodeToExtenders());
            Assert.AreEqual(true, title.NeedToMountBeforePlaying());
        }
    }
}
