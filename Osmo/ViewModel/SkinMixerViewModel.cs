using Osmo.Core;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
        }

        public ImageSource ImageLeft => mImageLeft;

        public bool PlayEnabledLeft { get; private set; }
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
            }
        }

        public ImageSource ImageRight => mImageRight;

        public bool PlayEnabledRight { get; private set; }
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
        }
    }
}
