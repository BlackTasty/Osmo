using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Osmo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const int CONFIG_INDEX = 2;

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
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void sidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (SkinEditor.Instance.DataContext as SkinViewModel).SelectedElement = new SkinElement();

            if (sidebarMenu.SelectedIndex != CONFIG_INDEX && !configuration.IsValid)
            {
                sidebarMenu.SelectedIndex = CONFIG_INDEX;
            }
        }

        private void sidebarMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (!configuration.IsValid)
                sidebarMenu.SelectedIndex = CONFIG_INDEX;
        }
    }
}
