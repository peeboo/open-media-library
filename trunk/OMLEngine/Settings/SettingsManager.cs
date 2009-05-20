using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine.Dao;

namespace OMLEngine.Settings
{
    public static class SettingsManager
    {
        private static Dictionary<string, string> settingsCache = new Dictionary<string, string>();

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

        public static string GetSettingByName(string name, string instanceName)
        {
            string cachedValue;
            
            if (settingsCache.TryGetValue(name + "|" + instanceName, out cachedValue))
                return cachedValue;

            Setting setting = OMLDataSettingsDBContext.Instance.Settings.SingleOrDefault(s => s.SettingName == name 
                                && s.InstanceName == instanceName);

            // if the setting isn't found for the instance fallback to the main one
            if (setting == null)
            {
                setting = OMLDataSettingsDBContext.Instance.Settings.SingleOrDefault(s => s.SettingName == name 
                                    && s.InstanceName == "main");                
            }

            cachedValue = (setting == null)
                        ? null
                        : string.IsNullOrEmpty(setting.SettingValue)
                            ? null
                            : setting.SettingValue;

            settingsCache[name + "|" + instanceName] = cachedValue;

            return cachedValue;
        }

        public static int? GetSettingByNameInt(string name, string instanceName)
        {
            string value = SettingsManager.GetSettingByName(name, instanceName);

            if (string.IsNullOrEmpty(value))
                return null;

            int selection;

            return int.TryParse(value, out selection) ? selection : -1;
        }

        public static float? GetSettingByNameFloat(string name, string instanceName)
        {
            string value = SettingsManager.GetSettingByName(name, instanceName);

            if (string.IsNullOrEmpty(value))
                return null;

            float selection;

            return float.TryParse(value, out selection) ? selection : -1;
        }

        public static bool? GetSettingByNameBool(string name, string instanceName)
        {
            string value = SettingsManager.GetSettingByName(name, instanceName);

            if (string.IsNullOrEmpty(value))
                return null;
            
            bool selection;

            return bool.TryParse(value, out selection) ? selection : false;
        }

        public static IList<string> GetSettingByNameListString(string name, string instanceName)
        {
            string value = SettingsManager.GetSettingByName(name, instanceName);

            if (string.IsNullOrEmpty(value))
                return null;

            return new List<string>(value.Split('\t'));
        }

        public static void SaveSettingByName(string name, IList<string> values, string instanceName)
        {
            StringBuilder sb = new StringBuilder();

            if (values != null && values.Count > 0)
            {
                for (int x = 0; x < values.Count; x++)
                {
                    sb.Append(values[x]);
                    if (x != values.Count - 1)
                        sb.Append('\t');
                }
            }

            SaveSettingByName(name, sb.ToString(), instanceName);
        }

        public static void SaveSettingByName(string name, string value, string instanceName)
        {
            Setting setting = OMLDataSettingsDBContext.Instance.Settings.SingleOrDefault(s => s.SettingName == name
                                && s.InstanceName == instanceName);

            if (setting == null)
            {
                setting = new Setting();                
                setting.SettingName = name;
                setting.InstanceName = instanceName;                

                OMLDataSettingsDBContext.Instance.Settings.InsertOnSubmit(setting);
            }

            setting.SettingValue = value;

            OMLDataSettingsDBContext.Instance.SubmitChanges();

            settingsCache.Clear();
        }

        public static void Save()
        {
            OMLDataSettingsDBContext.Instance.SubmitChanges();
        }
    }
}
