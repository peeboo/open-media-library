using System;
using System.IO;
using OMLTranscoder;
using NUnit.Framework;
using OMLEngine;
using OMLGetDVDInfo;
using System.Diagnostics;

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
                AudioEncoderFormat = MEncoder.AudioFormat.Copy,
                VideoEncoderFormat = MEncoder.VideoFormat.Copy,
                OutputFile  = @".\mymovie",
            };

            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd:// -dvd-device """ + drive.ToLower() +
                            @":\"" -oac copy -ovc copy -of mpeg -mpegopts format=dvd:tsaf " +
                            @"-vf harddup -really-quiet -o "".\mymovie.mpg""", builder.GetArguments().ToLower());
            Assert.IsNotNull(builder.GetCommand());
            Assert.AreEqual(@"c:\program files\openmedialibrary\mencoder.exe", builder.GetCommand().ToLower());
        }

        [Test]
        public void TEST_DVD_IFO_PARSING()
        {
            MediaSource ms = TestDVD(TestDVD_Dir);
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
            Assert.AreEqual(AudioExtension.Director_s_comments, title.AudioTracks[2].Extension);
        }

        [Test]
        public void TEST_DVD_IFO_PARSING_2()
        {
            MediaSource ms = TestDVD(@"C:\Users\Public\Videos\DVDs\Apocalypto");
            Assert.AreEqual(29, ms.DVDDiskInfo.Titles.Length);
            DVDTitle title = ms.DVDDiskInfo.GetMainTitle();
            Assert.AreEqual(4, title.TitleNumber);
            Assert.AreEqual(19, title.Chapters.Count);
            Assert.AreEqual(3, title.AudioTracks.Count);
            //audio stream: 0 format: ac3 (5.1) language: unknown aid: 128.
            Assert.AreEqual("", title.AudioTracks[0].LanguageID);
            Assert.AreEqual(128, title.AudioTracks[0].ID);
            //audio stream: 1 format: dts (5.1) language: unknown aid: 137.
            Assert.AreEqual("", title.AudioTracks[1].LanguageID);
            Assert.AreEqual(137, title.AudioTracks[1].ID);
            //audio stream: 2 format: ac3 (mono) language: en aid: 130.
            Assert.AreEqual("en", title.AudioTracks[2].LanguageID);
            Assert.AreEqual(130, title.AudioTracks[2].ID);

            Assert.AreEqual(5, title.Subtitles.Count);
            //subtitle ( sid ): 0 language: en
            Assert.AreEqual("en", title.Subtitles[0].LanguageID);
            //subtitle ( sid ): 1 language: en
            Assert.AreEqual("en", title.Subtitles[1].LanguageID);
            //subtitle ( sid ): 2 language: fr
            Assert.AreEqual("fr", title.Subtitles[2].LanguageID);
            //subtitle ( sid ): 3 language: es
            Assert.AreEqual("es", title.Subtitles[3].LanguageID);
            //subtitle ( sid ): 4 language: en
            Assert.AreEqual("en", title.Subtitles[4].LanguageID);
        }

        [Test]
        public void TEST_COMMAND_BUILDER_A_S()
        {
            MediaSource ms = TestDVD_A_S(TestDVD_Dir, 1, 0);
            MEncoderCommandBuilder builder = new MEncoderCommandBuilder(ms, ".");

            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd://1 -dvd-device """ + Path.GetFullPath(ms.MediaPath).ToLower() + @"""" +
                            @" -aid 129 -oac lavc -ovc lavc" +
                            @" -font c:\windows\fonts\arial.ttf -sid 0" +
                            @" -of mpeg -mpegopts format=dvd:tsaf" +
                            @" -vf harddup -really-quiet -o "".\testdvd.mpg""", builder.GetArguments().ToLower());
        }

        static MediaSource TestDVD_A_S(string dir, int at, int st)
        {
            MediaSource ms = TestDVD(dir);
            DVDTitle main = ms.DVDDiskInfo.GetMainTitle();
            if (at >= 0)
                ms.AudioStream = new AudioStream(main.AudioTracks[at]);
            if (st >= 0)
                ms.Subtitle = new SubtitleStream(main.Subtitles[st]);
            return ms;
        }

        const string TestDVD_Dir = @"..\..\..\Sample Files\TestDVD";
        static MediaSource TestDVD(string dir)
        {
            return new MediaSource(new Disk("TestDVD", dir, VideoFormat.DVD));
        }

        [Test]
        public void EXECUTE_COMMAND_BUILDER_A_S()
        {
            MediaSource ms = TestDVD_A_S(@"C:\Users\Public\Videos\DVDs\Apocalypto", 2, 2);
            ms.StartChapter = 2;
            ms.EndChapter = 2;

            MEncoderCommandBuilder builder = new MEncoderCommandBuilder(ms, ".");
            Process process = new Process();
            process.StartInfo.FileName = builder.GetCommand();
            process.StartInfo.Arguments = builder.GetArguments().Replace("-really-quiet", "");
            process.StartInfo.ErrorDialog = true;
            process.Start();
            process.WaitForExit();
        }
    }
}
