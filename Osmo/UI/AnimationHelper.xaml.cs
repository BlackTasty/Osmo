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
            vm.Animation.Add(element.GetAnimatedElements());

            SkinningEntry details = element.ElementDetails as SkinningEntry;
            vm.FrameOrder = details.FrameOrder;
        }

        private void StartAnimation_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as AnimationViewModel).StartAnimation();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            (DataContext as AnimationViewModel).StopAnimation();
        }
    }
}
