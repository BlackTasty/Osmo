using Osmo.Core;
using Osmo.Core.FileExplorer;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Osmo.ViewModel
{
    class FilePickerViewModel : ViewModelBase
    {
        private SharedFileExplorer fileExplorer;
        private FolderEntry mSelectedFolder;
        private IFilePickerEntry mSelectedEntry;
        private int mDisplayOption;
        private int mSelectedIndex = -1;
        private string mCurrentPath;

        public VeryObservableCollection<FolderEntry> RootFolders
        {
            get
            {
                if (fileExplorer == null)
                {
                    fileExplorer = SharedFileExplorer.Instance;
                }
                return fileExplorer.RootFolders;
            }
        }

        public FolderEntry SelectedFolder
        {
            get => mSelectedFolder;
            set
            {
                mSelectedFolder = value;
                CurrentPath = value.Path;
                InvokePropertyChanged();
                InvokePropertyChanged("FileList");
                InvokePropertyChanged("IsRoot");
            }
        }

        public IFilePickerEntry SelectedEntry
        {
            get => mSelectedEntry;
            set
            {
                mSelectedEntry = value;
                if (!(value is ClassicEntry))
                {
                    InvokePropertyChanged("SelectedEntry");
                    InvokePropertyChanged("IsSelectEnabled");
                }
            }
        }

        public IEnumerable<IFilePickerEntry> FileList
        {
            //get => IsFolderSelect ? SelectedFolder?.SubDirectories?.Cast<IFilePickerEntry>() : SelectedFolder?.JoinedContent;
            get
            {
                if (!IsFolderSelect)
                {
                    if (Filters != null && Filters.Count > 0)
                    {
                        return SelectedFolder?.JoinedContent?.Where(x => !x.IsFile || 
                        Filters.Any(f => f.FilterMatch((x as FileEntry).Extension)));
                    }
                    else
                    {
                        return SelectedFolder?.JoinedContent;
                    }
                }
                else
                {
                    return SelectedFolder?.SubDirectories?.Cast<IFilePickerEntry>();
                }
            }
        }

        public int DisplayOption
        {
            get => mDisplayOption;
            set
            {
                mDisplayOption = value;
                InvokePropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get => mSelectedIndex;
            set
            {
                mSelectedIndex = value;
                InvokePropertyChanged();
            }
        }

        public string CurrentPath
        {
            get => mCurrentPath;
            set
            {
                mCurrentPath = value;
                InvokePropertyChanged();
            }
        }

        public bool IsSelectEnabled
        {
            get => IsFolderSelect ? (!mSelectedEntry?.IsFile ?? false) || (!mSelectedFolder?.IsFile ?? false) : 
                mSelectedEntry?.IsFile ?? false;
        }

        public bool IsRoot
        {
            get => SelectedFolder?.IsRoot ?? true;
        }

        public List<FileFilter> Filters { get; private set; }

        public bool IsFolderSelect { get; set; }

        public FilePickerViewModel()
        {
            if (App.ProfileManager.Profile.UseExperimentalFileExplorer)
            {
                fileExplorer = SharedFileExplorer.Instance;
            }
        }

        public void SetFilters(string filtersRaw)
        {
            Filters = new List<FileFilter>();
            string[] filters = filtersRaw.Split('|');
            for (int i = 0; i < filters.Length; i += 2)
            {
                Filters.Add(new FileFilter(filters[i], filters[i + 1]));
            }
        }

        public StructureBuilder SetCurrentDirectory(string path)
        {
            if (!path.Equals(SelectedFolder?.Path))
            {
                StructureBuilder structure = new StructureBuilder(path);
                SelectedFolder = RootFolders.FirstOrDefault(x => x.Path.Equals(structure.RootFolder))?.BuildSubTree(structure, !IsFolderSelect);
                return structure;
            }
            return null;
        }

        public void RefreshDrives()
        {
            if (fileExplorer == null)
            {
                fileExplorer = SharedFileExplorer.Instance;
            }
            fileExplorer.LoadDrives(false);
        }
    }
}
