using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Osmo.ViewModel
{
    class OsmoViewModel : ViewModelBase
    {
        private SkinManager mManager;
        
        private int mSelectedSkinIndex = -1;
        private int mSelectedSidebarIndex = 0;
        
        private string mBackupDirectory = "";
        private double mBackupDirectorySize = 0;

        private bool mWindowButtonsEnabled = true;

        public SkinManager SkinManager
        {
            get => mManager;
            set
            {
                if (mManager != null)
                {
                    mManager.SkinChanged -= MManager_SkinChanged;
                    mManager.SkinRenamed -= MManager_SkinRenamed;
                }

                mManager = value;
                if (mManager != null)
                {
                    mManager.SkinChanged += MManager_SkinChanged;
                    mManager.SkinRenamed += MManager_SkinRenamed;
                }
            }
        }

        public bool WindowButtonsEnabled
        {
            get => mWindowButtonsEnabled;
            set
            {
                mWindowButtonsEnabled = value;
                InvokePropertyChanged("WindowButtonsEnabled");
            }
        }

        public VeryObservableCollection<Skin> Skins { get => SkinManager?.Skins; }

        public List<SidebarEntry> Items { get; private set; }

        public int SelectedSkinIndex
        {
            get => mSelectedSkinIndex;
            set
            {
                mSelectedSkinIndex = value;
                InvokePropertyChanged();
            }
        }

        public int SelectedSidebarIndex
        {
            get => mSelectedSidebarIndex;
            set
            {
                if (Items[value].HasChildren)
                {
                    Items[mSelectedSidebarIndex].ToggleSubEntries(false);
                    Items[value].ToggleSubEntries(true);
                }
                else if (Items[mSelectedSidebarIndex] is SidebarSubEntry subEntry)
                {
                    subEntry.ParentEntry.ToggleSubEntries(false);
                    Items[value].ToggleSubEntries(true);
                }

                mSelectedSidebarIndex = value;

                //SelectedEntry = Items[value];
                InvokePropertyChanged("Items");
                InvokePropertyChanged();
            }
        }

        public string OsuDirectory
        {
            get => SkinManager != null ? SkinManager.SkinDirectory : "";
            set
            {
                if (SkinManager == null)
                {
                    SkinManager = SkinManager.Instance;
                    SkinManager.SkinDirectoryChanged += SkinManager_SkinDirectoryChanged;
                }
                else
                {
                    SkinManager.SkinDirectory = value;
                }
                InvokePropertyChanged();
            }
        }

        public string BackupDirectory
        {
            get => mBackupDirectory;
            set
            {
                mBackupDirectory = value;
                InvokePropertyChanged();
            }
        }

        public double BackupDirectorySize
        {
            get => mBackupDirectorySize;
            set
            {
                mBackupDirectorySize = value;
                InvokePropertyChanged();
            }
        }

        public bool IsEditorEnabled
        {
            set
            {
                Items[FixedValues.EDITOR_INDEX].IsEnabled = value;
                InvokePropertyChanged("Items");
            }
        }

        public bool IsMixerEnabled
        {
            set
            {
                Items[FixedValues.MIXER_INDEX].IsEnabled = value;
                InvokePropertyChanged("Items");
            }
        }
        public OsmoViewModel()
        {
            if (!App.IsDesigner)
            {
                FixedValues.InitializeReader();
                string osuDir = App.ProfileManager.Profile.OsuDirectory;

                if (!string.IsNullOrWhiteSpace(osuDir))
                {
                    SkinManager = SkinManager.Instance;
                    SkinManager.SkinDirectoryChanged += SkinManager_SkinDirectoryChanged;
                }

                SidebarEntry tools = new SidebarEntry("sidebar_tools", MaterialDesignThemes.Wpf.PackIconKind.Toolbox, null, 4);
                List<SidebarSubEntry> toolsSubEntries = new List<SidebarSubEntry>()
                {
                    new SidebarSubEntry("sidebar_resizeTool", MaterialDesignThemes.Wpf.PackIconKind.MoveResizeVariant, ResizeTool.Instance, 20, tools),
                    new SidebarSubEntry("sidebar_tools_sbCreator", MaterialDesignThemes.Wpf.PackIconKind.Animation, SliderballCreator.Instance, 21, tools),
                    new SidebarSubEntry("Simulator", MaterialDesignThemes.Wpf.PackIconKind.Eye, Simulator.Instance, 22, tools)
                };

                tools.SetSubEntries(toolsSubEntries);
                
                Items = new List<SidebarEntry>()
                {
                    new SidebarEntry("sidebar_home", MaterialDesignThemes.Wpf.PackIconKind.Home, SkinSelect.Instance, 0),
                    new SidebarEntry("sidebar_wizard", MaterialDesignThemes.Wpf.PackIconKind.AutoFix, SkinCreationWizard.Instance, 1),
                    new SidebarEntry("sidebar_editor", MaterialDesignThemes.Wpf.PackIconKind.Pencil, SkinEditor.Instance, 2, false),
                    new SidebarEntry("sidebar_mixer", MaterialDesignThemes.Wpf.PackIconKind.PotMix, SkinMixer.Instance, 3, false),
                    tools,
                    toolsSubEntries[0],
                    toolsSubEntries[1],
                    new SidebarEntry("sidebar_templateManager", MaterialDesignThemes.Wpf.PackIconKind.Archive, TemplateManager.Instance, 5),
                    new SidebarEntry("sidebar_settings", MaterialDesignThemes.Wpf.PackIconKind.Settings, Settings.Instance, 6),
                    new SidebarEntry("sidebar_about", MaterialDesignThemes.Wpf.PackIconKind.Information, About.Instance, 7),
                    new SidebarEntry("sidebar_templateEditor", MaterialDesignThemes.Wpf.PackIconKind.Pencil, TemplateEditor.Instance, 8, Visibility.Hidden)
                };
            }
        }

        private void SkinManager_SkinDirectoryChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged("Skins");
        }

        private void MManager_SkinChanged(object sender, SkinChangedEventArgs e)
        {
            //if (Skins != null)
            //{
            //    //TODO: isSkin may be removed in case the menu background isn't needed
            //    bool isSkin = System.IO.File.GetAttributes(e.Path) == System.IO.FileAttributes.Directory;

            //    //switch (e.ChangeType)
            //    //{
            //    //    //case System.IO.WatcherChangeTypes.Changed:
            //    //    //    if (!isSkin)
            //    //    //    {
            //    //    //        Skin changed = mSkins.FirstOrDefault(x => x == System.IO.Path.GetDirectoryName(e.Path));
            //    //    //        if (changed != null)
            //    //    //        {
            //    //    //        }
            //    //    //    }
            //    //    //    break;
            //    //    case System.IO.WatcherChangeTypes.Created:
            //    //        if (isSkin)
            //    //            Skins.Add(new Skin(e.Path));
            //    //        break;
            //    //    case System.IO.WatcherChangeTypes.Deleted:
            //    //        if (isSkin)
            //    //        {
            //    //            Skin removed = Skins.FirstOrDefault(x => x == e.Path);
            //    //            if (removed != null)
            //    //                Skins.Remove(removed);
            //    //        }
            //    //        break;
            //    //}
            //}
        }

        private void MManager_SkinRenamed(object sender, SkinRenamedEventArgs e)
        {
            if (Skins != null && System.IO.File.GetAttributes(e.Path) == System.IO.FileAttributes.Directory)
            {
                Skin renamed = Skins.First(x => x.Path == e.Path);
                if (renamed != null)
                    Skins[Skins.IndexOf(renamed)].Path = e.Path;
            }
        }
    }
}
