using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ComponentFactory.Krypton.Toolkit;

using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class NewMovieName : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public NewMovieName()
        {
            InitializeComponent();
        }

        public string MovieName()
        {
            return txtNewMovie.Text.Trim();
        }
    }
}