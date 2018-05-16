using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.ViewModel
{
    class SkinViewModel : ViewModelBase
    {
        private Skin mLoadedSkin;
        private bool mShowImage = true;

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
            }
        }

        public SkinElement SelectedElement
        {
            get => mSelectedElement;
            set
            {
                mSelectedElement = value;
                InvokePropertyChanged("SelectedElement");
                InvokePropertyChanged("IsEmptyEnabled");
                InvokePropertyChanged("Image");
            }
        }

        public ImageSource Image
        {
            get => ShowImage ? SelectedElement.Image : mTempImage;
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
            mLoadedSkin = new Skin(@"D:\Program Files (x86)\osu!\Skins\Osmo Test");
        }
    }
}
