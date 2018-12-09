using System;

namespace Installer_Online.Objects
{
    class GlobalValues
    {
        string instPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Vibrance Player\\";

        public string GText { get; set; }

        public string InstallationPath { get { return instPath; } }
    }
}
