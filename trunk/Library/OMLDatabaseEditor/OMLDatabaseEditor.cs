using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class OMLDatabaseEditor : Form
    {
        private TitleCollection _titleCollection;
        private Title current_title;

        public OMLDatabaseEditor()
        {
            InitializeComponent();
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            current_title = new Title();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            _titleCollection.Remove(current_title);
        }
    }
}
