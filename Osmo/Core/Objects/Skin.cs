using MaterialDesignThemes.Wpf;
using Osmo.Core.Configuration;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Osmo.Core.Objects
{
    public class Skin
    {
        private FileSystemWatcher mWatcher;
        #region Properties
        /// <summary>
        /// The visible name of this <see cref="Skin"/> object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The root folder of this <see cref="Skin"/> object.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The root folder of this <see cref="Skin"/> object.
        /// </summary>
        public string Author { get; set; }

        public bool IsEmpty { get; private set; }

        public bool UnsavedChanges { get => Elements.Any(x => !string.IsNullOrWhiteSpace(x.TempPath)); }

        /// <summary>
        /// This list contains all filenames of this <see cref="Skin"/> object.
        /// </summary>
        public VeryObservableCollection<SkinElement> Elements { get; private set; } = new VeryObservableCollection<SkinElement>("Elements", true);

        /// <summary>
        /// This returns the amount of elements this <see cref="Skin"/> object contains.
        /// </summary>
        public int ElementCount { get => Elements.Count; }
        #endregion
        
        internal Skin()
        {
            IsEmpty = true;
        }

        internal Skin(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);

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
            Name = vm.Name;
            Author = vm.Author;
        }

        public void Save()
        {
            foreach (SkinElement element in Elements.Where(x=> !string.IsNullOrWhiteSpace(x.TempPath)))
            {
                element.Save();
            }
        }

        public static async Task<Skin> Import(FileInfo oskPath)
        {
            string skinPath = AppConfiguration.GetInstance().OsuDirectory + "\\Skins\\" + oskPath.Name.Replace(oskPath.Extension, "");

            MessageBoxResult result = MessageBoxResult.OK;
            if (Directory.Exists(skinPath))
            {
                var msgBox = MaterialMessageBox.Show("Skin exists already!",
                    "A skin with the name \"" + oskPath.Name + "\" exists already! Would you like to overwrite it?",
                    MessageBoxButton.OKCancel);

                await DialogHost.Show(msgBox);

                result = msgBox.Result;
                if (result == MessageBoxResult.OK)
                {
                    Directory.Delete(skinPath, true);
                }
            }

            if (result == MessageBoxResult.OK)
            {
                Directory.CreateDirectory(skinPath);
                ZipFile.ExtractToDirectory(oskPath.FullName, skinPath);

                return new Skin(skinPath);
            }
            else
            {
                return new Skin();
            }
        }

        public void Export(string targetDir)
        {
            ZipFile.CreateFromDirectory(Path, targetDir + "\\" + Name + ".osk");
        }

        public void RevertAll()
        {
            foreach (SkinElement element in Elements.Where(x => !string.IsNullOrWhiteSpace(x.TempPath)))
            {
                element.Reset();
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
            foreach (FileInfo fi in new DirectoryInfo(Path).EnumerateFiles())
            {
                Elements.Add(new SkinElement(fi, Name));
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
            int index = Elements.IndexOf(Elements.FirstOrDefault(x => x == e.OldFullPath) ?? null);

            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                if (index > -1)
                {
                    Elements[index].Name = e.Name;
                    Elements[index].Path = e.FullPath;
                }
            });
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Elements.Refresh();
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                Elements.Add(new SkinElement(new FileInfo(e.FullPath), Name));
            });
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            SkinElement element = Elements.FirstOrDefault(x => x == e.FullPath);
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                if (element != null)
                    Elements.Remove(element);
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
