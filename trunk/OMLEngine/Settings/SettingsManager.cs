using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine.Dao;
namespace OMLEngine
{


    public static class SettingsManager
    {

        public static IEnumerable<OMLEngine.Dao.MataDataMapping> MetaDataMap_GetMappings()
        {
            var mdm = from t in OMLDataSettingsDBContext.Instance.MataDataMappings
                         select t;

            return mdm;
        }

        public static string MetaDataMap_PluginForProperty(string propertyName)
        {
            var mdm = from t in OMLDataSettingsDBContext.Instance.MataDataMappings
                      where t.MatadataProperty == propertyName
                      select t.MetadataProvider;

            if (mdm.Count() == 0) { return ""; }
        
            return mdm.ToArray()[0];
        }

        public static Dictionary<string, List<string>> MetaDataMap_PropertiesByPlugin()
        {
            var mdm = from t in OMLDataSettingsDBContext.Instance.MataDataMappings
                      orderby t.MetadataProvider
                      select t;

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (OMLEngine.Dao.MataDataMapping mapping in mdm)
            {
               /* if (result.ContainsKey(map.Value))
                    result[map.Value].Add(map.Key);
                else
                    result[map.Value] = new List<string>() { map.Key };*/
                if (result.ContainsKey(mapping.MetadataProvider))
                    result[mapping.MetadataProvider].Add(mapping.MatadataProperty);
                else
                    result[mapping.MetadataProvider] = new List<string>() { mapping.MatadataProperty };
            }

            return result;
        }

        public static void MetaDataMap_Clear()
        {
            var deleteMappings = from d in Dao.OMLDataSettingsDBContext.Instance.MataDataMappings
                 select d;

            Dao.OMLDataSettingsDBContext.Instance.MataDataMappings.DeleteAllOnSubmit(deleteMappings);

            Dao.OMLDataSettingsDBContext.Instance.SubmitChanges();
        }

        public static void MetaDataMap_Remove(string genre)
        {

        }

        public static void MetaDataMap_Add(OMLEngine.Dao.MataDataMapping maping)
        {
            OMLDataSettingsDBContext.Instance.MataDataMappings.InsertOnSubmit(maping);
            OMLDataSettingsDBContext.Instance.SubmitChanges();
        }


        public static string GenreMap_GetMapping(string genreName)
        {
            var mdm = from t in OMLDataSettingsDBContext.Instance.GenreMappings
                      where t.GenreName == genreName
                      select t.GenreMapTo;

            if (mdm.Count() == 0) { return ""; }

            return mdm.ToArray()[0];
        }


        public static void GenreMap_Remove(string genre)
        {

        }

        public static void GenreMap_Add(OMLEngine.Dao.GenreMapping maping)
        {
            OMLDataSettingsDBContext.Instance.GenreMappings.InsertOnSubmit(maping);
            OMLDataSettingsDBContext.Instance.SubmitChanges();
        }

        public static void Save()
        {
            OMLDataSettingsDBContext.Instance.SubmitChanges();
        }
    }
}
