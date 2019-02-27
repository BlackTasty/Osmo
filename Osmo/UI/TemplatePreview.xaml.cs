using MaterialDesignThemes.Wpf;
using Osmo.Core;
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
    /// Interaction logic for TemplatePreview.xaml
    /// </summary>
    public partial class TemplatePreview : Grid, IHotkeyHelper
    {
        public TemplatePreview()
        {
            InitializeComponent();
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

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            TemplatePreviewViewModel vm = DataContext as TemplatePreviewViewModel;
            if (!string.IsNullOrWhiteSpace(vm.PreviewText))
            {
                Clipboard.SetText(vm.PreviewText);
            }
        }
    }
}
