using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OMLDatabaseEditor
{
    public partial class StSanaEvents : Form
    {
        static public void UpdateStatus(string msg)
        {
            StSanaEvents.instance.StSanaEventLabel.Text = msg;
            StSanaEvents.instance.Refresh();
        }

        static private StSanaEvents instance;
        public StSanaEvents()
        {
            StSanaEvents.instance = this;
            InitializeComponent();
        }
    }
}
