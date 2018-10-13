using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.FileExplorer;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Grid
    {
        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                    instance = new Settings();
                return instance;
            }
        }

        private Settings()
        {
            InitializeComponent();
        }

        public void SaveSettings()
        {
            if ((DataContext as SettingsViewModel).SelectedProfileIndex > 0)
            {
                App.ProfileManager.DefaultProfile.ProfilePath = App.ProfileManager.Profile.FilePath;
            }
            else
            {
                App.ProfileManager.DefaultProfile.ProfilePath = "";
            }

            App.ProfileManager.DefaultProfile.Save();
            App.ProfileManager.Profile.Save();

            string message = Helper.FindString("snackbar_settingsSavedText");
            snackbar.MessageQueue.Enqueue(message, Helper.FindString("ok"), 
                param => Trace.WriteLine("Actioned: " + param), message, false, true);
            //Task.Factory.StartNew(() => snackbar.MessageQueue.Enqueue("Your settings have been saved!"));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add message box asking the user if the folder should be automatically detected
            FilePicker folderPicker = (sender as Button).CommandParameter as FilePicker;

            switch (folderPicker.Tag)
            {
                case "osu":
                    folderPicker.InitialDirectory = txt_osuPath.Text;
                    break;
                case "backup":
                    folderPicker.InitialDirectory = txt_backupPath.Text;
                    break;
            }
        }

        public static void ChangeLanguage(Language lang)
        {
            ((App)Application.Current).ChangeLanguage(lang);
        }

        private async void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            bool exitAutoDetect;
            do
            {
                exitAutoDetect = true;
                SettingsViewModel vm = DataContext as SettingsViewModel;
                if (string.IsNullOrWhiteSpace(vm.OsuDirectory) || !Directory.Exists(vm.OsuDirectory))
                {
                    if (Helper.IsOsuInstalled())
                    {
                        var msgBox = MaterialMessageBox.Show(Helper.FindString("settings_title_autodetectOsu"),
                            Helper.FindString("settings_descr_autodetectOsu"), OsmoMessageBoxButton.YesNo);

                        await DialogHelper.Instance.ShowDialog(msgBox);

                        if (msgBox.Result == OsmoMessageBoxResult.Yes)
                        {
                            string osuPath = Helper.FindOsuInstallation();

                            if (!string.IsNullOrWhiteSpace(osuPath))
                            {
                                vm.OsuDirectory = osuPath;
                            }
                            else
                            {
                                msgBox = MaterialMessageBox.Show(Helper.FindString("settings_title_autodetectFailed"),
                                    Helper.FindString("settings_descr_autodetectFailed"), OsmoMessageBoxButton.OKRetry);

                                await DialogHelper.Instance.ShowDialog(msgBox);

                                if (msgBox.Result == OsmoMessageBoxResult.Retry)
                                {
                                    exitAutoDetect = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        var msgBox = MaterialMessageBox.Show(Helper.FindString("settings_title_autodetectNoOsuInstalled"),
                            Helper.FindString("settings_descr_autodetectNoOsuInstalled"), OsmoMessageBoxButton.OKRetry);

                        await DialogHelper.Instance.ShowDialog(msgBox);

                        if (msgBox.Result == OsmoMessageBoxResult.Retry)
                        {
                            exitAutoDetect = false;
                        }
                    }
                }
            } while (!exitAutoDetect); 
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            App.ProfileManager.Profile.Load();
        }

        private void FolderPicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            if (sender is FilePicker folderPicker)
            {
                FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

                if (args.Path != null)
                {
                    SettingsViewModel vm = DataContext as SettingsViewModel;
                    switch (folderPicker.Tag)
                    {
                        case "osu":
                            vm.OsuDirectory = args.Path;
                            break;
                        case "backup":
                            vm.BackupDirectory = args.Path;
                            break;
                        case "template":
                            vm.TemplateDirectory = args.Path;
                            break;
                    }
                }
            }
        }

        private void FolderPicker_DialogOpened(object sender, RoutedEventArgs e)
        {
            switch ((sender as FilePicker).Tag)
            {
                case "osu":
                    (sender as FilePicker).InitialDirectory = (DataContext as SettingsViewModel).OsuDirectory;
                    break;
                case "backup":
                    (sender as FilePicker).InitialDirectory = (DataContext as SettingsViewModel).BackupDirectory;
                    break;
                case "template":
                    (sender as FilePicker).InitialDirectory = (DataContext as SettingsViewModel).TemplateDirectory;
                    break;
            }
        }

        private async void RemoveProfile_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("settings_title_profileRemove"), 
                Helper.FindString("settings_descr_profileRemove"), OsmoMessageBoxButton.YesNo);

            if (await DialogHelper.Instance.ShowDialog(msgBox) == OsmoMessageBoxResult.Yes)
            {
                App.ProfileManager.RemoveProfile((DataContext as SettingsViewModel).SelectedProfile.ProfileName);
            }
        }
    }
}
