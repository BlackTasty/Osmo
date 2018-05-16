using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using Vibrance.Core.Classes;
using Vibrance.Core.Classes.Online;
using System.IO.Compression;
using System.Windows.Input;
using System.Linq;

namespace Skin_Manager.Core.Patcher
{
    public partial class PatcherCore
    {
        private DispatcherTimer checkUpdate = new DispatcherTimer();
        private string _versionLocal;
        private int _versionLocalRaw;
        private string _versionOnline;
        private bool _forceVersion;
        private List<Server> servers = FixedValues.DEFAULT_SERVERS;

        internal Queue<string> PatchList { get; set; }

        internal bool UpdateAvailable { get; set; }

        internal string Version
        {
            get { return _versionLocal; }
            private set
            {
                _versionLocal = value;
                _versionLocalRaw = Helper.ParseVersion(_versionLocal, 0);
            }
        }

        internal int VersionRaw { get { return _versionLocalRaw; } }

        internal string ForcedVersion
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Version = Helper.SerializeVersionNumber(value, 3);
                    _forceVersion = true;
                }
            }
        }

        internal PatcherCore(Player player)
        {
            PatchList = new Queue<string>();

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "version.txt"))
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "version.txt");

            if (!_forceVersion)
                Version = Helper.SerializeVersionNumber(Assembly.GetExecutingAssembly().GetName().Version.ToString(), 3);

            this.player = player;

            checkUpdate.Tick += CheckUpdate_Tick;
            checkUpdate.Interval = TimeSpan.FromMinutes(player.AppSettings.SearchUpdatesInterval);
            checkUpdate.IsEnabled = player.AppSettings.SearchUpdates;
            if (player.AppSettings.SearchUpdates)
                checkUpdate.Start();
        }

        internal void RefreshSettings()
        {
            checkUpdate.Stop();
            checkUpdate.Interval = TimeSpan.FromMinutes(player.AppSettings.SearchUpdatesInterval);
            checkUpdate.IsEnabled = player.AppSettings.SearchUpdates;
        }

        private void CheckUpdate_Tick(object sender, EventArgs e)
        {
            CheckUpdates();
        }

        internal void CheckUpdates()
        {
            player.ViewModel.UpdateTitle = Helper.FindString("patch_search");
            player.context_update.IsEnabled = false;
            Thread updater = new Thread(CheckUpdatesAsync);
            updater.Start();
        }

        private void CheckUpdatesAsync()
        {
            try
            {
                bool getStableBuild = false;
                //if (File.Exists(player.AppSettings.InstallPath + FixedValues.LOCAL_FILENAME))
                //    UpdateAvailable = true;

                if (!UpdateAvailable)
                {
                    player.update.NotificationVisibility(true, player.ViewModel.LowMode);

                    player.Logger.WriteLog("Searching for updates...");
                    GetFileFromHTTP("version.txt", AppDomain.CurrentDomain.BaseDirectory + "version.txt", true, "");
                    CheckIfPlayerUpToDate(getStableBuild);
                }
                else
                    ApplyUpdate();
            }
            catch (Exception ex)
            {
                player.Logger.WriteLog("Can't connect to the server. Either your connection is too slow or the server is currently offline.", ex);
                player.ViewModel.UpdateTitle = Helper.FindString("patch_serverWarn");
                Thread.Sleep(4500);
                player.update.NotificationVisibility(false, player.ViewModel.LowMode);
            }
            finally
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "version.txt"))
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "version.txt");
                Invoker.InvokeMenuItem(player.context_update, true);
            }
        }

        private void GetFileFromHTTP(string fileName, string targetPath, bool removeTargetPathFirst, string msg)
        {
            if (removeTargetPathFirst)
                File.Delete(targetPath);
            GetFileFromHTTP(fileName, targetPath);
            if (!string.IsNullOrWhiteSpace(msg))
                player.ViewModel.UpdateTitle = msg;
        }

        internal void GetFileFromHTTP(string fileName, string targetPath, bool isRuntime = false)
        {
            Server current = null;
            foreach (Server s in servers)
            {
                if (s.IsAvailable)
                {
                    current = s;
                    break;
                }
            }

            if (current != null)
            {
                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                using (WebClient wc = new WebClient())
                {
                    if (isRuntime)
                    {
                        player.ViewModel.UpdateIndeterminate = false;
                        wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                        sw.Start();
                        wc.DownloadFileAsync(new Uri(current.URL + fileName), targetPath);
                    }
                    else
                        wc.DownloadFile(new Uri(current.URL + fileName), targetPath);
                }
            }
            else
                throw new Exception("No server is available right now!");
        }

        internal bool PreloadList()
        {
            string listPath = Path.Combine(Path.GetTempPath(), "Vibrance Player\\Patchnotes\\patch.txt");
            try
            {
                try
                {
                    GetFileFromHTTP("Patchnotes\\_Patches.txt", listPath);
                }
                catch
                {
                    if (!File.Exists(listPath))
                        return false;
                }

                player.Logger.WriteLogVerbose(this, "Queueing patches...", listPath);
                foreach (string patch in File.ReadAllLines(listPath))
                    PatchList.Enqueue(patch);
                player.Logger.WriteLogVerbose(this, "Done: Queued patches: {0}", PatchList.Count);
                return true;
            }
            catch
            {
                return false;
            }
        }

        Stopwatch sw = new Stopwatch();
        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            player.ViewModel.UpdateMessage = string.Format("{0}MB/{1}MB ({2})",
                Math.Round((e.BytesReceived / 1024d) / 1024d, 2).ToString("0.00"),
                Math.Round((e.TotalBytesToReceive / 1024d) / 1024d, 2).ToString("0.00"),
                CalculateSpeed(e.BytesReceived));
            player.ViewModel.UpdateValue = e.BytesReceived;
            player.ViewModel.UpdateMaximum = e.TotalBytesToReceive;

        }

        private string CalculateSpeed(long bytesReceived)
        {
            if (bytesReceived / 1024d > 1000)
            {
                return (bytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00") + " kb/s";
            }
            else
            {
                return ((bytesReceived / 1024d) / 1024 / sw.Elapsed.TotalSeconds).ToString("0.00") + " Mb/s";
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            sw.Reset();
            InstallUpdate();
        }

        internal void ApplyUpdate()
        {
            if (UpdateAvailable)
            {
                player.Dispatcher.Invoke(async() =>
                {
                    player.update.Cursor = Cursors.Hand;
                    MessageDialogResult res = await DialogMaster.GetInstance().ShowUpdateDialog(_versionLocal, _versionOnline);
                    if (res == MessageDialogResult.Affirmative)
                    {
                        player.RestartApp();
                    }
                });
            }
        }

        private async void InstallUpdate()
        {
            player.ViewModel.UpdateTitle = Helper.FindString("patch_installUpdate");
            player.ViewModel.UpdateMessage = "";
            player.ViewModel.UpdateIndeterminate = true;

            #region Actual "installaton" (unzipping and replacing)
            try
            {
                string updateFolder = player.AppSettings.InstallPath + "Update\\";
                if (Directory.Exists(updateFolder))
                    Directory.Delete(updateFolder, true);
                ZipFile.ExtractToDirectory(player.AppSettings.InstallPath + FixedValues.LOCAL_FILENAME, updateFolder);
                Thread.Sleep(200);
                File.Delete(player.AppSettings.InstallPath + FixedValues.LOCAL_FILENAME);
                player.Logger.WriteLog("Package successfully unpacked! Starting installation...");
                DirectoryInfo diRoot = new DirectoryInfo(updateFolder);

                List<string> backupFiles = new List<string>();
                foreach (DirectoryInfo di in diRoot.GetDirectories())
                {
                    backupFiles.AddRange(MoveDirectory(di.FullName, player.AppSettings.InstallPath + di.Name));
                    //DeleteDirectory(player.AppSettings.InstallPath + di.Name);
                    //Directory.Move(di.FullName, player.AppSettings.InstallPath + di.Name);
                }

                foreach (FileInfo fi in diRoot.EnumerateFiles())
                {
                    backupFiles.Add(MoveFile(fi.FullName, player.AppSettings.InstallPath + fi.Name));
                }

                File.WriteAllLines(player.AppSettings.InstallPath + "cleanup.txt", backupFiles.ToArray());

                player.Logger.WriteLog("Main application updated! Ensuring that additional software is up-to-date...");

                string currentAddon = "Lyrics Creator";
                try
                {
                    player.Logger.WriteLog(await CheckAddon(player.AppSettings.InstallPath, "Lyrics Creator", "licon"));
                    currentAddon = "Visualizer Studio";
                    player.Logger.WriteLog(await CheckAddon(player.AppSettings.InstallPath, "Visualizer Studio", "vicon"));
                }
                catch (Exception ex)
                {
                    player.Logger.WriteLog("Error checking addons! Can't access one or more files because they are opened! (Failed addon: {0})", ex, currentAddon);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                player.Logger.WriteLog("Error while installing updates! Can't access one or more files because they are opened!", ex);
            }
            #endregion

            CleanupFiles(false);
            player.Logger.WriteLog("Updates have been installed!");
            UpdateAvailable = true;
            ApplyUpdate();
            player.ViewModel.UpdateTitle = Helper.FindString("patch_restartTitle");
            player.ViewModel.UpdateMessage = Helper.FindString("patch_restartMessage");
        }

        internal void CleanupFiles(bool alsoBackup)
        {
            player.Logger.WriteLogVerbose(this, "Cleaning up files... (alsoBackup: {0})", alsoBackup);
            if (alsoBackup && File.Exists(player.AppSettings.InstallPath + "cleanup.txt"))
            {
                string[] files = File.ReadAllLines(player.AppSettings.InstallPath + "cleanup.txt");
                for(int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        player.Logger.WriteLogVerbose(this, "Removing file: {0}", files[i]);
                        if (File.Exists(files[i]))
                            File.Delete(files[i]);

                    }
                    catch(Exception ex)
                    {
                        player.Logger.WriteLog("Error while removing file!", ex, files[i]);
                        player.Logger.WriteLogVerbose(this, "Unable to delete \"{0}\"!", files[i]);
                    }
                }
                File.Delete(player.AppSettings.InstallPath + "cleanup.txt");
            }

            if (File.Exists(player.AppSettings.InstallPath + "Update.zip"))
                File.Delete(player.AppSettings.InstallPath + "Update.zip");

            if (Directory.Exists(player.AppSettings.InstallPath + "Update"))
                Directory.Delete(player.AppSettings.InstallPath + "Update", true);
            player.Logger.WriteLogVerbose(this, "Files have been removed!", alsoBackup);
        }

        void CheckIfPlayerUpToDate(bool getStableBuild)
        {
            GetVersion(true, 0, out string newVersion, out bool isNewer);
            player.Logger.WriteLog("Current version: {0}; Server version: {1}", _versionLocal, newVersion);
            if (isNewer)
            {
                _versionOnline = newVersion;
                player.ViewModel.UpdateTitle = Helper.FindString("patch_downloadUpdate");
                //player.ViewModel.UpdateMessage = Helper.FindString("patch_available");
                //player.update.NotificationVisibility(null, player.ViewModel.LowMode);
                checkUpdate.Stop();
                //TODO: Implement in-app download of update package
                player.Logger.WriteLog("Updates found! Beginning download...");
                GetFileFromHTTP(FixedValues.LOCAL_FILENAME, player.AppSettings.InstallPath + FixedValues.LOCAL_FILENAME, true);
            }
            else
            {
                player.Logger.WriteLog("Everything is up-to-date!");
                player.ViewModel.UpdateTitle = Helper.FindString("patch_uptodate");
                Thread.Sleep(4500);
                player.update.NotificationVisibility(false, player.ViewModel.LowMode);
            }
        }

        void GetVersion(bool deleteFile, int vFileRow, out string versionOnline, out bool isNewer)
        {
            string[] vFile = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "version.txt");
            versionOnline = Helper.SerializeVersionNumber(vFile[vFileRow], 3);

            if (deleteFile)
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "version.txt");

            isNewer = Helper.ParseVersion(versionOnline, 0) > _versionLocalRaw;
        }
    }
}
