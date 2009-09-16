namespace OMLFWService
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
            this.OMLFWServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.OMLFWService = new System.ServiceProcess.ServiceInstaller();
            // 
            // OMLFWServiceProcessInstaller
            // 
            this.OMLFWServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.OMLFWServiceProcessInstaller.Password = null;
            this.OMLFWServiceProcessInstaller.Username = null;
            // 
            // OMLFWService
            // 
            this.OMLFWService.Description = "Watches for changes to files of interest to OML";
            this.OMLFWService.DisplayName = "OMLFileWatcher";
            this.OMLFWService.ServiceName = "OMLFWService";
            this.OMLFWService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.OMLFWServiceProcessInstaller,
            this.OMLFWService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller OMLFWServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller OMLFWService;
    }
}