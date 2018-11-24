using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Uninstaller
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserControl activeControl;
        UserControl lastActiveControl;

        public MainWindow()
        {
            InitializeComponent();
            activeControl = startup;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            switch (activeControl.Name)
            {
                case "startup":
                    FadeControls(startup, uninstall, false, false);
                    uninstall.RegisterParent(this);
                    break;

                case "uninstall":
                    FadeControls(uninstall, finishedUninstall, true, false);
                    break;

                case "finishedUninstall":
                    Close();
                    break;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            FadeControls(activeControl, lastActiveControl, true, true);
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FadeControls(UserControl fadeOut, UserControl fadeIn, bool nextEnabled, bool backEnabled)
        {
            fadeOut.BeginAnimation(OpacityProperty, GetAnimation(1, 0));
            fadeIn.BeginAnimation(OpacityProperty, GetAnimation(0, 1));
            fadeOut.IsHitTestVisible = false;
            fadeIn.IsHitTestVisible = true;
            activeControl = fadeIn;
            lastActiveControl = fadeOut;
            btn_next.IsEnabled = nextEnabled;
            btn_back.IsEnabled = backEnabled;
        }
        private DoubleAnimation GetAnimation(double from, double to)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            return da;
        }
    }
}
