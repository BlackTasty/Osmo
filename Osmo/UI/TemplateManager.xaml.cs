using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for TemplateManager.xaml
    /// </summary>
    public partial class TemplateManager : Grid
    {
        #region Singleton implementation
        private static TemplateManager instance;

        public static TemplateManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TemplateManager();

                return instance;
            }
        }
        #endregion

        private TemplateManager()
        {
            InitializeComponent();
        }

        private void Templates_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Templates_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private async void TemplateDelete_Click(object sender, RoutedEventArgs e)
        {
            var msgBox = MaterialMessageBox.Show("Delete template?",
                "Are you sure you want to delete this template?",
                MessageBoxButton.YesNo);

            await DialogHost.Show(msgBox);

            if (msgBox.Result == MessageBoxResult.Yes)
            {
                (DataContext as TemplateManagerViewModel).DeleteTemplate((sender as Button).Tag.ToString());
            }
        }
    }
}
