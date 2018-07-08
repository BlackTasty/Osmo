using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    public class FolderEntry : ViewModelBase
    {
        private DirectoryInfo mDi;
        private UnloadedFolderEntry mUnloadedEntry;
        private bool mFoldersLoaded;
        private bool mFilesLoaded;

        public string Path
        {
            get => mDi.FullName;
        }

        public string Name
        {
            get => mDi?.Name ?? mUnloadedEntry.Name;
        }

        public bool FoldersLoaded {
            get => mFoldersLoaded;
            private set
            {
                mFoldersLoaded = value;
                InvokePropertyChanged("FoldersLoaded");
            }
        }

        public List<FileEntry> Files { get; private set; } 
            = new List<FileEntry>();

        public List<FolderEntry> SubDirectories { get; private set; } 
            = new List<FolderEntry>();

        public FolderEntry(DirectoryInfo di)
        {
            mDi = di;

            foreach (DirectoryInfo subDi in di.EnumerateDirectories())
            {
                try
                {
                    subDi.EnumerateDirectories();
                }
                catch (Exception)
                {
                    continue;
                }
                SubDirectories.Add(new FolderEntry(new UnloadedFolderEntry(subDi)));
            }

            FoldersLoaded = true;
        }

        private FolderEntry(UnloadedFolderEntry unloadedEntry)
        {
            this.mUnloadedEntry = unloadedEntry;
        }

        public void LoadFoldersOnDemand()
        {
            for (int i = 0; i < SubDirectories.Count; i++)
            {
                FolderEntry entry = SubDirectories[i];
                if (!entry.FoldersLoaded)
                {
                    SubDirectories[i] = new FolderEntry(entry.mUnloadedEntry.DirectoryInfo);
                }
            }
        }

        public void LoadFilesOnDemand()
        {
            if (!mFilesLoaded)
            {
                foreach (FileInfo fi in mDi.EnumerateFiles())
                {
                    Files.Add(new FileEntry(fi));
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
