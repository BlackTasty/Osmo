using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    class SharedFileExplorer : ViewModelBase
    {
        private VeryObservableCollection<FolderEntry> mRootFolders = new VeryObservableCollection<FolderEntry>("RootFolders");

        private static SharedFileExplorer instance;

        public static SharedFileExplorer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SharedFileExplorer();
                }

                return instance;
            }
        }


        public VeryObservableCollection<FolderEntry> RootFolders
        {
            get => mRootFolders;
            set
            {
                mRootFolders = value;
                InvokePropertyChanged("RootFolders");
            }
        }

        private SharedFileExplorer()
        {
            LoadDrives(true);
        }

        internal void LoadDrives(bool isInit)
        {
            new Thread(() =>
            {
                List<RootFolderEntry> rootFolders = new List<RootFolderEntry>();
                foreach (FolderEntry existingEntry in RootFolders)
                {
                    rootFolders.Add(new RootFolderEntry(existingEntry));
                }

                //entries.Add(new FolderEntry(new DirectoryInfo(@"D:\Android")));
                foreach (var drive in DriveInfo.GetDrives().Where(x => x.IsReady))
                {
                    RootFolderEntry root = rootFolders.FirstOrDefault(x => x.Root.Path.Equals(drive.RootDirectory.FullName));
                    if (root != null) //Check if drive is already added
                    {
                        root.DriveState = true;
                    }
                    else
                    {
                        FolderEntry rootFolder = new FolderEntry(drive.RootDirectory, true, 0);
                        rootFolders.Add(new RootFolderEntry(rootFolder, false));
                        //StructureBuilder.CacheDriveContent(rootFolder);
                    }
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (RootFolderEntry root in rootFolders)
                    {
                        if (root.DriveState == false)
                        {
                            RootFolders.Add(root.Root);
                        }
                        else if (root.DriveState == null)
                        {
                            RootFolders.Remove(root.Root);
                        }
                    }
                });
            }).Start();
        }
    }
}
