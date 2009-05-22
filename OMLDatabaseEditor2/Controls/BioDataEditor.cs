using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OMLEngine;

namespace OMLDatabaseEditor.Controls
{
    public partial class BioDataEditor : UserControl
    {
        BioData _bioData;

        public BioDataEditor()
        {
            InitializeComponent();
        }

        public void LoadBioData(BioData bioData)
        {
            _bioData = bioData;
            biodatasource.DataSource = _bioData;
        }
    }
}
