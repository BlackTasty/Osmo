using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core
{
    interface ISkinContainer
    {
        Skin LoadedSkin { get; }

        void UnloadSkin(Skin skin);
    }
}
