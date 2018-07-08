using Microsoft.Win32;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.FileExplorer;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public AppConfiguration Configuration
        {
            get => AppConfiguration.GetInstance();
        }

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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            AppConfiguration config = AppConfiguration.GetInstance();

            config.BackupBeforeMixing = (bool)cb_backupSkin.IsChecked;
            config.BackupDirectory = txt_backupPath.Text;
            config.OsuDirectory = txt_osuPath.Text;
            config.ReopenLastSkin = (bool)cb_reopenLastSkin.IsChecked;
            config.Language = (Language)combo_language.SelectedIndex;


            config.Save();

            string message = Helper.FindString("snackbar_settingsSavedText");
            snackbar.MessageQueue.Enqueue(message, Helper.FindString("ok"), 
                param => Trace.WriteLine("Actioned: " + param), message, false, true);
            //Task.Factory.StartNew(() => snackbar.MessageQueue.Enqueue("Your settings have been saved!"));
        }

        private void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add message box asking the user if the folder should be automatically detected
            FilePicker folderPicker = FindResource("folderPicker") as FilePicker;
            folderPicker.Tag = (sender as Button).Tag;


            switch ((sender as Button).Tag)
            {
                case "osu":
                    folderPicker.Title = Helper.FindString("settings_selectOsuDirectory");
                    break;
                case "backup":
                    folderPicker.Title = Helper.FindString("settings_selectBackupDirectory");
                    break;
            }
        }

        public static void ChangeLanguage(Language lang)
        {
            ((App)Application.Current).ChangeLanguage(lang);
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            AppConfiguration config = AppConfiguration.GetInstance();

            cb_backupSkin.IsChecked = config.BackupBeforeMixing;
            txt_backupPath.Text = config.BackupDirectory;
            txt_osuPath.Text = config.OsuDirectory;
            cb_reopenLastSkin.IsChecked = config.ReopenLastSkin;
            combo_language.SelectedIndex = (int)config.Language;
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        private void FolderPicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            if (sender is FilePicker folderPicker)
            {
                FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

                if (args.Path != null)
                {
                    switch (folderPicker.Tag)
                    {
                        case "osu":
                            txt_osuPath.Text = args.Path;
                            break;
                        case "backup":
                            txt_backupPath.Text = args.Path;
                            break;
                    }
                }
            }
        }
    }
}
