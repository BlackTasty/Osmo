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
            LoadUISettings();
        }

        private void Configuration_SettingsSaved(object sender, EventArgs e)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;

            vm.BackupDirectory = configuration.BackupDirectory;
            if (!string.IsNullOrWhiteSpace(configuration.OsuDirectory))
            {
                vm.OsuDirectory = configuration.OsuDirectory;
            }
            LoadUISettings();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void sidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sidebarMenu.SelectedIndex != FixedValues.ABOUT_INDEX)
            {
                if (sidebarMenu.SelectedIndex != FixedValues.CONFIG_INDEX &&
                    !configuration.IsValid)
                {
                    sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;
                }
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

#if DEBUG
        private void sidebarMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (!configuration.IsValid)
                sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;

            dialg_newSkin.SetMasterViewModel(DataContext as OsmoViewModel);
        }
#else
        private async void sidebarMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppConfiguration.GetInstance().DisclaimerRead)
            {
                var msgBox = MaterialMessageBox.Show(Helper.FindString("alpha_disclaimer_title"),
                    string.Format("{0}\n\n{1}", Helper.FindString("alpha_disclaimer_description1"), Helper.FindString("alpha_disclaimer_description2")),
                    MessageBoxButton.YesNo);

                await DialogHost.Show(msgBox);

                if (msgBox.Result == MessageBoxResult.Yes)
                {
                    AppConfiguration.GetInstance().DisclaimerRead = true;
                    AppConfiguration.GetInstance().Save();
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            if (!configuration.IsValid)
                sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;

            dialg_newSkin.SetMasterViewModel(DataContext as OsmoViewModel);
        }
#endif

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
                var msgBox = MaterialMessageBox.Show(Helper.FindString("main_unsavedChangesTitle"),
                    Helper.FindString("main_unsavedChangesDescription"),
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
                var msgBox = MaterialMessageBox.Show(Helper.FindString("main_unsavedChangesTitle"),
                    Helper.FindString("main_unsavedChangesDescription"),
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

        private void SaveSkinOrTemplate_Click(object sender, RoutedEventArgs e)
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
            if (sidebarMenu.SelectedIndex == FixedValues.EDITOR_INDEX || sidebarMenu.SelectedIndex == FixedValues.MIXER_INDEX)
            {
                Helper.ExportSkin(sidebarMenu.SelectedIndex);
            }
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

        private async void RevertAll_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("main_revertAllTitle"),
                Helper.FindString("main_revertAllDescription"),
                MessageBoxButton.YesNoCancel);

            await DialogHost.Show(msgBox);
            if (msgBox.Result == MessageBoxResult.Yes)
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
            if (DialogHost.OpenDialogCommand.CanExecute(btn_import.CommandParameter, btn_import))
                DialogHost.OpenDialogCommand.Execute(btn_import.CommandParameter, btn_import);
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

        private void LoadUISettings()
        {
            Settings.ChangeLanguage(configuration.Language);
        }

        private void CreateForumEntry_Click(object sender, RoutedEventArgs e)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;
            TemplatePreviewViewModel templateVm = templatePreview.DataContext as TemplatePreviewViewModel;
            templateVm.LoadTemplates();
            if (vm.SelectedSidebarIndex == FixedValues.EDITOR_INDEX)
            {
                templateVm.Skin = SkinEditor.Instance.LoadedSkin;
            }
            else if (vm.SelectedSidebarIndex == FixedValues.MIXER_INDEX)
            {
                templateVm.Skin = SkinMixer.Instance.LoadedSkin;
            }
        }

        private void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (content.Content is IShortcutHelper target)
            {
                if (!target.ForwardKeyboardInput(e))
                {
                    e.Handled = true;
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        switch (e.Key)
                        {
                            case Key.S:
                                SaveSkinOrTemplate_Click(null, null);
                                break;
                            case Key.E:
                                ExportSkin_Click(null, null);
                                break;
                        }
                    }
                    else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Z)
                    {
                        RevertAll_Click(null, null);
                    }
                }
            }
        }
    }
}
