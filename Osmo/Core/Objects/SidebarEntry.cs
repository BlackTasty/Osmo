using MaterialDesignThemes.Wpf;
using Osmo.Core.Configuration;
using Osmo.ViewModel;
using System.Windows;

namespace Osmo.Core.Objects
{
    class SidebarEntry : ViewModelBase
    {
        private string localisationName;

        public string Name { get; private set; }

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

        public int Index { get; private set; }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="localisationName">The name of the resource inside the localisation files</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="index">The index of this entry</param>
        public SidebarEntry(string localisationName, PackIconKind icon, object content, int index)
        {
            App.LanguageChanged += Instance_LanguageChanged;
            this.localisationName = localisationName;
            Name = Helper.FindString(localisationName);
            Icon = icon;
            Content = content;
            Index = index;
        }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="name">The name of the resource inside the localisation files</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="index">The index of this entry</param>
        /// <param name="isEnabled">Toggles whether this item is enabled by default or not</param>
        public SidebarEntry(string name, PackIconKind icon, object content, int index, bool isEnabled) : this(name, icon, content, index)
        {
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="name">The name of the resource inside the localisation files</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="index">The index of this entry</param>
        /// <param name="isGhost">Determines if this entry is visible or not in the sidebar</param>
        public SidebarEntry(string name, PackIconKind icon, object content, int index, Visibility isGhost) : this(name, icon, content, index)
        {
            IsVisible = isGhost;
        }

        private void Instance_LanguageChanged(object sender, System.EventArgs e)
        {
            Name = Helper.FindString(localisationName);
            InvokePropertyChanged("Name");
        }
    }
}
