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
        SortedDictionary<string, string> UnusedProperties = new SortedDictionary<string, string>();
        SortedDictionary<string, string> UsedProperties = new SortedDictionary<string, string>();
        OMLDatabaseEditor.Controls.TitleEditor titleeditor;
        public MetadataMappings(OMLDatabaseEditor.Controls.TitleEditor ptitleeditor)
        {
            titleeditor = ptitleeditor;
            InitializeComponent();
            RenderMetaDataMappings();
        }

        public void RenderMetaDataMappings()
        {   
            // Build properties assigned to a metadata provider
            // ----------------------------------------------
            foreach (OMLEngine.Dao.MataDataMapping map in OMLEngine.Settings.SettingsManager.MetaDataMap_GetMappings())
            {
                UsedProperties[map.MatadataProperty] = map.MetadataProvider;
            } 


            // Build properties not assigned to a metadata provider
            // --------------------------------------------------

            // Add possible properties to the dialog
            AddMappingProprty("FanArt");
            AddMappingProprty("Genres");

            // Scan titleeditor for bound controls
            foreach (Control c in titleeditor.Controls) 
            {
                if (c is DevExpress.XtraEditors.PanelControl)
                {
                    foreach (Control d in c.Controls)
                    {
                        if (d is DevExpress.XtraTab.XtraTabControl)
                        {
                            DevExpress.XtraTab.XtraTabControl tabctrl = d as DevExpress.XtraTab.XtraTabControl;
                            foreach (DevExpress.XtraTab.XtraTabPage tabpage in tabctrl.TabPages)
                            {
                                foreach (Control g in tabpage.Controls)
                                {
                                    if (g is DevExpress.XtraEditors.TextEdit)
                                    {
                                        //c.DataBindings[0].BindingMemberInfo.BindingMember;
                                        if (g.DataBindings.Count > 0)
                                        {
                                            AddMappingProprty(g.DataBindings[0].BindingMemberInfo.BindingMember);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // remove OML Specifics
            UnusedProperties.Remove("WatchedCount");
            UnusedProperties.Remove("UserStarRating");
            UnusedProperties.Remove("SortName");
            UnusedProperties.Remove("ImporterSource");
            UnusedProperties.Remove("DateAdded");




  //                 foreach (string name in from t in TitleCollectionManager.GetAllAspectRatios(null) select t.Name)
 
 // teVideoResolution.MaskBox.AutoCompleteCustomSource.Add(name);
 
 


            // Add all properties into the dialog box
            // --------------------------------------
            int i = 0;
            foreach (KeyValuePair<string, string> map in UsedProperties)
            {
                AddMappingToDialog(i, map.Key, map.Value);
                i++;
            }
        
            foreach (KeyValuePair<string, string> map in UnusedProperties)
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
            if (!UsedProperties.ContainsKey(property))
            {
                UnusedProperties[property] = "";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            OMLEngine.Settings.SettingsManager.MetaDataMap_Clear();

            Control[] controls;
            int i = 0;
            while ( (controls = tableLayoutPanel1.Controls.Find("lblProperty" + i, false)).Length != 0)
            {
                LabelControl lblProperty = controls[0] as LabelControl;

                ComboBoxEdit cbeMapping = tableLayoutPanel1.Controls.Find("cbeMapping" + i, false)[0] as ComboBoxEdit;

                if (cbeMapping.Text != "")
                {
                    OMLEngine.Dao.MataDataMapping map = new OMLEngine.Dao.MataDataMapping();
                    map.MatadataProperty = lblProperty.Text;
                    map.MetadataProvider = cbeMapping.Text;
                    OMLEngine.Settings.SettingsManager.MetaDataMap_Add(map);
                }

                i++;
            }
            //SettingsManager.Save();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}