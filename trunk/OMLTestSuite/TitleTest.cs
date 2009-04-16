using System;
using System.Collections.Generic;
using OMLSDK;
using OMLEngine;
using NUnit.Framework;
using System.Linq;

namespace OMLTestSuite
{
    [TestFixture]
    public class TitleTest : TestBase
    {
        [Test]
        public void TEST_BASE_CASE()
        {
            Title t = new Title();
            t.AspectRatio = "Widescreen";
            t.BackCoverPath = "back.jpg";
            t.CountryOfOrigin = "US";
            t.DateAdded = new DateTime(2008, 01, 01);
            t.Studio = "Paramount";
            Disk disk = new Disk();
            disk.Name = "Disk 1";
            disk.Path = "myfile.wpl";
            disk.Format = VideoFormat.WPL;
            t.Disks.Add(disk);
            t.FrontCoverPath = "front.jpg";
            t.WatchedCount = 1;
            t.ImporterSource = "MyMovies";
            t.MetadataSourceID = "123";
            t.MetadataSourceName = "MyMovies";
            t.ParentalRating = "R";
            t.Name = "My Movie";
            t.OfficialWebsiteURL = "http://www.mymovie.com";
            t.OriginalName = "My Movie";
            t.ReleaseDate = new DateTime(2008, 1, 1);
            t.Runtime = 110;
            t.Synopsis = "This is my long synopsis of my movie";
            t.UPC = "123ABC";
            t.UserStarRating = 5;
            t.VideoStandard = "NTSC";
            t.AddActingRole("Translucent", "Actor");
            t.AddActingRole("taranu", "Actor");
            t.AddNonActingRole("KingManon", "crew");
            t.AddNonActingRole("Chris", "crew");
            t.AddDirector(new Person("Tim"));
            t.AddGenre("Comedy");
            t.AddAudioTrack("English");
            t.AddAudioTrack("French");
            t.AddProducer(new Person("Sony"));
            t.AddWriter(new Person("Timothy"));

            Assert.AreEqual("Widescreen", t.AspectRatio);
            Assert.AreEqual("back.jpg", t.BackCoverPath);
            Assert.AreEqual("US", t.CountryOfOrigin);
            Assert.IsTrue(new DateTime(2008, 01, 01).CompareTo(t.DateAdded) == 0);
            Assert.AreEqual("Paramount", t.Studio);
            Assert.AreEqual("myfile.wpl", t.Disks[0].Path);
            Assert.AreEqual("Disk 1", t.Disks[0].Name);
            Assert.AreEqual("front.jpg", t.FrontCoverPath);
            Assert.AreEqual(1, t.WatchedCount);
            Assert.AreEqual("MyMovies", t.ImporterSource);
            Assert.AreEqual("123", t.MetadataSourceID);
            Assert.AreEqual("MyMovies", t.MetadataSourceName);
            Assert.AreEqual("R", t.ParentalRating);
            Assert.AreEqual("My Movie", t.Name);
            Assert.AreEqual("http://www.mymovie.com", t.OfficialWebsiteURL);
            Assert.AreEqual("My Movie", t.OriginalName);
            Assert.IsTrue(new DateTime(2008, 01, 01).CompareTo(t.ReleaseDate) == 0);
            Assert.AreEqual("123ABC", t.UPC);
            Assert.AreEqual(5, t.UserStarRating);
            Assert.AreEqual(VideoFormat.WPL, t.Disks[0].Format);
            Assert.AreEqual("NTSC", t.VideoStandard);
            Assert.AreEqual(2, t.ActingRoles.Count);
            IEnumerable<string> actors = t.ActingRoles.Select(a => a.PersonName);
            Assert.That(actors.Contains("Translucent"));
            Assert.That(actors.Contains("taranu"));
            //Assert.AreEqual("KingManon", ((Person)t.Crew[0]).full_name);
            //Assert.AreEqual("Chris", ((Person)t.Crew[1]).full_name);
            Assert.AreEqual(1, t.Directors.Count);
            Assert.AreEqual("Tim", ((Person)t.Directors[0]).full_name);
            Assert.AreEqual(1, t.Genres.Count);
            Assert.AreEqual("Comedy", t.Genres[0]);
            Assert.AreEqual(2, t.AudioTracks.Count);
            Assert.AreEqual("English", t.AudioTracks[0]);
            Assert.AreEqual("French", t.AudioTracks[1]);
            Assert.AreEqual(1, t.Producers.Count);
            Assert.AreEqual("Sony", t.Producers[0]);
            Assert.AreEqual(1, t.Writers.Count);
            Assert.AreEqual("Timothy", ((Person)t.Writers[0]).full_name);
        }

        [Test]
        public void TEST_LOAD_FROM_XML()
        {
            string xml_file = @"..\..\..\Sample Files\sample-oml.xml";

            Title t = Title.CreateFromXML(xml_file, false);

            Assert.IsInstanceOfType(typeof(Title), t);
            Assert.AreEqual("GoldenEye", t.OriginalName);
            Assert.AreEqual(1, t.Disks.Count);
        }
    }
}
