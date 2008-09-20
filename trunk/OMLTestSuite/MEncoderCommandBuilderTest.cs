using System;
using System.IO;
using OMLTranscoder;
using NUnit.Framework;
using OMLEngine;
using OMLGetDVDInfo;

namespace OMLTestSuite
{
    [TestFixture]
    public class MEncoderCommandBuilderTest : TestBase
    {
        [Test]
        public void TEST_BASIC_COMMAND_BUILDER()
        {
            string drive = OMLEngine.Properties.Settings.Default.VirtualDiscDrive.ToLower();
            MediaSource ms = new MediaSource(new Disk("", drive + ":", VideoFormat.DVD));
            MEncoderCommandBuilder builder = new MEncoderCommandBuilder(ms, ".")
            {
                AudioFormat = MEncoder.AudioFormat.Copy,
                VideoFormat = MEncoder.VideoFormat.Copy,
                OutputFile  = @".\mymovie",
            };

            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd:// -dvd-device """ + drive +
                            @":\"" -oac copy -ovc copy -of mpeg -mpegopts format=dvd:tsaf " +
                            @"-vf harddup -really-quiet -o "".\mymovie.mpg""", builder.GetArguments().ToLower());
            Assert.IsNotNull(builder.GetCommand());
            Assert.AreEqual(@"c:\program files\openmedialibrary\mencoder.exe", builder.GetCommand().ToLower());
        }

        [Test]
        public void TEST_DVD_IFO_PARSING()
        {
            string dir = @"..\..\..\Sample Files\TestDVD";
            MediaSource ms = new MediaSource(new Disk("", dir, VideoFormat.DVD));
            Assert.IsNotNull(ms.DVDDiskInfo);
            Assert.AreEqual(1, ms.DVDDiskInfo.Titles.Length);
            DVDTitle title = ms.DVDDiskInfo.Titles[0];
            Assert.AreEqual("vts 1", title.File);
            Assert.IsTrue(title.Main);
            Assert.AreEqual(30, title.FPS);
            Assert.AreEqual(480, title.Resolution.Height);
            Assert.AreEqual(720, title.Resolution.Width);
            Assert.AreEqual(1.77999997f, title.AspectRatio);
            Assert.AreEqual(2, title.Subtitles.Count);
            Assert.AreEqual("en", title.Subtitles[0].LanguageID);
            Assert.AreEqual("fr", title.Subtitles[1].LanguageID);
            Assert.AreEqual(3, title.AudioTracks.Count);
            Assert.AreEqual("en", title.AudioTracks[0].LanguageID);
            Assert.AreEqual(6, title.AudioTracks[0].Channels);
            Assert.AreEqual(AudioEncoding.AC3, title.AudioTracks[0].Format);
            Assert.AreEqual("fr", title.AudioTracks[1].LanguageID);
            Assert.AreEqual("en", title.AudioTracks[2].LanguageID);
            Assert.AreEqual("Director's comments", title.AudioTracks[2].Extension);
            Assert.AreEqual(16, title.Chapters.Count);
            Assert.AreEqual(TimeSpan.Parse("02:06:12.532"), title.Duration);
        }
    }
}
