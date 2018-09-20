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
        private SkinManager mSkinManager;

        private bool mResizeOption_optimalSize = true;
        private bool mResizeOption_halveSize;
        private bool mFileOption_keepOriginal = true;
        private bool mFileOption_overrideOriginal;

        private bool mSelect_all;
        private bool mSelect_hdElements;
        private bool mSelect_nonHdElements;
        private bool mSelect_none = true;

        private bool mIsResizing;
        private string mCurrentFile;
        private double mElementsResizeMaximum;
        private double mElementsResizeValue;

        public ResizeToolViewModel()
        {
            mSkinManager = SkinManager.Instance;
            mSkinManager.SkinDirectoryChanged += SkinManager_SkinDirectoryChanged;
        }

        private void SkinManager_SkinDirectoryChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged("Skins");
        }

        public List<Skin> Skins
        {
            get => mSkinManager != null && mSkinManager.IsValid ? mSkinManager.Skins.SkipWhile(x => x.IsEmpty).ToList() : null;
        }

        public int SelectedSkinIndex
        {
            get => mSelectedSkinIndex;
            set
            {
                //SelectedSkin = Skins?[value];
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

        public bool Select_All
        {
            get => mSelect_all;
            set
            {
                if (value && mSkinManager.IsValid)
                {
                    foreach (SkinElement item in SelectedSkin.Elements.Where(x => x.FileType == FileType.Image && x.ImageSize.Width > 1 && x.ImageSize.Height > 1))
                    {
                        item.IsResizeSelected = true;
                    }
                }

                mSelect_all = value;
                InvokePropertyChanged("Select_All");
            }
        }

        public bool Select_HdElements
        {
            get => mSelect_hdElements;
            set
            {

                if (value && mSkinManager.IsValid)
                {
                    foreach (SkinElement item in SelectedSkin.Elements.Where(x => x.FileType == FileType.Image && x.ImageSize.Width > 1 && x.ImageSize.Height > 1))
                    {
                        item.IsResizeSelected = item.IsHighDefinition;
                    }
                }

                mSelect_hdElements = value;
                InvokePropertyChanged("Select_HdElements");
            }
        }

        public bool Select_NonHdElements
        {
            get => mSelect_nonHdElements;
            set
            {
                if (value && mSkinManager.IsValid)
                {
                    foreach (SkinElement item in SelectedSkin.Elements.Where(x => x.FileType == FileType.Image && x.ImageSize.Width > 1 && x.ImageSize.Height > 1))
                    {
                        item.IsResizeSelected = !item.IsHighDefinition;
                    }
                }

                mSelect_nonHdElements = value;
                InvokePropertyChanged("Select_NonHdElements");
            }
        }

        public bool Select_None
        {
            get => mSelect_none;
            set
            {
                if (value && mSkinManager.IsValid)
                {
                    foreach (SkinElement item in SelectedSkin.Elements.Where(x => x.FileType == FileType.Image))
                    {
                        item.IsResizeSelected = false;
                    }
                }

                mSelect_none = value;
                InvokePropertyChanged("Select_None");
            }
        }

        public Skin SelectedSkin
        {
            get => mSelectedSkin;
            set
            {
                mSelectedSkin = value;
                if (value != null)
                {
                    mSelectedSkinIndex = Skins.IndexOf(value);
                }
                else
                {
                    mSelectedSkinIndex = -1;
                }
                InvokePropertyChanged("SelectedSkin");
            }
        }

        public bool IsResizing
        {
            get => mIsResizing;
            set
            {
                mIsResizing = value;
                InvokePropertyChanged("IsResizing");
                InvokePropertyChanged("ControlsEnabled");
            }
        }

        public bool ControlsEnabled
        {
            get => !mIsResizing;
        }

        public string CurrentFile
        {
            get => mCurrentFile;
            set
            {
                mCurrentFile = value;
                InvokePropertyChanged("CurrentFile");
            }
        }

        public double ElementsResizeMaximum
        {
            get => mElementsResizeMaximum;
            set
            {
                mElementsResizeMaximum = value;
                InvokePropertyChanged("ElementsResizeMaximum");
            }
        }

        public double ElementsResizeValue
        {
            get => mElementsResizeValue;
            set
            {
                mElementsResizeValue = value;
                InvokePropertyChanged("ElementsResizeValue");
            }
        }
    }
}
