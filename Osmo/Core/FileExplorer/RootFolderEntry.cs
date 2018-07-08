using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    class RootFolderEntry
    {
        public FolderEntry Root { get; private set; }

        public bool? DriveState { get; set; }

        public RootFolderEntry(FolderEntry root, bool? isAdded)
        {
            Root = root;
            DriveState = isAdded;
        }

        public RootFolderEntry(FolderEntry root) : this(root, null) { }
    }
}
