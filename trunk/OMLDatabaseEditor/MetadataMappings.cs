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
    public partial class MetadataMappings : DevExpress.XtraEditors.XtraForm
    {    
        //List<MetaDMapping> _genreMapping = new List<GenreMapping>();

        public MetadataMappings()
        {
            InitializeComponent();
            RenderMetaDataMappings();
        }

        public void RenderMetaDataMappings()
        {
            //MainEditor._titleCollection.MetadataMap[_propertyName] = pluginResult.PluginName;
            
            int i = 0;
            foreach (KeyValuePair<string, string> map in MainEditor._titleCollection.MetadataMap)
            {
                LabelControl lblProperty = new LabelControl();
                lblProperty.Name = "lblProperty" + i;
                lblProperty.Text = map.Key;
                lblProperty.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(lblProperty);
                ComboBoxEdit cbeMetaProvider = new ComboBoxEdit();
                cbeMetaProvider.Name = "cbeMapping" + i;
                cbeMetaProvider.Text = map.Value;
                cbeMetaProvider.Properties.Items.Add("");
                foreach (IOMLMetadataPlugin plugin in MainEditor._metadataPlugins)
                {
                    cbeMetaProvider.Properties.Items.Add(plugin.PluginName);
                }
                cbeMetaProvider.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(cbeMetaProvider);
                i++;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Control[] controls;
            int i = 0;
            while ( (controls = tableLayoutPanel1.Controls.Find("lblProperty" + i, false)).Length != 0)
            {
                LabelControl lblProperty = controls[0] as LabelControl;

                ComboBoxEdit cbeMapping = tableLayoutPanel1.Controls.Find("cbeMapping" + i, false)[0] as ComboBoxEdit;

                if (cbeMapping.Text != "")
                {
                    MainEditor._titleCollection.MetadataMap[lblProperty.Text] = cbeMapping.Text;
                }
                else
                {
                    MainEditor._titleCollection.MetadataMap.Remove(lblProperty.Text);
                }

                i++;
            } 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}