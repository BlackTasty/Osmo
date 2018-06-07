using Osmo.Core;
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
        public SkinManager SkinManager
        {
            get => SkinManager.GetInstance();
        }

        public List<Skin> Skins { get => SkinManager != null ? SkinManager.Skins.SkipWhile(x => x.IsEmpty).ToList() : null; }
    }
}
