using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace Uninstaller
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserControl activeControl;
        UserControl lastActiveControl;
        string path;

        public MainWindow()
        {
            InitializeComponent();
            activeControl = startup;

            RegistryKey edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player");
            if (edgeKey == null)
                Close();
            else
                path = edgeKey.GetValue("Path").ToString();
            edgeKey.Close();

            var app = (App)Application.Current;
            app.ChangeTheme(GetThemeUri());
        }

        private Uri GetThemeUri()
        {
            if (File.Exists(path + "settings.cfg"))
            {
                string[] content = File.ReadAllLines(path + "settings.cfg");
                foreach (string str in content)
                {
                    string[] setting = str.Split(':');
                    if (setting[0].Equals("Theme"))
                    {
                        switch (setting[1])
                        {
                            case "0":
                                return new Uri("Themes/Dark.xaml", UriKind.Relative);
                            case "1":
                                return new Uri("Themes/Light.xaml", UriKind.Relative);
                            case "2":
                                return new Uri("Themes/Edge.xaml", UriKind.Relative);
                            case "3":
                                return new Uri("Themes/Osu.xaml", UriKind.Relative);
                        }
                    }
                }

                return null;
            }
            else
                return null;
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
