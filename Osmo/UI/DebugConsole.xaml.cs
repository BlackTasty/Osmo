using Osmo.Core;
using Osmo.Core.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for DebugConsole.xaml
    /// </summary>
    public partial class DebugConsole : Border, IConsole
    {
        private static DebugConsole _instance;

        public static DebugConsole Instance
        {
            get
            {
                return _instance;
            }
        }

        public DebugConsole()
        {
            InitializeComponent();
            _instance = this;
        }

        public void ShowConsole()
        {
            Visibility = Visibility.Visible;
        }

        public void Log(string msg, LogType logType)
        {
            SolidColorBrush brush;
            switch (logType)
            {
                case LogType.INFO:
                    brush = Brushes.Green;
                    break;

                case LogType.WARNING:
                    brush = Brushes.DarkOrange;
                    break;

                case LogType.ERROR:
                case LogType.FATAL:
                    brush = Brushes.Red;
                    break;

                case LogType.CONSOLE:
                    brush = Brushes.MediumVioletRed;
                    break;

                case LogType.DEBUG:
                    brush = Brushes.SlateBlue;
                    break;

                case LogType.VERBOSE:
                    brush = Brushes.LightGray;
                    break;

                default:
                    brush = FixedValues.DEFAULT_BRUSH;
                    break;
            }

            SetText(msg, brush);
        }

        void SetText(string msg, SolidColorBrush brush)
        {
            Dispatcher.Invoke(() =>
            {
                TextRange tr = new TextRange(rtb_sessionLog.Document.ContentEnd, rtb_sessionLog.Document.ContentEnd)
                {
                    Text = msg
                };
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            });
        }

        private void rtb_sessionLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtb_sessionLog.ScrollToEnd();
        }

        private void CloseConsole_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
