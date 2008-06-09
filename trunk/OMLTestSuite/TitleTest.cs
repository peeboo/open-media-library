using System;
using OMLSDK;
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
            Title t = new Title();
            t.AspectRatio = "Widescreen";
            t.BackCoverPath = "back.jpg";
            t.CountryOfOrigin = "US";
            t.DateAdded = new DateTime(2008, 01, 01);
            t.Description = "This is my description";
            t.Distributor = "Paramount";
            t.FileLocation = "myfile.wpl";
            t.FrontCoverPath = "front.jpg";
            t.HasWatched = 1;
            t.ImporterSource = "MyMovies";
            t.MetadataSourceID = "123";
            t.MetadataSourceName = "MyMovies";
            t.MPAARating = "R";
            t.Name = "My Movie";
            t.OfficialWebsiteURL = "http://www.mymovie.com";
            t.OriginalName = "My Movie";
            t.ReleaseDate = new DateTime(2008, 1, 1);
            t.Runtime = 110;
            t.Synopsis = "This is my long synopsis of my movie";
            t.UPC = "123ABC";
            t.UserStarRating = 5;
            t.VideoFormat = VideoFormat.WPL;
            t.VideoStandard = "NTSC";
            t.AddActor(new Person("Translucent"));
            t.AddActor(new Person("taranu"));
            t.AddCrew(new Person("KingManon"));
            t.AddCrew(new Person("Chris"));
            t.AddDirector(new Person("Tim"));
            t.AddGenre("Comedy");
            t.AddLanguageFormat("English");
            t.AddLanguageFormat("French");
            t.AddProducer("Sony");
            t.AddWriter(new Person("Timothy"));

            Assert.AreEqual("Widescreen", t.AspectRatio);
            Assert.AreEqual("back.jpg", t.BackCoverPath);
            Assert.AreEqual("US", t.CountryOfOrigin);
            Assert.IsTrue(new DateTime(2008, 01, 01).CompareTo(t.DateAdded) == 0);
            Assert.AreEqual("This is my description", t.Description);
            Assert.AreEqual("Paramount", t.Distributor);
            Assert.AreEqual("myfile.wpl", t.FileLocation);
            Assert.AreEqual("front.jpg", t.FrontCoverPath);
            Assert.AreEqual(1, t.HasWatched);
            Assert.AreEqual("MyMovies", t.ImporterSource);
            Assert.AreEqual("123", t.MetadataSourceID);
            Assert.AreEqual("MyMovies", t.MetadataSourceName);
            Assert.AreEqual("R", t.MPAARating);
            Assert.AreEqual("My Movie", t.Name);
            Assert.AreEqual("http://www.mymovie.com", t.OfficialWebsiteURL);
            Assert.AreEqual("My Movie", t.OriginalName);
            Assert.IsTrue(new DateTime(2008, 01, 01).CompareTo(t.ReleaseDate) == 0);
            Assert.AreEqual("123ABC", t.UPC);
            Assert.AreEqual(5, t.UserStarRating);
            Assert.AreEqual(VideoFormat.WPL, t.VideoFormat);
            Assert.AreEqual("NTSC", t.VideoStandard);
            Assert.AreEqual(2, t.Actors.Count);
            Assert.AreEqual("Translucent", ((Person)t.Actors[0]).full_name);
            Assert.AreEqual("taranu", ((Person)t.Actors[1]).full_name);
            Assert.AreEqual(2, t.Crew.Count);
            Assert.AreEqual("KingManon", ((Person)t.Crew[0]).full_name);
            Assert.AreEqual("Chris", ((Person)t.Crew[1]).full_name);
            Assert.AreEqual(1, t.Directors.Count);
            Assert.AreEqual("Tim", ((Person)t.Directors[0]).full_name);
            Assert.AreEqual(1, t.Genres.Count);
            Assert.AreEqual("Comedy", t.Genres[0]);
            Assert.AreEqual(2, t.LanguageFormats.Count);
            Assert.AreEqual("English", t.LanguageFormats[0]);
            Assert.AreEqual("French", t.LanguageFormats[1]);
            Assert.AreEqual(1, t.Producers.Count);
            Assert.AreEqual("Sony", t.Producers[0]);
            Assert.AreEqual(1, t.Writers.Count);
            Assert.AreEqual("Timothy", ((Person)t.Writers[0]).full_name);
        }
    }
}
