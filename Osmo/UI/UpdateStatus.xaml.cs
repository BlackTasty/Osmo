using MaterialDesignThemes.Wpf;
using Osmo.Core.Patcher;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        public PackIconKind CurrentIcon
        {
            get { return (PackIconKind)GetValue(CurrentIconProperty); }
            set { SetValue(CurrentIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentIconProperty =
            DependencyProperty.Register("CurrentIcon", typeof(PackIconKind), typeof(UpdateStatus), new PropertyMetadata(PackIconKind.CloudDownload));
        
        public bool IsIdle
        {
            get { return (bool)GetValue(IsIdleProperty); }
            set { SetValue(IsIdleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIdle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIdleProperty =
            DependencyProperty.Register("IsIdle", typeof(bool), typeof(UpdateStatus), new PropertyMetadata(true));

        public UpdateStatus()
        {
            InitializeComponent();
            (DataContext as UpdaterViewModel).FrontendChanged += FrontendChanged;
            (DataContext as UpdaterViewModel).UpdateManager.UpdateFound += UpdateManager_UpdateFound;
        }

        private void UpdateManager_UpdateFound(object sender, UpdateFoundEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                IsIdle = false;
            });
        }

        private void FrontendChanged(object sender, FrontendChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                CurrentToolTip = e.Tooltip;
                CurrentIcon = e.Icon;
            });
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((DataContext as UpdaterViewModel).Status)
            {
                case Core.Patcher.UpdateStatus.ERROR:
                    (DataContext as UpdaterViewModel).UpdateManager.RetryLastAction();
                    break;

                case Core.Patcher.UpdateStatus.UPDATES_FOUND:
                case Core.Patcher.UpdateStatus.READY:
                    MainWindow.Instance.HideUpdater();
                    break;
            }
        }
        
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((DataContext as UpdaterViewModel).Status)
            {
                case Core.Patcher.UpdateStatus.IDLE:
                    (DataContext as UpdaterViewModel).UpdateManager.CheckForUpdates();
                    break;
                case Core.Patcher.UpdateStatus.UPDATES_FOUND:
                    (DataContext as UpdaterViewModel).UpdateManager.DownloadUpdate();
                    break;
                case Core.Patcher.UpdateStatus.READY:
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Osmo.exe");
                    MainWindow.Instance.RequestShutdown();
                    break;
            }
        }
    }
}
