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
                yield return new CultureInfo("en-US");
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
                            if (culture != null) yield return culture;
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
