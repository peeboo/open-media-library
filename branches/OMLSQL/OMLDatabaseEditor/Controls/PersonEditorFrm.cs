using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor.Controls
{
    public partial class PersonEditorFrm : DevExpress.XtraEditors.XtraForm
    {
        public string PersonName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public string PersonRole
        {
            get { return txtRole.Text; }
            set { txtRole.Text = value; }
        }

        public PersonEditorFrm(bool showRole)
        {
            InitializeComponent();
            txtRole.Visible = lblRole.Visible = showRole;
        }

        private void PersonEditorFrm_Load(object sender, EventArgs e)
        {

        }
    }
}