using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.ViewModel;
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
    /// Interaction logic for SkinMixer.xaml
    /// </summary>
    public partial class SkinMixer : Grid
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

        private SkinMixer()
        {
            InitializeComponent();
            audio = new AudioEngine((AudioViewModel)DataContext);
        }

        internal void LoadSkin(Skin skin, bool isLeft)
        {
            if (isLeft)
            {
                (DataContext as SkinMixerViewModel).SkinLeft = skin;
            }
            else
            {
                (DataContext as SkinMixerViewModel).SkinRight = skin;
            }
        }

        internal void SaveSkin()
        {
            ((SkinMixerViewModel)DataContext).SkinLeft.Save();
        }

        internal void ExportSkin(string targetDir, bool alsoSave)
        {
            if (alsoSave)
            {
                ((SkinMixerViewModel)DataContext).SkinLeft.Save();
            }

            ((SkinMixerViewModel)DataContext).SkinLeft.Export(targetDir);
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

        private void ChangeList_Revert_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Reset" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to revert all changes made to this element?",
                "Revert changes?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
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
                    audio.StopAudio();
                }
            }
            else
            {
                if (vm.AudioPlayingRight)
                {
                    vm.AudioPlayingRight = false;
                    audio.StopAudio();
                }
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            AppConfiguration.GetInstance().IsMuted = cb_mute.IsChecked == true;
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
                AppConfiguration.GetInstance().Volume = slider_volume.Value;
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
            if ((DataContext as SkinMixerViewModel).AudioPlayingLeft)
            {
                StopAudio(true);
            }
            else
            {
                StopAudio(false);
                audio.PlayAudio((DataContext as SkinMixerViewModel).SelectedElementLeft.Path);
                if (cb_mute.IsChecked == true)
                    audio.SetVolume(0);
                else
                    audio.SetVolume(slider_volume.Value);
                (DataContext as SkinMixerViewModel).AudioPlayingLeft = true;
            }
        }

        private void PlaybackToggleRight_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as SkinMixerViewModel).AudioPlayingRight)
            {
                StopAudio(false);
            }
            else
            {
                StopAudio(true);
                audio.PlayAudio((DataContext as SkinMixerViewModel).SelectedElementRight.Path);
                if (cb_mute.IsChecked == true)
                    audio.SetVolume(0);
                else
                    audio.SetVolume(slider_volume.Value);
                (DataContext as SkinMixerViewModel).AudioPlayingRight = true;
            }
        }

        private void LoadRightSkin_Click(object sender, RoutedEventArgs e)
        {
            skinSelect.Visibility = Visibility.Visible;
        }

        private void MoveElement_Click(object sender, RoutedEventArgs e)
        {
            SkinMixerViewModel vm = DataContext as SkinMixerViewModel;

            vm.SelectedElementLeft.ReplaceBackup(new System.IO.FileInfo(vm.SelectedElementRight.Path));
            StopAudio(true);
            vm.RefreshImage();
        }

        private void RevertSelected_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Reset" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to revert all changes made to this element?",
                "Revert changes?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
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
    }
}
