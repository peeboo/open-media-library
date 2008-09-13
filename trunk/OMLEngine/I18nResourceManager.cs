using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace OMLEngine
{
    public class I18nResourceManager : ResourceManager
    {

        public I18nResourceManager(string baseName, Assembly assembly)
            : base(baseName, assembly)
        {
            
        }

        public IEnumerable<CultureInfo> AvailableCultures
        {
            get
            {
                // First make a list of all available resource sets
                Dictionary<string, CultureInfo> availableResourceSets = new Dictionary<string, CultureInfo>();

                if (FileSystemWalker.TranslationsDirExists)
                {
                    foreach (string file in Directory.GetFiles(FileSystemWalker.TranslationsDirectory, BaseName + ".*.xml"))
                    {
                        Match m = Regex.Match(file, @"\\(?'name'[^\\]+)\.(?'culture'[^\.\\]+)\.xml$", RegexOptions.IgnoreCase);
                        if (m.Success && CultureInfo.InvariantCulture.CompareInfo.Compare(m.Groups["name"].Value, BaseName, CompareOptions.IgnoreCase) == 0)
                        {
                            CultureInfo culture = null;
                            try
                            {
                                culture = new CultureInfo(m.Groups["culture"].Value);

                            }
                            catch (Exception ex)
                            {
                                Utilities.DebugLine("[I18nResourceManager] Unknown resource language '{0}': {1}", 
                                                    file, ex.GetBaseException().Message);
                            }
                            if (culture != null) availableResourceSets.Add(culture.Name, culture);
                        }
                    }
                }
                // English is always available though the build in resources
                CultureInfo enDefault = new CultureInfo("en");
                if (!availableResourceSets.ContainsKey(enDefault.Name)) availableResourceSets.Add(enDefault.Name, enDefault);

                // Now loop though all possible resources on the system yielding anything that has an available culture

                foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.FrameworkCultures))
                {
                    if (culture.IsNeutralCulture) continue;
                    for (CultureInfo parent = culture; !parent.Equals(CultureInfo.InvariantCulture); parent = parent.Parent)
                    {
                        if (availableResourceSets.ContainsKey(parent.Name))
                        {
                            yield return culture;
                            break;
                        }
                    }
                }
            }   
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            string fileName = null;
            try
            {
                fileName = Path.Combine(FileSystemWalker.TranslationsDirectory, GetResourceFileName(culture));
                fileName = Path.ChangeExtension(fileName, ".xml");

                if (File.Exists(fileName))
                {
                    return new ResXResourceSet(fileName);
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[I18nResourceManager] Error loading resource file {0}: {1}",
                                    fileName ?? "null", ex.GetBaseException().Message);
            }
            
            return base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
        }
    }
}
