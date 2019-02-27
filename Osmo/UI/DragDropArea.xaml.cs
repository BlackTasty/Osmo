using Osmo.Core;
using Osmo.Core.FileExplorer;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for DragDropArea.xaml
    /// </summary>
    public partial class DragDropArea : Grid
    {
        private FileInfo tempOskPath;
        private double origWidth;
        private double origHeight;

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

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            if (tempOskPath != null)
            {
                FileInfo oskPath = tempOskPath;
                Close();

                var skin = await Skin.Import(oskPath);

                if (!skin.IsEmpty)
                {
                    (DataContext as OsmoViewModel).Skins.Add(skin);
                }
            }
        }

        private void Close()
        {
            tempOskPath = null;
            OskPath = null;
            DialogHelper.Instance.NotifyDialogClosed();
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            origWidth = Width;
            origHeight = Height;
            filePicker.Visibility = Visibility.Visible;
            dragDrop.Margin = new Thickness(0);
            Width = filePicker.ActualWidth + 32;
            Height = filePicker.ActualHeight + 32;
            dropArea.Visibility = Visibility.Hidden;
            btn_cancel.Visibility = Visibility.Hidden;
            btn_ok.Visibility = Visibility.Hidden;
            filePicker.Filter = string.Format("{0}|*.osk", Helper.FindString("dragDrop_filter"));
        }

        private void CancelSkin_Click(object sender, RoutedEventArgs e)
        {
            tempOskPath = null;
            OskPath = null;
        }

        private void FilePicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            Width = origWidth;
            Height = origHeight;
            filePicker.Visibility = Visibility.Hidden;
            dragDrop.Margin = new Thickness(16);
            dropArea.Visibility = Visibility.Visible;
            btn_cancel.Visibility = Visibility.Visible;
            btn_ok.Visibility = Visibility.Visible;

            FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

            if (args.Path != null)
            {
                tempOskPath = new FileInfo(args.Path);
                OskPath = tempOskPath.Name;
            }
        }

        private void dragDrop_Loaded(object sender, RoutedEventArgs e)
        {
            DialogHelper.Instance.NotifyDialogOpened();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
