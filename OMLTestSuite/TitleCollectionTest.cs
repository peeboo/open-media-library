using System;
using OMLEngine;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class TitleCollectionTest
    {
        [Test]
        public void TEST_BASE_CASE()
        {
            TitleCollection tc = new TitleCollection();
            Title title1 = new Title();
            title1.Name = "B Movie";
            Title title2 = new Title();
            title2.Name = "A Movie";

            tc.Add(title1);
            tc.Add(title2);

            Assert.AreEqual(2, tc.Count);
        }
    }
}
