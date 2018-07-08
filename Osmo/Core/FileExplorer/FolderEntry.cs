using MaterialDesignThemes.Wpf;
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
    public class FolderEntry : ViewModelBase, IFilePickerEntry
    {
        private DirectoryInfo mDi;
        private UnloadedFolderEntry mUnloadedEntry;
        private bool mFoldersLoaded;
        private bool mFilesLoaded;

        public string Path
        {
            get => mDi?.FullName ?? mUnloadedEntry.Path;
        }

        public string Name
        {
            get => mDi?.Name ?? mUnloadedEntry.Name;
        }

        public bool IsFile { get => false; }

        public bool IsRoot { get; private set; }

        public PackIconKind Icon { get => PackIconKind.Folder; }

        public FolderEntry Parent { get; private set; }

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

        public List<IFilePickerEntry> JoinedContent { get; private set; }
            = new List<IFilePickerEntry>();

        public FolderEntry(DirectoryInfo di, bool isRoot)
        {
            IsRoot = isRoot;
            InitializeFolderStructure(di);
        }

        private FolderEntry(UnloadedFolderEntry unloadedEntry, FolderEntry parent)
        {
            mUnloadedEntry = unloadedEntry;
            Parent = parent;
        }

        private void InitializeFolderStructure(DirectoryInfo di)
        {
            mDi = di;

            foreach (DirectoryInfo subDi in di.EnumerateDirectories())
            {
                try
                {
                    subDi.EnumerateDirectories();
                    FolderEntry entry = new FolderEntry(new UnloadedFolderEntry(subDi), this);
                    SubDirectories.Add(entry);
                }
                catch (Exception)
                {
                }
            }

            SubDirectories.Sort(new NaturalFilePickerEntryComparer());
            JoinedContent.AddRange(SubDirectories);

            FoldersLoaded = true;
        }

        public void LoadAllOnDemand()
        {
            if (!FoldersLoaded)
            {
                InitializeFolderStructure(mUnloadedEntry.DirectoryInfo);
            }

            LoadFoldersOnDemand();
            LoadFilesOnDemand();
        }

        public void LoadFoldersOnDemand()
        {
            for (int i = 0; i < SubDirectories.Count; i++)
            {
                FolderEntry entry = SubDirectories[i];
                if (!entry.FoldersLoaded)
                {
                    SubDirectories[i] = new FolderEntry(entry.mUnloadedEntry.DirectoryInfo, false);
                }
            }
        }

        public void LoadFilesOnDemand()
        {
            if (!mFilesLoaded)
            {
                foreach (FileInfo fi in mDi.EnumerateFiles())
                {
                    FileEntry entry = new FileEntry(fi);
                    Files.Add(entry);
                }
                Files.Sort(new NaturalFilePickerEntryComparer());
                JoinedContent.AddRange(Files);
                mFilesLoaded = true;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
