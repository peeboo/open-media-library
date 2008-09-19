using System;
using System.IO;
using OMLTranscoder;
using NUnit.Framework;
using OMLEngine;

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
                OutputFile  = @"mymovie",
            };

            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd:// -dvd-device """ + drive +
                            @":\"" -oac copy -ovc copy -of mpeg -mpegopts format=dvd:tsaf " +
                            @"-vf harddup -really-quiet -o "".\mymovie.mpg""", builder.GetArguments().ToLower());
            Assert.IsNotNull(builder.GetCommand());
            Assert.AreEqual(@"c:\program files\openmedialibrary\mencoder.exe", builder.GetCommand().ToLower());

        }
    }
}
