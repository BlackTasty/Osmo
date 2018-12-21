using Osmo.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Osmo.Core.Patcher
{
    class UpdateManager
    {
        private DispatcherTimer checkInterval = new DispatcherTimer();
        private Stopwatch downloadSpeedStopWatch = new Stopwatch();
        private string currentVersion;
        private int currentVersionNumber;
        private string newVersion;

        private bool updatesReady;
        private UpdateStatus status = UpdateStatus.INIT;

        private List<Server> servers = ServerList.DEFAULT_SERVERS;

        public event EventHandler<bool> SearchStatusChanged;
        public event EventHandler<UpdateFoundEventArgs> UpdateFound;
        public event EventHandler<UpdateStatus> StatusChanged;
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public event EventHandler<UpdateFailedEventArgs> UpdateFailed;

        private static readonly string tempDownloadPath = Path.Combine(Path.GetTempPath(), "Osmo\\");
        private static readonly string versionFilePath = Path.Combine(tempDownloadPath, "version.txt");
        private static readonly string installPath = AppDomain.CurrentDomain.BaseDirectory + "\\";

        internal string Version
        {
            get => currentVersion;
            set
            {
                currentVersion = value;
                currentVersionNumber = Helper.ParseVersion(value, 0);
            }
        }

        internal bool UpdatesReady
        {
            get => updatesReady;
        }

        internal UpdateStatus Status
        {
            get => status;
            private set
            {
                status = value;
                OnStatusChanged(value);
            }
        }

        internal UpdateManager()
        {
            if (File.Exists(versionFilePath))
                File.Delete(versionFilePath);

            Version = Helper.SerializeVersionNumber(Assembly.GetExecutingAssembly().GetName().Version.ToString(), 3);
            checkInterval.Tick += CheckForUpdates;
            checkInterval.Interval = TimeSpan.FromMinutes(15);
            checkInterval.IsEnabled = true;
            checkInterval.Start();
            Status = UpdateStatus.IDLE;
        }

        internal void SetIntervall(double interval)
        {
            checkInterval.Stop();
            checkInterval.Interval = TimeSpan.FromMinutes(interval);
            checkInterval.Start();
        }

        private void CheckForUpdates(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        /// Starts the update process on a separate thread
        /// </summary>
        public void CheckForUpdates()
        {
            OnSearchStatusChanged(true);
            Thread updater = new Thread(SearchForUpdates);
            updater.Start();
        }

        /// <summary>
        /// Initializes the update process. Do not call this method directly on the UI thread or your
        /// interface may freeze for the duration of this process!
        /// </summary>
        private void SearchForUpdates()
        {
            Status = UpdateStatus.SEARCHING;
            try
            {
                if (!UpdatesReady)
                {
                    Logger.Instance.WriteLog("Searching for updates...");
                    if (File.Exists(versionFilePath))
                    {
                        File.Delete(versionFilePath);
                    }
                    DownloadFile("version.txt", versionFilePath, false);
                    CheckAppUpdates();
                }
                else
                {
                    DownloadUpdate();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLog("Can't connect to the server. Either your connection is too slow or the server is currently offline.", ex);
                OnUpdateFailed(new UpdateFailedEventArgs(Helper.FindString("update_searchFailed")));
            }
            finally
            {
                if (File.Exists(versionFilePath))
                {
                    File.Delete(versionFilePath);
                }
            }
            OnSearchStatusChanged(false);
        }

        public void DownloadUpdate()
        {
            Status = UpdateStatus.DOWNLOADING;
            checkInterval.Stop();
            Logger.Instance.WriteLog("Started download of Osmo v{0}...", newVersion);

            DownloadFile(FixedValues.LOCAL_FILENAME, tempDownloadPath + "\\" + FixedValues.LOCAL_FILENAME, true);
        }

        /// <summary>
        /// Checks if Osmo is up-to-date
        /// </summary>
        private void CheckAppUpdates()
        {
            updatesReady = CheckVersion(true, 0);
            Logger.Instance.WriteLog("Current version: {0}; Server version: {1}", currentVersion, newVersion);
            if (updatesReady)
            {
                Logger.Instance.WriteLog("Updates found!");
                Status = UpdateStatus.UPDATES_FOUND;
                OnUpdateFound(new UpdateFoundEventArgs(currentVersion, newVersion));
            }
            else
            {
                Status = UpdateStatus.IDLE;
            }
        }

        private void InstallUpdate()
        {
            Status = UpdateStatus.EXTRACTING;

            try
            {
                if (File.Exists(versionFilePath))
                {
                    File.Delete(versionFilePath);
                }

                string runtimePath = tempDownloadPath + "\\" + FixedValues.LOCAL_FILENAME;

                ZipFile.ExtractToDirectory(runtimePath, tempDownloadPath);
                Thread.Sleep(200);
                File.Delete(runtimePath);
                Status = UpdateStatus.INSTALLING;

                List<string> backupFiles = new List<string>();
                DirectoryInfo root = new DirectoryInfo(tempDownloadPath);
                foreach (DirectoryInfo di in root.EnumerateDirectories())
                {
                    backupFiles.AddRange(Util.MoveDirectory(di.FullName, installPath + di.Name));
                }

                foreach (FileInfo fi in root.EnumerateFiles())
                {
                    backupFiles.Add(Util.MoveFile(fi.FullName, installPath + fi.Name));
                }

                File.WriteAllLines(installPath + "cleanup.txt", backupFiles.ToArray());

                Status = UpdateStatus.READY;
            }
            catch (UnauthorizedAccessException ex)
            {
                OnUpdateFailed(new UpdateFailedEventArgs("Installation failed! Reason: One or more files are locked!"));
                Logger.Instance.WriteLog("There was an error during the installation of an update! Can't access one or more files because they are opened!", ex);
            }
            catch (Exception ex)
            {
                OnUpdateFailed(new UpdateFailedEventArgs("Something went wrong during installation!"));
                Logger.Instance.WriteLog("There was an error during the installation of an update!", ex);
            }
        }

        internal void CleanupFiles(bool alsoBackup)
        {
            Logger.Instance.WriteLogVerbose(this, "Cleaning up files... (alsoBackup: {0})", alsoBackup);
            if (alsoBackup && File.Exists(installPath + "cleanup.txt"))
            {
                string[] files = File.ReadAllLines(installPath + "cleanup.txt");
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        Logger.Instance.WriteLogVerbose(this, "Removing file: {0}", files[i]);
                        if (File.Exists(files[i]))
                            File.Delete(files[i]);

                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.WriteLog("Error while removing file!", ex, files[i]);
                        Logger.Instance.WriteLogVerbose(this, "Unable to delete \"{0}\"!", files[i]);
                    }
                }
                File.Delete(installPath + "cleanup.txt");
            }
            
            if (Directory.Exists(tempDownloadPath))
                Directory.Delete(tempDownloadPath, true);
            Logger.Instance.WriteLogVerbose(this, "Files have been removed!", alsoBackup);
        }

        /// <summary>
        /// Checks if an update is available
        /// </summary>
        /// <param name="deleteFile">Determines if the version.txt is deleted after use</param>
        /// <param name="vFileRow">The row inside the version.txt where the patch number is expected</param>
        /// <returns>Returns true if a new version is available, else false</returns>
        bool CheckVersion(bool deleteFile, int vFileRow)
        {
            string[] vFile = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "version.txt");
            newVersion = Helper.SerializeVersionNumber(vFile[vFileRow], 3);

            if (deleteFile)
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "version.txt");

            return Helper.ParseVersion(newVersion, 0) > currentVersionNumber;
        }

        /// <summary>
        /// Try to download a file from any of the available servers and save it to a specific directory
        /// </summary>
        /// <param name="fileName">The name of the file to download</param>
        /// <param name="targetDir">The target directory where your file shall be saved</param>
        /// <param name="notifyProgress">True to enable progress updates (Via <see cref="DownloadProgressChanged"/>)</param>
        private void DownloadFile(string fileName, string targetDir, bool notifyProgress)
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
                if (File.Exists(targetDir))
                {
                    File.Delete(targetDir);
                }

                using (WebClient wc = new WebClient())
                {
                    if (notifyProgress)
                    {
                        wc.DownloadProgressChanged += DownloadChanged;
                        wc.DownloadFileCompleted += DownloadFileCompleted;
                        downloadSpeedStopWatch.Start();
                        wc.DownloadFileAsync(new Uri(targetServer.URL + fileName), targetDir);
                    }
                    else
                        wc.DownloadFile(new Uri(targetServer.URL + fileName), targetDir);
                }
            }
            else
            {
                throw new Exception("No server is available right now!");
            }
        }

        /// <summary>
        /// Returns the download speed in kb/s or Mb/s (if fast enough)
        /// </summary>
        /// <param name="bytesReceived">The number of bytes received in a second</param>
        /// <returns>A formatted string which shows how fast a download is progressing</returns>
        public string CalculateSpeed(long bytesReceived)
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

        private void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            downloadSpeedStopWatch.Reset();
        }

        private void DownloadChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        protected void OnSearchStatusChanged(bool isSearching)
        {
            SearchStatusChanged?.Invoke(this, isSearching);
        }

        protected void OnUpdateFailed(UpdateFailedEventArgs e)
        {
            Status = UpdateStatus.ERROR;
            UpdateFailed?.Invoke(this, e);
        }

        protected void OnUpdateFound(UpdateFoundEventArgs e)
        {
            UpdateFound?.Invoke(this, e);
        }

        protected void OnStatusChanged(UpdateStatus status)
        {
            StatusChanged?.Invoke(this, status);
        }
    }
}
