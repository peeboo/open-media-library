namespace OMLEngineService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OMLEngineServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.OMLEngineService = new System.ServiceProcess.ServiceInstaller();
            // 
            // OMLEngineServiceProcessInstaller
            // 
            this.OMLEngineServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.OMLEngineServiceProcessInstaller.Password = null;
            this.OMLEngineServiceProcessInstaller.Username = null;
            // 
            // OMLEngineService
            // 
            this.OMLEngineService.Description = "Provides OML with Title Collection and Management services";
            this.OMLEngineService.DisplayName = "OMLEngineService";
            this.OMLEngineService.ServiceName = "OMLEngineService";
            this.OMLEngineService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.OMLEngineServiceProcessInstaller,
            this.OMLEngineService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller OMLEngineServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller OMLEngineService;
    }
}