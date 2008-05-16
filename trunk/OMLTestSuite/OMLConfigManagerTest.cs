using System;
using OMLEngine;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class OMLConfigManagerTest
    {
        [TearDown]
        public void TearDownMethod()
        {
        }

        [Test]
        public void TEST_BASE_CASE()
        {
            OMLConfigManager cm = new OMLConfigManager();
        }
    }
}
