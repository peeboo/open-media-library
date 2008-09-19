using System;
using System.IO;
using OMLTranscoder;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class MEncoderCommandBuilderTest : TestBase
    {
        [Test]
        public void TEST_BASIC_COMMAND_BUILDER()
        {
            string drive = OMLEngine.Properties.Settings.Default.VirtualDiscDrive.ToLower();
            MEncoderCommandBuilder builder = new MEncoderCommandBuilder()
            {
                AudioFormat = MEncoder.AudioFormat.Copy,
                VideoFormat = MEncoder.VideoFormat.Copy,
                OutputPath  = ".",
                OutputFile  = @"mymovie",
                InputLocation = drive + ":",
            };

            Assert.IsNotNull(builder.GetCommand());
            Assert.AreEqual(@"c:\program files\openmedialibrary\mencoder.exe", builder.GetCommand().ToLower());
            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd:// -dvd-device """ + drive +
                            @":\"" -oac copy -ovc copy -of mpeg -mpegopts format=dvd:tsaf " +
                            @"-vf harddup -quiet -o ""c:\programdata\openmedialibrary\mymovie.mpg""", builder.GetArguments().ToLower());

        }
    }
}
