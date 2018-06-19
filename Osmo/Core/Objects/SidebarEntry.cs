using MaterialDesignThemes.Wpf;
using System.Windows;

namespace Osmo.Core.Objects
{
    class SidebarEntry
    {
        public string Name { get; }

        public PackIconKind Icon { get; }

        public object Content { get; }

        public Visibility IsVisible { get; private set; } = Visibility.Visible;

        public bool IsEnabled
        {
            get
            {
                if (Content.GetType().IsSubclassOf(typeof(FrameworkElement)))
                {
                    return (Content as FrameworkElement).IsEnabled;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (Content.GetType().IsSubclassOf(typeof(FrameworkElement)))
                {
                    (Content as FrameworkElement).IsEnabled = value;
                }
            }
        }

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

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="name">The name to display in the list</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="isEnabled">Toggles whether this item is enabled by default or not</param>
        public SidebarEntry(string name, PackIconKind icon, object content, bool isEnabled) : this(name, icon, content)
        {
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="name">The name to display in the list</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="isGhost">Determines if this entry is visible or not in the sidebar</param>
        public SidebarEntry(string name, PackIconKind icon, object content, Visibility isGhost) : this(name, icon, content)
        {
            IsVisible = isGhost;
        }
    }
}
