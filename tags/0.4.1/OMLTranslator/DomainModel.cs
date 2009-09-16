using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace OMLTranslator
{
    public class DomainModel : ObservableObject
    {
        private CultureInfo currentLanguage = CultureInfo.InvariantCulture;
        readonly List<TranslatableResXFile> translatableResXFiles = new List<TranslatableResXFile>();

        public DomainModel()
        {
            Assembly assembly = GetType().Assembly;

            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.StartsWith(App.TranslatableResourcePrefix, StringComparison.InvariantCulture))
                {
                    var file = new TranslatableResXFile(assembly, resourceName);
                    translatableResXFiles.Add(file);
                    file.PropertyChanged += ResXFilePropertyChangedHandler;
                    
                }
            }
            translatableResXFiles.Sort((a, b) => string.Compare(a.BaseName, b.BaseName, StringComparison.CurrentCultureIgnoreCase));
            CurrentLanguage = new CultureInfo("en");
            
        }

        void ResXFilePropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                RaisePropertyChanged("IsDirty"); // Too few to worry about raising this multiple times
            }
        }

        public CultureInfo CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                if (!value.Equals(currentLanguage))
                {
                    // TODO: Deal with dirty flag
                    currentLanguage = value;
                    foreach (var resxFile in translatableResXFiles)
                    {
                        resxFile.CurrentLanguage = currentLanguage;
                    }
                    RaisePropertyChanged("CurrentLanguage");
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                // Too few to make it worth caching the value
                foreach (var resxFile in translatableResXFiles)
                {
                    if (resxFile.IsDirty) return true;
                }
                return false;
            }   
        }

        public static IEnumerable<CultureInfo> TargetLanguages
        {
            get
            {
                return
                    from culInfo in CultureInfo.GetCultures(CultureTypes.FrameworkCultures)
                    orderby culInfo.EnglishName
                    where culInfo.Name != "en-US" && !string.IsNullOrEmpty(culInfo.Name)
                    select culInfo;

            }
        }

        public IEnumerable<TranslatableResXFile> TranslatableResXFiles
        {
            get { return translatableResXFiles; }
        }

        public void Save()
        {
            foreach (var resxFile in translatableResXFiles)
            {
                resxFile.Save();
            }
        }

        public void PseudoTranslate(bool includeTranslated)
        {
            foreach (var resxFile in translatableResXFiles)
            {
                resxFile.PseudoTranslate(includeTranslated);
            }
        }

        public void RemovePseudoTranslation()
        {
            foreach (var resxFile in translatableResXFiles)
            {
                resxFile.RemovePseudoTranslation();
            }
        }

    }
}
