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
    public class ForumTemplate //Extension: .oft
    {
        private FileInfo fileInfo;
        
        public string Name { get; private set; }

        public string Path { get => fileInfo?.FullName; }

        public string Content { get; set; }

        public bool IsEmpty { get; private set; }

        public ForumTemplate()
        {
            IsEmpty = true;
        }

        public ForumTemplate(string path)
        {
            fileInfo = new FileInfo(path);
            Name = fileInfo.Name.Replace(fileInfo.Extension, "");
            Content = File.ReadAllText(Path);
        }

        public void Rename(string newName)
        {
            if (!Name.Equals(newName))
            {
                fileInfo?.MoveTo(fileInfo.DirectoryName + "\\" + newName + fileInfo.Extension);
            }
        }

        public void Save(string newContent)
        {
            Content = newContent;
            File.WriteAllText(Path, Content);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
