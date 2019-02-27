using System;

namespace Installer_Online.Objects
{
    static class GlobalValues
    {
        private static string instPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string appName = "Osmo";

        public static string InstallationPath { get => instPath + "\\" + AppName + "\\"; }

        public static string AppName { get => appName; }
    }
}
