using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MetaDataSettings : Form
    {
        private MetaDataPluginDescriptor _selectdPlugin = null;

        public MetaDataSettings()
        {
            InitializeComponent();
        }

        public DialogResult Show(List<MetaDataPluginDescriptor> plugins)
        {
            _selectdPlugin = null;
            if (plugins != null)
            {
                foreach (MetaDataPluginDescriptor plugin in plugins)
                {
                    lbMetadataPlugins.Items.Add(plugin);
                }
                lbMetadataPlugins.DisplayMember = "DataProviderName";
            }
            return ShowDialog();
        }

        private void lbMetadataPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbMetadataPlugins.SelectedItem != null)
            {
                grdOptions.Rows.Clear();
                MetaDataPluginDescriptor plugin = lbMetadataPlugins.SelectedItem as MetaDataPluginDescriptor;
                _selectdPlugin = plugin;
                List<OMLMetadataOption> options = plugin.PluginDLL.GetOptions();
                if (options != null)
                {
                    foreach (OMLMetadataOption option in options)
                    {
                        DataGridViewRow row = new DataGridViewRow();

                        // Code to get edited combo box based on-
                        // http://www.sommergyll.com/datagridview-usercontrols/datagridview-with-combobox.htm
                        // Create the combo box
                        DataGridViewComboBoxCell cbcell = new DataGridViewComboBoxCell();

                        if (!option.AllowOnlyPossibleValues)
                            grdOptions.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(grdOptions_EditingControlShowing);
                        else
                            grdOptions.EditingControlShowing -= new DataGridViewEditingControlShowingEventHandler(grdOptions_EditingControlShowing);


                        // Add possible values to the combo
                        if (option.PossibleValues != null)
                        {
                            if (option.PossibleValues.Count > 0)
                            {
                                foreach (KeyValuePair<string, string> v in option.PossibleValues)
                                {
                                    cbcell.Items.Add(v.Value);
                                }
                            }
                        }

                        // Select currently selected value
                        if (!string.IsNullOrEmpty(option.Value))
                        {
                            string currentselectedoption = option.Value;

                            if (option.PossibleValues != null)
                                if (option.PossibleValues.ContainsKey(option.Value))
                                    currentselectedoption = option.PossibleValues[option.Value];


                            if (!cbcell.Items.Contains(currentselectedoption))
                            {
                                cbcell.Items.Add(option.Value);
                            }
                            cbcell.Value = currentselectedoption;
                        }

                        // Create the trext box option name
                        DataGridViewTextBoxCell b;
                        b = new DataGridViewTextBoxCell();
                        b.Value = option.Name;

                        // Create the row
                        row.Cells.Add(b);
                        row.Cells.Add(cbcell);
                        row.Tag = option;
                        grdOptions.Rows.Add(row);
                    }
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_selectdPlugin != null)
            {
                foreach (DataGridViewRow row in grdOptions.Rows)
                {
                    string optionname = (string)row.Cells[colOptionName.Index].Value;
                    string valuestr = (string)row.Cells[colOptionValue.Index].Value;

                    OMLMetadataOption option = row.Tag as OMLMetadataOption;

                    // Reverse lookup the description to find the option name
                    if (option.PossibleValues != null)
                    {
                        if (option.PossibleValues.ContainsValue(valuestr))
                        {
                            var key = (from k in option.PossibleValues
                                       where string.Compare(k.Value, valuestr) == 0
                                       select k.Key).FirstOrDefault();


                            valuestr = key;
                        }
                    }

                    
                    // Set the value in the plugin
                    _selectdPlugin.PluginDLL.SetOptionValue(optionname, valuestr);

                    // Set the value in the db
                    OMLEngine.Settings.SettingsManager.SaveSettingByName(optionname, valuestr, "PLG-" + _selectdPlugin.DataProviderName);
                }
            }
        }

        private void grdOptions_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewComboBoxEditingControl comboControl
                = e.Control as DataGridViewComboBoxEditingControl;
            if (comboControl != null)
            {
                // Set the DropDown style to get an editable ComboBox
                if (comboControl.DropDownStyle != ComboBoxStyle.DropDown)
                {
                    comboControl.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }

        }

        private void grdOptions_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewComboBoxCell cell = grdOptions.CurrentCell as DataGridViewComboBoxCell;

            if (cell != null &&
                !cell.Items.Contains(e.FormattedValue))
            {

                // Insert the new value into position 0
                // in the item collection of the cell
                cell.Items.Insert(0, e.FormattedValue);
                // When setting the Value of the cell, the  
                // string is not shown until it has been
                // comitted. The code below will make sure 
                // it is committed directly.
                if (grdOptions.IsCurrentCellDirty)
                {
                    // Ensure the inserted value will 
                    // be shown directly.
                    // First tell the DataGridView to commit 
                    // itself using the Commit context...
                    grdOptions.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
                // ...then set the Value that needs 
                // to be committed in order to be displayed directly.
                cell.Value = cell.Items[0];
            }

        }
    }
}
