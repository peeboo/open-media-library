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
        public delegate void BioDataChangeEventHandler(object sender, EventArgs e);
        public event BioDataChangeEventHandler BioDataChanged;

        public enum BioDataStatus { Normal, UnsavedChanges }

        BioData _bioData;
        private BioDataStatus _status;
        private bool _isLoading = false;


        public BioDataEditor()
        {
            InitializeComponent();
        }

        public BioData EditedBioData
        {
            get { return _bioData; }
        }

        public BioDataStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public void LoadBioData(BioData bioData)
        {
            _isLoading = true;

            _bioData = bioData;
            biodatasource.DataSource = _bioData;
            _status = BioDataStatus.Normal;

            _isLoading = false;
        }

        private void teBio_TextChanged(object sender, EventArgs e)
        {
            if (BioDataChanged != null && !_isLoading)
            {
                Status = BioDataStatus.UnsavedChanges;
                BioDataChanged(this, e);
            }
        }
    }
}
