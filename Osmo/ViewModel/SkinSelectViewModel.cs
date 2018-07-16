using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class SkinSelectViewModel : ViewModelBase
    {
        private SkinManager mSkinManager;

        public SkinSelectViewModel()
        {
            mSkinManager = SkinManager.Instance;
            mSkinManager.SkinDirectoryChanged += SkinManager_SkinDirectoryChanged;
        }

        private void SkinManager_SkinDirectoryChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged("Skins");
        }

        public SkinManager SkinManager
        {
            get => mSkinManager;
        }

        public List<Skin> Skins { get => SkinManager != null ? SkinManager.Skins.SkipWhile(x => x.IsEmpty).ToList() : null; }
    }
}
