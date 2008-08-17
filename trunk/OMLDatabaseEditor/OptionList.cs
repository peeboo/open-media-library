using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OMLDatabaseEditor
{
    public partial class OptionValues : Form
    {
        string m_selectedValue = null;
        string m_selectedDescription = null;

        public string SelectedDescription
        {
            get { return m_selectedDescription; }
        }

        public string SelectedValue
        {
            get { return m_selectedValue; }
        }

        public OptionValues()
        {
            InitializeComponent();
        }

        private void OptionList_Load(object sender, EventArgs e)
        {

        }

        public DialogResult Show(string optionName, Dictionary<string, string> values)
        {
            Text = "Select value for " + optionName;
            grdOptions.Rows.Clear();
            if (values != null)
            {
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    grdOptions.Rows.Add(kvp.Key, kvp.Value);
                }
            }
            return ShowDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (grdOptions.SelectedRows != null && grdOptions.SelectedRows.Count > 0)
            {
                m_selectedValue = (string)grdOptions.SelectedRows[0].Cells[0].Value;
                m_selectedDescription = (string)grdOptions.SelectedRows[0].Cells[1].Value;
            }
            else
            {
                m_selectedValue = null;
                m_selectedDescription = null;
            }
        }
    }
}
