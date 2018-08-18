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
        private int mSelectedIndex;
        private OsmoViewModel mMaster;

        public SkinSelectViewModel()
        {
            mSkinManager = SkinManager.Instance;
            mSkinManager.SkinDirectoryChanged += SkinManager_SkinDirectoryChanged;
        }

        private void SkinManager_SkinDirectoryChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged("Skins");
            InvokePropertyChanged("SkinsMixer");
        }

        public SkinManager SkinManager
        {
            get => mSkinManager;
        }

        public int SelectedIndex
        {
            get => mSelectedIndex;
            set
            {
                mSelectedIndex = value;
                InvokePropertyChanged("SelectedIndex");
                InvokePropertyChanged("SkinOptionsEnabled");
            }
        }

        public bool SkinOptionsEnabled
        {
            get => SelectedIndex > 0;
        }

        public OsmoViewModel Master
        {
            get => mMaster;
            set => mMaster = value;
        }

        public int SelectedSidebarIndex {
            get => mMaster?.SelectedSidebarIndex ?? 0;
            set
            {
                if (mMaster != null)
                {
                    mMaster.SelectedSidebarIndex = value;
                }
            }
        }

        public List<Skin> Skins { get => SkinManager != null ? SkinManager.Skins.ToList() : null; }

        public List<Skin> SkinsMixer { get => SkinManager != null ? SkinManager.Skins.SkipWhile(x => x.IsEmpty).ToList() : null; }
    }
}
