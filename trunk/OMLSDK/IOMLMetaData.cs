using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;

namespace OMLSDK
{
    public interface IOMLMetadataPlugin
    {
        string PluginName { get; }

        // these 2 methods must be called in sequence
        bool Initialize( Dictionary<string,string> parameters );
        bool SearchForMovie( string movieName );

        // these methods are to be called after the 2 methods above

        // get the best match
        Title GetBestMatch();

        // or choose among all the titles
        Title[] GetAvailableTitles();    // could be just summaries
        Title GetTitle(int index);      // get the actual Title

        // can do additional searches
        bool AdditonalSearchForMovie(string movieName);

        List<OMLMetadataOption> GetOptions();
        bool SetOptionValue(string option, string value);
    }

    public class OMLMetadataOption
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
}
