using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.Core.Reader;
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
    /// Interaction logic for AnimationHelper.xaml
    /// </summary>
    public partial class AnimationHelper : Grid
    {
        public AnimationHelper()
        {
            InitializeComponent();
        }

        public void LoadAnimation(SkinElement element)
        {
            AnimationViewModel vm = DataContext as AnimationViewModel;
            vm.Animation.Clear();
            
            vm.Animation.Add(element.GetAnimatedElements(), false);
            vm.CurrentFrame = 0;

            SkinningEntry details = element.ElementDetails as SkinningEntry;
            vm.FrameOrder = details.FrameOrder;
        }

        private void StartStopAnimation_Click(object sender, RoutedEventArgs e)
        {
            AnimationViewModel vm = DataContext as AnimationViewModel;
            if (!vm.IsAnimationPlaying)
                vm.StartAnimation();
            else
                vm.StopAnimation();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            (DataContext as AnimationViewModel).StopAnimation();
        }

        private void PreviousFrame_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as AnimationViewModel).PreviousFrame();
        }

        private void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as AnimationViewModel).NextFrame();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as AnimationViewModel).StopAnimation();
        }
    }
}
