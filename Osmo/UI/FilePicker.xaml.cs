using MaterialDesignThemes.Wpf;
using Osmo.Core.FileExplorer;
using Osmo.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for FilePicker.xaml
    /// </summary>
    public partial class FilePicker : Grid
    {
        //TODO: Add InitialDirectory property

        public static readonly RoutedEvent DialogClosedEvent = EventManager.RegisterRoutedEvent(
            "DialogClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FilePicker));

        public event RoutedEventHandler DialogClosed
        {
            add { AddHandler(DialogClosedEvent, value); }
            remove { RemoveHandler(DialogClosedEvent, value); }
        }
        
        private void RaiseDialogClosedEvent(string path)
        {
            FilePickerClosedEventArgs eventArgs = new FilePickerClosedEventArgs(FilePicker.DialogClosedEvent, path);
            RaiseEvent(eventArgs);
        }

        #region IsFolderSelect
        public bool IsFolderSelect
        {
            get { return (bool)GetValue(IsFolderSelectProperty); }
            set
            {
                (DataContext as FilePickerViewModel).IsFolderSelect = value;
                SetValue(IsFolderSelectProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsFolderSelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFolderSelectProperty =
            DependencyProperty.Register("IsFolderSelect", typeof(bool), typeof(FilePicker), 
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsFolderSelectChanged)));
        #endregion

        #region Filter


        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FilePicker),
                new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnFilterChanged)));
        #endregion

        #region Title
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FilePicker), new PropertyMetadata(""));
        #endregion
        
        private static void OnIsFolderSelectChanged(DependencyObject depO, DependencyPropertyChangedEventArgs e)
        {
            if (depO is FilePicker filePicker)
            {
                (filePicker.DataContext as FilePickerViewModel).IsFolderSelect = Convert.ToBoolean(e.NewValue);
            }
        }

        private static void OnFilterChanged(DependencyObject depO, DependencyPropertyChangedEventArgs e)
        {
            if (depO is FilePicker filePicker)
            {
                (filePicker.DataContext as FilePickerViewModel).SetFilters(Convert.ToString(e.NewValue));
            }
        }

        public FilePicker()
        {
            InitializeComponent();
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;

            if (item.DataContext is FolderEntry entry)
            {
                entry.LoadFoldersOnDemand();
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;

            if (item.DataContext is FolderEntry entry)
            {
                entry.LoadFilesOnDemand();
                FilePickerViewModel vm = DataContext as FilePickerViewModel;
                vm.SelectedFolder = entry;
                vm.SelectedEntry = entry;
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as FilePickerViewModel).RefreshDrives();
        }

        private void FileView_Details_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileView_DoubleClick();
        }

        private void StepUpHierarchy_Click(object sender, RoutedEventArgs e)
        {
            FilePickerViewModel vm = DataContext as FilePickerViewModel;
            vm.SelectedFolder = vm.SelectedFolder.Parent;
        }

        private void FileView_Symbols_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileView_DoubleClick();
        }

        private void FileView_DoubleClick()
        {
            FilePickerViewModel vm = DataContext as FilePickerViewModel;

            if (vm.SelectedIndex > -1)
            {
                if (vm.SelectedEntry is FolderEntry folder)
                {
                    if (folder != null)
                    {
                        folder.LoadAllOnDemand();
                        (DataContext as FilePickerViewModel).SelectedFolder = folder;
                    }
                }
                else if (vm.SelectedEntry is FileEntry file)
                {
                    RaiseDialogClosedEvent(file.Path);
                    if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                        DialogHost.CloseDialogCommand.Execute(null, null);
                }
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            RaiseDialogClosedEvent((DataContext as FilePickerViewModel).SelectedEntry.Path);
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            RaiseDialogClosedEvent(null);
        }
    }
}
