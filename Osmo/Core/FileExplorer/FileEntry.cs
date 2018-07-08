using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    public class FileEntry
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

        public string Extension
        {
            get => fi.Extension;
        }

        public FileEntry(FileInfo fi)
        {
            this.fi = fi;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
