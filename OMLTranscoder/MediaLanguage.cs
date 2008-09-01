using System;
using System.Collections;

namespace OMLTranscoder
{
    public class MediaLanguage
    {
        Hashtable languageMap;
        string currentLanguage;

        public MediaLanguage()
        {
            currentLanguage = string.Empty;

            languageMap = new Hashtable();
            languageMap.Add("AB", "Abkhazian");
            languageMap.Add("AA", "Afar");
            languageMap.Add("AF", "Afrikaans");
            languageMap.Add("SQ", "Albanian");
            languageMap.Add("AM", "Amharic, Ameharic");
            languageMap.Add("AR", "Arabic");
            languageMap.Add("HY", "Armenian");
            languageMap.Add("AS", "Assamese");
            languageMap.Add("AY", "Aymara");
            languageMap.Add("AZ", "Azerbaijani");
            languageMap.Add("BA", "Bashkir");
            languageMap.Add("EU", "Basque");
            languageMap.Add("BN", "Bihari");
            languageMap.Add("BI", "Bislama");
            languageMap.Add("BR", "Breton");
            languageMap.Add("BG", "Bulgarian");
            languageMap.Add("MY", "Burmese");
            languageMap.Add("BE", "Byelorussian");
            languageMap.Add("KM", "Cambodian");
            languageMap.Add("CA", "Catalan");
            languageMap.Add("ZH", "Chinese");
            languageMap.Add("CO", "Corsican");
            languageMap.Add("HR", "Hrvatski (Croatian)");
            languageMap.Add("CS", "Czech (Ceske)");
            languageMap.Add("DA", "Dansk (Danish)");
            languageMap.Add("NL", "Dutch (Nederlands)");
            languageMap.Add("EN", "English");
            languageMap.Add("EO", "Esperanto");
            languageMap.Add("ET", "Estonian");
            languageMap.Add("FO", "Faroese");
            languageMap.Add("FJ", "Fiji");
            languageMap.Add("FI", "Finnish");
            languageMap.Add("FR", "French");
            languageMap.Add("FY", "Frisian");
            languageMap.Add("GL", "Galician");
            languageMap.Add("KA", "Georgian");
            languageMap.Add("DE", "Deutsch (German)");
            languageMap.Add("EL", "Greek");
            languageMap.Add("KL", "Greenlandic");
            languageMap.Add("GN", "Guarani");
            languageMap.Add("GU", "Gujarati");
            languageMap.Add("HA", "Hausa");
            languageMap.Add("IW", "Hebrew");
            languageMap.Add("HI", "Hindi");
            languageMap.Add("HU", "Hungarian");
            languageMap.Add("IS", "Islenka (Icelandic)");
            languageMap.Add("IN", "Indonesian");
            languageMap.Add("IA", "Interlingua");
            languageMap.Add("IE", "Interlingue");
            languageMap.Add("IK", "Inupiak");
            languageMap.Add("GA", "Irish");
            languageMap.Add("IT", "Italian");
            languageMap.Add("JA", "Japanese");
            languageMap.Add("JW", "Javanese");
            languageMap.Add("KN", "Kannada");
            languageMap.Add("KS", "Kashmiri");
            languageMap.Add("KK", "Kazakh");
            languageMap.Add("RW", "Kinyarwanda");
            languageMap.Add("KY", "Kirghiz");
            languageMap.Add("RN", "Kirundi");
            languageMap.Add("KO", "Korean");
            languageMap.Add("KU", "Kurdish");
            languageMap.Add("LO", "Laothian");
            languageMap.Add("LA", "Latin");
            languageMap.Add("LV", "Latvian, Lettish");
            languageMap.Add("LN", "Lingala");
            languageMap.Add("LT", "Lithuanian");
            languageMap.Add("MK", "Macedonian");
            languageMap.Add("MG", "Malagasy");
            languageMap.Add("MS", "Malay");
            languageMap.Add("ML", "Malayalam");
            languageMap.Add("MT", "Maltese");
            languageMap.Add("MI", "Maori");
            languageMap.Add("MR", "Marathi");
            languageMap.Add("MO", "Moldavian");
            languageMap.Add("MN", "Mongolian");
            languageMap.Add("NA", "Nauru");
            languageMap.Add("NE", "Nepali");
            languageMap.Add("NO", "Norwegian (Norsk)");
            languageMap.Add("OC", "Occitan");
            languageMap.Add("OR", "Oriya");
            languageMap.Add("OM", "Afan (Oromo)");
            languageMap.Add("PA", "Panjabi");
            languageMap.Add("PS", "Pashto, Pushto");
            languageMap.Add("FA", "Persian");
            languageMap.Add("PL", "Polish");
            languageMap.Add("PT", "Portuguese");
            languageMap.Add("QU", "Quechua");
            languageMap.Add("RM", "Rhaeto-Romance");
            languageMap.Add("RO", "Romanian");
            languageMap.Add("RU", "Russian");
            languageMap.Add("SM", "Samoan");
            languageMap.Add("SG", "Sangho");
            languageMap.Add("SA", "Sanskrit");
            languageMap.Add("GD", "Scots Gaelic");
            languageMap.Add("SH", "Serbo-Crotain");
            languageMap.Add("ST", "Sesotho");
            languageMap.Add("SR", "Serbian");
            languageMap.Add("TN", "Setswana");
            languageMap.Add("SN", "Shona");
            languageMap.Add("SD", "Sindhi");
            languageMap.Add("SI", "Singhalese");
            languageMap.Add("SS", "Siswati");
            languageMap.Add("SK", "Slovak");
            languageMap.Add("SL", "Slovenian");
            languageMap.Add("SO", "Somali");
            languageMap.Add("ES", "Spanish (Espanol)");
            languageMap.Add("SU", "Sundanese");
            languageMap.Add("SW", "Swahili");
            languageMap.Add("SV", "Svenska (Swedish)");
            languageMap.Add("TL", "Tagalog");
            languageMap.Add("TG", "Tajik");
            languageMap.Add("TT", "Tatar");
            languageMap.Add("TA", "Tamil");
            languageMap.Add("TE", "Telugu");
            languageMap.Add("TH", "Thai");
            languageMap.Add("BO", "Tibetian");
            languageMap.Add("TI", "Tigrinya");
            languageMap.Add("TO", "Tonga");
            languageMap.Add("TS", "Tsonga");
            languageMap.Add("TR", "Turkish");
            languageMap.Add("TK", "Turkmen");
            languageMap.Add("TW", "Twi");
            languageMap.Add("UK", "Ukranian");
            languageMap.Add("UR", "Urdu");
            languageMap.Add("UZ", "Uzbek");
            languageMap.Add("VI", "Vietnamese");
            languageMap.Add("VO", "Volapuk");
            languageMap.Add("CY", "Welsh");
            languageMap.Add("WO", "Wolof");
            languageMap.Add("JI", "Yiddish");
            languageMap.Add("YO", "Yoruba");
            languageMap.Add("XH", "Xhosa");
            languageMap.Add("ZU", "Zulu");

            //set currentlang to the default lang
        }

        public string LanguageNameForId(string id)
        {
            if (languageMap.Contains(id.ToUpper()))
                return (string)languageMap[id];

            return null;
        }

        public string LanguageIdForName(string name)
        {
            foreach (DictionaryEntry kvPair in languageMap)
            {
                if (kvPair.Value.ToString().ToUpper().CompareTo(name.ToUpper()) == 0)
                    return kvPair.Key.ToString();
            }
            return null;
        }
    }
}
