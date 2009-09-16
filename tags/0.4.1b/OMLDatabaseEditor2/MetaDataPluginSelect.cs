using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using DevExpress.XtraEditors;

using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MetaDataPluginSelect : XtraForm
    {
        List<MetaDataPluginDescriptor> _metadataPlugins;
        public MetaDataPluginSelect(List<MetaDataPluginDescriptor> metadataPlugins)
        {
            InitializeComponent();
            _metadataPlugins = metadataPlugins;

            // Preferred sources
            if (!String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin))
            {
                cmbPlugins.Properties.Items.Add("From Preferred Sources");
            }

            foreach (MetaDataPluginDescriptor provider in metadataPlugins)
            {
                cmbPlugins.Properties.Items.Add(provider.DataProviderName);
            }
        }

        public bool SelectedPlugin(out MetaDataPluginDescriptor selectedplugin)
        {
            selectedplugin = null;
            
            if (cmbPlugins.SelectedItem == "From Preferred Sources")
            {
                // Preferred metadata selected. Leave selectedplugin as null
                return true;
            }

            var qry = from t in _metadataPlugins
                      where t.DataProviderName == (cmbPlugins.SelectedItem as string)
                      select t;

            if (qry.Count() > 0)
            {
                // A Plugin has been selected
                selectedplugin = qry.First();
                return true;
            }

            // Nothing selected
            return false;
        }
    }
}