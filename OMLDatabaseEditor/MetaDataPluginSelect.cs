using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ComponentFactory.Krypton.Toolkit;

using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MetaDataPluginSelect : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public MetaDataPluginSelect(List<IOMLMetadataPlugin> metadataPlugins)
        {
            InitializeComponent();
            cmbPlugins.DataSource = metadataPlugins;
        }

        public IOMLMetadataPlugin SelectedPlugin()
        {
            return cmbPlugins.SelectedItem as IOMLMetadataPlugin;
        }
    }
}