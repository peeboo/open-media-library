using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PostInstallerWizard
{
    public partial class PleaseWait : Form
    {
        public PleaseWait()
        {
            InitializeComponent();
        }

        public void SetMessage(string message)
        {
            label1.Text = message;
            label1.Invalidate();
            label1.Update();
        }
    }
}
