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

            foreach (MetaDataPluginDescriptor provider in metadataPlugins)
            {
                cmbPlugins.Properties.Items.Add(provider.DataProviderName);
            }
        }

        public MetaDataPluginDescriptor SelectedPlugin()
        {
            var qry = from t in _metadataPlugins
                      where t.DataProviderName == (cmbPlugins.SelectedItem as string)
                      select t;

            return qry.First();


            //return cmbPlugins.SelectedItem as MetaDataPluginDescriptor;
        }
    }
}