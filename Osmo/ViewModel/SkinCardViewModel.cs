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
    class SkinCardViewModel : ViewModelBase
    {
        private Skin mSkin;
        private ImageSource mImage;

        public Skin Skin
        {
            get => mSkin;
            set
            {
                mSkin = value;
                SkinElement element = mSkin.Elements.FirstOrDefault(x => x.Name.Contains("menu-background"));
                if (element != null)
                {
                    mImage = Helper.LoadImage(element?.Path ?? "");
                    InvokePropertyChanged("Image");
                }

                if (mImage == null)
                {
                    ShowIcon = Visibility.Visible;
                    InvokePropertyChanged("ShowIcon");
                }
                InvokePropertyChanged("Skin");
            }
        }

        public ImageSource Image => mImage;

        public Visibility ShowIcon { get; private set; } = Visibility.Hidden;
    }
}
