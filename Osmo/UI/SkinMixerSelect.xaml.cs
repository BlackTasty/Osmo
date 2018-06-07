using Osmo.Core.Objects;
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
    /// Interaction logic for SkinMixerSelect.xaml
    /// </summary>
    public partial class SkinMixerSelect : Grid
    {
        public SkinMixerSelect()
        {
            InitializeComponent();
        }

        private void Skins_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadSkin();
        }

        private void SelectSkin_Click(object sender, RoutedEventArgs e)
        {
            LoadSkin();
        }

        private void LoadSkin()
        {
            SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin, false);
            Visibility = Visibility.Hidden;
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
