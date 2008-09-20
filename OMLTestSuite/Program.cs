//#define CUSTOM

using System;

namespace OMLTestSuite
{
    class Program
    {
        static void Main(string[] args)
        {
#if !CUSTOM
            AppleTrailersTest att = new AppleTrailersTest();
            Console.WriteLine("Testing: AppleTrailers");
            att.TEST_APPLE_TRAILERS_GET_TITLES();

            MoviePlayerDVDTest mpdt = new MoviePlayerDVDTest();
            Console.WriteLine("Testing: MoviePlayerDVD");
            mpdt.TEST_GENERATE_STRING_FOR_A_STANDARD_DVD();
            mpdt.TEST_GENERATE_STRING_CORRECTLY_BUILDS_TITLE_SELECTION_STRING();
            mpdt.TEST_GENERATE_STRING_CORRECTLY_BUILDS_TITLE_AND_CHAPTER_SELECTION_STRING();
            mpdt.TEST_GENERATE_STRING_CORRECTLY_BUILDS_START_TIME();

            MyMoviesPluginTest mmpt = new MyMoviesPluginTest();
            Console.WriteLine("Testing: MyMoviesPlugin");
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
            mecbt.TEST_DVD_IFO_PARSING();
            mecbt.TEST_COMMAND_BUILDER_A_S();
            mecbt.TEST_BASIC_COMMAND_BUILDER();
#if !CUSTOM

            VirtualDirectoryTest vdt = new VirtualDirectoryTest();
            Console.WriteLine("Testing: VirtualDirectory");
            vdt.TEST_CREATE_VIRTUAL_FOLDER();
            vdt.TEST_MULTIPLE_BASE_FOLDERS_WORK();
#endif
        }
    }
}
