using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Objects
{
    /// <summary>
    /// The main class which contains name, path and content of a template. File extension: .oft
    /// </summary>
    class ForumTemplate //Extension: .oft
    {
        private FileInfo fileInfo;

        private string name;
        private string[] lines;

        public string Name { get => fileInfo?.Name; }

        public string Path { get => fileInfo?.FullName; }

        public bool IsEmpty { get; private set; }

        public ForumTemplate()
        {
            IsEmpty = true;
        }

        public ForumTemplate(string path)
        {
            fileInfo = new FileInfo(path);
            name = fileInfo.Name.Replace(fileInfo.Extension, "");
            lines = File.ReadAllLines(Path);
        }

        public void Rename(string newName)
        {
            if (!name.Equals(newName))
            {
                fileInfo?.MoveTo(fileInfo.DirectoryName + "\\" + newName + fileInfo.Extension);
            }
        }
    }
}
