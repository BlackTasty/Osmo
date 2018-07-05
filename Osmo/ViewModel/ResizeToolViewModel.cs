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
