using MaterialDesignThemes.Wpf;
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

            if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void SelectSkin_Click(object sender, RoutedEventArgs e)
        {
            LoadSkin();
        }

        private void LoadSkin()
        {
            if (Tag != null)
            {
                string target = Tag.ToString();
                if (target.Equals("mixer"))
                {
                    SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin, false);
                }
                else if (target.Equals("template"))
                {
                    TemplateEditor.Instance.MakePreview(lv_skins.SelectedItem as Skin);
                }
            }
        }
    }
}
