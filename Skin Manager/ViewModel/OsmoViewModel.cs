using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class OsmoViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private SkinManager mManager;

        private VeryObservableCollection<Skin> mSkins = new VeryObservableCollection<Skin>("Skins");
        private SidebarEntry[] mSidebarItems;
        
        private int mSelectedSkinIndex = -1;

        private string mOsuDirectory = "";

        public SkinManager SkinManager
        {
            get => mManager; set
            {
                mManager = value;
                if (mManager != null)
                {
                    mManager.SkinChanged += MManager_SkinChanged;
                    mManager.SkinRenamed += MManager_SkinRenamed;
                }
            }
        }

        public VeryObservableCollection<Skin> Skins { get => mSkins; set => mSkins = value; }

        public SidebarEntry[] Items { get => mSidebarItems; }

        public int SelectedSkinIndex { get => mSelectedSkinIndex; set => mSelectedSkinIndex = value; }

        public string OsuDirectory
        {
            get => mOsuDirectory; set
            {
                mOsuDirectory = value;
                InvokePropertyChanged("OsuDirectory");
            }
        }

        public OsmoViewModel()
        {
            mSkins.Add(new Skin());

            mSidebarItems = new SidebarEntry[]
            {
                new SidebarEntry("Home", MaterialDesignThemes.Wpf.PackIconKind.Home, new SkinSelect()),
                new SidebarEntry("Settings", MaterialDesignThemes.Wpf.PackIconKind.Settings, new Settings())
            };
        }
        
        private void MManager_SkinChanged(object sender, SkinChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    break;
                case System.IO.WatcherChangeTypes.Created:
                    mSkins.Add(new Skin(e.Path));
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    Skin removed = mSkins.FirstOrDefault(x => x == e.Path);
                    if (removed != null)
                        mSkins.Remove(removed);
                    break;
            }
        }

        private void MManager_SkinRenamed(object sender, SkinRenamedEventArgs e)
        {
            Skin renamed = mSkins.First(x => x == e.Path);
            if (renamed != null)
                mSkins[mSkins.IndexOf(renamed)].Path = e.Path;
        }
    }
}
