using Microsoft.Win32;
using System;
using System.Reflection;

namespace Installer_Online.Objects
{
    class UninstallEntry
    {
        public GlobalValues GV = new GlobalValues();
        public UninstallEntry()
        {
            GV.GText = App.AppName;
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
                        key = parent.OpenSubKey(GV.GText, true) ??
                              parent.CreateSubKey(GV.GText);

                        if (key == null)
                        {
                            throw new Exception(string.Format("Unable to create uninstaller \"{0}\\{1}\"", UninstallRegKeyPath, GV.GText));
                        }

                        Assembly asm = GetType().Assembly;
                        Version v = asm.GetName().Version;
                        string exe = GV.InstallationPath + App.AppName + ".exe";

                        key.SetValue("ApplicationVersion", v.ToString());
                        key.SetValue("HelpLink", "https://osu.ppy.sh/forum/t/756318");
                        key.SetValue("DisplayIcon", exe);
                        key.SetValue("DisplayName", App.AppName);
                        key.SetValue("DisplayVersion", v.ToString(2));
                        key.SetValue("EstimatedSize", estimatedSize, RegistryValueKind.DWord);
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                        key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                        key.SetValue("Publisher", "BlackTasty");
                        key.SetValue("URLInfoAbout", "");
                        key.SetValue("InstallLocation", GV.InstallationPath);
                        key.SetValue("UninstallString", GV.InstallationPath + "uninstall.exe");
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
                    "An error occurred writing uninstall information to the registry. The application has been fully installed but can only be uninstalled manually through deleting the files located in " + GV.InstallationPath + ".",
                    ex);
            }
        }
    }
}
