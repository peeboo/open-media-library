extern alias system;
using System.Globalization;
using System.Resources;
using OMLEngine;
using cm = system::System.ComponentModel;

/**********************************************************************
 * 
 * Replaces the existing ComponentResourceManager with a version
 * able to load resources out of resx files instead of compiled 
 * .dll or .resources files. This is done by loading the System.dll
 * into both the global and system namespaces, then defining this
 * System.ComponentModel.ComponentResourceManager in the global
 * namespace only. 
 * 
 * *********************************************************************/


namespace System.ComponentModel
{
    public class ComponentResourceManager : cm.ComponentResourceManager
    {
        private readonly I18nResourceManager resxBasedResourceManager;

        public ComponentResourceManager(Type type)
            :base(type)
        {
            resxBasedResourceManager = new I18nResourceManager(type);
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            return resxBasedResourceManager.PublicGetResourceSet(culture, createIfNotExists, tryParents);
        }
    }
}
