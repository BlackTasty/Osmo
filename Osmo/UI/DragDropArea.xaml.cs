using Microsoft.Win32;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for DragDropArea.xaml
    /// </summary>
    public partial class DragDropArea : Grid
    {
        private FileInfo tempOskPath;

        public bool IsOskPathOkay
        {
            get { return (bool)GetValue(IsOskPathOkayProperty); }
            set { SetValue(IsOskPathOkayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOskPathOkay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOskPathOkayProperty =
            DependencyProperty.Register("IsOskPathOkay", typeof(bool), typeof(DragDropArea), new PropertyMetadata(false));



        public string OskPath
        {
            get { return (string)GetValue(OskPathProperty); }
            set
            {
                SetValue(OskPathProperty, value);
                IsOskPathOkay = !string.IsNullOrWhiteSpace(value);
                if (IsOskPathOkay)
                    dropArea.Cursor = null;
                else
                    dropArea.Cursor = Cursors.Hand;
            }
        }

        // Using a DependencyProperty as the backing store for OskPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OskPathProperty =
            DependencyProperty.Register("OskPath", typeof(string), typeof(DragDropArea), new PropertyMetadata(null));

        public DragDropArea()
        {
            InitializeComponent();
        }

        private void Control_DragLeave(object sender, DragEventArgs e)
        {
            Close();
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            OskPath = tempOskPath.Name;
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (files != null)
            {
                foreach (string path in files)
                {
                    FileInfo fi = new FileInfo(path);
                    if (fi.Extension.Equals(".osk"))
                    {
                        e.Effects = DragDropEffects.Link;
                        tempOskPath = fi;
                        break;
                    }
                }
            }
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (tempOskPath != null)
            {
                var skin = Skin.Import(tempOskPath);

                if (!skin.IsEmpty)
                {
                    (DataContext as OsmoViewModel).Skins.Add(skin);
                }

                Close();
            }
        }

        private void Close()
        {
            tempOskPath = null;
            OskPath = null;
            Visibility = Visibility.Collapsed;
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Executable skin file.|*.osk",
                Title = "Select an osu skin file to import..."
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tempOskPath = new FileInfo(openFileDialog.FileName);
                OskPath = tempOskPath.Name;
            }
        }

        private void CancelSkin_Click(object sender, RoutedEventArgs e)
        {
            tempOskPath = null;
            OskPath = null;
        }
    }
}
