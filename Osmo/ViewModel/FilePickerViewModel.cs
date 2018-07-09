using Osmo.Core;
using Osmo.Core.FileExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class FilePickerViewModel : ViewModelBase
    {
        private SharedFileExplorer fileExplorer;
        private FolderEntry mSelectedFolder;
        private IFilePickerEntry mSelectedEntry;
        private int mDisplayOption;
        private int mSelectedIndex = -1;

        public VeryObservableCollection<FolderEntry> RootFolders
        {
            get => fileExplorer.RootFolders;
        }

        public FolderEntry SelectedFolder
        {
            get => mSelectedFolder;
            set
            {
                mSelectedFolder = value;
                InvokePropertyChanged("SelectedFolder");
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
                InvokePropertyChanged("SelectedEntry");
                InvokePropertyChanged("IsSelectEnabled");
            }
        }

        public IEnumerable<IFilePickerEntry> FileList
        {
            //get => IsFolderSelect ? SelectedFolder?.SubDirectories?.Cast<IFilePickerEntry>() : SelectedFolder?.JoinedContent;
            get
            {
                if (!IsFolderSelect)
                {
                    if (Filters != null || Filters.Count > 0)
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
                InvokePropertyChanged("DisplayOption");
            }
        }

        public int SelectedIndex
        {
            get => mSelectedIndex;
            set
            {
                mSelectedIndex = value;
                InvokePropertyChanged("SelectedIndex");
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
            fileExplorer = SharedFileExplorer.Instance;
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

        public void RefreshDrives()
        {
            fileExplorer.LoadDrives(false);
        }
    }
}
