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
        private int mSelectedSkinIndex = 0;
        private SkinManager mSkinManager = SkinManager.GetInstance();

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
