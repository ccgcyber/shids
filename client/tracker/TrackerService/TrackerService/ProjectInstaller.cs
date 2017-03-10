using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace TrackerService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstallerTraker.DisplayName = Constant.SERVICE_DESC;
            serviceInstallerTraker.ServiceName = Constant.SERVICE_DESC;
        }
    }
}
