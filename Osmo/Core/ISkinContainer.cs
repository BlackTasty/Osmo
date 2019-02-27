using Osmo.Core.Objects;

namespace Osmo.Core
{
    interface ISkinContainer
    {
        Skin LoadedSkin { get; }
        void UnloadSkin(Skin skin);
    }
}