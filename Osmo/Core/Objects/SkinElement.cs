using Osmo.Core.Configuration;
using Osmo.Core.Logging;
using Osmo.Core.Reader;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Osmo.Core.Objects
{
    public class SkinElement : ViewModelBase
    {
        private FileType fileType;
        private string mPath;
        private bool mIsResizeSelected;
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight = FontWeights.Normal;
        string backupPath = Directory.GetParent(AppConfiguration.GetInstance().BackupDirectory).FullName +
            "\\Edited\\";

        public string Path
        {
            get => !string.IsNullOrWhiteSpace(TempPath) ? TempPath : mPath;
            set => mPath = value;
        }

        public FontStyle FontStyle
        {
            get => fontStyle;
            private set
            {
                fontStyle = value;
                FontWeight = value.Equals(FontStyles.Normal) ? FontWeights.Normal : FontWeights.Bold;
                InvokePropertyChanged("FontStyle");
            }
        }

        public FontWeight FontWeight
        {
            get => fontWeight;
            private set
            {
                fontWeight = value;
                InvokePropertyChanged("FontWeight");
                InvokePropertyChanged("HasChanges");
            }
        }

        public string TempPath { get; set; }

        public bool HasChanges { get => !string.IsNullOrWhiteSpace(TempPath); }

        public string Name { get; set; }

        public FileType FileType { get => fileType; }

        public IEntry ElementDetails { get; private set; }

        //TODO: Implement detection of HD elements (see https://osu.ppy.sh/help/wiki/Ranking_Criteria/Skin_Set_List/ for recommended sizes)
        public bool IsHighDefinition { get => Name.Contains("@2x"); }

        public bool IsEmpty { get => string.IsNullOrWhiteSpace(mPath); }

        public Size ImageSize { get; private set; }

        /// <summary>
        /// This value is currently only used in Resize Tool
        /// </summary>
        public bool IsResizeSelected {
            get => mIsResizeSelected;
            set
            {
                mIsResizeSelected = value;
                InvokePropertyChanged("IsResizeSelected");
            }
        }

        public string Extension { get; private set; }

        internal SkinElement(FileInfo fi, string skinName)
        {
            Path = fi.FullName;
            Name = fi.Name;
            Extension = fi.Extension;
            backupPath += skinName + "\\";

            fileType = GetFileType(fi.Extension);


            if (fileType == FileType.Image)
            {
                ElementDetails = FixedValues.readerInterface.FindElement(Name) ??
                    FixedValues.readerStandard.FindElement(Name) ??
                    FixedValues.readerCatch.FindElement(Name) ??
                    FixedValues.readerMania.FindElement(Name) ??
                    FixedValues.readerTaiko.FindElement(Name);

                try
                {
                    var bitmapFrame = BitmapFrame.Create(new Uri(Path, UriKind.Absolute), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    bitmapFrame.Freeze();
                    ImageSize = new Size(bitmapFrame.PixelWidth, bitmapFrame.PixelHeight);
                }
                catch (Exception ex)
                {
                    ImageSize = new Size();
                    Logger.Instance.WriteLog("Failed to retrieve image size for element {0}!", ex, Name);
                }
            }
            else if (fileType == FileType.Audio)
            {
                ElementDetails = FixedValues.readerSounds.FindElement(Name);
            }

            Directory.CreateDirectory(backupPath);
            if (File.Exists(backupPath + Name))
            {
                TempPath = backupPath + Name;
                FontStyle = FontStyles.Italic;
            }
        }

        internal SkinElement(SkinElement copyFrom)
        {
            Path = copyFrom.Path;
            Name = copyFrom.Name;
            fileType = copyFrom.FileType;
            Extension = copyFrom.Extension;
            backupPath = copyFrom.backupPath;
            ElementDetails = copyFrom.ElementDetails;
            ImageSize = copyFrom.ImageSize;
        }

        internal SkinElement()
        {
            Path = "";
            Name = "";
            fileType = FileType.Unknown;
        }

        internal List<string> GetAnimatedElements()
        {
            if (ElementDetails != null)
            {
                List<string> elementPaths = new List<string>();
                string pattern;
                if (IsHighDefinition)
                {
                    pattern = @"\b" + ElementDetails.GetRegexName() + @"\d+\b@2x\.png";
                }
                else
                {
                    pattern = @"\b" + ElementDetails.GetRegexName() + @"\d+\b\.png";
                }
                
                int addedFrames = 0;
                foreach (FileInfo fi in new DirectoryInfo(System.IO.Path.GetDirectoryName(mPath)).EnumerateFiles(ElementDetails.Name + "*"))
                {
                    if (Regex.IsMatch(fi.Name, pattern))
                    {
                        elementPaths.Add(fi.FullName);
                        addedFrames++;

                        if (ElementDetails.MaximumFrames > 0 && addedFrames == ElementDetails.MaximumFrames)
                        {
                            break;
                        }
                    }
                }

                elementPaths.Sort(new NaturalStringComparer());
                return elementPaths;
            }
            else
            {
                return null;
            }
        }

        internal void Save()
        {
            if (File.Exists(TempPath))
            {
                File.Copy(TempPath, mPath, true);
                File.Delete(TempPath);
            }
            TempPath = null;
            FontStyle = FontStyles.Normal;
        }

        /// <summary>
        /// Instead of copying the target file directly into the skin folder, 
        /// we copy the new file into a temporary directory and show the new file in frontend
        /// </summary>
        /// <param name="fi">The new file which shall be used. Providing null results in a backup of this <see cref="SkinElement"/>.</param>
        /// <returns>The path to the temporary file</returns>
        internal string ReplaceBackup(FileInfo fi)
        {
            Directory.CreateDirectory(backupPath);

            TempPath = backupPath + Name;

            if (fi != null)
                File.Copy(fi.FullName, TempPath, true);
            else
                File.Copy(mPath, TempPath, true);

            FontStyle = FontStyles.Italic;
            return TempPath;
        }

        internal void Rename(string newName)
        {
        }

        internal void Reset()
        {
            if (File.Exists(TempPath))
                File.Delete(TempPath);
            TempPath = null;
            FontStyle = FontStyles.Normal;
        }

        internal void Delete()
        {
            if (File.Exists(TempPath))
                File.Delete(TempPath);

            File.Delete(mPath);
            TempPath = null;
        }

        private FileType GetFileType(string extension)
        {
            switch (extension)
            {
                case ".ini":
                    return FileType.Configuration;
                case ".png":
                case ".jpg":
                case ".jpeg":
                    return FileType.Image;
                case ".ogg":
                case ".wav":
                case ".mp3":
                    return FileType.Audio;
                default:
                    return FileType.Unknown;
            }
        }

        #region Method and operator overrides
        public static bool operator ==(SkinElement element, string path)
        {
            if (path != null)
                return element.Path.Contains(path);
            else
                return false;
        }

        public static bool operator !=(SkinElement element, string path)
        {
            if (path != null)
                return !element.Path.Contains(path);
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj != BindingOperations.DisconnectedSource &&
                mPath != null && (obj as SkinElement).Path != null)
                return mPath.Contains((obj as SkinElement).Path);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
