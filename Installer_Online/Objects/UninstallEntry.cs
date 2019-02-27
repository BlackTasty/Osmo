using Microsoft.Win32;
using System;
using System.Reflection;

namespace Installer_Online.Objects
{
    class UninstallEntry
    {
        public UninstallEntry()
        {
        }

        public virtual string UninstallRegKeyPath
        {
            get
            {
                return @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
            }
        }

        public void RemoveUninstaller()
        {
            Registry.CurrentUser.DeleteSubKeyTree(UninstallRegKeyPath);
        }

        public void CreateUninstaller(double estimatedSize)
        {
            try
            {
                try
                {
                    if (Registry.CurrentUser.OpenSubKey(UninstallRegKeyPath) == null)
                        Registry.CurrentUser.CreateSubKey(UninstallRegKeyPath);
                }
                catch
                {
                    Registry.CurrentUser.CreateSubKey(UninstallRegKeyPath);
                }

                using (RegistryKey parent = Registry.CurrentUser.OpenSubKey(
                             UninstallRegKeyPath, true))
                {
                    if (parent == null)
                    {
                        throw new Exception("Uninstall registry key not found.");
                    }
                    RegistryKey key = null;

                    try
                    {
                        key = parent.OpenSubKey(GlobalValues.AppName, true) ??
                              parent.CreateSubKey(GlobalValues.AppName);

                        if (key == null)
                        {
                            throw new Exception(string.Format("Unable to create uninstaller \"{0}\\{1}\"", UninstallRegKeyPath, GlobalValues.AppName));
                        }

                        Assembly asm = GetType().Assembly;
                        Version v = asm.GetName().Version;
                        string exe = GlobalValues.InstallationPath + GlobalValues.AppName + ".exe";

                        key.SetValue("ApplicationVersion", v.ToString());
                        key.SetValue("HelpLink", "https://osu.ppy.sh/forum/t/756318");
                        key.SetValue("DisplayIcon", exe);
                        key.SetValue("DisplayName", GlobalValues.AppName);
                        key.SetValue("DisplayVersion", v.ToString(2));
                        key.SetValue("EstimatedSize", estimatedSize, RegistryValueKind.DWord);
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                        key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                        key.SetValue("Publisher", "BlackTasty");
                        key.SetValue("URLInfoAbout", "");
                        key.SetValue("InstallLocation", GlobalValues.InstallationPath);
                        key.SetValue("UninstallString", GlobalValues.InstallationPath + "uninstall.exe");
                    }
                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An error occurred writing uninstall information to the registry. The application has been fully installed but can only be uninstalled manually through deleting the files located in " + GlobalValues.InstallationPath + ".",
                    ex);
            }
        }
    }
}
