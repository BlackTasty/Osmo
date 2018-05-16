using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Vibrance.Core.Classes;
using Vibrance.Core.Enums;

namespace Osmo.Core.Patcher
{
    partial class PatcherCore
    {
        void DeleteDirectory(string path, bool recreate = false)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            System.Threading.Thread.Sleep(100);

            if (recreate)
                Directory.CreateDirectory(path);
        }

        void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        string MoveFile(string sourcePath, string destPath, string[] whitelist = null)
        {
            if (!IsWhitelistFile(sourcePath, whitelist))
            {
                if (File.Exists(destPath))
                {
                    if (File.Exists(destPath + ".BAK"))
                        File.Delete(destPath + ".BAK");
                    File.Move(destPath, destPath + ".BAK");
                }
                File.Move(sourcePath, destPath);
            }
            return destPath + ".BAK";
        }

        List<string> MoveDirectory(string sourcePath, string destPath)
        {
            return MoveFiles(null, new DirectoryInfo(sourcePath), destPath);

            //Directory.Move(destPath, destPath + ".BAK");
            //Directory.Move(sourcePath, destPath);
        }

        List<string> MoveFiles(string root, DirectoryInfo di, string destPath)
        {
            List<string> paths = new List<string>();
            foreach (FileInfo fi in di.GetFiles())
            {
                paths.Add(MoveFile(fi.FullName, destPath + "\\" + fi.Name));
            }

            return paths;
        }

        async Task<string> CheckAddon(string root, string name, string iconName)
        {
            string exePath = string.Format("{0}{1}.exe", root, name);
            
            if (File.Exists(exePath) && !await CheckToolInstalled(name))
            {
                File.Delete(exePath);
                DeleteFile(string.Format("{0}{1}.ico", root, iconName));
                return "{0} installed!";
            }
            else
                return "{0} updated!";
        }

        async Task<bool> CheckToolInstalled(string displayName)
        {
            string name = displayName.Trim();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player", true);
            object obj = key.GetValue(name);
            if (obj != null)
            {
                key.Close();
                return (int)obj == 1;
            }
            else
            {
                await DialogMaster.GetInstance().ShowDialogMessage(DialogType.ADDON_UPDATE, DialogSettingsType.YESNO, displayName);
                if (await DialogMaster.GetInstance().ShowDialogMessage(DialogType.ADDON_UPDATE, 
                    DialogSettingsType.YESNO, displayName) == MessageDialogResult.Affirmative)
                    key.SetValue(name, 1);
                else
                    key.SetValue(name, 0);
                key.Close();
                return await CheckToolInstalled(displayName);
            }
        }

        bool IsWhitelistFile(string path, string[] whitelist)
        {
            if (whitelist != null)
            {
                foreach (string str in whitelist)
                {
                    if (path.Contains(str))
                        return true;
                }
            }
            return false;
        }
    }
}
