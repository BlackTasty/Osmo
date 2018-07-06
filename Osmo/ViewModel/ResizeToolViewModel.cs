using Osmo.Core;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class ResizeToolViewModel : ViewModelBase
    {
        private Skin mSelectedSkin;
        private int mSelectedSkinIndex = -1;
        private SkinManager mSkinManager = SkinManager.GetInstance();

        private bool mResizeOption_optimalSize = true;
        private bool mResizeOption_halveSize;
        private bool mFileOption_keepOriginal = true;
        private bool mFileOption_overrideOriginal;

        public List<Skin> Skins
        {
            get => mSkinManager != null ? mSkinManager.Skins.SkipWhile(x => x.IsEmpty).ToList() : null;
        }

        public int SelectedSkinIndex
        {
            get => mSelectedSkinIndex;
            set
            {
                SelectedSkin = Skins?[value];
                mSelectedSkinIndex = value;
                InvokePropertyChanged("SelectedSkinIndex");
            }
        }

        public bool ResizeOption_OptimalSize
        {
            get => mResizeOption_optimalSize;
            set
            {
                mResizeOption_optimalSize = value;
                InvokePropertyChanged("ResizeOption_OptimalSize");
            }
        }

        public bool ResizeOption_HalveSize
        {
            get => mResizeOption_halveSize;
            set
            {
                mResizeOption_halveSize = value;
                InvokePropertyChanged("ResizeOption_HalveSize");
            }
        }

        public bool FileOption_keepOriginal
        {
            get => mFileOption_keepOriginal;
            set
            {
                mFileOption_keepOriginal = value;
                InvokePropertyChanged("FileOption_keepOriginal");
            }
        }

        public bool FileOption_overrideOriginal
        {
            get => mFileOption_overrideOriginal;
            set
            {
                mFileOption_overrideOriginal = value;
                InvokePropertyChanged("FileOption_overrideOriginal");
            }
        }

        public Skin SelectedSkin
        {
            get => mSelectedSkin;
            set
            {
                mSelectedSkin = value;
                InvokePropertyChanged("SelectedSkin");
            }
        }


    }
}
