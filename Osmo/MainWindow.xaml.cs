using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.Core.Reader;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
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
            FixedValues.InitializeReader();
            SkinCreationWizard.Instance.SetMasterViewModel(DataContext as OsmoViewModel);
            TemplateManager.Instance.SetMasterViewModel(DataContext as OsmoViewModel);
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

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            
            configuration.Save();
            (SkinEditor.Instance.animationHelper.DataContext as AnimationViewModel).StopAnimation();

            OsmoViewModel vm = DataContext as OsmoViewModel;
            SkinViewModel skinVm = SkinEditor.Instance.DataContext as SkinViewModel;
            if (skinVm.UnsavedChanges)
            {
                vm.SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
                var msgBox = MaterialMessageBox.Show("You have unsaved changes!",
                    "You have unsaved changes! Do you want to save before closing? (Your changes will be remembered if you choose No)",
                    MessageBoxButton.YesNoCancel);

                await DialogHost.Show(msgBox);

                if (msgBox.Result == MessageBoxResult.Cancel)
                {
                    return;
                }
                else if (msgBox.Result == MessageBoxResult.Yes)
                {
                    skinVm.LoadedSkin.Save();
                }
            }

            SkinMixerViewModel mixerVm = SkinMixer.Instance.DataContext as SkinMixerViewModel;
            if (mixerVm.UnsavedChanges)
            {
                vm.SelectedSidebarIndex = FixedValues.MIXER_INDEX;
                var msgBox = MaterialMessageBox.Show("You have unsaved changes!",
                    "You have unsaved changes! Do you want to save before closing? (Your changes will be remembered if you choose No)",
                    MessageBoxButton.YesNoCancel);

                await DialogHost.Show(msgBox);

                if (msgBox.Result == MessageBoxResult.Cancel)
                {
                    return;
                }
                else if (msgBox.Result == MessageBoxResult.Yes)
                {
                    mixerVm.SkinLeft.Save();
                }
            }

            Environment.Exit(0);
        }

        private void SaveSkin_Click(object sender, RoutedEventArgs e)
        {
            if (sidebarMenu.SelectedIndex == FixedValues.EDITOR_INDEX)
            {
                SkinEditor.Instance.SaveSkin();
            }
            else if (sidebarMenu.SelectedIndex == FixedValues.MIXER_INDEX)
            {
                SkinMixer.Instance.SaveSkin();
            }
            else if (sidebarMenu.SelectedIndex == FixedValues.TEMPLATE_EDITOR_INDEX)
            {
                TemplateEditor.Instance.SaveTemplate();
            }
        }

        private void ExportSkin_Click(object sender, RoutedEventArgs e)
        {
            Helper.ExportSkin(sidebarMenu.SelectedIndex);
        }

        private void OpenInFileExplorer_OnClick(object sender, RoutedEventArgs e)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;
            if (vm.SelectedSidebarIndex == FixedValues.EDITOR_INDEX)
            {
                Process.Start((SkinEditor.Instance.DataContext as SkinViewModel).LoadedSkin.Path);
            }
            else if (vm.SelectedSidebarIndex == FixedValues.MIXER_INDEX)
            {
                Process.Start((SkinMixer.Instance.DataContext as SkinMixerViewModel).SkinLeft.Path);
            }
        }

        private void RevertAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to revert all changes made to this skin? This cannot be undone!",
                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                OsmoViewModel vm = DataContext as OsmoViewModel;
                if (vm.SelectedSidebarIndex == FixedValues.EDITOR_INDEX)
                {
                    (SkinEditor.Instance.DataContext as SkinViewModel).LoadedSkin.RevertAll();
                }
                else if (vm.SelectedSidebarIndex == FixedValues.MIXER_INDEX)
                {
                    (SkinMixer.Instance.DataContext as SkinMixerViewModel).SkinLeft.RevertAll();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (DialogHost.OpenDialogCommand.CanExecute(btn_import.CommandParameter, null))
                DialogHost.OpenDialogCommand.Execute(btn_import.CommandParameter, null);
        }

        private void OpenInSkinMixer_Click(object sender, RoutedEventArgs e)
        {
            SkinMixer.Instance.LoadSkin((SkinEditor.Instance.DataContext as SkinViewModel).LoadedSkin, true);
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.MIXER_INDEX;
        }

        private void OpenInSkinEditor_Click(object sender, RoutedEventArgs e)
        {
            SkinEditor.Instance.LoadSkin((SkinMixer.Instance.DataContext as SkinMixerViewModel).SkinLeft);
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
        }
    }
}
