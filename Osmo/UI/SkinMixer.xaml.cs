using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.FileExplorer;
using Osmo.Core.Objects;
using Osmo.ViewModel;
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
    /// Interaction logic for SkinMixer.xaml
    /// </summary>
    public partial class SkinMixer : Grid, IShortcutHelper
    {
        private static SkinMixer instance;
        AudioEngine audio;

        public static SkinMixer Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinMixer();

                return instance;
            }
        }

        public Skin LoadedSkin { get => (DataContext as SkinMixerViewModel).SkinLeft; }

        public bool ForwardKeyboardInput(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.T:
                        e.Handled = true;
                        if (btn_transfer.IsEnabled)
                        {
                            MoveElement_Click(null, null);
                        }
                        break;
                    case Key.O:
                        e.Handled = true;
                        Helper.ExecuteDialogOpenCommand(btn_loadRight);
                        DialogHelper.Instance.NotifyDialogOpened(btn_loadRight);
                        break;
                    case Key.Z:
                        e.Handled = true;
                        if (btn_revert.IsEnabled)
                        {
                            RevertSelected_Click(null, null);
                        }
                        break;
                }
            }

            return e.Handled;
        }

        private SkinMixer()
        {
            InitializeComponent();
            audio = new AudioEngine((AudioViewModel)DataContext);
        }

        internal async Task<bool> LoadSkin(Skin skin, bool isLeft)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;

            if (isLeft)
            {
                if (LoadedSkin != null && !LoadedSkin.Equals(new Skin()) && LoadedSkin != skin && LoadedSkin.UnsavedChanges)
                {
                    var msgBox = MaterialMessageBox.Show(Helper.FindString("main_unsavedChangesTitle"),
                        Helper.FindString("main_unsavedChangesDescription"),
                        OsmoMessageBoxButton.YesNoCancel);

                    await DialogHelper.Instance.ShowDialog(msgBox);

                    if (msgBox.Result == OsmoMessageBoxResult.Cancel)
                    {
                        return false;
                    }
                    else if (msgBox.Result == OsmoMessageBoxResult.Yes)
                    {
                        vm.SkinLeft.Save();
                    }
                }

                vm.SkinLeft = skin;
            }
            else
            {
                vm.SkinRight = skin;
            }

            return true;
        }

        internal void SaveSkin()
        {
            ((SkinMixerViewModel)DataContext).SkinLeft.Save();
            snackbar.MessageQueue.Enqueue(Helper.FindString("snackbar_saveText"), Helper.FindString("snackbar_saveButton"),
                param => DialogHost.Show(FindResource("folderPicker") as FilePicker,
                delegate (object sender, DialogOpenedEventArgs args)
                {
                    DialogHelper.Instance.NotifyDialogOpened(args.Session);
                }), false, true);
        }

        internal void ExportSkin(string targetDir, bool alsoSave)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;
            if (alsoSave)
            {
                vm.SkinLeft.Save();
            }

            vm.SkinLeft.Export(targetDir);

            vm.ExportSkin(targetDir, alsoSave);
            snackbar.MessageQueue.Enqueue(Helper.FindString("snackbar_exportText"), Helper.FindString("snackbar_exportButton"),
                param => Process.Start(targetDir), false, true);
        }

        private void LeftSkin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;

            if (lv_elementsLeft.SelectedIndex != -1)
                vm.SelectedElementLeft = (SkinElement)lv_elementsLeft.SelectedItem;
            else
                vm.SelectedElementLeft = new SkinElement();

            StopAudio(true);

            vm.ShowIconLeft = vm.SelectedElementLeft.FileType == FileType.Image ? Visibility.Hidden : Visibility.Visible;
            if (vm.ShowIconLeft == Visibility.Visible)
            {
                vm.IconLeft = GetIconKind(vm.SelectedElementLeft);
            }
        }

        private void RightSkin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SkinMixerViewModel vm = (SkinMixerViewModel)DataContext;

            if (lv_elementsRight.SelectedIndex != -1)
                vm.SelectedElementRight = (SkinElement)lv_elementsRight.SelectedItem;
            else
                vm.SelectedElementRight = new SkinElement();

            StopAudio(false);

            vm.ShowIconRight = vm.SelectedElementRight.FileType == FileType.Image ? Visibility.Hidden : Visibility.Visible;
            if (vm.ShowIconRight == Visibility.Visible)
            {
                vm.IconRight = GetIconKind(vm.SelectedElementRight);
            }
        }

        private async void ChangeList_Revert_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("edit_revertTitle"),
                Helper.FindString("edit_revertDescription"), OsmoMessageBoxButton.YesNo);

            await DialogHelper.Instance.ShowDialog(msgBox);

            if (msgBox.Result == OsmoMessageBoxResult.Yes)
            {
                StopAudio(true);
                SkinMixerViewModel vm = (SkinMixerViewModel)DataContext;
                SkinElement element = vm.SkinLeft.Elements.FirstOrDefault(x => x.Name.Equals(
                    (sender as Button).Tag)) ?? null;

                if (element != null)
                {
                    element.Reset();
                    if (element.Equals(vm.SelectedElementLeft))
                    {
                        vm.RefreshImage();
                    }
                }
            }
        }

        private void StopAudio(bool stopLeft)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;

            if (stopLeft)
            {
                if (vm.AudioPlayingLeft)
                {
                    vm.AudioPlayingLeft = false;
                    vm.PlayStatusLeft = 0;
                    audio.StopAudio();
                }
            }
            else
            {
                if (vm.AudioPlayingRight)
                {
                    vm.AudioPlayingRight = false;
                    vm.PlayStatusRight = 0;
                    audio.StopAudio();
                }
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            RecallConfiguration.Instance.IsMuted = cb_mute.IsChecked == true;
            if (cb_mute.IsChecked == true)
                audio.SetVolume(0);
            else
                audio.SetVolume(slider_volume.Value);
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audio != null)
            {
                cb_mute.IsChecked = false;
                RecallConfiguration.Instance.Volume = slider_volume.Value;
                audio.SetVolume(slider_volume.Value);
            }
        }

        private PackIconKind GetIconKind(SkinElement element)
        {
            switch (element.FileType)
            {
                case FileType.Audio:
                    return PackIconKind.FileMusic;
                case FileType.Configuration:
                    return PackIconKind.FileXml;
                case FileType.Image:
                    return PackIconKind.FileImage;
                default:
                    return PackIconKind.File;
            }
        }

        private void PlaybackToggleLeft_Click(object sender, RoutedEventArgs e)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;
            if (vm.AudioPlayingLeft)
            {
                if (vm.AudioEnded)
                {
                    audio.PlayAudio(vm.SelectedElementLeft.Path);
                }
                else
                {
                    audio.PauseAudio();
                }
            }
            else
            {
                StopAudio(false);
                audio.PlayAudio(vm.SelectedElementLeft.Path);
                if (cb_mute.IsChecked == true)
                    audio.SetVolume(0);
                else
                    audio.SetVolume(slider_volume.Value);
                vm.AudioPlayingLeft = true;
            }
        }

        private void PlaybackStopLeft_Click(object sender, RoutedEventArgs e)
        {
            StopAudio(true);
        }

        private void PlaybackStopRight_Click(object sender, RoutedEventArgs e)
        {
            StopAudio(false);
        }

        private void PlaybackToggleRight_Click(object sender, RoutedEventArgs e)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;
            if (vm.AudioPlayingRight)
            {
                if (vm.AudioEnded)
                {
                    audio.PlayAudio(vm.SelectedElementRight.Path);
                }
                else
                {
                    audio.PauseAudio();
                }
            }
            else
            {
                StopAudio(true);
                audio.PlayAudio(vm.SelectedElementRight.Path);
                if (cb_mute.IsChecked == true)
                    audio.SetVolume(0);
                else
                    audio.SetVolume(slider_volume.Value);
                vm.AudioPlayingRight = true;
            }
        }

        //private void LoadRightSkin_Click(object sender, RoutedEventArgs e)
        //{
        //    skinSelect.Visibility = Visibility.Visible;
        //}

        private void MoveElement_Click(object sender, RoutedEventArgs e)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;

            vm.SelectedElementLeft.ReplaceBackup(new System.IO.FileInfo(vm.SelectedElementRight.Path));
            StopAudio(true);
            vm.RefreshImage();
        }

        private async void RevertSelected_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("edit_revertTitle"),
                Helper.FindString("edit_revertDescription"), OsmoMessageBoxButton.YesNo);

            await DialogHelper.Instance.ShowDialog(msgBox);

            if (msgBox.Result == OsmoMessageBoxResult.Yes)
            {
                StopAudio(true);
                SkinMixerViewModel vm = (SkinMixerViewModel)DataContext;
                vm.SelectedElementLeft.Reset();
                /*string path = AppConfiguration.GetInstance().BackupDirectory + "\\" + 
                    vm.LoadedSkin.Name + "\\";
                File.Copy(path + vm.SelectedElement.Name, vm.SelectedElement.Path, true);*/
                vm.RefreshImage();
            }
        }

        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            ForwardKeyboardInput(e);
        }

        private void FolderPicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

            if (args.Path != null)
            {
                Helper.ExportSkin(args.Path, FixedValues.EDITOR_INDEX, true);
            }
            DialogHelper.Instance.NotifyDialogClosed();
        }

        private void Slider_Audio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue % 1 != 0)
            {
                audio.SetPosition((sender as Slider).Value);
            }
        }

        private void SkinMixerSelect_Loaded(object sender, RoutedEventArgs e)
        {
            DialogHelper.Instance.NotifyDialogOpened(btn_loadRight);
        }

        private void SkinMixerSelect_DialogClosed(object sender, EventArgs e)
        {
            DialogHelper.Instance.NotifyDialogOpened(btn_loadRight);
        }
    }
}
