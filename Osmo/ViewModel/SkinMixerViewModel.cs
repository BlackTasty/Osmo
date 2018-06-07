using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    class SkinMixerViewModel : AudioViewModel
    {
        private Skin mSkinLeft;
        private Skin mSkinRight;
        private SkinElement mSelectedElementLeft;
        private SkinElement mSelectedElementRight;
        private ImageSource mImageLeft;
        private ImageSource mImageRight;
        private PackIconKind mIconLeft = PackIconKind.File;
        private PackIconKind mIconRight = PackIconKind.File;
        private Visibility mShowIconLeft = Visibility.Hidden;
        private Visibility mShowIconRight = Visibility.Hidden;

        private bool mAudioPlayingLeft;
        private bool mAudioPlayingRight;

        public Skin SkinLeft
        {
            get => mSkinLeft;
            set
            {
                mSkinLeft = value;
                InvokePropertyChanged("SkinLeft");
            }
        }

        public Skin SkinRight
        {
            get => mSkinRight;
            set
            {
                mSkinRight = value;
                InvokePropertyChanged("SkinRight");
            }
        }

        #region Selected element left side
        public SkinElement SelectedElementLeft
        {
            get => mSelectedElementLeft;
            set
            {
                mSelectedElementLeft = value;
                if (value != new SkinElement())
                {
                    mImageLeft = RefreshImage(value);
                }
                else
                {
                    mImageLeft = null;
                }

                PlayEnabledLeft = value.FileType == FileType.Audio;

                InvokePropertyChanged("ImageLeft");
                InvokePropertyChanged("PlayEnabledLeft");
                InvokePropertyChanged("SelectedElementLeft");
                InvokePropertyChanged("FileTypeMatch");
            }
        }

        public ImageSource ImageLeft => mImageLeft;

        public PackIconKind IconLeft
        {
            get => mIconLeft;
            set
            {
                mIconLeft = value;
                InvokePropertyChanged("IconLeft");
            }
        }

        public Visibility ShowIconLeft
        {
            get => mShowIconLeft;
            set
            {
                mShowIconLeft = value;
                InvokePropertyChanged("ShowIconLeft");
            }
        }

        public bool PlayEnabledLeft { get; private set; }

        public bool AudioPlayingLeft
        {
            get => mAudioPlayingLeft;
            set
            {
                mAudioPlayingLeft = value;
                if (value)
                {
                    AudioPlayingRight = false;
                }
                InvokePropertyChanged("AudioPlayingLeft");
            }
        }
        #endregion

        #region Selected element right side
        public SkinElement SelectedElementRight
        {
            get => mSelectedElementRight;
            set
            {
                mSelectedElementRight = value;
                if (value != new SkinElement())
                {
                    mImageRight = RefreshImage(value);
                }
                else
                {
                    mImageRight = null;
                }

                PlayEnabledRight = value.FileType == FileType.Audio;

                InvokePropertyChanged("ImageRight");
                InvokePropertyChanged("PlayEnabledRight");
                InvokePropertyChanged("SelectedElementRight");
                InvokePropertyChanged("FileTypeMatch");
            }
        }

        public ImageSource ImageRight => mImageRight;

        public PackIconKind IconRight
        {
            get => mIconRight;
            set
            {
                mIconRight = value;
                InvokePropertyChanged("IconRight");
            }
        }

        public Visibility ShowIconRight
        {
            get => mShowIconRight;
            set
            {
                mShowIconRight = value;
                InvokePropertyChanged("ShowIconRight");
            }
        }

        public bool PlayEnabledRight { get; private set; }

        public bool AudioPlayingRight
        {
            get => mAudioPlayingRight;
            set
            {
                mAudioPlayingRight = value;
                if (value)
                {
                    AudioPlayingLeft = false;
                }
                InvokePropertyChanged("AudioPlayingRight");
            }
        }

        public bool FileTypeMatch
        {
            get
            {
                FileType typeLeft = SelectedElementLeft?.FileType ?? FileType.Configuration;
                FileType typeRight = SelectedElementRight?.FileType ?? FileType.Unknown;

                return typeLeft == typeRight;
            }
        }
        #endregion

        private ImageSource RefreshImage(SkinElement target)
        {
            if (target.FileType == FileType.Image)
            {
                return Helper.LoadImage(target.Path);
            }
            else
            {
                return null;
            }
        }

        internal void RefreshImage()
        {
            mImageLeft = RefreshImage(SelectedElementLeft);
            InvokePropertyChanged("ImageLeft");
        }
    }
}
