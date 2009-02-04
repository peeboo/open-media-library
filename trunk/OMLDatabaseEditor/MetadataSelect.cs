using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MetadataSelect : DevExpress.XtraEditors.XtraForm
    {
        private Title _title;
        private string _propertyName;
        private PropertyTypeEnum _type;
        private PropertyInfo _propertyInfo;

        public MetadataSelect(Title title, string propertyName, PropertyTypeEnum type)
        {
            InitializeComponent();
            _title = title;
            _propertyName = propertyName;
            _type = type;
            Type tTitle = title.GetType();
            _propertyInfo = tTitle.GetProperty(propertyName);
            PrepareForm();
        }

        private void PrepareForm()
        {
            lblTitleProperty.Text = String.Format("{0} : {1}", _title.Name, _propertyName);
            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, PluginTypes.MetadataPlugin);
            IOMLMetadataPlugin objPlugin;
            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                Dictionary<string, object> dataCollection = new Dictionary<string, object>();
                foreach (PluginServices.AvailablePlugin oPlugin in plugins)
                {
                    objPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);
                    objPlugin.Initialize(new Dictionary<string, string>());
                    objPlugin.SearchForMovie(_title.Name);
                    Title title = objPlugin.GetBestMatch();
                    if (title != null)
                    {
                        dataCollection[objPlugin.PluginName] = _propertyInfo.GetValue(title, null);
                    }
                }
                plugins = null;

                foreach (KeyValuePair<string, object> data in dataCollection)
                {
                    LabelControl lblPlugin = new LabelControl();
                    lblPlugin.Text = data.Key;
                    if (MainEditor._titleCollection.PluginForProperty(_propertyName) == data.Key)
                        lblPlugin.Font = new Font(lblPlugin.Font, FontStyle.Bold);
                    tblData.Controls.Add(lblPlugin);
                    switch (_type)
                    {
                        case PropertyTypeEnum.Image:
                            PictureBox pbValue = new PictureBox();
                            pbValue.ImageLocation = data.Value.ToString();
                            pbValue.SizeMode = PictureBoxSizeMode.Zoom;
                            pbValue.Dock = DockStyle.Fill;
                            pbValue.Height = 200;
                            pbValue.Tag = data.Key;
                            pbValue.DoubleClick += new EventHandler(data_DoubleClick);
                            tblData.Controls.Add(pbValue);
                            break;
                        case PropertyTypeEnum.Number:
                        case PropertyTypeEnum.Date:
                        case PropertyTypeEnum.String:
                            if (data.Value.ToString().Length > 20)
                            {
                                MemoEdit txtValue = new MemoEdit();
                                txtValue.Text = data.Value.ToString();
                                txtValue.Dock = DockStyle.Fill;
                                txtValue.Tag = data.Key;
                                txtValue.DoubleClick += new EventHandler(data_DoubleClick);
                                tblData.Controls.Add(txtValue);
                            }
                            else
                            {
                                LabelControl lblValue = new LabelControl();
                                lblValue.Text = data.Value.ToString();
                                lblValue.Tag = data.Key;
                                lblValue.Dock = DockStyle.Fill;
                                lblValue.DoubleClick += new EventHandler(data_DoubleClick);
                                tblData.Controls.Add(lblValue);
                            }
                            break;
                    }
                }
            }
        }

        void data_DoubleClick(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (cbDefault.Checked)
            {
                MainEditor._titleCollection.MetadataMap[_propertyName] = ctrl.Tag as string;
                MainEditor._titleCollection.saveTitleCollection();
            }
            if (sender is PictureBox)
            {
                if (_propertyName == "FrontCoverPath")
                    _title.CopyFrontCoverFromFile((sender as PictureBox).ImageLocation, true);
                else
                    _title.CopyBackCoverFromFile((sender as PictureBox).ImageLocation, true);
            }
            else
                _propertyInfo.SetValue(_title, ctrl.Text, null);
            DialogResult = DialogResult.OK;
        }
    }

    public enum PropertyTypeEnum
    {
        String
        , Number
        , Image
        , Date
    }
}