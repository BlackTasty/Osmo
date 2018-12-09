using System;

namespace Installer.Objects
{
    class GlobalValues
    {
        string instPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + App.AppName;

        public string GText { get; set; }

        public string InstallationPath { get { return instPath; } }
    }
}
