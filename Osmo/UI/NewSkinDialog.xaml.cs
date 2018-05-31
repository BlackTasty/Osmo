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
    /// Interaction logic for NewSkinDialog.xaml
    /// </summary>
    public partial class NewSkinDialog : DockPanel
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
    }
}
