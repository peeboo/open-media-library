using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;

namespace OMLSDK
{
    public interface IOMLMetadataPlugin
    {
        string GetPluginName();

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
    }

}
