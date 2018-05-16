using System.IO;
using System.Linq;

namespace Osmo.Core.Objects
{
    class Skin
    {
        private string mName;
        private string mAuthor;
        private string mPath;

        //This contains a list of file names inside the skin folder
        private VeryObservableCollection<SkinElement> mElements = new VeryObservableCollection<SkinElement>("Elements");

        private FileSystemWatcher mWatcher;

        #region Properties
        /// <summary>
        /// The visible name of this <see cref="Skin"/> object.
        /// </summary>
        public string Name { get => mName; set => mName = value; }

        /// <summary>
        /// The root folder of this <see cref="Skin"/> object.
        /// </summary>
        public string Path { get => mPath; set => mPath = value; }

        /// <summary>
        /// The root folder of this <see cref="Skin"/> object.
        /// </summary>
        public string Author { get => mAuthor; set => mAuthor = value; }

        /// <summary>
        /// This list contains all filenames of this <see cref="Skin"/> object.
        /// </summary>
        public VeryObservableCollection<SkinElement> Elements { get => mElements; set => mElements = value; }

        /// <summary>
        /// This returns the amount of elements this <see cref="Skin"/> object contains.
        /// </summary>
        public int ElementCount { get => mElements.Count; }
        #endregion

        public Skin(string path)
        {
            mPath = path;
            mName = System.IO.Path.GetFileName(path);

            mWatcher = new FileSystemWatcher(path, "*.*")
            {
                EnableRaisingEvents = true
            };

            mWatcher.Changed += Watcher_Changed;
            mWatcher.Renamed += Watcher_Renamed;

            ReadElements();
        }

        /// <summary>
        /// Creates a backup 
        /// </summary>
        /// <param name="backupFolder">The destination where the backup should be created.</param>
        /// <param name="overrideBackup">If true and an old backup is found, it is overridden.</param>
        /// <returns>Returns true if the backup has been created.</returns>
        public bool BackupSkin(string backupFolder, bool overrideBackup)
        {
            string skinBackupPath = System.IO.Path.Combine(backupFolder, Name);
            if (Directory.Exists(skinBackupPath))
            {
                if (overrideBackup)
                    Directory.Delete(skinBackupPath);
                else
                    return false;
            }

            Directory.CreateDirectory(skinBackupPath);
            foreach (FileInfo fi in new DirectoryInfo(Path).EnumerateFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(skinBackupPath, fi.Name));
            }

            return true;
        }

        private void ReadElements()
        {
            foreach (FileInfo fi in new DirectoryInfo(mPath).EnumerateFiles())
            {
                mElements.Add(new SkinElement(fi));
            }
        }

        #region Watcher Events
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            int index = mElements.IndexOf(mElements.FirstOrDefault(x => x == e.OldFullPath) ?? SkinElement.Empty);

            if (index > -1)
            {
                mElements[index].Name = e.Name;
                mElements[index].Path = e.FullPath;
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    mElements.Refresh();
                    break;
                case WatcherChangeTypes.Created:
                    mElements.Add(new SkinElement(new FileInfo(e.FullPath)));
                    break;
                case WatcherChangeTypes.Deleted:
                    mElements.Remove(mElements.FirstOrDefault(x => x == e.FullPath) ?? SkinElement.Empty);
                    break;
            }
        }
        #endregion

        #region Method and operator overrides
        public static bool operator ==(Skin skin, string path)
        {
            if (path != null)
                return skin.Path.Contains(path);
            else
                return false;
        }

        public static bool operator !=(Skin skin, string path)
        {
            if (path != null)
                return !skin.Path.Contains(path);
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && Path != null && (obj as Skin).Path != null)
                return Path.Contains((obj as Skin).Path);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
