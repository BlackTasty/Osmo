using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    public class FileEntry : IFilePickerEntry
    {
        private FileInfo fi;

        public string Path
        {
            get => fi.FullName;
        }

        public string Name
        {
            get => fi.Name;
        }

        public bool IsFile { get => true; }

        public string Extension
        {
            get => fi.Extension;
        }

        public PackIconKind Icon
        {
            get;private set;
        }

        public FileEntry(FileInfo fi)
        {
            this.fi = fi;

            switch (fi.Extension.ToLower())
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                    Icon = PackIconKind.FileImage;
                    break;
                case ".wav":
                case ".ogg":
                case ".mp3":
                    Icon = PackIconKind.FileMusic;
                    break;
                case ".doc":
                case ".dot":
                case ".wbk":
                case "docx":
                case ".docm":
                case ".docx":
                case ".dotm":
                case ".docb":
                    Icon = PackIconKind.FileWord;
                    break;
                case ".xls":
                case ".xlt":
                case ".xlm":
                case ".xlsx":
                case ".xlsm":
                case ".xltx":
                case ".xltm":
                case ".xlsb":
                case ".xla":
                case ".xlam":
                case ".xll":
                case ".xtw":
                    Icon = PackIconKind.FileExcel;
                    break;
                case ".pdf":
                    Icon = PackIconKind.FilePdf;
                    break;
                case ".ppt":
                case ".pot":
                case ".pps":
                case ".pptx":
                case ".pptm":
                case ".potx":
                case ".potm":
                case ".ppam":
                case ".ppsx":
                case ".ppsm":
                case ".sldx":
                case ".sldm":
                    Icon = PackIconKind.FilePowerpoint;
                    break;
                case ".mp4":
                case ".avi":
                case ".flv":
                    Icon = PackIconKind.FileVideo;
                    break;
                case ".xml":
                case ".xaml":
                case ".axml":
                case ".xps":
                    Icon = PackIconKind.FileXml;
                    break;
                case ".txt":
                    Icon = PackIconKind.FileDocument;
                    break;
                case ".zip":
                case ".rar":
                case ".tar":
                    Icon = PackIconKind.ZipBox;
                    break;
                case "":
                case null:
                    Icon = PackIconKind.FileQuestion;
                    break;
                default:
                    Icon = PackIconKind.File;
                    break;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
