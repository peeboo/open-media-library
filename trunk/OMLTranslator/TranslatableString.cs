using System;
using System.Text.RegularExpressions;

namespace OMLTranslator
{
    public class TranslatableString : ObservableObject
    {
        private readonly string key;
        private readonly string source;
        private readonly TranslatableResXFile resxFile;
        private string target;
        private string persistedTarget;
        private bool isDirty;
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
            get { return target; }
            set
            {
                if (value != target)
                {
                    target = value;
                    RaisePropertyChanged("Target");
                    IsDirty = target != persistedTarget;
                    UpdateStatus();

                }
            }
        }

        public string Key
        {
            get { return key; }
        }

        public bool IsDirty
        {
            get { return isDirty; }
            internal set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    if (!isDirty) persistedTarget = target;
                    RaisePropertyChanged("IsDirty");
                }
            }
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

            if (string.IsNullOrEmpty(target))
            {
                if (!string.IsNullOrEmpty(source))
                {
                    newStatusText = "Missing translation.";
                    newStatus = TranslationStatus.Error;
                }
            }
            else
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
