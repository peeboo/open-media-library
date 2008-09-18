using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using OMLEngine;

namespace OMLTranslator
{
    public class TranslatableResXFile : ObservableObject
    {
        private readonly ObservableCollection<TranslatableString> translatableStrings = new ObservableCollection<TranslatableString>();
        private readonly string baseName;
        private CultureInfo currentLanguage;

        private bool? cachedIsDirty;
        private TranslationStatus? cachedStatus;


        public TranslatableResXFile(Assembly assembly, string resourceName)
        {
            baseName = Path.ChangeExtension(resourceName.Substring(App.TranslatableResourcePrefix.Length), null);

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (ResourceReader resReader = new ResourceReader(stream))
                {
                    foreach (System.Collections.DictionaryEntry entry in resReader)
                    {
                        if (entry.Value is string)
                        {
                            var translatableString = new TranslatableString((string)entry.Key, (string)entry.Value, this);
                            translatableStrings.Add(translatableString);
                            translatableString.PropertyChanged += TranslatableStringPropertyChangedHandler;
                        }
                    }
                }
            }
        }

        void TranslatableStringPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsDirty":
                    bool? oldIsDirty = cachedIsDirty;
                    cachedIsDirty = null;
                    if (!oldIsDirty.HasValue || IsDirty != oldIsDirty.Value)
                    {
                        RaisePropertyChanged("IsDirty");
                    }
                    break;
                case "Status":
                    TranslationStatus? oldStatus = cachedStatus;
                    cachedStatus = null;
                    if (!oldStatus.HasValue || Status != oldStatus.Value)
                    {
                        RaisePropertyChanged("Status");
                    }
                    break;
            }
        }


        public string BaseName
        {
            get { return baseName; }
        }

        public string TargetResXFileName
        {
            get
            {
                return GetResXFileName(currentLanguage);
            }
        }

        private string GetResXFileName(CultureInfo cultureInfo)
        {
            return Path.Combine(FileSystemWalker.TranslationsDirectory,
                                BaseName + "." + cultureInfo.Name + ".xml");
        }

        public ObservableCollection<TranslatableString> TranslatableStrings
        {
            get { return translatableStrings; }
        }

        private IEnumerable<TranslatableString> NonEmptyTranslatableStrings
        {
            get
            {
                return
                    from str in translatableStrings
                    where !string.IsNullOrEmpty(str.Target) && !str.IsInherited
                    select str;
            }
        }

        public CultureInfo CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                if (currentLanguage != value)
                {
                    currentLanguage = value;
                    ResourceSet resourceSet = null;
                    bool isEnglish = false;
                    for (CultureInfo culture = currentLanguage; !isEnglish && !string.IsNullOrEmpty(culture.Name); culture = culture.Parent)
                    {
                        if (culture.Name == "en") isEnglish = true;
                    }

                    if (File.Exists(TargetResXFileName))
                    {
                        resourceSet = new ResXResourceSet(TargetResXFileName);
                    }

                    var inheritedResources = InheritedResourceSets;
                    foreach (var str in TranslatableStrings)
                    {
                        string inheritedTarget = null;
                        foreach (var inheritedResourceSet in inheritedResources)
                        {
                            inheritedTarget = inheritedResourceSet.GetString(str.Key);
                            if (!string.IsNullOrEmpty(inheritedTarget)) break;
                        }

                        // Special rule - as the program default resources are en English, always use them as the ultimate
                        // fallback for any English language culture
                        if (isEnglish && string.IsNullOrEmpty(inheritedTarget)) inheritedTarget = str.Source;
                        
                        string newTarget = "";
                        if (resourceSet != null)
                        {
                            newTarget = resourceSet.GetString(str.Key);
                        }
                        str.InheritedTarget = inheritedTarget;
                        str.Target = newTarget;
                        str.ClearDirty();
                    }
                }
            }
        }

        private IEnumerable<ResourceSet> InheritedResourceSets
        {
            get
            {
                for (CultureInfo culture = currentLanguage.Parent; !string.IsNullOrEmpty(culture.Name); culture = culture.Parent)
                {
                    string path = GetResXFileName(culture);
                    if (File.Exists(path))
                    {
                        yield return new ResXResourceSet(path);
                    }
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                if (!cachedIsDirty.HasValue)
                {
                    cachedIsDirty = false;
                    foreach (var str in translatableStrings)
                    {
                        if (str.IsDirty)
                        {
                            cachedIsDirty = true;
                            break;
                        }
                    }
                }
                return cachedIsDirty.Value;
            }
        }

        public TranslationStatus Status
        {
            get
            {
                if (!cachedStatus.HasValue)
                {
                    int result = (int)TranslationStatus.Ok;
                    foreach (var str in translatableStrings)
                    {
                        result = Math.Max(result, (int)str.Status);
                    }
                    cachedStatus = (TranslationStatus)result;
                }
                return cachedStatus.Value;
            }
        }

        public override string ToString()
        {
            return BaseName;
        }

        

        public void Save()
        {
            if (!IsDirty) return;
            if (File.Exists(TargetResXFileName)) File.Delete(TargetResXFileName);

            bool containsTranslation = false;
            foreach (var str in NonEmptyTranslatableStrings)
            {
                containsTranslation = true;
                break;
            }

            if (containsTranslation)
            {
                if (!FileSystemWalker.TranslationsDirExists)
                    Directory.CreateDirectory(FileSystemWalker.TranslationsDirectory);

                using (ResXResourceWriter writer = new ResXResourceWriter(TargetResXFileName))
                {
                    foreach (var str in NonEmptyTranslatableStrings)
                    {
                        writer.AddResource(str.Key, str.Target);
                    }
                }
            }
            else
            {
                if (File.Exists(TargetResXFileName)) File.Delete(TargetResXFileName);
            }


            // Clear dirty flag
            foreach (var str in translatableStrings)
            {
                str.ClearDirty();
            }
        }

        public void PseudoTranslate(bool includeTranslated)
        {
            foreach (var str in translatableStrings)
            {
                str.PseudoTranslate(includeTranslated);
            }
        }

        public void RemovePseudoTranslation()
        {
            foreach (var str in translatableStrings)
            {
                str.RemovePseudoTranslation();
            }
        }
    }
}
