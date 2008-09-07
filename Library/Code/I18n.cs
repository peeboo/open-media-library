using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class I18n : I18nResources, IPropertyObject
    {
        private static I18n instance;
        public event PropertyChangedEventHandler PropertyChanged;

        [Obsolete("Use the Instance static property instead of creating a new instance.")]
        public I18n()
            :this(false)
        {
        }

        private I18n(bool isInstance)
        {
            if (!isInstance) throw new NotSupportedException("Use the Instance static property insted of creating a new instance.");
        }

        public static I18n Instance
        {
            get
            {
                if (instance == null) instance = new I18n(true);
                return instance;
            }
        }
        
        public static void SetCulture(CultureInfo culture)
        {
            if (culture == null) culture = Thread.CurrentThread.CurrentUICulture;
            if (culture != Culture)
            {
                Culture = culture;
                Instance.FirePropertyChanged();
            }
        }

        private void FirePropertyChanged()
        {
            if (PropertyChanged != null)
            {
                foreach (PropertyInfo propInfo in typeof (I18nResources).GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    PropertyChanged(this, propInfo.Name);
                }
            }
        }
    }
}
