using Microsoft.Win32;
using Osmo.Core.Configuration;
using System;
using System.Collections.Generic;
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
            config.PlaySoundWhenHovering = (bool)cb_playSoundOnHover.IsChecked;
            config.ReopenLastSkin = (bool)cb_reopenLastSkin.IsChecked;
            config.Save();

            snackbar.MessageQueue.Enqueue("Your settings have been saved!");
            //Task.Factory.StartNew(() => snackbar.MessageQueue.Enqueue("Your settings have been saved!"));
        }

        private void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace OpenFolderDialog with custom FilePicker control (and remove Winforms dependency)
            //TODO: Add message box asking the user if the folder should be automatically detected
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "Select your osu! directory"
            })
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    switch((sender as Button).Tag)
                    {
                        case "osu":
                            txt_osuPath.Text = dlg.SelectedPath;
                            break;
                        case "backup":
                            txt_backupPath.Text = dlg.SelectedPath;
                            break;
                    }
                }
            }
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
            cb_playSoundOnHover.IsChecked = config.PlaySoundWhenHovering;
            cb_reopenLastSkin.IsChecked = config.ReopenLastSkin;
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }
    }
}
