using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Patcher
{
    class FrontendChangedEventArgs : EventArgs
    {
        private string mTooltip;
        private PackIconKind mIcon;

        public string Tooltip { get => mTooltip; }

        public PackIconKind Icon { get => mIcon; }

        public FrontendChangedEventArgs(string tooltip, PackIconKind icon)
        {
            mTooltip = tooltip;
            mIcon = icon;
        }
    }
}
