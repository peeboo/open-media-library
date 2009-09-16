using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;

using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MetaDataPluginSelect : XtraForm
    {
        public MetaDataPluginSelect(List<IOMLMetadataPlugin> metadataPlugins)
        {
            InitializeComponent();
            cmbPlugins.Properties.Items.AddRange(metadataPlugins);
        }

        public IOMLMetadataPlugin SelectedPlugin()
        {
            return cmbPlugins.SelectedItem as IOMLMetadataPlugin;
        }
    }
}