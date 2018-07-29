using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.FileExplorer;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        //TODO: Try to fix dependency property of InitialDirectory (get/set and Changed event not fired when using Binding)
        #region DialogClosed Event
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
        #endregion

        #region DialogOpened Event
        public static readonly RoutedEvent DialogOpenedEvent = EventManager.RegisterRoutedEvent(
            "DialogOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FilePicker));

        public event RoutedEventHandler DialogOpened
        {
            add { AddHandler(DialogOpenedEvent, value); }
            remove { RemoveHandler(DialogOpenedEvent, value); }
        }

        private void RaiseDialogOpenedEvent()
        {
            RaiseEvent(new RoutedEventArgs(FilePicker.DialogOpenedEvent, this));
        }
        #endregion

        #region InitialDirectory
        public string InitialDirectory
        {
            get { return (string)GetValue(InitialDirectoryProperty); }
            set { SetValue(InitialDirectoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitialDirectory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register("InitialDirectory", typeof(string), typeof(FilePicker),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnInitialDirectoryChanged)));
        #endregion
        
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

        #region SuppressCloseCommand
        public bool SuppressCloseCommand
        {
            get { return (bool)GetValue(SuppressCloseCommandProperty); }
            set { SetValue(SuppressCloseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SuppressCloseCommanf.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuppressCloseCommandProperty =
            DependencyProperty.Register("SuppressCloseCommand", typeof(bool), typeof(FilePicker), new PropertyMetadata(false));
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

        #region IsLoading
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(FilePicker), new PropertyMetadata(false));
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

        private static void OnInitialDirectoryChanged(DependencyObject depO, DependencyPropertyChangedEventArgs e)
        {
            if (depO is FilePicker filePicker)
            {
                string path = Convert.ToString(e.NewValue);
                StructureBuilder structure = (filePicker.DataContext as FilePickerViewModel).SetCurrentDirectory(path);
                JumpToNodeAsync(filePicker, filePicker.tree.Items, structure);
            }
        }

        private static void JumpToNodeAsync(FilePicker filePicker, ItemCollection items, StructureBuilder structure)
        {
            filePicker.IsLoading = true;
            bool done = false;
            int layer = 0;
            
            List<FolderEntry> entries = items.OfType<FolderEntry>().ToList();
            new Thread(() =>
            {
                while (!done)
                {
                    FolderEntry entry = entries.FirstOrDefault(x => x.Path.Equals(structure.GetPathAtTreeLayer(layer)));

                    if (entry != null)
                    {
                        entry.IsExpanded = true;
                        entries = entry.SubDirectories.ToList();

                        if (Helper.NormalizePath(entry.Path).Equals(Helper.NormalizePath(structure.TargetPath)))
                        {
                            entry.IsSelected = true;
                            done = true;

                            filePicker.Dispatcher.Invoke(() =>
                            {
                                filePicker.IsLoading = false;
                            });
                        }

                        layer++;
                    }
                }
            }).Start();
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

            item.BringIntoView();

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
                    
                    if (!SuppressCloseCommand && DialogHost.CloseDialogCommand.CanExecute(null, null))
                        DialogHost.CloseDialogCommand.Execute(null, null);
                }
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            FilePickerViewModel vm = DataContext as FilePickerViewModel;
            if (vm.SelectedEntry != null)
            {
                RaiseDialogClosedEvent(vm.SelectedEntry.Path);
            }
            else if (vm.SelectedFolder != null)
            {
                RaiseDialogClosedEvent(vm.SelectedFolder.Path);
            }
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            RaiseDialogClosedEvent(null);
        }

        private void filePicker_Loaded(object sender, RoutedEventArgs e)
        {
            RaiseDialogOpenedEvent();
        }
    }
}
