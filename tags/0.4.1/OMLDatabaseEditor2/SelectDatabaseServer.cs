using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor
{
    public partial class SelectDatabaseServer : DevExpress.XtraEditors.XtraForm
    {
        public SelectDatabaseServer()
        {
            InitializeComponent();
            Server = OMLEngine.DatabaseManagement.DatabaseInformation.SQLServerName;
            SQLInstance = OMLEngine.DatabaseManagement.DatabaseInformation.SQLInstanceName;
        }

        public string Server
        {
            get
            {
                return teServer.Text;
            }
            set
            {
                teServer.Text = value;
                if ((teServer.Text == ".") || (teServer.Text == "localhost"))
                {
                    radioGroup1.SelectedIndex = 0;
                }
                else
                {
                    radioGroup1.SelectedIndex = 1;
                }
            }
        }

        public string SQLInstance
        {
            get
            {
                return teSQLInstance.Text;
            }
            set
            {
                teSQLInstance.Text = value;
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.Text == "L")
            {
                teServer.Text = "localhost";
                teServer.Enabled = false;
            }
            else
            {
                teServer.Enabled = true;
            }
        }
    }
}