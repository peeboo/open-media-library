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
        GenreMetaData _genreMetaData;

        public GenreMetaDataEditor()
        {
            InitializeComponent();
        }

        public void LoadGenre(GenreMetaData genreMetaData)
        {
            _genreMetaData = genreMetaData;
            genremetadatasource.DataSource = _genreMetaData;
        }
    }
}
