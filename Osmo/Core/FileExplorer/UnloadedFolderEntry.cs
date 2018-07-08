using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    public class UnloadedFolderEntry
    {
        public string Path
        {
            get => DirectoryInfo.FullName;
        }

        public string Name
        {
            get => DirectoryInfo.Name;
        }

        public DirectoryInfo DirectoryInfo { get; private set; }

        public List<string> UnloadedSubDirectories { get; private set; }
            = new List<string>();

        public UnloadedFolderEntry(DirectoryInfo di)
        {
            DirectoryInfo = di;
            Console.WriteLine("{0}: Initializing folder content...", di.Name);
            foreach (DirectoryInfo subDi in di.EnumerateDirectories())
            {
                try
                {
                    subDi.EnumerateDirectories();
                }
                catch (Exception)
                {
                    Console.WriteLine("{0}: This folder seems to be protected!", subDi.Name);
                    continue;
                }
                UnloadedSubDirectories.Add(subDi.Name);
            }
            Console.WriteLine("{0}: Folder content initialized!", di.Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
