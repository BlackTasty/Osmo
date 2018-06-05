using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Osmo.Core.Objects
{
    public class Skin
    {
        private bool mIsEmpty;

        private string mName;
        private string mAuthor;
        private string mPath;

        //This contains a list of file names inside the skin folder
        private VeryObservableCollection<SkinElement> mElements = new VeryObservableCollection<SkinElement>("Elements", true);

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

        public bool IsEmpty { get => mIsEmpty; }

        /// <summary>
        /// This list contains all filenames of this <see cref="Skin"/> object.
        /// </summary>
        public VeryObservableCollection<SkinElement> Elements { get => mElements; set => mElements = value; }

        /// <summary>
        /// This returns the amount of elements this <see cref="Skin"/> object contains.
        /// </summary>
        public int ElementCount { get => mElements.Count; }
        #endregion
        
        internal Skin()
        {
            mIsEmpty = true;
        }

        internal Skin(string path)
        {
            mPath = path;
            mName = System.IO.Path.GetFileName(path);

            mWatcher = new FileSystemWatcher(path, "*.*")
            {
                EnableRaisingEvents = true
            };

            mWatcher.Changed += Watcher_Changed;
            mWatcher.Renamed += Watcher_Renamed;
            mWatcher.Deleted += Watcher_Deleted;
            mWatcher.Created += Watcher_Created;

            ReadElements();
        }

        internal Skin(NewSkinViewModel vm)
        {
            mName = vm.Name;
            mAuthor = vm.Author;
        }

        public void Save()
        {
            foreach (SkinElement element in mElements.Where(x=> !string.IsNullOrWhiteSpace(x.TempPath)))
            {
                element.Save();
            }
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
                    Directory.Delete(skinBackupPath, true);
                else
                    return false;
            }

            Directory.CreateDirectory(skinBackupPath);
            foreach (FileInfo fi in new DirectoryInfo(Path).EnumerateFiles())
            {
                File.Copy(fi.FullName, System.IO.Path.Combine(skinBackupPath, fi.Name));
            }

            return true;
        }

        public void Delete()
        {
            mWatcher.Dispose();
            Directory.Delete(Path, true);
        }

        private void ReadElements()
        {
            foreach (FileInfo fi in new DirectoryInfo(mPath).EnumerateFiles())
            {
                mElements.Add(new SkinElement(fi, Name));
                if (fi.Name.Equals("skin.ini", StringComparison.InvariantCultureIgnoreCase))
                {
                    GetAuthor(fi.FullName);
                }
            }
        }

        private void GetAuthor(string skinIniPath)
        {
            string[] content = File.ReadAllLines(skinIniPath);
            string authorLine = content.FirstOrDefault(x => x.StartsWith("Author:",
                    StringComparison.InvariantCultureIgnoreCase));
            Author = authorLine?.Trim().Substring(authorLine.IndexOf(':') + 1);
        }

        #region Watcher Events
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            int index = mElements.IndexOf(mElements.FirstOrDefault(x => x == e.OldFullPath) ?? null);

            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                if (index > -1)
                {
                    mElements[index].Name = e.Name;
                    mElements[index].Path = e.FullPath;
                }
            });
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            mElements.Refresh();
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                mElements.Add(new SkinElement(new FileInfo(e.FullPath), Name));
            });
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            SkinElement element = mElements.FirstOrDefault(x => x == e.FullPath);
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                if (element != null)
                    mElements.Remove(element);
            });
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
            if (obj != null && obj != BindingOperations.DisconnectedSource && 
                Path != null && (obj as Skin).Path != null)
                return Path.Contains((obj as Skin).Path);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Path;
        }
        #endregion
    }
}
