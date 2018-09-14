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

namespace Osmo.Controls
{
    /// <summary>
    /// Interaction logic for ModeProgressImage.xaml
    /// </summary>
    public partial class ModeProgressImage : Image
    {
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set
            {
                SetValue(ProgressProperty, value);
                (DataContext as ModeProgressViewModel).Progress = value;
            }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ModeProgressImage), new PropertyMetadata(0d));



        public double ProgressHD
        {
            get { return (double)GetValue(ProgressHDProperty); }
            set
            {
                SetValue(ProgressHDProperty, value);
                (DataContext as ModeProgressViewModel).ProgressHD = value;
            }
        }

        // Using a DependencyProperty as the backing store for ProgressHD.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressHDProperty =
            DependencyProperty.Register("ProgressHD", typeof(double), typeof(ModeProgressImage), new PropertyMetadata(0d));

        public ModeProgressImage()
        {
            InitializeComponent();
        }
    }
}
