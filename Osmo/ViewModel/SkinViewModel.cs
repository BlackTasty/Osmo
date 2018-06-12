using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.ViewModel
{
    class SkinViewModel : AudioViewModel
    {
        private Skin mLoadedSkin = null;
        private bool mResetEnabled;
        private bool mAnimationEnabled;
        private int mPlayStatus = 0;

        private Visibility mShowIcon = Visibility.Hidden;
        private Visibility mShowEditor = Visibility.Hidden;
        private PackIconKind mIcon = PackIconKind.File;

        private SkinElement mSelectedElement = new SkinElement();

        public Skin LoadedSkin
        {
            get => mLoadedSkin;
            set
            {
                mLoadedSkin = value;
                //if (AppConfiguration.GetInstance().BackupBeforeMixing)
                //    mLoadedSkin.BackupSkin(AppConfiguration.GetInstance().BackupDirectory, true);
                
                (Application.Current.MainWindow.DataContext as OsmoViewModel).IsEditorEnabled = value != null;
                InvokePropertyChanged("LoadedSkin");
                InvokePropertyChanged("Elements");
                InvokePropertyChanged("IsSkinLoaded");
            }
        }

        public bool ResetEnabled
        {
            get => mResetEnabled;
            set
            {
                mResetEnabled = value;
                InvokePropertyChanged("ResetEnabled");
            }
        }

        public bool UnsavedChanges { get => LoadedSkin?.UnsavedChanges ?? false; }

        public bool AnimationEnabled
        {
            get => mAnimationEnabled;
            set
            {
                mAnimationEnabled = value;
                InvokePropertyChanged("AnimationEnabled");
            }
        }

        public SkinElement SelectedElement
        {
            get => mSelectedElement;
            set
            {
                mSelectedElement = value;
                if (value != new SkinElement())
                {
                     RefreshImage();
                }
                else
                {
                    Image = null;
                    InvokePropertyChanged("Image");
                }

                PlayEnabled = value.FileType == FileType.Audio;

                ResetEnabled = !string.IsNullOrWhiteSpace(value.TempPath);
                if (value.ElementDetails != null)
                {
                    AnimationEnabled = value.ElementDetails.MultipleElementsAllowed && !value.ElementDetails.IsSound;
                }
                else
                {
                    AnimationEnabled = false;
                }

                InvokePropertyChanged("SelectedElement");
                InvokePropertyChanged("PlayEnabled");
                InvokePropertyChanged("IsFABEnabled");
            }
        }

        public bool PlayEnabled { get; private set; }

        public int PlayStatus
        {
            get => mPlayStatus;
            set
            {
                mPlayStatus = value;
                InvokePropertyChanged("PlayStatus");
            }
        }

        public ImageSource Image { get; private set; }

        public PackIconKind Icon
        {
            get => mIcon;
            set
            {
                mIcon = value;
                InvokePropertyChanged("Icon");
            }
        }

        public Visibility ShowIcon
        {
            get => mShowIcon;
            set
            {
                mShowIcon = value;
                InvokePropertyChanged("ShowIcon");
            }
        }

        public Visibility ShowEditor
        {
            get => mShowEditor;
            set
            {
                mShowEditor = value;
                InvokePropertyChanged("ShowEditor");
            }
        }

        public bool IsFABEnabled { get => mSelectedElement != null && !mSelectedElement.IsEmpty; }

        public bool IsSkinLoaded { get => !LoadedSkin?.IsEmpty ?? false; }

        internal void RefreshImage()
        {
            if (SelectedElement.FileType == FileType.Image)
            {
                Image = Helper.LoadImage(SelectedElement.Path);
            }
            else
            {
                Image = null;
            }
            InvokePropertyChanged("Image");
        }

        internal void SaveSkin()
        {
            LoadedSkin.Save();
            InvokePropertyChanged("ResetEnabled");
        }

        internal void ExportSkin(string targetDir, bool alsoSave)
        {
            if (alsoSave)
            {
                LoadedSkin.Save();
                InvokePropertyChanged("ResetEnabled");
            }

            LoadedSkin.Export(targetDir);
        }
    }
}
