using System;
using System.Collections.Generic;
using System.Collections;

using OMLEngine;

// Step 1: Make sure that you add OMLEngine and OMLSDK to your References
// Step 2: Make sure you add "using OMLENgine;" and "using OMSDK;" to your plugin code
// Step 3: Create a class that impelments interface IOMLMetadataPlugin
// Step 4: Implement the following methods from IOMLMetadatapluing:
//  Step 5: Implement PluginName property. This name is used in Menus
//  Step 6: Impelment Initialize method. You can do any initialization your plugin may need. If you don't need anything just return true;
//  Step 7: Implement SearchForMovies. OML will pass you the movie name, your code goes and searches for possible matches.
//          Use private data to keep whatever data you need around. Return true if the search was successful. You can keep an array or list of Title
//          objects to be retrieved later by OML.
//  Step 8: Impelment GetBestMatch. NOrmally returns the first Title on the list that was found using SearchForMovies.
//  Step 9: Implement GetAvailableTitles. Return an array of Title objects that were found in SearchMovies. This is to give the user
//          a chance to select the right movie. Note that you don't have to fill in all the fields in the Title object. For exapmle IMDB
//          may return only the name and possibly the year when responding to the search, while Amazon usually retrieves everything.
//  Step 10:  Implement GetTitle. OML asks for a Title at the index in the array from Step 9. At this point you may need to go back
//            to the metadata provider and dig deeper to get the full details of the movie. In IMDB, another HTTP request
//            has to be done (or possibly more, depending how much data you want). In Amazon it's easier, since the search gets the full
//            details for all the movies it finds.
//  Step 11: Implement GetOptions. Return null or an empty list if you plugin doesn't have any user settable options. The Amazon plugin
//           has one option for the locale (website to use, UK, Canada, etc). If you have options, you need to pass all the possible values to it.
//  Step 12: Implement SetOptionValue. Return true or false if you don't have options.
//  The options can be set in DBEditor.
// Step 13: compile and make sure you copy the plugin to the OML plugins directory.

namespace OMLSDK
{    
    public enum MetadataPluginCapabilities : int
    {
        SupportsBackDrops = 0x0001,
        SupportsMovieSearch = 0x0002,
        SupportsTVSearch = 0x0004,
    };

    public class MetaDataPluginDescriptor
    {
        public string DataProviderName;
        public string DataProviderMessage;
        public string DataProviderLink;
        public MetadataPluginCapabilities DataProviderCapabilities;
        public IOMLMetadataPlugin PluginDLL;
    }

    public interface IOMLMetadataPlugin
    {
        List<MetaDataPluginDescriptor> GetProviders { get; }

        // Obsolete methods & Properties
        // string PluginName { get; }
        // string MainFanArtDir { get; set; }
        // bool UseMainFanArtDir { set; }
        //MetadataPluginCapabilities GetPluginCapabilities { get; }
        //string ProviderMessage { get; }
        //string ProviderLink { get; }

        // these 2 methods must be called in sequence
        bool Initialize(string provider, Dictionary<string,string> parameters );
        bool SearchForMovie( string movieName, int maxResults );


        // TV Metadata support
        bool SearchForTVSeries(string SeriesName, string EpisodeName, int? SeriesNo, int? EpisodeNo);
        bool SearchForTVDrillDown(int id);
        // these methods are to be called after the 2 methods above

        // get the best match
        Title GetBestMatch();

        // or choose among all the titles
        Title[] GetAvailableTitles();    // could be just summaries
        Title GetTitle(int index);      // get the actual Title

        // ultimately this should use the generic base type option
        //List<OMLMetadataOptionBase> GetOptions();
        List<OMLMetadataOption> GetOptions();
        bool SetOptionValue(string option, string value);

        //bool SupportsBackDrops();

        void DownloadBackDropsForTitle(Title t, int index);
    }

    /*
        public interface IOMLMetaDataOption
        {
            string Name { get; set; }
            string Value { get; set; }
            string Description { get; set; }
            string AllowOnlyPossibleValues { get; set; }
        }
     
        public class OMLMetadataOptionBase
        {
            public string Name
            {
                get { return _optionName; }
                set { _optionName = value; }
            }

            public string Value
            {
                get { return _optionValue; }
                set { _optionValue = value; }
            }

            public OMLMetadataOptionBase()
            {
                _optionName = String.Empty;
                _optionValue = String.Empty;
            }

            public OMLMetadataOptionBase(string optionName, string optionvalue)
            {
                _optionValue = optionvalue;
                _optionName = optionName;
            }

            string _optionValue;
            string _optionName;
        }
    */

    public class OMLMetadataOption //: OMLMetadataOptionBase
    {

        public string Name
        {
            get { return _optionName; }
            set { _optionName = value; }
        }

        public string Value
        {
            get { return _optionValue.Key; }
        }

        public Dictionary<string, string> PossibleValues
        {
            get { return _optionValue.Value; }
        }

        public bool AllowOnlyPossibleValues
        {
            get { return _allowOnlyPossibleValues; }
            set { _allowOnlyPossibleValues = value; }
        }

        public OMLMetadataOption(string optionName, string value, Dictionary<string, string> possibleValuesWithDescription, bool allowOnlyPossibleValues)
        {
            _optionValue = new KeyValuePair<string, Dictionary<string, string>>(value, possibleValuesWithDescription);
            _allowOnlyPossibleValues = allowOnlyPossibleValues;
            _optionName = optionName;
        }

        string _optionName;
        KeyValuePair<string, Dictionary<string,string>> _optionValue;
        bool _allowOnlyPossibleValues = true;

    }

    // Should have Option Types for selecting Folders and Files

}
