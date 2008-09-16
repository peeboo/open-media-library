using System;
using System.Collections.Generic;
using NUnit.Framework;
using AppleTrailers;

namespace OMLTestSuite
{
    [TestFixture]
    public class AppleTrailersTest
    {
        [Test]
        public void TEST_APPLE_TRAILERS_GET_TITLES()
        {
            AppleTrailers.AppleTrailers appleTrailers = new AppleTrailers.AppleTrailers();
            appleTrailers.LoadTrailers();
            Assert.IsNotEmpty(appleTrailers.Trailers);
        }
    }
}
