using MaterialDesignThemes.Wpf;

namespace Osmo.Core.Objects
{
    class SidebarEntry
    {
        public string Name { get; }

        public PackIconKind Icon { get; }

        public object Content { get; }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="name">The name to display in the list</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        public SidebarEntry(string name, PackIconKind icon, object content)
        {
            Name = name;
            Icon = icon;
            Content = content;
        }
    }
}
