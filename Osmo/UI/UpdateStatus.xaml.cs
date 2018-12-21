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
    /// Interaction logic for UpdateStatus.xaml
    /// </summary>
    public partial class UpdateStatus : DockPanel
    {
        #region CurrentToolTip
        public string CurrentToolTip
        {
            get { return (string)GetValue(CurrentToolTipProperty); }
            set { SetValue(CurrentToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentToolTipProperty =
            DependencyProperty.Register("CurrentToolTip", typeof(string), typeof(UpdateStatus), new PropertyMetadata(""));
        #endregion

        public UpdateStatus()
        {
            InitializeComponent();
            (DataContext as UpdaterViewModel).ToolTipChanged += ToolTipChanged;
        }

        private void ToolTipChanged(object sender, string e)
        {
            CurrentToolTip = e;
        }
    }
}
