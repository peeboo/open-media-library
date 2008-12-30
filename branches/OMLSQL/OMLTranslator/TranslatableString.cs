using System;
using System.Text.RegularExpressions;

namespace OMLTranslator
{
    public class TranslatableString : ObservableObject
    {
        private readonly string key;
        private readonly string source;
        private string inheritedTarget;
        private readonly TranslatableResXFile resxFile;
        private string target;
        private string translatedSource; // The source at the time the translation was done.
        private bool translatedSourceUpdated;
        private string persistedTarget;
        private static readonly Regex trailingSpacesRegex = new Regex(@"\s*$");
        private static readonly Regex prefixSpacesRegex = new Regex(@"^\s*");
        private string statusText = "";
        private TranslationStatus status = TranslationStatus.Ok;

        public TranslatableString(string key, string source, TranslatableResXFile resxFile)
        {
            this.key = key;
            this.source = source;
            this.resxFile = resxFile;
        }

        public string Source
        {
            get { return source; }
        }

        public string Target
        {
            get
            {
                if (string.IsNullOrEmpty(target)) return inheritedTarget;
                return target;
            }
            set
            {
                bool currentIsDirty = IsDirty;
                if (value == inheritedTarget) value = null;
                if (value != target)
                {
                    bool currentIsInherited = IsInherited;
                    target = value;
                    RaisePropertyChanged("Target");
                    if (currentIsInherited != IsInherited) RaisePropertyChanged("IsInherited");
                    TranslatedSource = Source;
                }
                
                UpdateStatus();
                if (currentIsDirty != IsDirty) RaisePropertyChanged("IsDirty");
            }
        }

        public string InheritedTarget
        {
            get { return target; }
            set
            {
                bool currentIsDirty = IsDirty;
                if (inheritedTarget != value)
                {
                    string currentTarget = Target;
                    bool currentIsInherited = IsInherited;
                    inheritedTarget = value;
                    if (target == inheritedTarget) target = null;
                    RaisePropertyChanged("InheritedTarget");
                    if (currentTarget != Target) RaisePropertyChanged("Target");
                    if (currentIsInherited != IsInherited) RaisePropertyChanged("IsInherited");
                }
                UpdateStatus();
                if (currentIsDirty != IsDirty) RaisePropertyChanged("IsDirty");
            }
        }

        public string TranslatedSource
        {
            get { return translatedSource; }
            set
            {
                if (translatedSource != value)
                {
                    bool isDirty = IsDirty;
                    translatedSource = value;
                    translatedSourceUpdated = true;
                    RaisePropertyChanged("TranslatedSource");
                    if (IsDirty != isDirty) RaisePropertyChanged("IsDirty");
                    UpdateStatus();
                }
            }
        }


        public bool IsInherited
        {
            get
            {
                return (string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(inheritedTarget));
            }
        }

        public string Key
        {
            get { return key; }
        }

        public bool IsDirty
        {
            get
            {
                return persistedTarget != Target || translatedSourceUpdated;
            }
        }

        public void ClearDirty()
        {
            persistedTarget = Target;
            translatedSourceUpdated = false;
            RaisePropertyChanged("IsDirty");
        }



        public TranslatableResXFile ResxFile
        {
            get { return resxFile; }
        }

        public TranslationStatus Status
        {
            get { return status; }
        }

        public string StatusText
        {
            get { return statusText; }
        }

        private void UpdateStatus()
        {

            TranslationStatus newStatus = TranslationStatus.Ok;
            string newStatusText = "";

            if (string.IsNullOrEmpty(Target)) // Includes inherited text
            {
                if (!string.IsNullOrEmpty(source))
                {
                    newStatusText = "Missing translation.";
                    newStatus = TranslationStatus.Error;
                }
            }
            else if (!string.IsNullOrEmpty(target)) // Only translations at this level.
            {

                if (!StringIsMultiline(source) && StringIsMultiline(target))
                {
                    newStatusText = "Newlines might not be valid in this text.";
                    newStatus = TranslationStatus.Warning;
                }
                if (trailingSpacesRegex.Match(source).Value != trailingSpacesRegex.Match(target).Value)
                {
                    newStatusText = "Trailing spaces do not match.";
                    newStatus = TranslationStatus.Warning;
                }
                if (prefixSpacesRegex.Match(source).Value != prefixSpacesRegex.Match(target).Value)
                {
                    newStatusText = "Initial spaces do not match.";
                    newStatus = TranslationStatus.Warning;
                }
                if (!string.IsNullOrEmpty(translatedSource) && translatedSource != Source)
                {
                    newStatusText = "Source text has changed after translation.";
                    newStatus = TranslationStatus.Warning;
                }
            }

            if (newStatus != status)
            {
                status = newStatus;
                RaisePropertyChanged("Status");
            }
            if (newStatusText != statusText)
            {
                statusText = newStatusText;
                RaisePropertyChanged("StatusText");
            }
        }

        private static bool StringIsMultiline(string text)
        {
            // TODO: Validate newline type as well?
            if (text.Contains("\r")) return true;
            if (text.Contains("\n")) return true;
            return false;
        }

        public void RemovePseudoTranslation()
        {
            if (IsPseudoTranslation) Target = "";
        }

        public void PseudoTranslate(bool includeTranslated)
        {
            if (string.IsNullOrEmpty(Target) || IsPseudoTranslation || includeTranslated)
            {
                Target = "「" + source + "」";
            }
        }

        private bool IsPseudoTranslation
        {
            get
            {
                if (string.IsNullOrEmpty(target)) return false;
                if (!target.StartsWith("「", StringComparison.InvariantCulture)) return false;
                if (!target.EndsWith("」", StringComparison.InvariantCulture)) return false;
                return true;
            }   
        }
    }
}
