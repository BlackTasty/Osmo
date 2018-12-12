using System.IO;
using MaterialDesignThemes.Wpf;

namespace Osmo.Core.FileExplorer
{
    class ClassicEntry : IFilePickerEntry
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public PackIconKind Icon { get; private set; }

        public bool IsFile { get; private set; }

        public ClassicEntry(string path, bool isFile)
        {
            Path = path;
            if (isFile)
            {
                Name = new FileInfo(path).Name;
            }
            else
            {
                Name = new DirectoryInfo(path).Name;
            }
            IsFile = isFile;
        }
    }
}
