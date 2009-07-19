using System;
using OMLEngine;
using NUnit.Framework;
using NetFlixMetadata;

namespace OMLTestSuite
{
    [TestFixture]
    public class NetFlixMetadataTest
    {
        [Test]
        public void TEST_LOGIN()
        {
            NetFlixDb netFlix = new NetFlixDb();
            netFlix.Initialize("", null);
        }
    }
}
