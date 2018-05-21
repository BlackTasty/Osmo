using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.ViewModel
{
    class SkinViewModel : ViewModelBase
    {
        private Skin mLoadedSkin;
        private bool mShowImage = true;
        private int mPlayStatus = 0;
        private ImageSource mImage;

        private double mAudioPosition = 0;
        private double mAudioLength = 0;

        private Visibility mShowIcon = Visibility.Hidden;
        private Visibility mShowEditor = Visibility.Hidden;
        private PackIconKind mIcon = PackIconKind.File;

        private ImageSource mTempImage;

        private SkinElement mSelectedElement;
        private AudioEngine mAudioEngine;

        public Skin LoadedSkin
        {
            get => mLoadedSkin;
            set
            {
                mLoadedSkin = value;
                if (AppConfiguration.GetInstance().BackupBeforeMixing)
                    mLoadedSkin.BackupSkin(AppConfiguration.GetInstance().BackupDirectory, true);

                InvokePropertyChanged("LoadedSkin");
                InvokePropertyChanged("Elements");
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

                InvokePropertyChanged("SelectedElement");
                InvokePropertyChanged("IsEmptyEnabled");
                InvokePropertyChanged("PlayEnabled");
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

        public bool ShowImage
        {
            get => mShowImage;
            set
            {
                mShowImage = value;
                mTempImage = null;
                if (!value)
                {
                    string tempFilePath = Path.Combine(Path.GetTempPath(),
                    "Osmo");
                    Directory.CreateDirectory(tempFilePath);
                    tempFilePath += "\\ImageElement.tmp";
                    
                    File.Copy(SelectedElement.Path, tempFilePath, true);
                    mTempImage = new BitmapImage(new Uri(tempFilePath));
                }
                else
                {

                }

                InvokePropertyChanged("Image");
            }
        }

        public bool IsEmptyEnabled { get => !mSelectedElement.Equals(null) ? mSelectedElement.FileType == FileType.Image : false; }

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
    }
}
