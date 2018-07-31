using Osmo.Core.Configuration;
using Osmo.Core.Logging;
using Osmo.Core.Objects;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osmo.Core
{
    public class SkinManager : ViewModelBase
    {
        private int mSkinLoadCurrent;
        private int mSkinLoadMaximum;
        private string mSkinNameCurrent;
        private bool mIsLoadingSkins;

        //This is separate from the mIsLoadingSkins field. The reason is that mIsLoadingSkins controls the visibility of the loading bar
        private bool loadSkinFlag;

        #region Singleton implementation
        private static SkinManager instance;

        public static SkinManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkinManager(AppConfiguration.Instance.OsuDirectory);
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

        public int SkinLoadCurrent
        {
            get => mSkinLoadCurrent;
            set
            {
                mSkinLoadCurrent = value;
                InvokePropertyChanged("SkinLoadCurrent");
                InvokePropertyChanged("SkinLoadStatusText");
            }
        }

        public int SkinLoadMaximum
        {
            get => mSkinLoadMaximum;
            set
            {
                mSkinLoadMaximum = value;
                InvokePropertyChanged("SkinLoadMaximum");
                InvokePropertyChanged("SkinLoadStatusText");
            }
        }

        public string SkinLoadStatusText
        {
            get => string.Format("({1}/{2}) {0}", mSkinNameCurrent, SkinLoadCurrent, SkinLoadMaximum);
        }

        public string SkinNameCurrent
        {
            get => mSkinNameCurrent;
            set
            {
                mSkinNameCurrent = value;
                InvokePropertyChanged("SkinNameCurrent");
                InvokePropertyChanged("SkinLoadStatusText");
            }
        }

        public bool IsLoadingSkins
        {
            get => mIsLoadingSkins;
            set
            {
                mIsLoadingSkins = value;
                InvokePropertyChanged("IsLoadingSkins");
            }
        }

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

        /// <summary>
        /// This is called if either the skin directory changes or a skin is removed
        /// </summary>
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
                Logger.Instance.WriteLog("Skin Manager not initialized! (the skin directory is not configured yet!)", LogType.WARNING);
            }
        }

        public void DeleteSkin(string name)
        {
            Skin target = Skins.FirstOrDefault(x => !x.IsEmpty && x.Name.Equals(name));

            if (target != null)
            {
                Skins.Remove(target);
                target.Delete();
                OnSkinDirectoryChanged();
            }
        }

        public void ExportSkin(string name, string targetDir)
        {
            Skins.FirstOrDefault(x => !x.IsEmpty && x.Name.Equals(name))?.Export(targetDir);
        }

        private void LoadSkins()
        {
            if (!loadSkinFlag)
            {
                loadSkinFlag = true;
                if (Skins.Count > 0)
                {
                    Skins.Clear();
                }

                Skins.Add(new Skin());
                RecallConfiguration recall = RecallConfiguration.Instance;
                bool reopenLastSkin = AppConfiguration.Instance.ReopenLastSkin;
                new Thread(() =>
                {
                    if (!string.IsNullOrWhiteSpace(SkinDirectory))
                    {
                        IsLoadingSkins = true;
                        SkinLoadMaximum = new DirectoryInfo(SkinDirectory).GetDirectories().Count();
                        foreach (DirectoryInfo di in new DirectoryInfo(SkinDirectory).EnumerateDirectories())
                        {
                            mSkinNameCurrent = di.Name;
                            SkinLoadCurrent++;
                            Skin skin = new Skin(di.FullName);

                            App.Current.Dispatcher.Invoke(async () =>
                            {
                                Skins.Add(skin);
                                OnSkinDirectoryChanged();
                                
                                if (reopenLastSkin)
                                {
                                    if (skin.Path.Equals(recall.LastSkinPathEditor))
                                    {
                                        await SkinEditor.Instance.LoadSkin(skin);
                                    }

                                    if (skin.Path.Equals(recall.LastSkinPathMixerLeft))
                                    {
                                        await SkinMixer.Instance.LoadSkin(skin, true);
                                    }

                                    if (skin.Path.Equals(recall.LastSkinPathMixerRight))
                                    {
                                        await SkinMixer.Instance.LoadSkin(skin, false);
                                    }
                                }
                            });
                        }
                        IsLoadingSkins = false;
                        loadSkinFlag = false;
                        Logger.Instance.WriteLog("{0} skins have been loaded!", Skins.Count);
                    }
                }).Start();
            }
        }
    }
}
