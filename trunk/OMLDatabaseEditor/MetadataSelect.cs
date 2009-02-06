using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

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
                string pluginForProperty = MainEditor._titleCollection.PluginForProperty(_propertyName);
                cbDefault.Checked = String.IsNullOrEmpty(pluginForProperty);

                foreach (KeyValuePair<string, object> data in dataCollection)
                {
                    LabelControl lblPlugin = new LabelControl();
                    lblPlugin.Text = data.Key;
                    if (pluginForProperty == data.Key)
                        lblPlugin.Font = new Font(lblPlugin.Font, FontStyle.Bold);
                    tblData.Controls.Add(lblPlugin);
                    switch (_type)
                    {
                        case PropertyTypeEnum.Image:
                            PictureEdit peValue = new PictureEdit();
                            peValue.Properties.ShowMenu = false;
                            peValue.Tag = data.Key + "|" + data.Value.ToString();
                            peValue.Image = Utilities.ReadImageFromFile(data.Value.ToString());
                            if (peValue.Image == null)
                            {
                                tblData.Controls.Remove(lblPlugin);
                                continue;
                            }
                            peValue.ToolTip = String.Format("{0}x{1}", peValue.Image.Width, peValue.Image.Height);
                            peValue.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                            peValue.Height = 200;
                            peValue.Dock = DockStyle.Fill;
                            peValue.DoubleClick += new EventHandler(data_DoubleClick);
                            tblData.Controls.Add(peValue);
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
            string pluginName = ctrl.Tag as string;
            if (sender is PictureEdit)
            {
                string[] tagContent = pluginName.Split('|');
                pluginName = tagContent[0];
                string imagePath = tagContent[1];
                if (_propertyName == "FrontCoverPath")
                    _title.CopyFrontCoverFromFile(imagePath, true);
                else
                    _title.CopyBackCoverFromFile(imagePath, true);
            }
            else
                SetPropertyValue(_propertyInfo, _title, ctrl.Text);
            if (cbDefault.Checked)
            {
                MainEditor._titleCollection.MetadataMap[_propertyName] = pluginName;
                MainEditor._titleCollection.saveTitleCollection();
            }
            DialogResult = DialogResult.OK;
        }

        private void SetPropertyValue(PropertyInfo property, Title title, string value)
        {
            if (property.PropertyType == typeof(Int32))
                property.SetValue(title, Int32.Parse(value), null);
            else if (property.PropertyType == typeof(bool))
                property.SetValue(title, bool.Parse(value), null);
            else if (property.PropertyType == typeof(DateTime))
                property.SetValue(title, DateTime.Parse(value), null);
            else
                property.SetValue(title, value, null);
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