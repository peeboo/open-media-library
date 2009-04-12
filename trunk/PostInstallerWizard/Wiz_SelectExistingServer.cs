using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PostInstallerWizard
{
    public partial class Wiz_SelectExistingServer : UserControl
    {
        public Wiz_SelectExistingServer()
        {
            InitializeComponent();
        }

        public void ShowAdvancedFields(bool advanced)
        {
            labelInstanceName.Visible = advanced;
            labelSAPwd.Visible = advanced;
            teInstance.Visible = advanced;
            teSAPwd.Visible = advanced;
   
        }

    }
}
