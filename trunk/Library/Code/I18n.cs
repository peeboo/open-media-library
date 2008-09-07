using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class I18n : I18nResources, IPropertyObject
    {
        private static I18n instance;
        private static I18nResourceManager i18nResourceMan;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initialized the resource manager. This must be done before accessing any other method of property (including static).
        /// </summary>
        internal static void InitializeResourceManager()
        {
            if (i18nResourceMan == null)
            {
                i18nResourceMan = new I18nResourceManager("Library.I18nResources", typeof(I18nResources).Assembly);
                FieldInfo field = typeof(I18nResources).GetField("resourceMan",
                                                                  BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, i18nResourceMan);
            }
        }

        [Obsolete("Use the Instance static property instead of creating a new instance.")]
        public I18n()
            : this(false)
        {
        }

        private I18n(bool isInstance)
        {
            // Ensures the instance can only be created from within the class (Singleton) while
            // still having a public constructor making the MCML compiler happy.
            if (!isInstance) throw new NotSupportedException("Use the Instance static property insted of creating a new instance.");
        }

        private static void AssertInitialized()
        {
            // It would be possible to simply initialize at this stage, but the resource manager really
            // need to be initialized as early as possible, so it's better to fix the calling code to
            // initialize before it access anything else (including translations).
            if (i18nResourceMan == null)
            {
                throw new InvalidOperationException("Call InitializeResourceManager before accessing the culture list");
            }
        }

        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static I18n Instance
        {
            get
            {
                AssertInitialized();
                if (instance == null) instance = new I18n(true);
                return instance;
            }
        }

        /// <summary>
        /// Gets the available cultures.
        /// </summary>
        public static IEnumerable<CultureInfo> AvailableCultures
        {
            get
            {
                foreach (CultureInfo culture in i18nResourceMan.AvailableCultures)
                {
                    yield return culture;
                }
            }
        }

        /// <summary>
        /// Sets the culture to use when retrieving resources.
        /// </summary>
        /// <remarks>
        /// Fires the <see cref="PropertyChanged"/> event for all resources.
        /// </remarks>
        /// <param name="culture">The UI culture. Set to null to use the system UI culture.</param>
        public static void SetCulture(CultureInfo culture)
        {
            AssertInitialized();
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
                foreach (PropertyInfo propInfo in typeof(I18nResources).GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    PropertyChanged(this, propInfo.Name);
                }
            }
        }
    }
}
