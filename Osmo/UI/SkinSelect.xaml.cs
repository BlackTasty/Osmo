using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Objects;
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
    /// Interaction logic for SkinSelect.xaml
    /// </summary>
    public partial class SkinSelect : Grid
    {
        private static SkinSelect instance;

        public static SkinSelect Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinSelect();
                return instance;
            }
        }

        private SkinSelect()
        {
            InitializeComponent();
            FixedValues.InitializeReader();
        }

        private void LoadSkin_Click(object sender, RoutedEventArgs e)
        {
            SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin);
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
        }

        private void MixSkins_Click(object sender, RoutedEventArgs e)
        {
            SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin);
            (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.MIXER_INDEX;
        }

        private void Skins_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_skins.SelectedIndex > 0)
            {
                SkinEditor.Instance.LoadSkin(lv_skins.SelectedItem as Skin);
                (DataContext as OsmoViewModel).SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
            }
        }

        private void SkinDelete_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Delete" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Are you sure you want to delete this skin?",
                "Delete skin?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                (DataContext as OsmoViewModel).SkinManager.DeleteSkin((sender as Button).Tag.ToString());
            }
        }

        private void Skins_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lv_skins.SelectedIndex == 0)
            {
                //TODO: Implement "New skin" 
                if (DialogHost.OpenDialogCommand.CanExecute(null, null))
                    DialogHost.OpenDialogCommand.Execute(null, null);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (Skin skin in (DataContext as OsmoViewModel).Skins)
            //{
            //    uniGrid_skins.Children.Add(new SkinCard().InitializeSkin(skin));
            //}
        }
    }
}
