using Installer.Objects;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace Installer.UI
{
    /// <summary>
    /// Interaktionslogik für Install.xaml
    /// </summary>
    public partial class Uninstall : UserControl, IManagedUI
    {
        MainWindow window;
        GlobalValues GV = new GlobalValues();
        private bool keepData;
        private string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        BackgroundWorker setup = new BackgroundWorker();

        public Uninstall()
        {
            InitializeComponent();
            setup.DoWork += Setup_DoWork;
            setup.RunWorkerCompleted += Setup_RunWorkerCompleted;
        }

        private void Setup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progress.Value = progress.Maximum;
            txt_status.Text = "Vibrance Player uninstalled!";
            txt_log.Text += "\n\nVibrance Player uninstalled!";
            window.btn_next.IsEnabled = true;
        }

        private void Setup_DoWork(object sender, DoWorkEventArgs e)
        {
            KillPlayer("Edge Player");
            KillPlayer("Vibrance Player");
            RemoveFiles();
            RemoveFromRegistry();
        }

        private void RemoveFiles()
        {
            DirectoryInfo root = new DirectoryInfo(GV.InstallationPath);
            int files = root.GetDirectories().Length + root.GetFiles().Length;
            
            Invoker.InvokeProgress(progress, 0, files);
            foreach (DirectoryInfo di in root.GetDirectories())
            {
                if (!KeepData(di.Name))
                {
                    Directory.Delete(di.FullName, true);
                    PrintMessage(di.Name, true);
                }
            }

            foreach (FileInfo fi in root.EnumerateFiles())
            {
                if (!KeepData(fi.Name))
                {
                    File.Delete(fi.FullName);
                    PrintMessage(fi.Name, false);
                }
            }

            Helper.DeleteFile(string.Format("{0}\\Vibrance Player.lnk", desktop));
            Helper.DeleteFile(string.Format("{0}\\Lyrics Creator.lnk", desktop));
            Helper.DeleteFile(string.Format("{0}\\Visualizer Studio.lnk", desktop));
            Helper.DeleteDirectory(string.Format("{0}\\Vibrance Player",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)), false);
        }

        void RemoveStartMenuEntry(string shortcutName)
        {
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            string appStartMenuPath = Path.Combine(startMenuPath, "Vibrance Player");
        }

        private void PrintMessage(string objName, bool isFolder)
        {
            string message;
            if (isFolder)
                message = string.Format("Deleting folder \"{0}\"", objName);
            else
                message = string.Format("Deleting file \"{0}\"", objName);
            Invoker.InvokeStatus(progress, txt_log, txt_status, message);
        }

        private bool KeepData(string name)
        {
            if (keepData)
            {
                if (name.Equals("Lyrics") || name.Equals("Visuals") ||
                    name.Contains(".cfg") || name.Contains(".sqlite"))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private void RemoveFromRegistry()
        {
            RegistryKey edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player", false);
            string guidPath = edgeKey.GetValue("GUID").ToString();
            edgeKey.Close();
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + guidPath);
            Registry.CurrentUser.DeleteSubKey(@"Software\Vibrance Player", false);
            Registry.CurrentUser.DeleteSubKeyTree("Vibrance Player", false);

            Registry.ClassesRoot.DeleteSubKeyTree("Lyrics Creator", false);
            Registry.ClassesRoot.DeleteSubKeyTree("Vibrance Player", false);
            Registry.ClassesRoot.DeleteSubKeyTree("Visualizer Studio", false);
            Registry.ClassesRoot.DeleteSubKeyTree(@"Applications\Vibrance Player", false);
        }

        private void KillPlayer(string processName)
        {
            bool isRunning = Process.GetProcessesByName(processName).Length > 0;
            if (isRunning)
            {
                Process[] MatchingProcesses = Process.GetProcessesByName(processName);
                foreach (Process p in MatchingProcesses)
                    p.Kill();
            }
            Thread.Sleep(500);
        }

        public void RegisterParent(MainWindow window)
        {
            this.window = window;
            window.btn_cancel.IsEnabled = false;
            keepData = MessageBox.Show("Do you want to keep your personal data? (Settings, lyrics, visuals and databases)", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            setup.RunWorkerAsync();
        }
    }
}
