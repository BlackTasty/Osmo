using MaterialDesignThemes.Wpf;
using Osmo.Core.Configuration;
using Osmo.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Osmo.Core.Objects
{
    class SidebarEntry : ViewModelBase
    {
        private string localisationName;
        private Visibility mIsVisible = Visibility.Visible;

        public string Name { get; private set; }

        public PackIconKind Icon { get; }

        public object Content { get; set; }

        public bool ContentIsCopy { get => (Content as Panel).Name.ToString() != localisationName; }

        public Visibility IsVisible
        {
            get => mIsVisible;
            set
            {
                mIsVisible = value;
                InvokePropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get
            {
                /*if (Content == null)
                    return true;*/

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
                //if (Content?.GetType().IsSubclassOf(typeof(FrameworkElement)) ?? false)
                //{
                //    (Content as FrameworkElement).IsEnabled = value;
                //}
                if (Content.GetType().IsSubclassOf(typeof(FrameworkElement)))
                {
                    (Content as FrameworkElement).IsEnabled = value;
                }
            }
        }

        public int Index { get; private set; }

        public List<SidebarSubEntry> SubEntries { get; private set; }

        public bool HasChildren { get => SubEntries?.Count > 0; }
        
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
            if (content != null) {
                (Content as Panel).Name = localisationName;
            }
            Index = index;
        }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="localisationName">The name of the resource inside the localisation files</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="index">The index of this entry</param>
        /// <param name="isEnabled">Toggles whether this item is enabled by default or not</param>
        public SidebarEntry(string localisationName, PackIconKind icon, object content, int index, bool isEnabled)
            : this(localisationName, icon, content, index)
        {
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Creates a new item for the sidebar.
        /// </summary>
        /// <param name="localisationName">The name of the resource inside the localisation files</param>
        /// <param name="icon">The icon to display in the list</param>
        /// <param name="content">The content which should be displayed in a content presenter</param>
        /// <param name="index">The index of this entry</param>
        /// <param name="isGhost">Determines if this entry is visible or not in the sidebar</param>
        public SidebarEntry(string localisationName, PackIconKind icon, object content, int index, Visibility isGhost)
            : this(localisationName, icon, content, index)
        {
            IsVisible = isGhost;
        }

        public void SetSubEntries(List<SidebarSubEntry> subEntries)
        {
            SubEntries = subEntries;
        }

        public void ToggleSubEntries(bool isOpen)
        {
            if (this.GetType() == typeof(SidebarEntry))
            {
                if (SubEntries?.Count > 0)
                {
                    foreach (SidebarSubEntry subEntry in SubEntries)
                    {
                        subEntry.IsVisible = isOpen ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
            else
            {
                (this as SidebarSubEntry).ParentEntry.ToggleSubEntries(isOpen);
            }
        }

        public bool MatchContentTag(string name)
        {
            return (Content as Panel).Name == name;
        }

        private void Instance_LanguageChanged(object sender, System.EventArgs e)
        {
            Name = Helper.FindString(localisationName);
            InvokePropertyChanged("Name");
        }
    }
}
