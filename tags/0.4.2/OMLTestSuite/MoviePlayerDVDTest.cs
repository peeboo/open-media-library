using System;
using Library;
using OMLEngine;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class MoviePlayerDVDTest : TestBase
    {
        [Test]
        public void TEST_GENERATE_STRING_FOR_A_STANDARD_DVD()
        {
            string originalPath = "..\\..\\..\\Sample Files\\TestFolder\\TestFolder1";

            Assert.AreEqual("DVD://../../../Sample Files/TestFolder/TestFolder1/VIDEO_TS",
                            DVDPlayer.GeneratePlayString(originalPath, 0, 0));
        }

        [Test]
        public void TEST_GENERATE_STRING_CORRECTLY_BUILDS_TITLE_SELECTION_STRING()
        {
            string originalPath = "..\\..\\..\\Sample Files\\TestFolder\\TestFolder1";

            Assert.AreEqual("DVD://../../../Sample Files/TestFolder/TestFolder1/VIDEO_TS?2",
                            DVDPlayer.GeneratePlayString(originalPath, 2, 0));
        }

        [Test]
        public void TEST_GENERATE_STRING_CORRECTLY_BUILDS_TITLE_AND_CHAPTER_SELECTION_STRING()
        {
            string originalPath = "..\\..\\..\\Sample Files\\TestFolder\\TestFolder1";

            Assert.AreEqual("DVD://../../../Sample Files/TestFolder/TestFolder1/VIDEO_TS?2/5",
                            DVDPlayer.GeneratePlayString(originalPath, 2, 5));
        }

        [Test]
        public void TEST_GENERATE_STRING_CORRECTLY_BUILDS_START_TIME()
        {
            string originalPath = "..\\..\\..\\Sample Files\\TestFolder\\TestFolder1";

            Assert.AreEqual("DVD://../../../Sample Files/TestFolder/TestFolder1/VIDEO_TS?1/0:18:32",
                            DVDPlayer.GeneratePlayString(originalPath, 1, new DateTime(2008, 01, 01, 0, 18, 32)));
        }
    }
}
