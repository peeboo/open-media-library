using System;
using System.Collections.Generic;
using OMLEngine;
using NUnit.Framework;
using DVDProfilerPlugin;
using System.Linq;
using System.Text.RegularExpressions;

namespace OMLTestSuite
{
    [TestFixture]
    public class DVDProfilerTest : TestBase
    {
        [Test]
        public void TEST_SPACING_IS_CORRECT_FOR_ACTOR_NAMES()
        {
            Title title = LoadTitle("1");
            var expectedNames = new List<string>
                                             {
                                                 "First1 Middle1 Last1",
                                                 "FirstNoMiddle2 Last2",
                                                 "FirstNoMiddleOrLast3",
                                                 "First4 Middle4 Last4 (1984)"
                                             };
            var missing = (from name in expectedNames
                           where !title.ActingRoles.ContainsKey(name)
                           select name).FirstOrDefault();
            Assert.IsNull(missing, "Name missing from cast: " + missing);
        }

        [Test]
        public void TEST_ROLES_ARE_CORRECT_FOR_ACTORS()
        {
            Title title = LoadTitle("1");
            // Test single role and dual credit
            var expectedRoles = new Dictionary<string, string>
                                    {
                                        {"First1 Middle1 Last1", "Role1/Role5"},
                                        {"First4 Middle4 Last4 (1984)", "Role4"}
                                    };
            var missing = (from expected in expectedRoles
                           where
                               !title.ActingRoles.ContainsKey(expected.Key) ||
                               title.ActingRoles[expected.Key] != expected.Value
                           select new {Name = expected.Key, Role = expected.Value}).FirstOrDefault();
            Assert.IsNull(missing, "Unexpected name/role combination");
        }

        [Test]
        public void TEST_NAMES_AND_EAN()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual("Title 1", title.Name, "Name incorrect");
            Assert.AreEqual("Sort title 1", title.SortName, "Sort name incorrect");
            Assert.AreEqual("Original title 1", title.OriginalName, "Original name incorrect");
            Assert.AreEqual("012345-678901", title.UPC, "UPC - which should be called EAN :) - is incorrect");

            title = LoadTitle("2");
            Assert.AreEqual("Title 2", title.SortName, "The second DVD should not have a sort name set");
        }

