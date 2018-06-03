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
    class SkinViewModel : ViewModelBase
    {
        private Skin mLoadedSkin = null;
        private bool mResetEnabled;
        private bool mAnimationEnabled;
        private int mPlayStatus = 0;
        private ImageSource mImage;

        private double mAudioPosition = 0;
        private double mAudioLength = 0;

        private Visibility mShowIcon = Visibility.Hidden;
        private Visibility mShowEditor = Visibility.Hidden;
        private PackIconKind mIcon = PackIconKind.File;

        private SkinElement mSelectedElement = new SkinElement();
        private AudioEngine mAudioEngine;
        

        //public ICommand ResetCommand { get; } = new RelayCommand(o => ResetElement((SkinViewModel)o), 
        //    o => !string.IsNullOrWhiteSpace(((SkinViewModel)o).SelectedElement.TempPath));

        //private static void ResetElement(SkinViewModel vm)
        //{
        //    //TODO: Replace "Erase" MessageBox with MaterialDesign dialog
        //    var result = MessageBox.Show("Do you really want to erase the image?",
        //        "Erase image?",
        //        MessageBoxButton.YesNo,
        //        MessageBoxImage.Exclamation,
        //        MessageBoxResult.No);

        //    string path = vm.SelectedElement.ReplaceBackup(null);

        //    if (result == MessageBoxResult.Yes)
        //    {
        //        using (FileStream stream = new FileStream( vm.SelectedElement.ReplaceBackup(null),
        //            FileMode.Create))
        //        {
        //            PngBitmapEncoder encoder = new PngBitmapEncoder();
        //            encoder.Frames.Add(BitmapFrame.Create(
        //                new BitmapImage(new Uri("pack://application:,,,/Osmo;component/Resources/empty.png", UriKind.Absolute))));
        //            encoder.Save(stream);
        //        }
        //        vm.RefreshImage();
        //        vm.ResetEnabled = true;
        //    }
        //}

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

        public AudioEngine AudioEngine
        {
            get => mAudioEngine;
            set
            {
                mAudioEngine = value;
                InvokePropertyChanged("AudioEngine");
            }
        }

        public double AudioLength
        {
            get => mAudioLength;
            set
            {
                mAudioLength = value;
                InvokePropertyChanged("AudioLength");
            }
        }

        public double AudioPosition
        {
            get => mAudioPosition;
            set
            {
                mAudioPosition = value;
                InvokePropertyChanged("AudioPosition");
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
                    mImage = null;
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

        public ImageSource Image
        {
            get => mImage;
        }

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
                mImage = Helper.LoadImage(SelectedElement.Path);
            }
            else
            {
                mImage = null;
            }
            InvokePropertyChanged("Image");
        }

        internal void SaveSkin()
        {
            LoadedSkin.Save();
            InvokePropertyChanged("ResetEnabled");
        }
    }
}
