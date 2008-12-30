using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace OMLDatabaseEditor.Controls
{
    public partial class NonFatalErrors : Form
    {
        public NonFatalErrors()
        {
            InitializeComponent();
        }

        public void LoadErrors(string[] errorStrings)
        {
            foreach (string errorStr in errorStrings)
            {
                tbNonFatalErrors.AppendText(string.Format("{0}\n", errorStr));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            tbNonFatalErrors.Clear();
            this.Hide();
            this.Close();
        }
    }
}
