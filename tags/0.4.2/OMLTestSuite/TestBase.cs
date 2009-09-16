using System;
using NUnit.Framework;

namespace OMLTestSuite
{
    public class TestBase
    {
        DateTime startOfTest;
        [SetUp]
        public void SetupTest()
        {
            startOfTest = DateTime.Now;
        }

        [TearDown]
        public void TearDown()
        {
            TimeSpan tSpan = new TimeSpan(startOfTest.Ticks);
            Console.Error.WriteLine("Test Time: {0}.{1} seconds", tSpan.Seconds, tSpan.Milliseconds);
        }
    }
}
