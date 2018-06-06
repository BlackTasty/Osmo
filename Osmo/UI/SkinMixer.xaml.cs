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
    /// Interaction logic for SkinMixer.xaml
    /// </summary>
    public partial class SkinMixer : Grid
    {
        private static SkinMixer instance;
        AudioEngine audio;

        public static SkinMixer Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinMixer();

                return instance;
            }
        }

        private SkinMixer()
        {
            InitializeComponent();
            audio = new AudioEngine((AudioViewModel)DataContext);
        }

        //TODO: Remove method "Test" when done testing
        public void Test(Skin left, Skin right)
        {
            (DataContext as SkinMixerViewModel).SkinLeft = left;
            (DataContext as SkinMixerViewModel).SkinRight = right;
        }

        private void LeftSkin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SkinMixerViewModel vm = (SkinMixerViewModel)DataContext;

            if (lv_elementsLeft.SelectedIndex != -1)
                vm.SelectedElementLeft = (SkinElement)lv_elementsLeft.SelectedItem;
            else
                vm.SelectedElementLeft = new SkinElement();
        }

        private void RightSkin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SkinMixerViewModel vm = (SkinMixerViewModel)DataContext;

            if (lv_elementsRight.SelectedIndex != -1)
                vm.SelectedElementRight = (SkinElement)lv_elementsRight.SelectedItem;
            else
                vm.SelectedElementRight = new SkinElement();
        }

        private void ChangeList_Revert_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Reset" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to revert all changes made to this element?",
                "Revert changes?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SkinMixerViewModel vm = (SkinMixerViewModel)DataContext;
                SkinElement element = vm.SkinLeft.Elements.FirstOrDefault(x => x.Name.Equals(
                    (sender as Button).Tag)) ?? null;

                if (element != null)
                {
                    element.Reset();
                    if (element.Equals(vm.SelectedElementLeft))
                    {
                        vm.RefreshImage();
                    }
                }
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
