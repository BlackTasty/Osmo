using MaterialDesignThemes.Wpf;

namespace Osmo.Core.Objects
{
    class SidebarEntry
    {
        public string Name { get; }

        public PackIconKind Icon { get; }

        public object Content { get; }

        public SidebarEntry(string name, PackIconKind icon, object content)
        {
            Name = name;
            Icon = icon;
            Content = content;
        }
    }
}
