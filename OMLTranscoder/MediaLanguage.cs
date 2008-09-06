using System;
using System.Collections.Generic;

namespace OMLTranscoder
{
    public static class MediaLanguage
    {
        public static string LanguageNameForId(string id)
        {
            return sLanguageMap.ContainsKey(id.ToUpper()) ? sLanguageMap[id] : null;
        }

        public static string LanguageIdForName(string name)
        {
            foreach (KeyValuePair<string, string> kvPair in sLanguageMap)
                if (string.Compare(kvPair.Value, name, true) == 0)
                    return kvPair.Key;

            return null;
        }
        
        static IDictionary<string, string> sLanguageMap;

        static MediaLanguage()
        {
            sLanguageMap = new Dictionary<string, string>();
            sLanguageMap.Add("AB", "Abkhazian");
            sLanguageMap.Add("AA", "Afar");
            sLanguageMap.Add("AF", "Afrikaans");
            sLanguageMap.Add("SQ", "Albanian");
            sLanguageMap.Add("AM", "Amharic, Ameharic");
            sLanguageMap.Add("AR", "Arabic");
            sLanguageMap.Add("HY", "Armenian");
            sLanguageMap.Add("AS", "Assamese");
            sLanguageMap.Add("AY", "Aymara");
            sLanguageMap.Add("AZ", "Azerbaijani");
            sLanguageMap.Add("BA", "Bashkir");
            sLanguageMap.Add("EU", "Basque");
            sLanguageMap.Add("BN", "Bihari");
            sLanguageMap.Add("BI", "Bislama");
            sLanguageMap.Add("BR", "Breton");
            sLanguageMap.Add("BG", "Bulgarian");
            sLanguageMap.Add("MY", "Burmese");
            sLanguageMap.Add("BE", "Byelorussian");
            sLanguageMap.Add("KM", "Cambodian");
            sLanguageMap.Add("CA", "Catalan");
            sLanguageMap.Add("ZH", "Chinese");
            sLanguageMap.Add("CO", "Corsican");
            sLanguageMap.Add("HR", "Hrvatski (Croatian)");
            sLanguageMap.Add("CS", "Czech (Ceske)");
            sLanguageMap.Add("DA", "Dansk (Danish)");
            sLanguageMap.Add("NL", "Dutch (Nederlands)");
            sLanguageMap.Add("EN", "English");
            sLanguageMap.Add("EO", "Esperanto");
            sLanguageMap.Add("ET", "Estonian");
            sLanguageMap.Add("FO", "Faroese");
            sLanguageMap.Add("FJ", "Fiji");
            sLanguageMap.Add("FI", "Finnish");
            sLanguageMap.Add("FR", "French");
            sLanguageMap.Add("FY", "Frisian");
            sLanguageMap.Add("GL", "Galician");
            sLanguageMap.Add("KA", "Georgian");
            sLanguageMap.Add("DE", "Deutsch (German)");
            sLanguageMap.Add("EL", "Greek");
            sLanguageMap.Add("KL", "Greenlandic");
            sLanguageMap.Add("GN", "Guarani");
            sLanguageMap.Add("GU", "Gujarati");
            sLanguageMap.Add("HA", "Hausa");
            sLanguageMap.Add("IW", "Hebrew");
            sLanguageMap.Add("HI", "Hindi");
            sLanguageMap.Add("HU", "Hungarian");
            sLanguageMap.Add("IS", "Islenka (Icelandic)");
            sLanguageMap.Add("IN", "Indonesian");
            sLanguageMap.Add("IA", "Interlingua");
            sLanguageMap.Add("IE", "Interlingue");
            sLanguageMap.Add("IK", "Inupiak");
            sLanguageMap.Add("GA", "Irish");
            sLanguageMap.Add("IT", "Italian");
            sLanguageMap.Add("JA", "Japanese");
            sLanguageMap.Add("JW", "Javanese");
            sLanguageMap.Add("KN", "Kannada");
            sLanguageMap.Add("KS", "Kashmiri");
            sLanguageMap.Add("KK", "Kazakh");
            sLanguageMap.Add("RW", "Kinyarwanda");
            sLanguageMap.Add("KY", "Kirghiz");
            sLanguageMap.Add("RN", "Kirundi");
            sLanguageMap.Add("KO", "Korean");
            sLanguageMap.Add("KU", "Kurdish");
            sLanguageMap.Add("LO", "Laothian");
            sLanguageMap.Add("LA", "Latin");
            sLanguageMap.Add("LV", "Latvian, Lettish");
            sLanguageMap.Add("LN", "Lingala");
            sLanguageMap.Add("LT", "Lithuanian");
            sLanguageMap.Add("MK", "Macedonian");
            sLanguageMap.Add("MG", "Malagasy");
            sLanguageMap.Add("MS", "Malay");
            sLanguageMap.Add("ML", "Malayalam");
            sLanguageMap.Add("MT", "Maltese");
            sLanguageMap.Add("MI", "Maori");
            sLanguageMap.Add("MR", "Marathi");
            sLanguageMap.Add("MO", "Moldavian");
            sLanguageMap.Add("MN", "Mongolian");
            sLanguageMap.Add("NA", "Nauru");
            sLanguageMap.Add("NE", "Nepali");
            sLanguageMap.Add("NO", "Norwegian (Norsk)");
            sLanguageMap.Add("OC", "Occitan");
            sLanguageMap.Add("OR", "Oriya");
            sLanguageMap.Add("OM", "Afan (Oromo)");
            sLanguageMap.Add("PA", "Panjabi");
            sLanguageMap.Add("PS", "Pashto, Pushto");
            sLanguageMap.Add("FA", "Persian");
            sLanguageMap.Add("PL", "Polish");
            sLanguageMap.Add("PT", "Portuguese");
            sLanguageMap.Add("QU", "Quechua");
            sLanguageMap.Add("RM", "Rhaeto-Romance");
            sLanguageMap.Add("RO", "Romanian");
            sLanguageMap.Add("RU", "Russian");
            sLanguageMap.Add("SM", "Samoan");
            sLanguageMap.Add("SG", "Sangho");
            sLanguageMap.Add("SA", "Sanskrit");
            sLanguageMap.Add("GD", "Scots Gaelic");
            sLanguageMap.Add("SH", "Serbo-Crotain");
            sLanguageMap.Add("ST", "Sesotho");
            sLanguageMap.Add("SR", "Serbian");
            sLanguageMap.Add("TN", "Setswana");
            sLanguageMap.Add("SN", "Shona");
            sLanguageMap.Add("SD", "Sindhi");
            sLanguageMap.Add("SI", "Singhalese");
            sLanguageMap.Add("SS", "Siswati");
            sLanguageMap.Add("SK", "Slovak");
            sLanguageMap.Add("SL", "Slovenian");
            sLanguageMap.Add("SO", "Somali");
            sLanguageMap.Add("ES", "Spanish (Espanol)");
            sLanguageMap.Add("SU", "Sundanese");
            sLanguageMap.Add("SW", "Swahili");
            sLanguageMap.Add("SV", "Svenska (Swedish)");
            sLanguageMap.Add("TL", "Tagalog");
            sLanguageMap.Add("TG", "Tajik");
            sLanguageMap.Add("TT", "Tatar");
            sLanguageMap.Add("TA", "Tamil");
            sLanguageMap.Add("TE", "Telugu");
            sLanguageMap.Add("TH", "Thai");
            sLanguageMap.Add("BO", "Tibetian");
            sLanguageMap.Add("TI", "Tigrinya");
            sLanguageMap.Add("TO", "Tonga");
            sLanguageMap.Add("TS", "Tsonga");
            sLanguageMap.Add("TR", "Turkish");
            sLanguageMap.Add("TK", "Turkmen");
            sLanguageMap.Add("TW", "Twi");
            sLanguageMap.Add("UK", "Ukranian");
            sLanguageMap.Add("UR", "Urdu");
            sLanguageMap.Add("UZ", "Uzbek");
            sLanguageMap.Add("VI", "Vietnamese");
            sLanguageMap.Add("VO", "Volapuk");
            sLanguageMap.Add("CY", "Welsh");
            sLanguageMap.Add("WO", "Wolof");
            sLanguageMap.Add("JI", "Yiddish");
            sLanguageMap.Add("YO", "Yoruba");
            sLanguageMap.Add("XH", "Xhosa");
            sLanguageMap.Add("ZU", "Zulu");
        }

    }
}