        [Test]
        public void TEST_RATINGS()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual("R", title.ParentalRating, "Parental rating incorrect");
            Assert.AreEqual("Rating details 1", title.ParentalRatingReason, "Parental rating reason incorrect");
            Assert.AreEqual(70, title.UserStarRating, "User star rating incorrect"); // Could use a more extensive test
        }

        [Test]
        public void TEST_CREWCREDITS()
        {
            // The name parsing is done by the same code as used for the cast credit, so spacing etc is not rechecked here
            Title title = LoadTitle("1");
            Assert.AreEqual(2, title.Directors.Count, "Incorrect number of director credits");
            Assert.AreEqual(2, title.Writers.Count, "Incorrect number of writer credits");
            Assert.AreEqual(2, title.Producers.Count, "Incorrect number of producer credits");

            Assert.AreEqual("Director 1", title.Directors[0].full_name, "First director incorrect");
            Assert.AreEqual("Director 2", title.Directors[1].full_name, "Second director incorrect");
            Assert.AreEqual("Writer 1", title.Writers[0].full_name, "First Writer incorrect");
            Assert.AreEqual("Writer 2", title.Writers[1].full_name, "Second Writer incorrect");
            Assert.AreEqual("Executive producer 1", title.Producers[0], "Executive Producer incorrect"); //TODO: Do we want to import executive producers?
            Assert.AreEqual("Producer 1", title.Producers[1], "Producer incorrect");
        }

        [Test]
        public void TEST_STUDIO_AND_COUNTRY()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual("Studio 1", title.Studio, "Studio is incorrect.");
            Assert.AreEqual("Country 1", title.CountryOfOrigin, "Country of origin is incorrect.");
        }

        [Test]
        public void TEST_GENRES()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual(2, title.Genres.Count, "Number of genres incorrect ");
            Assert.AreEqual("Genre 1", title.Genres[0], "First genre is incorrect");
            Assert.AreEqual("Genre 2", title.Genres[1], "Second genre is incorrect");
        }

        [Test]
        public void TEST_SUBTITLES()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual(2, title.Subtitles.Count, "Number of subtitles incorrect ");
            Assert.AreEqual("Subtitle 1", title.Subtitles[0], "First subtitle is incorrect");
            Assert.AreEqual("Subtitle 2", title.Subtitles[1], "Second subtitle is incorrect");
        }

        [Test]
        public void TEST_AUDIO_TRACKS()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual(2, title.AudioTracks.Count, "Number of audio tracks incorrect ");
            Assert.AreEqual("English (Dolby Digital Surround EX)", title.AudioTracks[0], "First audio track is incorrect");
            Assert.AreEqual("Commentary (Dolby Digital Mono)", title.AudioTracks[1], "Second audio track is incorrect");
        }


        [Test]
        public void TEST_DATES()
        {
            DateTime before = DateTime.Now;
            Title title = LoadTitle("1");
            DateTime after = DateTime.Now;
            Assert.AreEqual(new DateTime(2001, 02, 03), title.ReleaseDate, "Release date is incorrect");

            //TODO: Should this be the date entered in DVD Profiler instead?
            Assert.IsTrue(before <= title.DateAdded && title.DateAdded <= after, "Date entered is incorrect");

            title = LoadTitle("2");
            Assert.AreEqual(DateTime.MinValue, title.ReleaseDate, "The second DVD should not have a release date");
        }

        [Test]
        public void TEST_RUNNINGTIME()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual(42, title.Runtime, "Running time is incorrect");
        }


        [Test]
        public void TEST_SYNOPSIS()
        {
            Title title = LoadTitle("1");

            Regex formattingRegex = new Regex(@"\</?(b|i)\>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            Assert.IsFalse(formattingRegex.IsMatch(title.Synopsis), "Formatting (italic or bold) has not been removed");

            var numberOfEmptyLines = (from line in title.Synopsis.Split('\n')
                                      where string.IsNullOrEmpty(line.Trim())
                                      select line).Count();

            Assert.AreEqual(0, numberOfEmptyLines, "Empty lines have not been removed");
        }

        [Test]
        public void TEST_VIDEO()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual("NTSC", title.VideoStandard, "Video standard for DVD one is incorrect");
            Assert.AreEqual(VideoFormat.MKV, title.VideoFormat, "Video Format for DVD one is incorrect"); // Set to the format of the first disc assuming it's the primary
            Assert.AreEqual("2.35", title.AspectRatio, "Aspect ratio for DVD one is incorrect");

            title = LoadTitle("2");
            Assert.AreEqual("PAL", title.VideoStandard, "Video standard for DVD two is incorrect");
            Assert.AreEqual("1.33", title.AspectRatio, "Aspect ratio for DVD two is incorrect");

        }

        [Test]
        public void TEST_DISCS()
        {
            Title title = LoadTitle("1");
            Assert.AreEqual(3, title.Disks.Count, "The number of disks is incorrect");
            
            Assert.AreEqual(VideoFormat.MKV, title.Disks[0].Format, "Video format for disk 1a is incorrect");
            Assert.AreEqual("Main Feature", title.Disks[0].Name, "Name for disk 1a is incorrect");
            Assert.AreEqual(@"\\unc_server\share\folder with space\file.mkv", title.Disks[0].Path, "Path for disk 1a is incorrect");

            Assert.AreEqual(VideoFormat.ISO, title.Disks[1].Format, "Video format for disk 1b is incorrect");
            Assert.AreEqual("Bonus Materials", title.Disks[1].Name, "Name for disk 1b is incorrect");
            Assert.AreEqual(@"Z:\folder\image.iso", title.Disks[1].Path, "Path for disk 1b is incorrect");
            
            Assert.AreEqual(VideoFormat.UNKNOWN, title.Disks[2].Format, "Video format for disk 2a is incorrect");
            Assert.AreEqual("Disc 2a", title.Disks[2].Name, "Name for disk 2a is incorrect");
            Assert.AreEqual(@"\\127.0.0.1\share\folder\unknown.unknown", title.Disks[2].Path, "Path for disk 2a is incorrect");

            title = LoadTitle("2");
            Assert.AreEqual(0, title.Disks.Count, "The second DVD should not have any disks defined.");

        }

        private static Title LoadTitle(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            Title result = null;
            DVDProfilerImporter importer = new DVDProfilerImporter();
            importer.DoWork(new [] { @"..\..\..\Sample Files\DVDProfilerUnitTest.xml" });
            IList<Title> titles = importer.GetTitles();
            Assert.AreEqual(2, titles.Count, "Unexpected number of profiles loaded.");
            foreach (Title title in titles)
            {
                if (title.MetadataSourceID == id)
                {
                    result = title;
                    break;
                }
            }
            Assert.IsNotNull(result, "The DVDProfilerUnitTest.xml file does not contain a title with ID " + id);
            return result;
        }


    }
}