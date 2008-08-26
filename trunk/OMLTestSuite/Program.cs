using System;

namespace OMLTestSuite
{
    class Program
    {
        static void Main(string[] args)
        {
            MyMoviesPluginTest mmpt = new MyMoviesPluginTest();
            Console.WriteLine("Testing: MyMoviesPlugin");
            //mmpt.TEST_MULTIPLE_DISCS_FAIL_TO_IMPORT();
            mmpt.TEST_BASE_CASE();

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

            OMLXMLImporterTest oxit = new OMLXMLImporterTest();
            Console.WriteLine("Testing: OMLXMLPlugin");
            oxit.TEST_CONVERT_MYMOVIES_XML_TO_OML_XML();
            oxit.TEST_AUTO_LOCATE_CONVERT_AND_LOAD_A_MYMOVIES_XML_INTO_AN_OML_XML();

            MEncoderCommandBuilderTest mecbt = new MEncoderCommandBuilderTest();
            Console.WriteLine("Testing: MEncoderCommandBuilder");
            mecbt.TEST_PRESET_COMMAND_TO_DETERMINE_SUBTITLE_STREAMS();

            VirtualDirectoryTest vdt = new VirtualDirectoryTest();
            Console.WriteLine("Testing: VirtualDirectory");
            vdt.TEST_CREATE_VIRTUAL_FOLDER();
            vdt.TEST_MULTIPLE_BASE_FOLDERS_WORK();
        }
    }
}
