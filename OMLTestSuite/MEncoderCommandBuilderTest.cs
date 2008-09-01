using System;
using System.IO;
using OMLTranscoder;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class MEncoderCommandBuilderTest
    {
        [Test]
        public void TEST_PRESET_COMMAND_TO_DETERMINE_SUBTITLE_STREAMS()
        {
            MEncoderCommandBuilder builder = new MEncoderCommandBuilder();
            builder.SetAudioOutputFormat(MEncoder.AudioFormat.COPY);
            builder.SetVideoOutputFormat(MEncoder.VideoFormat.Copy);
            builder.SetInputType(MEncoder.InputType.Drive);
            builder.SetInputLocation(new DriveInfo("R"));
            builder.SetOutputFile(@"mymovie");

            Assert.IsNotNull(builder.GetCommand());
            Assert.AreEqual(@"C:\Program Files\OpenMediaLibrary\mencoder.exe -alang en -oac copy -ovc copy " +
                            @"-dvd-device ""R:\"" dvd:// -of mpeg -mpegopts format=dvd:tsaf -vf harddup -quiet -o ""C:\ProgramData\OpenMediaLibrary\mymovie.mpg""",
                            builder.GetCommand());
        }
    }
}
