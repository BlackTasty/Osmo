using Uninstaller.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Uninstaller.UI
{
    /// <summary>
    /// Interaction logic for AbortingDialog.xaml
    /// </summary>
    public partial class AbortingDialog : DockPanel
    {
        public AbortingDialog()
        {
            InitializeComponent();
            DataContext = AbortingViewModel.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
