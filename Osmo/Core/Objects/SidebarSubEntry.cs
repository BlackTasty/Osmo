using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Objects
{
    class SidebarSubEntry : SidebarEntry
    {
        public SidebarEntry ParentEntry
        {
            get; private set;
        }

        public SidebarSubEntry(string localisationName, PackIconKind icon, object content, int index, 
            SidebarEntry parent) : this(localisationName, icon, content, index, true, parent)
        {
        }
        public SidebarSubEntry(string localisationName, PackIconKind icon, object content, int index, bool isEnabled, 
            SidebarEntry parent) : base(localisationName, icon, content, index, isEnabled)
        {
            ParentEntry = parent;
            IsVisible = System.Windows.Visibility.Collapsed;
        }
    }
}
