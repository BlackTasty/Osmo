using System;

namespace Installer.Objects
{
    class GlobalValues
    {
        string instPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Vibrance Player\\";

        public string GText { get; set; }

        public string InstallationPath { get { return instPath; } }
    }
}
