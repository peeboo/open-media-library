using System;
using System.Collections.Generic;
using System.Windows.Forms;

using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MetaDataSettings : Form
    {
        private IOMLMetadataPlugin _selectdPlugin = null;

        public MetaDataSettings()
        {
            InitializeComponent();
        }

        private void MetaDataSettings_Load(object sender, EventArgs e)
        {

        }

        public DialogResult Show(List<IOMLMetadataPlugin> plugins)
        {
            _selectdPlugin = null;
            if (plugins != null)
            {
                foreach (IOMLMetadataPlugin plugin in plugins)
                {

                    lbMetadataPlugins.Items.Add(plugin);
                }
                lbMetadataPlugins.DisplayMember = "PluginName";
            }
            return ShowDialog();
        }

        private void lbMetadataPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbMetadataPlugins.SelectedItem != null)
            {
                grdOptions.Rows.Clear();
                IOMLMetadataPlugin plugin = lbMetadataPlugins.SelectedItem as IOMLMetadataPlugin;
                _selectdPlugin = plugin;
                List<OMLMetadataOption> options = plugin.GetOptions();
                if (options != null)
                {
                    foreach (OMLMetadataOption option in options)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        // foreach (string possibleValue in option.PossibleValues)
                        // {
                        //     colOptionValue.Items.Add(possibleValue);
                        // }
                        string currentValueDescription = "";
                        if (option.PossibleValues.ContainsKey(option.Value))
                            currentValueDescription = option.PossibleValues[option.Value];
                        row.CreateCells(grdOptions, option.Name, option.Value, currentValueDescription);
                        row.Tag = option;
                        grdOptions.Rows.Add(row);
                    }
                }
            }
        }

        private void grdOptions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle the button click for the value
            if ( e.ColumnIndex == colOptionValue.Index)
            {
                OptionValues values = new OptionValues();
                OMLMetadataOption option = grdOptions.Rows[e.RowIndex].Tag as OMLMetadataOption;
                if (values.Show(option.Name, option.PossibleValues) == DialogResult.OK)
                {
                    if( values.SelectedValue != null )
                        grdOptions.Rows[e.RowIndex].Cells[colOptionValue.Index].Value = values.SelectedValue;

                    if (values.SelectedDescription != null)
                        grdOptions.Rows[e.RowIndex].Cells[colDescription.Index].Value = values.SelectedDescription;
                }
                
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_selectdPlugin != null)
            {
                foreach (DataGridViewRow row in grdOptions.Rows)
                {
                    _selectdPlugin.SetOptionValue((string)row.Cells[colOptionName.Index].Value, (string)row.Cells[colOptionValue.Index].Value);
                }
            }
        }

    }
}
