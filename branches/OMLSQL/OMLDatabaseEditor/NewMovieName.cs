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
    public partial class NewMovieName : XtraForm
    {
        public NewMovieName()
        {
            InitializeComponent();
        }

        public string MovieName()
        {
            return txtNewMovie.Text.Trim();
        }

        private void NewMovieName_Load(object sender, EventArgs e)
        {
            txtNewMovie.Focus();
        }
    }
}