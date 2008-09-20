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
            MediaSource ms = TestDVD();
            Assert.IsNotNull(ms.DVDDiskInfo);
            Assert.AreEqual(1, ms.DVDDiskInfo.Titles.Length);
            DVDTitle title = ms.DVDDiskInfo.Titles[0];
            Assert.AreEqual("vts 1", title.File);
            Assert.IsTrue(title.Main);
            Assert.AreEqual(30, title.FPS);
            Assert.AreEqual(480, title.Resolution.Height);
            Assert.AreEqual(720, title.Resolution.Width);
            Assert.AreEqual(1.78f, title.AspectRatio);

            Assert.AreEqual(16, title.Chapters.Count);
            Assert.AreEqual(TimeSpan.Parse("02:06:12.532"), title.Duration);

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
        }

        //[Test]
        public void TEST_COMMAND_BUILDER_A_S()
        {
            MediaSource ms = TestDVD_A_FR_S_EN();
            MEncoderCommandBuilder builder = new MEncoderCommandBuilder(ms, ".");

            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd:// -dvd-device """ + ms.MediaPath +
                            @":\"" -oac copy -ovc copy -of mpeg -mpegopts format=dvd:tsaf " +
                            @"-vf harddup -really-quiet -o "".\mymovie.mpg""", builder.GetArguments().ToLower());
        }

        static MediaSource TestDVD_A_FR_S_EN()
        {
            MediaSource ms = TestDVD();
            ms.AudioStream = new AudioStream(ms.DVDDiskInfo.Titles[0].AudioTracks[1]);
            ms.Subtitle = new SubtitleStream(ms.DVDDiskInfo.Titles[0].Subtitles[0]);
            return ms;
        }

        static MediaSource TestDVD()
        {
            string dir = @"..\..\..\Sample Files\TestDVD";
            return new MediaSource(new Disk("TestDVD", dir, VideoFormat.DVD));
        }
    }
}
