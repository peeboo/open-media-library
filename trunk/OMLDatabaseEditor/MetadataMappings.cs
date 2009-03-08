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
            // Add possible properties to the dialog
            AddMappingProprty("BoxArt");
            AddMappingProprty("FanArt");
            AddMappingProprty("Genres");


            int i = 0;
            foreach (KeyValuePair<string, string> map in MainEditor._titleCollection.MetadataMap)
            {
                AddMappingToDialog(i, map.Key, map.Value);
                i++;
            }
        }


        private void AddMappingToDialog(int no, string property, string value)
        {
            LabelControl lblProperty = new LabelControl();
            lblProperty.Name = "lblProperty" + no;
            lblProperty.Text = property;
            lblProperty.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(lblProperty);
            ComboBoxEdit cbeMetaProvider = new ComboBoxEdit();
            cbeMetaProvider.Name = "cbeMapping" + no;
            cbeMetaProvider.Text = value;
            cbeMetaProvider.Properties.Items.Add("");
            foreach (IOMLMetadataPlugin plugin in MainEditor._metadataPlugins)
            {
                cbeMetaProvider.Properties.Items.Add(plugin.PluginName);
            }

            cbeMetaProvider.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(cbeMetaProvider);
        }

        private void AddMappingProprty(string property)
        {
            if (!MainEditor._titleCollection.MetadataMap.ContainsKey(property))
            {
                MainEditor._titleCollection.MetadataMap[property] = "";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MainEditor._titleCollection.MetadataMap.Clear();

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

                i++;
            } 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}