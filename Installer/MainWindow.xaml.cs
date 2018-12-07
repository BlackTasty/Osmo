using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using Installer.ViewModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Installer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserControl activeControl;
        UserControl lastActiveControl;

        public ComponentsViewModel ViewModel { get; set; }
        
        public bool IsUpgrade { get; set; }

        public MainWindow()
        {
            InitializeComponent();
#if NIGHTLY
            textBlock.Text += " NIGHTLY";
#endif

            activeControl = agreement;
            DataContext = AbortingViewModel.Instance;
            (DataContext as AbortingViewModel).AbortInstallationClicked += MainWindow_AbortInstallationClicked;
        }

        private void MainWindow_AbortInstallationClicked(object sender, EventArgs e)
        {
            if (activeControl.Name == install.Name)
            {
                install.AbortInstallation();
                btn_cancel.IsEnabled = false;
                FadeControls(activeControl, aborted, true, false);
            }
            else
            {
                Close();
            }
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
                case "agreement":
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player", false);
                    IsUpgrade = key != null;
                    if (IsUpgrade)
                        FadeControls(agreement, appInstalled, true, true);
                    else
                        FadeControls(agreement, components, true, true);
                    break;

                case "appInstalled":
                    if (appInstalled.rb_uninstall.IsChecked == true)
                    {
                        FadeControls(appInstalled, uninstall, false, false);
                        uninstall.RegisterParent(this);
                    }
                    else
                        FadeControls(appInstalled, components, true, true);
                    break;

                case "components":
                    FadeControls(components, install, false, false);
                    install.RegisterParent(this);
                    break;

                case "install":
                    if (!install.Aborted)
                        FadeControls(install, finished, true, false, true);
                    else
                        FadeControls(install, aborted, true, false, true);
                    break;

                case "uninstall":
                    FadeControls(uninstall, finishedUninstall, true, false, true);
                    break;

                case "aborted":
                    Close();
                    break;

                case "finished":
                    if (finished.cb_runAfter.IsChecked == true)
                    {
                        Objects.GlobalValues GV = new Objects.GlobalValues();
                        Process.Start(GV.InstallationPath + "Vibrance Player.exe");
                    }
                    Close();
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

        private void FadeControls(UserControl fadeOut, UserControl fadeIn, bool nextEnabled, bool backEnabled, bool isFinish = false)
        {
            fadeOut.BeginAnimation(OpacityProperty, GetAnimation(1, 0));
            fadeIn.BeginAnimation(OpacityProperty, GetAnimation(0, 1));
            fadeOut.IsHitTestVisible = false;
            fadeIn.IsHitTestVisible = true;
            activeControl = fadeIn;
            lastActiveControl = fadeOut;
            btn_next.IsEnabled = nextEnabled;
            btn_back.IsEnabled = backEnabled;

            if (isFinish)
                btn_next.Content = "Finish";

            if (fadeIn.Name == "components")
                ShowUACIcon(true);
            else
                ShowUACIcon(false);
        }

        public void ShowUACIcon(bool isShown)
        {
            if (isShown)
                uac.Source = Objects.Helper.GetUACIcon();
            else
                uac.Source = null;

            if (uac.Source != null)
                uac.Visibility = Visibility.Visible;
            else
                uac.Visibility = Visibility.Collapsed;
        }
        
        private DoubleAnimation GetAnimation(double from, double to)
        {
            DoubleAnimation da = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            return da;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            components.RegisterParent(this);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
#if DEBUG
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                UserControl nextControl = null;
                bool reset = false;

                switch (e.Key)
                {
                    case Key.D1:
                        nextControl = appInstalled;
                        break;
                    case Key.D2:
                        nextControl = components;
                        break;
                    case Key.D3:
                        nextControl = install;
                        break;
                    case Key.D4:
                        nextControl = finished;
                        break;
                    case Key.D5:
                        nextControl = uninstall;
                        break;
                    case Key.D6:
                        nextControl = finishedUninstall;
                        break;
                    case Key.D7:
                        nextControl = aborted;
                        break;
                    default:
                        if (activeControl != agreement)
                        {
                            nextControl = agreement;
                            reset = true;
                        }
                        break;
                }

                if (nextControl != null && nextControl != activeControl)
                {
                    FadeControls(activeControl, nextControl, reset, false);

                    if (reset)
                    {
                        lastActiveControl = null;
                    }
                }
            }
#endif
        }

        //private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        //{
        //    ToggleButton toggle = (ToggleButton)sender;
        //    ((App)Application.Current).ChangeBaseTheme(toggle.IsChecked ?? false);
        //}
    }
}
