using System;
using System.Collections;
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
            lblTitleProperty.Text = String.Format("{0} : {1}", _title.Name, _propertyName);
        }

        private void PrepareForm()
        {
            Application.DoEvents();
            Cursor = Cursors.WaitCursor;
            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, PluginTypes.MetadataPlugin);
            IOMLMetadataPlugin objPlugin;
            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                string pluginForProperty = MainEditor._titleCollection.PluginForProperty(_propertyName);
                cbDefault.Checked = String.IsNullOrEmpty(pluginForProperty);

                Dictionary<string, object> dataCollection = new Dictionary<string, object>();
                foreach (PluginServices.AvailablePlugin oPlugin in plugins)
                {
                    objPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);
                    objPlugin.Initialize(new Dictionary<string, string>());
                    try
                    {
                        objPlugin.SearchForMovie(_title.Name);
                        Title title = objPlugin.GetBestMatch();
                        if (title != null)
                        {
                            AddResult(objPlugin, _propertyInfo.GetValue(title, null), pluginForProperty);
                        }
                    }
                    catch (Exception e)
                    {
                        Utilities.DebugLine("[OMLDatabaseEditor] Error loading metadata: " + e.Message);
                        continue;
                    }
                }
                plugins = null;
            }
            Cursor = Cursors.Default;
        }

        private void AddResult(IOMLMetadataPlugin plugin, object value, string defaultPlugin)
        {
            LabelControl lblPlugin = new LabelControl();
            lblPlugin.Text = plugin.PluginName;
            if (defaultPlugin == plugin.PluginName)
                lblPlugin.Font = new Font(lblPlugin.Font, FontStyle.Bold);
            // Add context menu of other search results
            ContextMenu menu = new ContextMenu();
            menu.Tag = plugin;
            Title[] matches = plugin.GetAvailableTitles();
            for (int i = 0; i < matches.Length; i++)
            {
                MenuItem item = new MenuItem(matches[i].Name, new EventHandler(otherTitle_Click));
                item.Tag = i;
                menu.MenuItems.Add(item);
            }
            lblPlugin.ContextMenu = menu;
            Control ctrl = CreateValueControl(plugin.PluginName, value);
            if (ctrl != null)
            {
                tblData.Controls.Add(lblPlugin);
                tblData.Controls.Add(ctrl);
            }
            Application.DoEvents();
        }

        private Control CreateValueControl(string pluginName, object value)
        {
            switch (_type)
            {
                case PropertyTypeEnum.Image:
                    PictureEdit peValue = new PictureEdit();
                    peValue.Properties.ShowMenu = false;
                    peValue.Tag = new PluginResult() { PluginName = pluginName, Value = value };
                    peValue.Image = Utilities.ReadImageFromFile(value.ToString());
                    if (peValue.Image == null)
                    {
                        return null;
                    }
                    peValue.ToolTip = String.Format("{0}x{1}", peValue.Image.Width, peValue.Image.Height);
                    peValue.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                    peValue.Height = 200;
                    peValue.Dock = DockStyle.Fill;
                    peValue.DoubleClick += new EventHandler(data_DoubleClick);
                    return peValue;
                case PropertyTypeEnum.Number:
                case PropertyTypeEnum.Date:
                case PropertyTypeEnum.String:
                    if (value.ToString().Length > 20)
                    {
                        MemoEdit txtValue = new MemoEdit();
                        txtValue.Text = value.ToString();
                        txtValue.Dock = DockStyle.Fill;
                        txtValue.Tag = new PluginResult() { PluginName = pluginName, Value = value };
                        txtValue.DoubleClick += new EventHandler(data_DoubleClick);
                        return txtValue;
                    }
                    else
                    {
                        LabelControl lblValue = new LabelControl();
                        lblValue.Text = value.ToString();
                        lblValue.Tag = new PluginResult() { PluginName = pluginName, Value = value };
                        lblValue.Dock = DockStyle.Fill;
                        lblValue.DoubleClick += new EventHandler(data_DoubleClick);
                        return lblValue;
                    }
                case PropertyTypeEnum.List:
                    MemoEdit txtList = new MemoEdit();
                    foreach (object item in (IEnumerable)value)
                    {
                        txtList.Text += item.ToString() + Environment.NewLine;
                    }
                    txtList.Dock = DockStyle.Fill;
                    txtList.Tag = new PluginResult() { PluginName = pluginName, Value = value };
                    txtList.DoubleClick += new EventHandler(data_DoubleClick);
                    return txtList;
                    break;
                default:
                    return null;
            }
        }

        void otherTitle_Click(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            IOMLMetadataPlugin plugin = ((IOMLMetadataPlugin)menu.Tag);
            Title selectedTitle = plugin.GetTitle((int)item.Tag);
            for (int i = 0; i < tblData.Controls.Count; i++)
            {
                Control ctrl = tblData.Controls[i];
                if (ctrl is LabelControl && ctrl.Text == plugin.PluginName)
                {
                    int row = i / 2;
                    tblData.Controls.RemoveAt(i + 1);
                    tblData.Controls.Add(CreateValueControl(plugin.PluginName, _propertyInfo.GetValue(selectedTitle, null)), 1, row);
                    return;
                }
            }
        }

        void data_DoubleClick(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            PluginResult pluginResult = ctrl.Tag as PluginResult;
            if (sender is PictureEdit)
            {
                if (_propertyName == "FrontCoverPath")
                    _title.CopyFrontCoverFromFile((string)pluginResult.Value, true);
                else
                    _title.CopyBackCoverFromFile((string)pluginResult.Value, true);
            }
            else
                _propertyInfo.SetValue(_title, pluginResult.Value, null);
            if (cbDefault.Checked)
            {
                MainEditor._titleCollection.MetadataMap[_propertyName] = pluginResult.PluginName;
                MainEditor._titleCollection.saveTitleCollection();
            }
            DialogResult = DialogResult.OK;
        }

        private void MetadataSelect_Shown(object sender, EventArgs e)
        {
            PrepareForm();
        }
    }

    public enum PropertyTypeEnum
    {
        String
        , Number
        , Image
        , Date
        , List
    }

    class PluginResult
    {
        public string PluginName;
        public object Value;
    }
}