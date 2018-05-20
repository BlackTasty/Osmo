using Microsoft.Win32;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for SkinEditor.xaml
    /// </summary>
    public partial class SkinEditor : Grid
    {
        private static SkinEditor instance;
        private string lastPath = null;
        private AudioEngine audio;

        public static SkinEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinEditor();
                return instance;
            }
        }

        private SkinEditor()
        {
            InitializeComponent();
            audio = new AudioEngine();
        }

        public void LoadSkin(Skin skin)
        {
            ((SkinViewModel)DataContext).LoadedSkin = skin;
        }

        private void lv_elements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SkinViewModel vm = (SkinViewModel)DataContext;
            vm.SelectedElement = (SkinElement)lv_elements.SelectedItem;

            audio.StopAudio();
            vm.PlayStatus = 0;

            vm.ShowIcon = vm.SelectedElement.FileType == FileType.Image ? Visibility.Hidden : Visibility.Visible;
            if (vm.ShowIcon == Visibility.Visible)
            {
                switch (vm.SelectedElement.FileType)
                {
                    case FileType.Audio:
                        vm.Icon = MaterialDesignThemes.Wpf.PackIconKind.FileMusic;
                        break;
                    case FileType.Configuration:
                        vm.Icon = MaterialDesignThemes.Wpf.PackIconKind.FileXml;
                        break;
                    case FileType.Unknown:
                        vm.Icon = MaterialDesignThemes.Wpf.PackIconKind.File;
                        break;
                }
            }
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            SkinViewModel vm = DataContext as SkinViewModel;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = GetFileFilter(vm.SelectedElement.FileType),
                InitialDirectory = vm.LoadedSkin.Path,
                Title = "Select a file as a replacement..."
            };

            if (!string.IsNullOrWhiteSpace(lastPath))
            {
                openFileDialog.InitialDirectory = lastPath;
            }
            
            if (openFileDialog.ShowDialog() == true)
            {
                File.Copy(openFileDialog.FileName, vm.SelectedElement.Path, true);
                ((SkinViewModel)DataContext).RefreshImage();

                //Save the last visited directory
                lastPath = Path.GetDirectoryName(openFileDialog.FileName);
                ((SkinViewModel)DataContext).SelectedElement.MadeChanges = true;
            }
        }

        private string GetFileFilter(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Audio:
                    return "Supported audio files|*.mp3;*.wav;*.ogg";
                case FileType.Configuration:
                    return "Supported configuration files|*.ini";
                case FileType.Image:
                    return "Supported image files|*.jpg;*.jpeg;*.png";
                default:
                    return "";
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Reset" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to revert all changes made to this image?",
                "Revert changes?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SkinViewModel vm = (SkinViewModel)DataContext;
                string path = AppConfiguration.GetInstance().BackupDirectory + 
                    vm.LoadedSkin.Name;
                File.Copy(path + vm.SelectedElement.Name, vm.SelectedElement.Path, true);
                vm.RefreshImage();
                vm.SelectedElement.MadeChanges = false;
            }
        }

        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Erase" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to erase the image?", 
                "Erase image?", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Exclamation, 
                MessageBoxResult.No);

            string path = ((SkinViewModel)DataContext).SelectedElement.Path;

            if (result == MessageBoxResult.Yes)
            {
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(
                        new BitmapImage(new Uri("pack://application:,,,/Osmo;component/Resources/empty.png", UriKind.Absolute))));
                    encoder.Save(stream);
                }
                ((SkinViewModel)DataContext).RefreshImage();
                ((SkinViewModel)DataContext).SelectedElement.MadeChanges = true;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Delete" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to delete this element?",
                "Erase image?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SkinElement element = ((SkinViewModel)DataContext).SelectedElement;
                if (lv_elements.SelectedIndex < lv_elements.Items.Count - 1)
                    lv_elements.SelectedIndex++;
                else
                    lv_elements.SelectedIndex--;
                File.Delete(element.Path);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            audio.PlayAudio((DataContext as SkinViewModel).SelectedElement.Path);
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            audio.StopAudio();
        }
    }
}
