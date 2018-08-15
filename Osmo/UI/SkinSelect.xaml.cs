using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Osmo.Core;
using Osmo.Core.FileExplorer;
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
    public partial class SkinSelect : Grid, IAsyncShortcutHelper
    {
        private static SkinSelect instance;
        private string exportName; //Only used when exporting skins

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

        private async void LoadSkin_Click(object sender, RoutedEventArgs e)
        {
            if (await SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin))
            {
                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
            }
        }

        private async void MixSkins_Click(object sender, RoutedEventArgs e)
        {
            if (await SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin, true))
            {
                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.MIXER_INDEX;
            }
        }

        private async void Skins_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_skins.SelectedIndex > 0)
            {
                if (await SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin))
                {
                    (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
                }
            }
        }

        private void SkinDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSkin((sender as Button).Tag.ToString());
        }

        private async void DeleteSkin(string name)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("skinSelect_deleteTitle"),
                Helper.FindString("skinSelect_deleteDescription"),
                OsmoMessageBoxButton.YesNo);

            await DialogHelper.Instance.ShowDialog(msgBox);

            if (msgBox.Result == OsmoMessageBoxResult.Yes)
            {
                (DataContext as OsmoViewModel).SkinManager.DeleteSkin(name);
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            ExportSkin((sender as Button).Tag.ToString());
        }

        private void ExportSkin(string name)
        {
            exportName = name;
        }

        private void FolderPicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

            if (args.Path != null)
            {
                (DataContext as OsmoViewModel).SkinManager.ExportSkin(exportName,
                    args.Path);
                exportName = null;
            }
        }

        private void Skins_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lv_skins.SelectedIndex == 0)
            {
                if (DialogHost.OpenDialogCommand.CanExecute(btn_newSkin.CommandParameter, btn_newSkin))
                    DialogHost.OpenDialogCommand.Execute(btn_newSkin.CommandParameter, btn_newSkin);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (Skin skin in (DataContext as OsmoViewModel).Skins)
            //{
            //    uniGrid_skins.Children.Add(new SkinCard().InitializeSkin(skin));
            //}
            dlg_newSkin.SetMasterViewModel(DataContext as OsmoViewModel);
        }

        public async Task<bool> ForwardKeyboardInput(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        e.Handled = true;
                        if (DialogHost.OpenDialogCommand.CanExecute(btn_newSkin.CommandParameter, btn_newSkin))
                            DialogHost.OpenDialogCommand.Execute(btn_newSkin.CommandParameter, btn_newSkin);
                        break;
                    case Key.O:
                        e.Handled = true;
                        if (lv_skins.SelectedIndex > 0)
                        {
                            if (await SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin))
                            {
                                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
                            }
                        }
                        break;
                    case Key.E:
                        if (lv_skins.SelectedIndex > 0)
                        {
                            ExportSkin((lv_skins.SelectedItem as Skin).Name);
                        }
                        break;
                }
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.O)
            {
                e.Handled = true;
                if (lv_skins.SelectedIndex > 0)
                {
                    if (await SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin, true))
                    {
                        (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.MIXER_INDEX;
                    }
                }
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                if (lv_skins.SelectedIndex > 0)
                {
                    DeleteSkin((lv_skins.SelectedItem as Skin).Name);
                }
            }

            return e.Handled;
        }

        private async void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            await ForwardKeyboardInput(e);
        }

        private void MenuItem_NewSkin_Click(object sender, RoutedEventArgs e)
        {
            if (DialogHost.OpenDialogCommand.CanExecute(btn_newSkin.CommandParameter, btn_newSkin))
                DialogHost.OpenDialogCommand.Execute(btn_newSkin.CommandParameter, btn_newSkin);
        }

        private void MenuItem_OpenInExplorer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_OpenInMixer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_OpenInEditor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ResizeTool_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Export_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
