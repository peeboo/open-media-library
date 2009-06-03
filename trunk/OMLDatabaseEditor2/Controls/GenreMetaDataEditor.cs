using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
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
    public partial class GenreMetaDataEditor : UserControl
    {
        public delegate void GenreMetaDataChangeEventHandler(object sender, EventArgs e);
        public event GenreMetaDataChangeEventHandler GenreMetaDataChanged;

        public enum GenreMetaDataStatus { Normal, UnsavedChanges }

        GenreMetaData _genreMetaData;
        private GenreMetaDataStatus _status;
        private bool _isLoading = false;


        public GenreMetaDataEditor()
        {
            InitializeComponent();
        }

        public GenreMetaData EditedGenreMetaData
        {
            get { return _genreMetaData; }
        }

        public GenreMetaDataStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public void LoadGenre(GenreMetaData genreMetaData)
        {
            _isLoading = true;

            _genreMetaData = genreMetaData;
            genremetadatasource.DataSource = _genreMetaData;
            _status = GenreMetaDataStatus.Normal;

            _isLoading = false;
        }

        private void teGenre_TextChanged(object sender, EventArgs e)
        {
            if (GenreMetaDataChanged != null && !_isLoading)
            {
                Status = GenreMetaDataStatus.UnsavedChanges;
                GenreMetaDataChanged(this, e);
            }
        }

        private void miSelectImage_Click(object sender, EventArgs e)
        {
            if (openCoverFile.ShowDialog() == DialogResult.OK)
            {
                _genreMetaData.ImagePath = openCoverFile.FileName;

                genremetadatasource.ResetCurrentItem();

                Status = GenreMetaDataStatus.UnsavedChanges;
                GenreMetaDataChanged(this, e);
            }
        }
    }
}
