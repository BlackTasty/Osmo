using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.FileExplorer;
using Osmo.Core.Logging;
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
        public MainWindow()
        {
            FixedValues.LoadCompletionData();
            FixedValues.InitializeReader();
            InitializeComponent();
            App.ProfileManager.ProfileChanged += ProfileManager_ProfileChanged;
            App.ProfileManager.SettingsSaved += Configuration_SettingsSaved;
            App.LanguageChanged += App_LanguageChanged;
            SkinCreationWizard.Instance.SetMasterViewModel(DataContext as OsmoViewModel);
            TemplateManager.Instance.SetMasterViewModel(DataContext as OsmoViewModel);
            LoadUISettings();
            Logger.Instance.WriteLog("Osmo has been successfully initialized!");
        }

        private void ProfileManager_ProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            Logger.Instance.WriteLog("Profile has changed!", LogType.CONSOLE);
            LoadSettings(false);
        }

        private void App_LanguageChanged(object sender, EventArgs e)
        {
            FixedValues.LoadCompletionData();
            SkinEditor.Instance.LoadCompletionData();
        }

        private void Configuration_SettingsSaved(object sender, ProfileEventArgs e)
        {
            Logger.Instance.WriteLog("Reloading osu! directory...", LogType.CONSOLE);
            LoadSettings(true);
        }

        private void LoadSettings(bool refreshDirectories)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;

            if (refreshDirectories)
            {
                vm.BackupDirectory = App.ProfileManager.Profile.BackupDirectory;
                if (!string.IsNullOrWhiteSpace(App.ProfileManager.Profile.OsuDirectory))
                {
                    vm.OsuDirectory = App.ProfileManager.Profile.OsuDirectory;
                }
            }
            LoadUISettings();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private async void sidebarMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sidebarMenu.SelectedIndex != FixedValues.ABOUT_INDEX)
            {
                if (sidebarMenu.SelectedIndex != FixedValues.CONFIG_INDEX &&
                    !App.ProfileManager.Profile.IsValid)
                {
                    sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;
                }
            }

            if (e.RemovedItems.Count > 0 && (e.RemovedItems[0] as SidebarEntry).Index == FixedValues.CONFIG_INDEX &&
                App.ProfileManager.Profile.UnsavedChanges)
            {
                var msgBox = MaterialMessageBox.Show(Helper.FindString("main_unsavedChangesTitle"),
                    Helper.FindString("settings_unsavedChangesDescription"),
                    OsmoMessageBoxButton.YesNoCancel);

                await DialogHelper.Instance.ShowDialog(msgBox);

                if (msgBox.Result == OsmoMessageBoxResult.Yes)
                {
                    Settings.Instance.SaveSettings();
                }
                else if (msgBox.Result == OsmoMessageBoxResult.No)
                {
                    App.ProfileManager.Profile.Load();
                }
                else
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
        
        private void sidebarMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ProfileManager.Profile.IsValid)
                sidebarMenu.SelectedIndex = FixedValues.CONFIG_INDEX;
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.Instance.WriteLog("Application shutdown initialized...");
            e.Cancel = true;
            
            (SkinEditor.Instance.animationHelper.DataContext as AnimationViewModel).StopAnimation();
            OsmoViewModel vm = DataContext as OsmoViewModel;

            if (App.ProfileManager.Profile.UnsavedChanges)
            {
                vm.SelectedSidebarIndex = FixedValues.CONFIG_INDEX;
                var msgBox = MaterialMessageBox.Show(Helper.FindString("main_unsavedChangesTitle"),
                    Helper.FindString("settings_unsavedChangesDescription"),
                    OsmoMessageBoxButton.YesNoCancel);

                await DialogHelper.Instance.ShowDialog(msgBox, true);

                if (msgBox.Result == OsmoMessageBoxResult.Cancel)
                {
                    Logger.Instance.WriteLog("Application shutdown aborted by user!");
                    return;
                }
                else if (msgBox.Result == OsmoMessageBoxResult.Yes)
                {
                    Settings.Instance.SaveSettings();
                }
            }

            SkinViewModel skinVm = SkinEditor.Instance.DataContext as SkinViewModel;
            if (skinVm.UnsavedChanges)
            {
                vm.SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
                var msgBox = MaterialMessageBox.Show(Helper.FindString("main_unsavedChangesTitle"),
                    Helper.FindString("main_unsavedChangesDescription"),
                    OsmoMessageBoxButton.YesNoCancel);

                await DialogHelper.Instance.ShowDialog(msgBox, true);

                if (msgBox.Result == OsmoMessageBoxResult.Cancel)
                {
                    Logger.Instance.WriteLog("Application shutdown aborted by user!");
                    return;
                }
                else if (msgBox.Result == OsmoMessageBoxResult.Yes)
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
                    OsmoMessageBoxButton.YesNoCancel);

                await DialogHelper.Instance.ShowDialog(msgBox, true);

                if (msgBox.Result == OsmoMessageBoxResult.Cancel)
                {
                    Logger.Instance.WriteLog("Application shutdown aborted by user!");
                    return;
                }
                else if (msgBox.Result == OsmoMessageBoxResult.Yes)
                {
                    mixerVm.SkinLeft.Save();
                }
            }

            RecallConfiguration recall = RecallConfiguration.Instance;
            if (App.ProfileManager.Profile.ReopenLastSkin)
            {
                recall.LastSkinPathEditor = SkinEditor.Instance.LoadedSkin?.Path;
                recall.LastSkinPathMixerLeft = SkinMixer.Instance.LoadedSkin?.Path;
                recall.LastSkinPathMixerRight = (SkinMixer.Instance.DataContext as SkinMixerViewModel).SkinRight?.Path;
            }
            else
            {
                recall.LastSkinPathEditor = null;
                recall.LastSkinPathMixerLeft = null;
                recall.LastSkinPathMixerRight = null;
            }

            recall.Save();

            Logger.Instance.WriteLog("Application closed with code 0!");
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

        private async void ShowSkinInfo_Click(object sender, RoutedEventArgs e)
        {
            Skin skin = sidebarMenu.SelectedIndex == FixedValues.EDITOR_INDEX ?
                SkinEditor.Instance.LoadedSkin : SkinMixer.Instance.LoadedSkin;

            var msgBox = MaterialMessageBox.Show(Helper.FindString("main_skinInfo"),
                string.Format("{0}\n\nInterface: {1}% SD; {2}% HD\n" +
                "Osu!: {3}% SD; {4}% HD\n" +
                "Mania: {5}% SD; {6}% HD\n" +
                "Taiko: {7}% SD; {8}% HD\n" +
                "CTB: {9}% SD; {10}% HD\n" +
                "Sounds: {11}%",
                Helper.FindString("skin_status_progress"), skin.ProgressInterface, skin.ProgressInterfaceHD,
                skin.ProgressOsu, skin.ProgressOsuHD,
                skin.ProgressMania, skin.ProgressManiaHD,
                skin.ProgressTaiko, skin.ProgressTaikoHD,
                skin.ProgressCTB, skin.ProgressCTBHD,
                skin.ProgressSounds), OsmoMessageBoxButton.OK);

            await DialogHelper.Instance.ShowDialog(msgBox);
        }

        private void OpenInFileExplorer_OnClick(object sender, RoutedEventArgs e)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;
            if (vm.SelectedSidebarIndex == FixedValues.EDITOR_INDEX)
            {
                Helper.OpenSkinInExplorer((SkinEditor.Instance.DataContext as SkinViewModel).LoadedSkin);
            }
            else if (vm.SelectedSidebarIndex == FixedValues.MIXER_INDEX)
            {
                Helper.OpenSkinInExplorer((SkinMixer.Instance.DataContext as SkinMixerViewModel).SkinLeft);
            }
        }

        private async void RevertAll_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("main_revertAllTitle"),
                Helper.FindString("main_revertAllDescription"),
                OsmoMessageBoxButton.YesNoCancel);

            await DialogHelper.Instance.ShowDialog(msgBox);
            if (msgBox.Result == OsmoMessageBoxResult.Yes)
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
            DialogHelper.Instance.NotifyDialogOpened(btn_import);
            if (DialogHost.OpenDialogCommand.CanExecute(btn_import.CommandParameter, btn_import))
                DialogHost.OpenDialogCommand.Execute(btn_import.CommandParameter, btn_import);
        }

        private async void OpenInSkinMixer_Click(object sender, RoutedEventArgs e)
        {
            if (await SkinMixer.Instance.LoadSkin(SkinEditor.Instance.LoadedSkin, true))
            {
                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.MIXER_INDEX;
            }
        }

        private async void OpenInSkinEditor_Click(object sender, RoutedEventArgs e)
        {
            if (await SkinEditor.Instance.LoadSkin(SkinMixer.Instance.LoadedSkin))
            {
                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
            }
        }

        private void LoadUISettings()
        {
            Settings.ChangeLanguage(App.ProfileManager.Profile.Language);
            Settings.ChangeBaseTheme(App.ProfileManager.Profile.DarkTheme);
            ColorZone test = new ColorZone();
            test.Mode = ColorZoneMode.Dark;
            Logger.Instance.WriteLog("UI specific settings loaded! Language: {0}", App.ProfileManager.Profile.Language);
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

        private async void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            bool inputHandled = false;

            if (content.Content is IHotkeyHelper target)
            {
                inputHandled = target.ForwardKeyboardInput(e);
            }
            else if (content.Content is IAsyncShortcutHelper asyncTarget)
            {
                inputHandled = await asyncTarget.ForwardKeyboardInput(e);
            }

            if (inputHandled)
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
                            if (DialogHost.OpenDialogCommand.CanExecute(btn_export.CommandParameter, btn_export))
                                DialogHost.OpenDialogCommand.Execute(btn_export.CommandParameter, btn_export);
                            break;
                    }
                }
                else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.Z)
                {
                    RevertAll_Click(null, null);
                }
            }
        }

        private void OpenInResizeTool_Click(object sender, RoutedEventArgs e)
        {
            OsmoViewModel vm = DataContext as OsmoViewModel;
            if (vm.SelectedSidebarIndex == FixedValues.EDITOR_INDEX)
            {
                ResizeTool.Instance.LoadSkin(SkinEditor.Instance.LoadedSkin);
            }
            else
            {
                ResizeTool.Instance.LoadSkin(SkinMixer.Instance.LoadedSkin);
            }
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.RESIZE_TOOL_INDEX;
        }

        private void FolderPicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

            if (args.Path != null)
            {
                Helper.ExportSkin(args.Path, sidebarMenu.SelectedIndex);
            }
        }

        private void DialogHost_Loaded(object sender, RoutedEventArgs e)
        {
            SkinSelect.Instance.SetOsmoViewModel(DataContext as OsmoViewModel);
#if !DEBUG
            ShowDisclaimer();
#endif
        }

        private async void ShowDisclaimer()
        {
            if (!App.ProfileManager.Profile.DisclaimerRead)
            {
                var msgBox = MaterialMessageBox.Show(Helper.FindString("alpha_disclaimer_title"),
                    string.Format("{0}\n\n{1}", Helper.FindString("alpha_disclaimer_description1"), Helper.FindString("alpha_disclaimer_description2")),
                    OsmoMessageBoxButton.YesNo);

                await DialogHelper.Instance.ShowDialog(msgBox);

                if (msgBox.Result == OsmoMessageBoxResult.Yes)
                {
                    App.ProfileManager.Profile.DisclaimerRead = true;
                    App.ProfileManager.Profile.Save();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        private void console_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                MinHeight += 200;
            }
            else
            {
                MinHeight -= 200;
                Height -= 200;
            }
        }

        private void console_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.Instance.AppendConsole(console);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
