using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for NewSkinDialog.xaml
    /// </summary>
    public partial class NewSkinDialog : DockPanel, IHotkeyHelper
    {
        public NewSkinDialog()
        {
            InitializeComponent();
        }

        internal void SetMasterViewModel(OsmoViewModel vm)
        {
            (DataContext as NewSkinViewModel).Master = vm;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_create.IsEnabled = !string.IsNullOrWhiteSpace(txt_author.Text) && !string.IsNullOrWhiteSpace(txt_name.Text);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            SkinCreationWizard.Instance.ApplyData(DataContext as NewSkinViewModel);
            (DataContext as NewSkinViewModel).Master.SelectedSidebarIndex = 1;
        }

        public bool ForwardKeyboardInput(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                    DialogHost.CloseDialogCommand.Execute(null, null);
            }

            return e.Handled;
        }

        private void DockPanel_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ForwardKeyboardInput(e);
        }
    }
}
