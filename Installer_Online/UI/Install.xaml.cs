using Installer_Online.Objects;
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
using System.Net;

namespace Installer_Online.UI
{
    /// <summary>
    /// Interaktionslogik für Install.xaml
    /// </summary>
    public partial class Install : UserControl, IManagedUI
    {
        private MainWindow window;
        private UninstallEntry uninstaller = new UninstallEntry();
        private string path, data = GlobalValues.AppName + ".exe";
        private string tempDownloadPath = Path.Combine(Path.GetTempPath(), "Osmo\\Runtime.zip");
        Logger logger = new Logger();
        BackgroundWorker setup = new BackgroundWorker();
        private Objects.Component shortcut;
        private bool isDownloading;
        
        private List<Server> servers = ServerList.DEFAULT_SERVERS;
        private Stopwatch downloadSpeedStopWatch = new Stopwatch();

        public bool Aborted { get; private set; }

        public InstallationStatus Status { get; set; }

        public double EstimatedSize { get; set; }

        public Install()
        {
            InitializeComponent();
            path = GlobalValues.InstallationPath;
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
            shortcut = window.ViewModel.GetComponent(ComponentType.SHORTCUT);
            CheckData();
            if (!DownloadRuntime())
            {
                SetStatusText("Unable to download files! Check your network connection.");
            }

            while (isDownloading)
            {
                Thread.Sleep(100);
            }
            ExtractFiles();
            RegisterInRegistry();
            CreateUninstaller();

            Aborted = setup.CancellationPending;
        }

