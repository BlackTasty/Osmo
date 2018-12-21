using Osmo.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Grid
    {
        private static About instance;

        private bool versionAdded;

        public static About Instance
        {
            get
            {
                if (instance == null)
                    instance = new About();
                return instance;
            }
        }       

        private About()
        {
            InitializeComponent();
        }

        string SessionID = App.SessionID.ToString();

        private void GitHubLink_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/BlackTasty/Osmo");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!versionAdded)
            {
                txt_header.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString();
                versionAdded = true;
            }
            string AboutSID = (string)(App.Current.Resources["about_sessionId"]);
            string AboutSIDFull = string.Format(AboutSID, SessionID);
            txt_session.Text = AboutSIDFull;
        }

        private void CopySessionID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(App.SessionID.ToString());
            snackbar.MessageQueue.Enqueue(Helper.FindString("snackbar_sessionId"));
        }

        private void OpenConsole_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                DebugConsole.Instance.ShowConsole();
            }
        }
    }
}
