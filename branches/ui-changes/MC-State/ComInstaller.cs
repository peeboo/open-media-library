using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;

using OML.MceState;
using OMLEngine;

namespace OML.MceState
{
    [RunInstaller(true)]
    public partial class ComInstaller : Installer
    {
        public ComInstaller() : base()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(typeof(ComInstaller).Assembly, AssemblyRegistrationFlags.SetCodeBase))
                throw new Exception("Failed to register for COM interop.");
            Utilities.DebugLine("[MsasSink] Installed COM assembly: " + typeof(ComInstaller).Assembly);
        }

        public override void Uninstall(IDictionary savedState)
        {
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(typeof(ComInstaller).Assembly))
                throw new Exception("Failed to unregister for COM interop.");
            Utilities.DebugLine("[MsasSink] Unistalled COM assembly: " + typeof(ComInstaller).Assembly);
        }
    }
}
