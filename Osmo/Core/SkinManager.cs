using Osmo.Core.Configuration;
using Osmo.Core.Logging;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core
{
    public class SkinManager
    {
        #region Singleton implementation
        private static SkinManager instance;

        public static SkinManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkinManager(AppConfiguration.GetInstance().OsuDirectory);
                }

                return instance;
            }
        }
        #endregion

        private FileSystemWatcher mSkinWatcher;
        private DirectoryInfo mDirectory;

        public event EventHandler<SkinChangedEventArgs> SkinChanged;
        public event EventHandler<SkinRenamedEventArgs> SkinRenamed;
        public event EventHandler<EventArgs> SkinDirectoryChanged;
        
        public VeryObservableCollection<Skin> Skins { get; private set; } = new VeryObservableCollection<Skin>("Skins", new Skin());

        internal string SkinDirectory
        {
            get
            {
                if (mDirectory != null && !string.IsNullOrWhiteSpace(mDirectory.FullName))
                {
                    if (mDirectory.Parent.Name.Equals("osu!"))
                    {
                        return mDirectory.FullName;
                    }
                    else
                    {
                        string predictedPath = mDirectory.FullName + "\\Skins";
                        if (Directory.Exists(predictedPath))
                        {
                            return predictedPath;
                        }
                        else
                        {
                            return mDirectory.FullName;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (mSkinWatcher != null)
                {
                    mSkinWatcher.Changed -= Watcher_Changed;
                    mSkinWatcher.Created -= Watcher_Created;
                    mSkinWatcher.Deleted -= Watcher_Deleted;
                    mSkinWatcher.Renamed -= Watcher_Renamed;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    string oldValue = SkinDirectory;
                    mDirectory = new DirectoryInfo(value);

                    if (mSkinWatcher == null) //Create a new FileSystemWatcher and register events
                    {

                        //TODO: Filter may be reset to *.* in case the menu background isn't needed. Also remove IncludeSubDirectories!
                        mSkinWatcher = new FileSystemWatcher(value)
                        {
                            EnableRaisingEvents = true
                            //IncludeSubdirectories = true
                        };
                        mSkinWatcher.Changed += Watcher_Changed;
                        mSkinWatcher.Created += Watcher_Created;
                        mSkinWatcher.Deleted += Watcher_Deleted;
                        mSkinWatcher.Renamed += Watcher_Renamed;
                    }
                    else
                        mSkinWatcher.Path = value;

                    if (oldValue != value)
                    {
                        LoadSkins();
                        OnSkinDirectoryChanged();
                    }
                }
            }
        }

        public bool IsValid { get => !string.IsNullOrWhiteSpace(SkinDirectory); }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            OnSkinChanged(e.FullPath, WatcherChangeTypes.Deleted);
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            OnSkinChanged(e.FullPath, WatcherChangeTypes.Created);
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            OnSkinRenamed(e.OldFullPath, e.FullPath);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            OnSkinChanged(e.FullPath, WatcherChangeTypes.Changed);
        }

        protected virtual void OnSkinChanged(string path, WatcherChangeTypes changeType)
        {
            SkinChanged?.Invoke(this, new SkinChangedEventArgs(path, changeType));
        }

        protected virtual void OnSkinRenamed(string pathBefore, string pathAfter)
        {
            SkinRenamed?.Invoke(this, new SkinRenamedEventArgs(pathBefore, pathAfter));
        }

        protected virtual void OnSkinDirectoryChanged()
        {
            SkinDirectoryChanged?.Invoke(this, EventArgs.Empty);
        }

        private SkinManager(string directory)
        {
            Logger.Instance.WriteLog("Inititalizing Skin Manager...");
            if (!string.IsNullOrWhiteSpace(directory))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                SkinDirectory = directory;
                LoadSkins();
                Logger.Instance.WriteLog("Skin Manager initialized!");
            }
            else
            {
                Logger.Instance.WriteLog("Skin Manager uninitialized! (the skin directory is not configured yet!)");
            }
        }

        public void DeleteSkin(string name)
        {
            Skin target = Skins.FirstOrDefault(x => !x.IsEmpty && x.Name.Equals(name));

            if (target != null)
            {
                Skins.Remove(target);
                target.Delete();
            }
        }

        public void ExportSkin(string name, string targetDir)
        {
            Skins.FirstOrDefault(x => !x.IsEmpty && x.Name.Equals(name))?.Export(targetDir);
        }

        private void LoadSkins()
        {
            Skins.Clear();
            if (!string.IsNullOrWhiteSpace(SkinDirectory))
            {
                foreach (DirectoryInfo di in new DirectoryInfo(SkinDirectory).EnumerateDirectories())
                {
                    Skins.Add(new Skin(di.FullName));
                }
            }
            Logger.Instance.WriteLog("{0} skins have been loaded!", Skins.Count);
        }
    }
}
