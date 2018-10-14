using MaterialDesignThemes.Wpf;
using Osmo.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for CreateProfile.xaml
    /// </summary>
    public partial class CreateProfile : DockPanel, IHotkeyHelper
    {
        public CreateProfile()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_create.IsEnabled = !string.IsNullOrWhiteSpace(txt_osuDir.Text) && !string.IsNullOrWhiteSpace(txt_name.Text);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            App.ProfileManager.CreateProfile(txt_name.Text, txt_osuDir.Text);
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
