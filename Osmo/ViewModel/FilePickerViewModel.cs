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
        private VeryObservableCollection<FolderEntry> mRootFolders = new VeryObservableCollection<FolderEntry>("RootFolders");
        private FolderEntry mSelectedFolder;

        public VeryObservableCollection<FolderEntry> RootFolders
        {
            get => mRootFolders;
            set
            {
                mRootFolders = value;
                InvokePropertyChanged("RootFolders");
            }
        }

        public FolderEntry SelectedFolder
        {
            get => mSelectedFolder;
            set
            {
                mSelectedFolder = value;
                InvokePropertyChanged("SelectedFolder");
            }
        }

        public FilePickerViewModel()
        {
            new Thread(() =>
            {
                List<FolderEntry> entries = new List<FolderEntry>();
                //entries.Add(new FolderEntry(new DirectoryInfo(@"D:\Android")));
                foreach (var drive in DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Fixed))
                {
                    entries.Add(new FolderEntry(drive.RootDirectory));
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (FolderEntry entry in entries)
                    {
                        RootFolders.Add(entry);
                    }
                });
            }).Start();
        }
    }
}
