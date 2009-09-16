using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Settings
{
    public class MetaDataSettings
    {
        private const char SETTINGS_SPLIT_CHAR = '\t';
        private const char OPTIONS_SPLIT_CHAR = '|';

        private string _name;
        private List<KeyValuePair<string, string>> _options;

        /// <summary>
        /// The name of the meta data source
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Any options and their values to use with the meta data source
        /// </summary>
        public IList<KeyValuePair<string, string>> Options { get { return _options.AsReadOnly(); } }


        /// <summary>
        /// Private constructor that creates a meta data settings from the db string
        /// </summary>
        /// <param name="metaString"></param>
        private MetaDataSettings(string metaString)
        {
            _options = new List<KeyValuePair<string, string>>();

            string[] pieces = metaString.Split(OPTIONS_SPLIT_CHAR);

            _name = pieces[0];

            if (pieces.Length > 1)
            {
                for (int x = 1; x < pieces.Length; x += 2)
                {
                    _options.Add(new KeyValuePair<string, string>(pieces[x], pieces[x + 1]));
                }
            }
        }

        /// <summary>
        /// Create a meta data settings to save
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public MetaDataSettings(string name, List<KeyValuePair<string, string>> options)
        {
            _name = name;
            _options = options;
        }

        /// <summary>
        /// Returns a collection of meta data settings
        /// </summary>
        /// <param name="dbString"></param>
        /// <returns></returns>
        internal static IEnumerable<MetaDataSettings> CreateMetaDataSettingsFromString(string dbString)
        {
            if (!string.IsNullOrEmpty(dbString))
            {
                string[] pieces = dbString.Split(SETTINGS_SPLIT_CHAR);

                foreach (string piece in pieces)
                    yield return new MetaDataSettings(piece);
            }
        }        

        /// <summary>
        /// Serializes the meta data to a string to store in the db
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal static string GetStringFromMetaDataSettings(IList<MetaDataSettings> settings)
        {
            if ( settings == null )
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < settings.Count; x++)
            {
                sb.Append(settings[x].Name);

                if (settings[x].Options.Count != 0)
                {
                    sb.Append(OPTIONS_SPLIT_CHAR);

                    foreach (KeyValuePair<string, string> pair in settings[x].Options)
                    {
                        sb.Append(pair.Key);
                        sb.Append(OPTIONS_SPLIT_CHAR);
                        sb.Append(pair.Value);
                    }
                }

                if (x != settings.Count - 1)
                    sb.Append(SETTINGS_SPLIT_CHAR);
            }

            return sb.ToString();
        }      
    }
}
