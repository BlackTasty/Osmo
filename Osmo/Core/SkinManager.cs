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

        public static SkinManager GetInstance()
        {
            if (instance == null)
                throw new NullReferenceException("You have to call MakeInstance(string) first!");

            return instance;
        }

        public static SkinManager MakeInstance(string directory)
        {
            if (instance == null)
                instance = new SkinManager(directory);

            return instance;
        }
        #endregion

        private FileSystemWatcher mSkinWatcher;
        private string mDirectory;

        public event EventHandler<SkinChangedEventArgs> SkinChanged;
        public event EventHandler<SkinRenamedEventArgs> SkinRenamed;
        
        public VeryObservableCollection<Skin> Skins { get; private set; } = new VeryObservableCollection<Skin>("Skins", new Skin());

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
                        mSkinWatcher.Created -= Watcher_Created;
                        mSkinWatcher.Deleted -= Watcher_Deleted;
                        mSkinWatcher.Renamed -= Watcher_Renamed;
                    }

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
            }
        }

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

        private SkinManager(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);
            Directory = directory;
            LoadSkins();
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

        private void LoadSkins()
        {
            if (!string.IsNullOrWhiteSpace(Directory))
            {
                foreach (DirectoryInfo di in new DirectoryInfo(Directory + "\\Skins").EnumerateDirectories())
                {
                    Skins.Add(new Skin(di.FullName));
                }
            }
        }
    }
}
