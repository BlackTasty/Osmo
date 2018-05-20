using Osmo.ViewModel;
using System.IO;
using System.Windows.Data;

namespace Osmo.Core.Objects
{
    public class SkinElement : ViewModelBase
    {
        private FileType fileType;
        private string extension;
        private bool mMadeChanges;

        public bool MadeChanges {
            get => mMadeChanges;
            set
            {
                mMadeChanges = value;
                InvokePropertyChanged("MadeChanges");
            }
        }

        public string Path { get; set; }

        public string TempPath { get
            {
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Osmo");


                foreach (string fi in Directory.EnumerateFiles(tempPath, "ImageElement.*"))
                {
                    File.Delete(fi);
                }

                return System.IO.Path.Combine(tempPath, 
                    "Osmo\\ImageElement." + extension);
            }
        }

        public string Name { get; set; }

        public FileType FileType { get => fileType; }

        //TODO: Implement detection of HD elements (see https://osu.ppy.sh/help/wiki/Ranking_Criteria/Skin_Set_List/ for recommended sizes)
        public bool IsHighDefinition { get => false; }

        internal SkinElement(FileInfo fi)
        {
            Path = fi.FullName;
            Name = fi.Name;
            extension = fi.Extension;

            fileType = GetFileType(fi.Extension);
        }

        internal SkinElement(SkinElement copyFrom)
        {
            Path = copyFrom.Path;
            Name = copyFrom.Name;
            fileType = copyFrom.FileType;
            extension = copyFrom.extension;
        }

        private SkinElement()
        {
            Path = "";
            Name = "";
            fileType = FileType.Unknown;
        }

        internal static SkinElement Empty { get => new SkinElement(); }

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
                Path != null && (obj as SkinElement).Path != null)
                return Path.Contains((obj as SkinElement).Path);
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
