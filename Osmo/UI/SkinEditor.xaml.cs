using Microsoft.Win32;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SkinEditor.xaml
    /// </summary>
    public partial class SkinEditor : Grid
    {
        private static SkinEditor instance;

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
        }

        private void lv_elements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SkinViewModel)DataContext).SelectedElement = (SkinElement)lv_elements.SelectedItem;
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
            
            if (openFileDialog.ShowDialog() == true)
            {
                ((SkinViewModel)DataContext).ShowImage = false;
                System.Threading.Thread.Sleep(150);
                File.Copy(openFileDialog.FileName, vm.SelectedElement.Path, true);
                vm.SelectedElement.RefreshImage();
                ((SkinViewModel)DataContext).ShowImage = true;
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

        }

        private void Erase_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
