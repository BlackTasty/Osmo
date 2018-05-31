using MahApps.Metro.Controls;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Osmo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        AppConfiguration configuration = AppConfiguration.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
            configuration.SettingsSaved += Configuration_SettingsSaved;
        }

        private void Configuration_SettingsSaved(object sender, EventArgs e)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;

            vm.BackupDirectory = configuration.BackupDirectory;
            vm.OsuDirectory = configuration.OsuDirectory;
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void sidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (SkinEditor.Instance.DataContext as SkinViewModel).SelectedElement = new SkinElement();

            if (sidebarMenu.SelectedIndex != FixedValues.CONFIG_INDEX && !configuration.IsValid)
            {
                sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;
            }


            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void sidebarMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (!configuration.IsValid)
                sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;

            dialg_newSkin.SetMasterViewModel(DataContext as OsmoViewModel);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO: Implement dialog asking the user if he really wants to close the application without saving
            configuration.Save();
        }

        private void SaveSkin_Click(object sender, RoutedEventArgs e)
        {
            SkinEditor.Instance.SaveSkin();
        }
    }
}