        private void Setup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!Aborted)
            {
                Status = InstallationStatus.FINISHED;
                progress.Value = progress.Maximum;
                SetStatusText(GlobalValues.AppName + " installed!");
                window.btn_next.IsEnabled = true;
                window.btn_cancel.IsEnabled = false;
                logger.WriteLog("Everything's done and worked so far!");
            }
            else
            {
                SetStatusText("Installation aborted!");
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
                logger.WriteLog(GlobalValues.AppName + " is currently not installed...");
                if (Directory.Exists(path))
                    Directory.Delete(path, true);

                Directory.CreateDirectory(path);
            }
            else
            {
                logger.WriteLog(GlobalValues.AppName + " is currently installed, upgrading to newest version...");
                Process[] proc = Process.GetProcessesByName(GlobalValues.AppName);
                if (proc.Length > 0)
                {
                    int procIndex = GetProcessIndex(proc, Process.GetCurrentProcess());

                    if (procIndex > -1)
                    {
                        logger.WriteLog(GlobalValues.AppName + " is running! Asking the user to kill the task...");
                        var res = MessageBox.Show(GlobalValues.AppName + " has to be closed in order to continue. Shall I kill the task now?", "", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes)
                        {
                            try
                            {
                                proc[procIndex].Kill();
                                Thread.Sleep(500);
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog("Exception thrown while closing " + GlobalValues.AppName + "!", LoggingType.WARNING, ex);
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
                        Helper.DeleteDirectory(GlobalValues.InstallationPath, false);
                        Status = InstallationStatus.FINISHED;
                        break;
                    case InstallationStatus.UNINSTALLER_CREATED:
                        uninstaller.RemoveUninstaller();
                        Status = InstallationStatus.CONTENT_EXTRACTED;
                        break;
                    case InstallationStatus.REGISTRY_EDITED:
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\Applications\" + GlobalValues.AppName + ".exe");
                        Status = InstallationStatus.UNINSTALLER_CREATED;
                        break;
                }

                CheckCancellation();
            }
        }

        private bool DownloadRuntime()
        {
            if (!setup.CancellationPending)
            {
                Server targetServer = null;
                foreach (Server s in servers)
                {
                    if (s.IsAvailable)
                    {
                        targetServer = s;
                        break;
                    }
                }

                if (targetServer != null)
                {
                    panel_downloader.Dispatcher.Invoke(() =>
                    {
                        panel_downloader.Visibility = Visibility.Visible;
                    });
                    Status = InstallationStatus.DOWNLOADING;
                    if (File.Exists(tempDownloadPath))
                    {
                        File.Delete(tempDownloadPath);
                    }

                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "Osmo\\"));

                    using (WebClient wc = new WebClient())
                    {
                        isDownloading = true;
                        wc.DownloadProgressChanged += DownloadProgressChanged;
                        wc.DownloadFileCompleted += DownloadFileCompleted;
                        downloadSpeedStopWatch.Start();
                        wc.DownloadFileAsync(new Uri(targetServer.URL + "Runtime.zip"), tempDownloadPath);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
            downloadSpeedStopWatch.Reset();
            isDownloading = false;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            panel_downloader.Dispatcher.Invoke(() =>
            {
                txt_statusDownload.Text = string.Format("Downloading files... {0}MB/{1}MB ({2})",
                Math.Round((e.BytesReceived / 1024d) / 1024d, 2).ToString("0.00"),
                Math.Round((e.TotalBytesToReceive / 1024d) / 1024d, 2).ToString("0.00"),
                CalculateSpeed(e.BytesReceived));
                progressDownload.Maximum = e.TotalBytesToReceive;
                progressDownload.Value = e.BytesReceived;
            });
        }

        /// <summary>
        /// Returns the download speed in kb/s or Mb/s (if fast enough)
        /// </summary>
        /// <param name="bytesReceived">The number of bytes received in a second</param>
        /// <returns>A formatted string which shows how fast a download is progressing</returns>
        private string CalculateSpeed(long bytesReceived)
        {
            if (bytesReceived / 1024d > 1000)
            {
                return (bytesReceived / 1024d / downloadSpeedStopWatch.Elapsed.TotalSeconds).ToString("0.00") + " kb/s";
            }
            else
            {
                return ((bytesReceived / 1024d) / 1024 / downloadSpeedStopWatch.Elapsed.TotalSeconds).ToString("0.00") + " Mb/s";
            }
        }

        private void ExtractFiles()
        {
            if (!setup.CancellationPending)
            {
                Helper.DeleteDirectory(GlobalValues.InstallationPath + "Update", true);
                Helper.DeleteFile(GlobalValues.InstallationPath + "Update\\Runtime.zip");

                logger.WriteLog("Unzipping content...");
                ZipFile.ExtractToDirectory(tempDownloadPath,
                    GlobalValues.InstallationPath + "Update\\");
                File.Delete(tempDownloadPath);

                logger.WriteLog("Replacing old content...");

                DirectoryInfo dirs = new DirectoryInfo(GlobalValues.InstallationPath + "Update");
                int dataCount = dirs.GetDirectories().Length + dirs.GetFiles().Length + 1;
                Invoker.InvokeProgress(progress, 0, dataCount);

                foreach (DirectoryInfo di in dirs.GetDirectories())
                {
                    Helper.DeleteDirectory(GlobalValues.InstallationPath + di.Name, false);
                    Directory.Move(di.FullName, GlobalValues.InstallationPath + di.Name);
                    PrintMessage(di.Name);
                }

                foreach (FileInfo fi in dirs.EnumerateFiles())
                {
                    string logEntry = Helper.DeleteFile(GlobalValues.InstallationPath + fi.Name, fi.Name);
                    if (logEntry != null)
                        logger.WriteLog(logEntry, LoggingType.INFO);
                    File.Move(fi.FullName, GlobalValues.InstallationPath + fi.Name);
                    logger.WriteLog(string.Format("Decompressed file \"{0}\" successfully!", fi.Name));
                    PrintMessage(fi.Name);
                }

                Helper.DeleteDirectory(GlobalValues.InstallationPath + "Update", false);
                Status = InstallationStatus.CONTENT_EXTRACTED;
                CheckCancellation();
            }
        }

        private void CreateUninstaller()
        {
            if (!setup.CancellationPending)
            {
                Invoker.InvokeStatus(progress, txt_log, txt_status, "Creating uninstall script...");
                try
                {
                    uninstaller.CreateUninstaller(EstimatedSize);
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
                RegistryKey edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\" + GlobalValues.AppName, true);
                if (edgeKey == null)
                {
                    edgeKey = Registry.CurrentUser.CreateSubKey(@"Software\" + GlobalValues.AppName);
                    edgeKey.SetValue("Path", path);
                    edgeKey.SetValue("Name", path + data);
                    edgeKey.SetValue("GUID", GlobalValues.AppName);
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
            Helper.DeleteFile(GlobalValues.InstallationPath + exeName);
            if (!string.IsNullOrEmpty(iconName))
                Helper.DeleteFile(GlobalValues.InstallationPath + iconName);
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
                    Process associator = Process.Start(GlobalValues.InstallationPath + "Associator.exe", raw);

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

        private void SetStatusText(string message)
        {
            txt_status.Text = message;
            txt_log.Text += "\n\n" + message;
        }
    }
}
