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
        private FileSystemWatcher mSkinWatcher;
        private string mDirectory;

        public event EventHandler<SkinChangedEventArgs> SkinChanged;
        public event EventHandler<SkinRenamedEventArgs> SkinRenamed;

        private VeryObservableCollection<Skin> mSkins = new VeryObservableCollection<Skin>("Skins");

        internal string Directory
        {
            get => mDirectory;
            set
            {
                mDirectory = value;
                if (mSkinWatcher == null) //Create a new FileSystemWatcher and register events
                {
                    if (mSkinWatcher != null)
                    {
                        mSkinWatcher.Changed -= Watcher_Changed;
                        mSkinWatcher.Renamed -= Watcher_Renamed;
                    }

                    //TODO: Filter may be reset to *.* in case the menu background isn't needed. Also remove IncludeSubDirectories!
                    mSkinWatcher = new FileSystemWatcher(value, "menu-background*.jpg")
                    {
                        EnableRaisingEvents = true
                        //IncludeSubdirectories = true
                    };
                    mSkinWatcher.Changed += Watcher_Changed;
                    mSkinWatcher.Renamed += Watcher_Renamed;
                }
                else
                    mSkinWatcher.Path = value;
            }
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            OnSkinRenamed(e.OldFullPath, e.FullPath);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Renamed)
                OnSkinChanged(e.FullPath, e.ChangeType);
        }

        protected virtual void OnSkinChanged(string path, WatcherChangeTypes changeType)
        {
            SkinChanged?.Invoke(this, new SkinChangedEventArgs(path, changeType));
        }

        protected virtual void OnSkinRenamed(string pathBefore, string pathAfter)
        {
            SkinRenamed?.Invoke(this, new SkinRenamedEventArgs(pathBefore, pathAfter));
        }

        public SkinManager(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            Directory = directory;
        }
    }
}
