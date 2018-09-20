using MaterialDesignThemes.Wpf;
using Osmo.Core.Configuration;
using Osmo.Core.Logging;
using Osmo.Core.Reader;
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
    public class Skin : ViewModelBase
    {
        private FileSystemWatcher mWatcher;
        private FileInfo skinIniHandle;

        private double mProgressOsu;
        private double mProgressOsuHD;
        private double mProgressCTB;
        private double mProgressCTBHD;
        private double mProgressTaiko;
        private double mProgressTaikoHD;
        private double mProgressMania;
        private double mProgressManiaHD;
        private double mProgressInterface;
        private double mProgressInterfaceHD;
        private double mProgressSounds;
        private double mProgressSoundsHD;

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

        public string Version
        {
            get
            {
                return GetSkinIniProperty("Version")?.Trim() ?? "1.0";
            }
        }

        public double ProgressOsu
        {
            get => mProgressOsu;
            private set
            {
                mProgressOsu = value;
                InvokePropertyChanged("ProgressOsu");
            }
        }

        public double ProgressOsuHD
        {
            get => mProgressOsuHD;
            private set
            {
                mProgressOsuHD = value;
                InvokePropertyChanged("ProgressOsuHD");
            }
        }

        public double ProgressMania
        {
            get => mProgressMania;
            private set
            {
                mProgressMania = value;
                InvokePropertyChanged("ProgressMania");
            }
        }

        public double ProgressManiaHD
        {
            get => mProgressManiaHD;
            private set
            {
                mProgressManiaHD = value;
                InvokePropertyChanged("ProgressManiaHD");
            }
        }

        public double ProgressCTB
        {
            get => mProgressCTB;
            private set
            {
                mProgressCTB = value;
                InvokePropertyChanged("ProgressCTB");
            }
        }

        public double ProgressCTBHD
        {
            get => mProgressCTBHD;
            private set
            {
                mProgressCTBHD = value;
                InvokePropertyChanged("ProgressCTBHD");
            }
        }

        public double ProgressTaiko
        {
            get => mProgressTaiko;
            private set
            {
                mProgressTaiko = value;
                InvokePropertyChanged("ProgressTaiko");
            }
        }

        public double ProgressTaikoHD
        {
            get => mProgressTaikoHD;
            private set
            {
                mProgressTaikoHD = value;
                InvokePropertyChanged("ProgressTaikoHD");
            }
        }

        public double ProgressInterface
        {
            get => mProgressInterface;
            private set
            {
                mProgressInterface = value;
                InvokePropertyChanged("ProgressInterface");
            }
        }

        public double ProgressInterfaceHD
        {
            get => mProgressInterfaceHD;
            private set
            {
                mProgressInterfaceHD = value;
                InvokePropertyChanged("ProgressInterfaceHD");
            }
        }

        public double ProgressSounds
        {
            get => mProgressSounds;
            private set
            {
                mProgressSounds = value;
                InvokePropertyChanged("ProgressSounds");
            }
        }
        #endregion

        private double CountFilesOfType(ElementType type, double targetElementCount, bool countHDElements)
        {
            double elementCount;

            if (countHDElements)
            {
                elementCount = Elements.Count(x => LinqCount(x, type, countHDElements));
            }
            else
            {
                elementCount = Elements.Count(x => LinqCount(x, type, countHDElements));
            }

            if (elementCount > 0)
            {
                double progress = Math.Round((elementCount / targetElementCount) * 100);
                return progress > 100 ? 100 : progress;
            }
            else
            {
                return 0;
            }
        }

        private bool LinqCount(SkinElement element, ElementType type, bool countHDElements)
        {
            
            if (element.ElementDetails?.ElementType == type && 
                (element.ElementDetails as ElementReader).VersionMatches(Version))
            {
                if (element.ElementDetails.MultipleElementsAllowed)
                {
                    string elementName = element.Name.Replace(element.Extension, "");
                    if (elementName.Equals(element.ElementDetails.Name))
                    {
                        return element.IsHighDefinition == countHDElements;
                    }
                    else
                    {
                        if (countHDElements && elementName.EndsWith("-0@2x"))
                        {
                            return true;
                        }
                        else if (!countHDElements && elementName.EndsWith("-0"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return element.IsHighDefinition == countHDElements;
                }
            }
            else
            {
                return false;
            }
        }

        private void RefreshProgressValues()
        {
            ProgressOsu = CountFilesOfType(ElementType.Osu, FixedValues.readerStandard.CountFiles(Version), false);
            ProgressOsuHD = CountFilesOfType(ElementType.Osu, FixedValues.readerStandard.CountFiles(Version), true);
            ProgressMania = CountFilesOfType(ElementType.Mania, FixedValues.readerMania.CountFiles(Version), false);
            ProgressManiaHD = CountFilesOfType(ElementType.Mania, FixedValues.readerMania.CountFiles(Version), true);
            ProgressTaiko = CountFilesOfType(ElementType.Taiko, FixedValues.readerTaiko.CountFiles(Version), false);
            ProgressTaikoHD = CountFilesOfType(ElementType.Taiko, FixedValues.readerTaiko.CountFiles(Version), true);
            ProgressCTB = CountFilesOfType(ElementType.CTB, FixedValues.readerCatch.CountFiles(Version), false);
            ProgressCTBHD = CountFilesOfType(ElementType.CTB, FixedValues.readerCatch.CountFiles(Version), true);
            ProgressInterface = CountFilesOfType(ElementType.Interface, FixedValues.readerInterface.CountFiles(Version), false);
            ProgressInterfaceHD = CountFilesOfType(ElementType.Interface, FixedValues.readerInterface.CountFiles(Version), true);
            ProgressSounds = CountFilesOfType(ElementType.Sound, FixedValues.readerSounds.CountFiles(Version), false);
        }

        internal Skin()
        {
            IsEmpty = true;
        }

        internal Skin(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            Logger.Instance.WriteLog("Loading skin: {0}", Name);

            mWatcher = new FileSystemWatcher(path, "*.*")
            {
                EnableRaisingEvents = true
            };

            mWatcher.Changed += Watcher_Changed;
            mWatcher.Renamed += Watcher_Renamed;
            mWatcher.Deleted += Watcher_Deleted;
            mWatcher.Created += Watcher_Created;

            ReadElements();
            Logger.Instance.WriteLog("Skin \"{0}\" loaded!", Name);
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
            Logger.Instance.WriteLog("Skin \"{0}\" saved!", Name);
        }

        public static async Task<Skin> Import(FileInfo oskPath)
        {
            string skinPath = AppConfiguration.Instance.OsuDirectory + "\\Skins\\" + oskPath.Name.Replace(oskPath.Extension, "");

            OsmoMessageBoxResult result = OsmoMessageBoxResult.OK;
            if (Directory.Exists(skinPath))
            {
                var msgBox = MaterialMessageBox.Show(Helper.FindString("skin_importTitle"),
                    Helper.FindString("skin_importDescription1") + oskPath.Name + Helper.FindString("skin_importDescription1"),
                    OsmoMessageBoxButton.OKCancel);

                await DialogHelper.Instance.ShowDialog(msgBox);

                result = msgBox.Result;
                if (result == OsmoMessageBoxResult.OK)
                {
                    Directory.Delete(skinPath, true);
                }
            }

            if (result == OsmoMessageBoxResult.OK)
            {
                Logger.Instance.WriteLog("Started importing skin...");
                Directory.CreateDirectory(skinPath);
                ZipFile.ExtractToDirectory(oskPath.FullName, skinPath);
                Logger.Instance.WriteLog("Skin Import successful!");

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
            Logger.Instance.WriteLog("Skin \"{0}\" has been exported!", Name);
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
            Logger.Instance.WriteLog("Skin \"{0}\" has been deleted!");
        }

        private void ReadElements()
        {
            foreach (FileInfo fi in new DirectoryInfo(Path).EnumerateFiles())
            {
                Elements.Add(new SkinElement(fi, Name));
                if (fi.Name.Equals("skin.ini", StringComparison.InvariantCultureIgnoreCase))
                {
                    skinIniHandle = fi;
                    Author = GetSkinIniProperty("Author");
                }
            }

            RefreshProgressValues();
        }

        private string GetSkinIniProperty(string propertyName)
        {
            string[] content = File.ReadAllLines(skinIniHandle.FullName);
            string propertyLine = content.FirstOrDefault(x => x.StartsWith(propertyName + ":",
                    StringComparison.InvariantCultureIgnoreCase));
            return propertyLine?.Trim().Substring(propertyLine.IndexOf(':') + 1);
        }

        #region Watcher Events
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            int index = Elements.IndexOf(Elements.FirstOrDefault(x => x == e.OldFullPath) ?? null);

            Application.Current.Dispatcher.Invoke(delegate
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
            Application.Current.Dispatcher.Invoke(delegate
            {
                Elements.Add(new SkinElement(new FileInfo(e.FullPath), Name));

                RefreshProgressValues();
            });
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            SkinElement element = Elements.FirstOrDefault(x => x == e.FullPath);
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (element != null)
                    Elements.Remove(element);
            });
        }
        #endregion

        #region Method and operator overrides
        //public static bool operator ==(Skin skin, string path)
        //{
        //    if (path != null)
        //        return skin.Path.Contains(path);
        //    else
        //        return false;
        //}

        //public static bool operator !=(Skin skin, string path)
        //{
        //    if (path != null)
        //        return !skin.Path.Contains(path);
        //    else
        //        return true;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj != null && obj != BindingOperations.DisconnectedSource && 
        //        Path != null && (obj as Skin).Path != null)
        //        return Path.Contains((obj as Skin).Path);
        //    else
        //        return false;
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        public override string ToString()
        {
            return Path;
        }
        #endregion
    }
}
