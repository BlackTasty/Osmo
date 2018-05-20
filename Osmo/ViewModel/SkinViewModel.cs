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
        private bool mPlayEnabled;
        private int mPlayStatus = 0;
        private ImageSource mImage;

        private Visibility mShowIcon = Visibility.Hidden;
        private PackIconKind mIcon = PackIconKind.File;

        private ImageSource mTempImage;

        private SkinElement mSelectedElement;

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

        public SkinElement SelectedElement
        {
            get => mSelectedElement;
            set
            {
                mSelectedElement = value;
                if (value != null && value.FileType == FileType.Image)
                {
                    RefreshImage();
                }
                else
                {
                    mImage = null;
                    InvokePropertyChanged("Image");
                }

                mPlayEnabled = value.FileType == FileType.Audio;

                InvokePropertyChanged("SelectedElement");
                InvokePropertyChanged("IsEmptyEnabled");
                InvokePropertyChanged("PlayEnabled");
            }
        }

        public bool PlayEnabled
        {
            get => mPlayEnabled;
        }

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

        public bool IsEmptyEnabled { get => mSelectedElement != null ? mSelectedElement.FileType == FileType.Image : false; }

        public SkinViewModel()
        {
            //mLoadedSkin = new Skin(@"D:\Program Files (x86)\osu!\Skins\Osmo Test");
        }

        internal void RefreshImage()
        {
            mImage = Helper.LoadImage(SelectedElement.Path);
            InvokePropertyChanged("Image");
        }
    }
}
