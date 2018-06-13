using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
    /// Interaction logic for SkinSelect.xaml
    /// </summary>
    public partial class SkinSelect : Grid
    {
        private static SkinSelect instance;

        public static SkinSelect Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinSelect();
                return instance;
            }
        }

        private SkinSelect()
        {
            InitializeComponent();
            FixedValues.InitializeReader();
        }

        private void LoadSkin_Click(object sender, RoutedEventArgs e)
        {
            SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin);
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
        }

        private void MixSkins_Click(object sender, RoutedEventArgs e)
        {
            SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin, true);
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.MIXER_INDEX;
        }

        private void Skins_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_skins.SelectedIndex > 0)
            {
                SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin);
                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
            }
        }

        private async void SkinDelete_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show("Delete skin?",
                "Are you sure you want to delete this skin?",
                MessageBoxButton.YesNo);

            await DialogHost.Show(msgBox);

            if (msgBox.Result == MessageBoxResult.Yes)
            {
                (DataContext as OsmoViewModel).SkinManager.DeleteSkin((sender as Button).Tag.ToString());
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace OpenFolderDialog with custom FilePicker control (and remove Winforms dependency)
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "Select the directory you want to export your skin to"
            })
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    (DataContext as OsmoViewModel).SkinManager.ExportSkin((sender as Button).Tag.ToString(), 
                        dlg.SelectedPath);
                }
            }
        }

        private void Skins_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lv_skins.SelectedIndex == 0)
            {
                if (DialogHost.OpenDialogCommand.CanExecute(btn_newSkin.CommandParameter, null))
                    DialogHost.OpenDialogCommand.Execute(btn_newSkin.CommandParameter, null);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (Skin skin in (DataContext as OsmoViewModel).Skins)
            //{
            //    uniGrid_skins.Children.Add(new SkinCard().InitializeSkin(skin));
            //}
        }
    }
}
