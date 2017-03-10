namespace TrackerService
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
            this.serviceProcessInstallerTracker = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerTraker = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerTracker
            // 
            this.serviceProcessInstallerTracker.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerTracker.Password = null;
            this.serviceProcessInstallerTracker.Username = null;
            // 
            // serviceInstallerTraker
            // 
            this.serviceInstallerTraker.DisplayName = "Tracker Service Installer";
            this.serviceInstallerTraker.ServiceName = "S_HIDS-Tracker";
            this.serviceInstallerTraker.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerTracker,
            this.serviceInstallerTraker});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller serviceInstallerTraker;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerTracker;
    }
}