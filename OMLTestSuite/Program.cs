//#define CUSTOM
//#define WCF_TEST

using System;

using OMLEngine;
using OMLEngineService;
using System.Threading;
using NUnit.Framework;

namespace OMLTestSuite
{
    class Program
    {
        static MediaSource GetTestMediaSource()
        {
            const string path = @"C:\Users\Public\Videos\DVDs\Apocalypto";
            //const string path = @"C:\Users\Public\Videos\DVDs\Rambo";

            var disk = new Disk("test", path, VideoFormat.DVD);
            var mediaSource = new MediaSource(disk);
            mediaSource.AudioStream = mediaSource.GetAudioSteam(OMLGetDVDInfo.AudioExtension.Director_s_comments);
            mediaSource.Subtitle = mediaSource.GetSubTitle("en");
            mediaSource.Title = disk.DVDDiskInfo.GetMainTitle().TitleNumber;
            mediaSource.StartChapter = 2;
            mediaSource.EndChapter = 3;
            return mediaSource;
        }

        static void Main(string[] args)
        {
#if WCF_TEST
            var ms = GetTestMediaSource();
            Console.WriteLine("Transcoding output: " + ms.GetTranscodingFileName());
            TranscodingAPI tAPI = new TranscodingAPI(ms, delegate(MediaSource source, TranscodingStatus status)
            {
                Console.WriteLine(string.Format("StatusChanged: {0}, {1}", source, status));
            });
            tAPI.Transcode();
            tAPI.Stop();

            return;
#endif
#if !CUSTOM
            Program.TEST_MEDIASOURCE_FROM_DISK_WITH_EXTRA_OPTIONS();
            Program.TEST_APPLETRAILERS();

            MoviePlayerDVDTest mpdt = new MoviePlayerDVDTest();
            Console.WriteLine("Testing: MoviePlayerDVD");
            mpdt.TEST_GENERATE_STRING_FOR_A_STANDARD_DVD();
            mpdt.TEST_GENERATE_STRING_CORRECTLY_BUILDS_TITLE_SELECTION_STRING();
            mpdt.TEST_GENERATE_STRING_CORRECTLY_BUILDS_TITLE_AND_CHAPTER_SELECTION_STRING();
            mpdt.TEST_GENERATE_STRING_CORRECTLY_BUILDS_START_TIME();

            MyMoviesPluginTest mmpt = new MyMoviesPluginTest();
            Console.WriteLine("Testing: MyMoviesPlugin");
            mmpt.TEST_FILE_APPEARS_TO_FAIL_COMPLETE_PARSING__SUPPLIED_BY_USER_SAXNIX();
            mmpt.TEST_BASE_CASE();
            mmpt.TEST_MULTIPLE_DISCS_FAIL_TO_IMPORT();
            mmpt.TEST_WHEN_NO_DISCS_ARE_DEFINED__LOOK_IN_THE_SAME_DIRECTORY_AS_THE_MYMOVIES_XML_FILE_FOR_ANY_VALID_FILES_TO_ADD_AS_DISCS();
            mmpt.TEST_WHEN_NO_DISCS_ARE_DEFINED__LOOK_IN_THE_SAME_DIRECTORY_AS_THE_MYMOVIES_XML_FILE_FOR_ANY_VALID_FILES_TO_ADD_AS_DISCS__MULTIPLE_FILES();
            mmpt.TEST_FOLDER_JPG_FILES_ARE_USED_IF_COVER_PATHS_DONT_APPEAR_TO_EXIST();
            mmpt.TEST_CORRECTLY_IMPORTS_DVRMS_FILES();

            DVDProfilerTest dpt = new DVDProfilerTest();
            Console.WriteLine("Testing: DVDProfilerPlugin");
            dpt.TEST_SPACING_IS_CORRECT_FOR_ACTOR_NAMES();
            dpt.TEST_SYNOPSIS_IS_CORRECTLY_SET_FROM_OVERVIEW();

            TitleTest tt = new TitleTest();
            Console.WriteLine("Testing: Title");
            tt.TEST_BASE_CASE();
            tt.TEST_LOAD_FROM_XML();

            TitleCollectionTest tct = new TitleCollectionTest();
            Console.WriteLine("Testing: TitleCollection");
            tct.TEST_BASE_CASE();
            tct.TEST_FIND_FOR_ID();
            tct.TEST_SOURCE_DATABASE_TO_USE();
#endif

            MEncoderCommandBuilderTest mecbt = new MEncoderCommandBuilderTest();
            Console.WriteLine("Testing: MEncoderCommandBuilder");
            //mecbt.TEST_DVD_IFO_PARSING_2();
            mecbt.TEST_DVD_IFO_PARSING();
            mecbt.TEST_COMMAND_BUILDER_A_S();
            //mecbt.EXECUTE_COMMAND_BUILDER_A_S();
#if !CUSTOM
            mecbt.TEST_BASIC_COMMAND_BUILDER();

            VirtualDirectoryTest vdt = new VirtualDirectoryTest();
            Console.WriteLine("Testing: VirtualDirectory");
            vdt.TEST_CREATE_VIRTUAL_FOLDER();
            vdt.TEST_MULTIPLE_BASE_FOLDERS_WORK();
#endif
        }

        [Test]
        public static void TEST_MEDIASOURCE_FROM_DISK_WITH_EXTRA_OPTIONS()
        {
            var mediaSource = GetTestMediaSource();
            Utilities.DebugLine("MediaSource: {0}", mediaSource);
            mediaSource.Disk.ExtraOptions = mediaSource.ExtraOptions;
            var ms = new MediaSource(mediaSource.Disk);
            Utilities.DebugLine("MediaSource: {0}", ms);
            Assert.AreEqual(mediaSource.ToString(), ms.ToString());
        }

        [Test]
        public static void TEST_APPLETRAILERS()
        {
            Console.WriteLine("Testing: AppleTrailer");
            Library.AppleTrailers trailers = new Library.AppleTrailers();
            trailers.LoadTrailers();
            Assert.Greater(trailers.trailers.Options.Count, 0);
        }
    }
}
