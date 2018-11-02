using Installer.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Installer.UI
{
    /// <summary>
    /// Interaktionslogik für Install.xaml
    /// </summary>
    public partial class Install : UserControl, IManagedUI
    {
        private MainWindow window;
        private UninstallEntry uninstaller = new UninstallEntry();
        private string path, data = "Vibrance Player.exe";
        Logger logger = new Logger();
        BackgroundWorker setup = new BackgroundWorker();
        private Objects.Component visualizerStudio, lyricsCreator, associate, shortcut;

        public bool Aborted { get; private set; }

        public InstallationStatus Status { get; set; }

        public Install()
        {
            InitializeComponent();
            path = uninstaller.GV.InstallationPath;
            Status = InstallationStatus.IDLE;
            setup.WorkerSupportsCancellation = true;
            setup.DoWork += Setup_DoWork;
            setup.RunWorkerCompleted += Setup_RunWorkerCompleted;
        }

        public void RegisterParent(MainWindow window)
        {
            this.window = window;
            setup.RunWorkerAsync();
        }

        public void AbortInstallation()
        {
            setup.CancelAsync();
        }

        private void Setup_DoWork(object sender, DoWorkEventArgs e)
        {
            lyricsCreator = window.ViewModel.GetComponent(ComponentType.LYRICS_CREATOR);
            visualizerStudio = window.ViewModel.GetComponent(ComponentType.VISUALIZER_STUDIO);
            associate = window.ViewModel.GetComponent(ComponentType.ASSOCIATE_EXTENSIONS);
            shortcut = window.ViewModel.GetComponent(ComponentType.SHORTCUT);
            CheckData();
            ExtractFiles();
            CreateUninstaller();
            RegisterInRegistry();

            Aborted = setup.CancellationPending;
        }

        private void Setup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!Aborted)
            {
                Status = InstallationStatus.FINISHED;
                progress.Value = progress.Maximum;
                txt_status.Text = "Vibrance Player installed!";
                txt_log.Text += "\n\nVibrance Player installed!";
                window.btn_next.IsEnabled = true;
                window.btn_cancel.IsEnabled = false;
                logger.WriteLog("Everything's done and worked so far!");
            }
            else
            {
                txt_status.Text = "Installation aborted!";
                txt_log.Text += "\n\nInstallation aborted!";
                window.btn_next.IsEnabled = true;
                window.btn_cancel.IsEnabled = false;
                logger.WriteLog("Installation aborted!");
            }
        }

        private void CheckData()
        {
            logger.WriteLog("Starting installation...");
            if (!window.IsUpgrade)
            {
                logger.WriteLog("Vibrance Player is currently not installed...");
                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                Directory.CreateDirectory(path);
            }
            else
            {
                logger.WriteLog("Vibrance Player is currently installed, upgrading to newest version...");
                Process[] proc = Process.GetProcessesByName("Vibrance Player");
                if (proc.Length > 0)
                {
                    int procIndex = GetProcessIndex(proc, Process.GetCurrentProcess());

                    if (procIndex > -1)
                    {
                        logger.WriteLog("Vibrance Player is running! Asking the user to kill the task...");
                        var res = MessageBox.Show("Vibrance Player has to be closed in order to continue. Shall I kill the task now?", "", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes)
                        {
                            try
                            {
                                proc[procIndex].Kill();
                                Thread.Sleep(500);
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog("Exception thrown while closing Vibrance Player!", LoggingType.WARNING, ex);
                            }
                        }
                        else
                        {
                            setup.CancelAsync();
                        }
                    }
                }
            }
            CheckCancellation();
        }

        private int GetProcessIndex(Process[] proc, Process current)
        {
            for (int i = 0; i < proc.Length; i++)
            {
                if (proc[i].Id != current.Id)
                    return i;
            }
            return -1;
        }

        private void CheckCancellation()
        {
            if (setup.CancellationPending && Status != InstallationStatus.FINISHED && Status != InstallationStatus.IDLE)
            {
                switch (Status)
                {
                    case InstallationStatus.CONTENT_EXTRACTED:
                        Helper.DeleteDirectory(uninstaller.GV.InstallationPath, false);
                        Status = InstallationStatus.FINISHED;
                        break;
                    case InstallationStatus.UNINSTALLER_CREATED:
                        uninstaller.RemoveUninstaller();
                        Status = InstallationStatus.CONTENT_EXTRACTED;
                        break;
                    case InstallationStatus.REGISTRY_EDITED:
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\Applications\Vibrance Player.exe");
                        Status = InstallationStatus.UNINSTALLER_CREATED;
                        break;
                }

                CheckCancellation();
            }
        }

        private void ExtractFiles()
        {
            if (!setup.CancellationPending)
            {
                Helper.DeleteDirectory(uninstaller.GV.InstallationPath + "Update", true);
                Helper.DeleteFile(uninstaller.GV.InstallationPath + "Update\\Runtime.zip");
                File.WriteAllBytes(uninstaller.GV.InstallationPath + "Update\\Runtime.zip", Properties.Resources.Runtime);

                logger.WriteLog("Unzipping content...");
                ZipFile.ExtractToDirectory(uninstaller.GV.InstallationPath + "Update\\Runtime.zip",
                    uninstaller.GV.InstallationPath + "Update\\");
                File.Delete(uninstaller.GV.InstallationPath + "Update\\Runtime.zip");

                RemoveAddOn("Lyrics Creator.exe", "licon.ico");
                RemoveAddOn("Visualizer Studio.exe", "vicon.ico");

                logger.WriteLog("Replacing old content...");

                DirectoryInfo dirs = new DirectoryInfo(uninstaller.GV.InstallationPath + "Update");
                int dataCount = dirs.GetDirectories().Length + dirs.GetFiles().Length + 1;
                Invoker.InvokeProgress(progress, 0, dataCount);

                foreach (DirectoryInfo di in dirs.GetDirectories())
                {
                    Helper.DeleteDirectory(uninstaller.GV.InstallationPath + di.Name, false);
                    Directory.Move(di.FullName, uninstaller.GV.InstallationPath + di.Name);
                    PrintMessage(di.Name);
                }

                foreach (FileInfo fi in dirs.EnumerateFiles())
                {
                    string logEntry = Helper.DeleteFile(uninstaller.GV.InstallationPath + fi.Name, fi.Name);
                    if (logEntry != null)
                        logger.WriteLog(logEntry, LoggingType.INFO);
                    if (CheckIfComponent(fi.Name))
                    {
                        File.Move(fi.FullName, uninstaller.GV.InstallationPath + fi.Name);
                        logger.WriteLog(string.Format("Decompressed file \"{0}\" successfully!", fi.Name));
                    }
                    PrintMessage(fi.Name);
                }

                if (!lyricsCreator.IsChecked)
                    RemoveAddOn("Lyrics Creator.exe", "licon.ico");
                if (!visualizerStudio.IsChecked)
                    RemoveAddOn("Visualizer Studio.exe", "vicon.ico");

                Helper.DeleteDirectory(uninstaller.GV.InstallationPath + "Update", false);
                Status = InstallationStatus.CONTENT_EXTRACTED;
                CheckCancellation();
            }
        }

        private bool CheckIfComponent(string name)
        {
            if (name.Equals(lyricsCreator.Name))
                return lyricsCreator.IsChecked;
            else if (name.Equals(visualizerStudio.Name))
                return visualizerStudio.IsChecked;
            else
                return true;
        }

        private void CreateUninstaller()
        {
            if (!setup.CancellationPending)
            {
                Invoker.InvokeStatus(progress, txt_log, txt_status, "Creating uninstall script...");
                try
                {
                    uninstaller.CreateUninstaller();
                }
                catch (Exception ex)
                {
                    logger.WriteLog("Couldn't create uninstaller!", LoggingType.WARNING, ex);
                    Invoker.InvokeStatus(progress, txt_log, txt_status, ex.ToString());
                }
                Status = InstallationStatus.UNINSTALLER_CREATED;
                CheckCancellation();
            }
        }

        private void RegisterInRegistry()
        {
            if (!setup.CancellationPending)
            {
                logger.WriteLog("Writing to registry...");
                Invoker.InvokeStatus(progress, txt_log, txt_status, "Creating Registry entries...");
                RegistryKey edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player", true);
                if (edgeKey == null)
                {
                    edgeKey = Registry.CurrentUser.CreateSubKey(@"Software\Vibrance Player");
                    edgeKey.SetValue("Path", path);
                    edgeKey.SetValue("Name", path + data);
                    edgeKey.SetValue("GUID", uninstaller.GV.GText);
                }

                List<Extension> extensions = new List<Extension>();
                List<StartmenuEntry> shortcuts = new List<StartmenuEntry>();
                if (!window.IsUpgrade)
                    extensions.Add(new Extension(".mpp", "Vibrance Player", "Vibrance Player playlist", uninstaller.GV.InstallationPath + data, path + "icon.ico"));

                if (shortcut.IsChecked)
                    shortcuts.Add(new StartmenuEntry("Vibrance Player", path + "icon.ico", path + data));

                if (lyricsCreator.IsChecked)
                {
                    if (edgeKey.GetValue("LyricsCreator", 0).ToString() == "0")
                        extensions.Add(new Extension(".lrc", "Lyrics Creator", "Lyrics file", path + "Lyrics Creator.exe", path + "licon.ico"));
                    edgeKey.SetValue("LyricsCreator", 1, RegistryValueKind.DWord);

                    if (shortcut.IsChecked)
                        shortcuts.Add(new StartmenuEntry("Lyrics Creator", path + "licon.ico", path + "Lyrics Creator.exe"));
                }
                else
                    edgeKey.SetValue("LyricsCreator", 0, RegistryValueKind.DWord);

                if (visualizerStudio.IsChecked)
                {
                    if (edgeKey.GetValue("VisualizerStudio", 0).ToString() == "0")
                        extensions.Add(new Extension(".vis", "Visualizer Studio", "Visualizer script", path + "Visualizer Studio.exe", path + "vicon.ico"));
                    edgeKey.SetValue("VisualizerStudio", 1, RegistryValueKind.DWord);

                    if (shortcut.IsChecked)
                        shortcuts.Add(new StartmenuEntry("Visualizer Studio", path + "vicon.ico", path + "Visualizer Studio.exe"));
                }
                else
                    edgeKey.SetValue("VisualizerStudio", 0, RegistryValueKind.DWord);


                if (associate.IsChecked)
                {
                    extensions.AddRange(Helper.GetAudioFormats(uninstaller.GV.InstallationPath + data, path + "icon.ico"));
                    edgeKey.SetValue("IsAssociated", 1, RegistryValueKind.DWord);
                }
                else
                    edgeKey.SetValue("IsAssociated", 0, RegistryValueKind.DWord);
                edgeKey.Close();

                edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Applications\Vibrance player.exe", true);
                if (edgeKey == null)
                {
                    CreateSubKey(@"Software\Classes\Applications\Vibrance Player.exe");
                    CreateSubKey(@"Software\Classes\Applications\Vibrance Player.exe\shell");
                    CreateSubKey(@"Software\Classes\Applications\Vibrance Player.exe\shell\open");
                    CreateSubKey(@"Software\Classes\Applications\Vibrance Player.exe\shell\open\command", path + "Vibrance Player.exe");
                }
                if (extensions.Count > 0 || shortcuts.Count > 0)
                {
                    Invoker.InvokeStatus(progress, txt_log, txt_status, Associate(extensions, shortcuts));
                    File.Delete(uninstaller.GV.InstallationPath + "Associator.exe");
                }

                Status = InstallationStatus.REGISTRY_EDITED;
                CheckCancellation();
            }
        }

        private void CreateSubKey(string subKey, string value = null)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(subKey);
            if (value != null)
                key.SetValue("", value);
            key.Close();
        }

        private void RemoveAddOn(string exeName, string iconName)
        {
            Helper.DeleteFile(uninstaller.GV.InstallationPath + exeName);
            if (!string.IsNullOrEmpty(iconName))
                Helper.DeleteFile(uninstaller.GV.InstallationPath + iconName);
        }

        private void PrintMessage(string objName)
        {
            string message = string.Format("Decompressing file \"{0}\"", objName);
            Invoker.InvokeStatus(progress, txt_log, txt_status, message);
        }

        private string Associate(List<Extension> extensions, List<StartmenuEntry> shortcuts)
        {
            try
            {
                string raw = "";
                foreach (Extension extension in extensions)
                    raw += string.Format("\"{0}\" ", extension.ToString());

                foreach (StartmenuEntry shortcut in shortcuts)
                {
                    if (window.ViewModel.GetComponent(ComponentType.STARTMENU).IsChecked)
                        raw += string.Format("\"{0}\" ", shortcut.ToString());

                    if (window.ViewModel.GetComponent(ComponentType.SHORTCUT).IsChecked)
                        Helper.CreateShortcut(shortcut.Path, shortcut.AppName, shortcut.Icon);

                }

                if (extensions.Count > 0)
                {
                    Invoker.InvokeStatus(progress, txt_log, txt_status, "Associating files...");
                    Process associator = Process.Start(uninstaller.GV.InstallationPath + "Associator.exe", raw);

                    while (associator.HasExited)
                    {
                        Thread.Sleep(500);
                    }

                    associator.WaitForExit(1500);
                    RemoveAddOn("Associator.exe", null);

                    if (associator.ExitCode == 0)
                        return "Files associated!";
                    else if (associator.ExitCode == 1013)
                        return "Some files could not be associated!";
                    else if (associator.ExitCode == 575)
                        return "Couldn't associate files!";
                    else
                        return "";
                }
                return "";
            }
            catch (Win32Exception ex)
            {
                logger.WriteLog("Exception while attempting to associate files!", LoggingType.WARNING, ex);
                return "Couldn't associate files!";
            }
        }
    }
}
