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
            MEncoderCommandBuilder builder = new MEncoderCommandBuilder();
            builder.SetAudioOutputFormat(MEncoder.AudioFormat.Copy);
            builder.SetVideoOutputFormat(MEncoder.VideoFormat.Copy);
            builder.SetInputType(MEncoder.InputType.Drive);
            builder.SetInputLocation(new DriveInfo("R"));
            builder.SetOutputFile(@"mymovie");

            Assert.IsNotNull(builder.GetCommand());
            Assert.AreEqual(@"c:\program files\openmedialibrary\mencoder.exe", builder.GetCommand().ToLower());
            Assert.IsNotNull(builder.GetArguments());
            Assert.AreEqual(@" dvd:// -dvd-device """ + OMLEngine.Properties.Settings.Default.VirtualDiscDrive.ToLower() +
                            @":\"" -oac copy -ovc copy -of mpeg -mpegopts format=dvd:tsaf " +
                            @"-vf harddup -quiet -o ""c:\programdata\openmedialibrary\mymovie.mpg""", builder.GetArguments().ToLower());

        }
    }
}
