using Osmo.Core;
using Osmo.Core.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for DebugConsole.xaml
    /// </summary>
    public partial class DebugConsole : Border, IConsole
    {
        private static DebugConsole _instance;
        private ConsoleHistory history = new ConsoleHistory(20);

        private string commandList;

        public static DebugConsole Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool VerboseLogging
        {
            get => false;
        }
        
        public bool IsWindowMode
        {
            get { return (bool)GetValue(IsWindowModeProperty); }
            set { SetValue(IsWindowModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWindowMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWindowModeProperty =
            DependencyProperty.Register("IsWindowMode", typeof(bool), typeof(DebugConsole), new PropertyMetadata(false));

        public DebugConsole()
        {
            InitializeComponent();
            if (_instance == null)
            {
                _instance = this;
            }
            commandList = Properties.Resources.ConsoleCommands;
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

            SetText(msg, logType, brush);
        }

        void SetText(string msg, LogType logType, SolidColorBrush brush)
        {
            Dispatcher.Invoke(() =>
            {
                TextRange tr = new TextRange(rtb_sessionLog.Document.ContentEnd, rtb_sessionLog.Document.ContentEnd)
                {
                    Text = msg
                };

                tr.ApplyPropertyValue(TextElement.TagProperty, logType.ToString());
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            });
        }

        private void rtb_sessionLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtb_sessionLog.ScrollToEnd();
        }

        private void CloseConsole_Click(object sender, RoutedEventArgs e)
        {
            if (!IsWindowMode)
            {
                Visibility = Visibility.Collapsed;
            }
            else if (Parent is Window window)
            {
                Instance.Visibility = Visibility.Collapsed;
                window.Close();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetCommand(txt_cmd.Text);
                txt_cmd.Text = "";
            }
            else if (e.Key == Key.Up)
            {
                txt_cmd.Text = history.Peek(true);
            }
            else if (e.Key == Key.Down)
            {
                txt_cmd.Text = history.Peek(false);
            }
        }

        private void GetCommand(string cmd)
        {
            Log("> " + cmd + "\n", LogType.NULL);

            SolidColorBrush brush = FixedValues.DEFAULT_BRUSH;
            string output = null;
            string[] args = cmd.ToLower().Split(' ');
            switch (args[0])
            {
                case "help":
                case "?":
                    output = commandList;
                    break;

                case "clear":
                    rtb_sessionLog.Document.Blocks.Clear();
                    break;

                case "wipelog":
                    File.Delete("Logs\\log.txt");
                    Logger.Instance.WriteLog("Log wiped!", LogType.CONSOLE);
                    break;

                case "wipecrashlog":
                    foreach (FileInfo fi in new DirectoryInfo("Logs").EnumerateFiles("*osmo_crashlog.txt"))
                    {
                        fi.Delete();
                    }
                    Logger.Instance.WriteLog("Crash logs deleted!", LogType.CONSOLE);
                    break;

                case "wipelogfolder":
                    Directory.Delete("Logs", true);
                    Directory.CreateDirectory("Logs");
                    Logger.Instance.WriteLog("Log folder wiped!", LogType.CONSOLE);
                    break;

                case "openlog":
                    Process.Start("Logs\\log.txt");
                    break;

                case "listprofiles":
                    Log("Profiles:\n", LogType.CONSOLE);
                    foreach (var profile in App.ProfileManager.Profiles)
                    {
                        if (profile.ProfileName == null)
                        {
                            Log("\tDefault", LogType.CONSOLE);
                        }
                        else
                        {
                            Log("\t" + profile.ProfileName, LogType.CONSOLE);
                        }

                        if (profile == App.ProfileManager.Profile)
                        {
                            Log(" (active)\n", LogType.CONSOLE);
                        }
                        else
                        {
                            Log("\n", LogType.CONSOLE);
                        }
                    }
                    break;

                case "printconfig":
                    string configPath = GetConfigPath(args);

                    if (configPath != null)
                    {
                        PrintFile(configPath);
                    }
                    else
                    {
                        MakeError(out brush, out output);
                        output = "Invalid configuration name!\n";
                    }
                    break;

                case "openconfig":
                    string configPathOpen = GetConfigPath(args);

                    if (configPathOpen != null)
                    {
                        Process.Start(configPathOpen);
                    }
                    else
                    {
                        MakeError(out brush, out output);
                        output = "Invalid configuration name!\n";
                    }
                    break;

                default:
                    MakeError(out brush, out output);
                    break;
            }

            history.Push(cmd);

            if (output != null && brush != null)
                SetText(output, LogType.NULL, brush);
        }

        private void MakeError(out SolidColorBrush brush, out string output)
        {
            brush = FixedValues.DEFAULT_BRUSH;
            output = "Unknown command!\n";
        }

        private string GetConfigPath(string[] args)
        {
            if (args.Length > 1)
            {
                var targetConfig = App.ProfileManager.GetProfileByName(args[1]);
                if (targetConfig != null)
                {
                    return targetConfig.FilePath;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return App.ProfileManager.Profile.FilePath;
            }
        }

        private void PrintFile(string path)
        {
            if (File.Exists(path))
            {
                Log("---------------------------------------\n", LogType.CONSOLE);
                string[] content = File.ReadAllLines(path);

                string msg = string.Format("{0} content:", new FileInfo(path).Name);
                for (int i = 0; i < content.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(content[i]))
                        msg += "\n  " + content[i];
                }

                Log(msg + "\n---------------------------------------\n\n", LogType.CONSOLE);
            }
            else
            {
                Log("File does not exist!\n", LogType.CONSOLE);
            }
        }

        private void Popout_Click(object sender, RoutedEventArgs e)
        {
            if (!IsWindowMode)
            {
                new ConsoleWindow().Show();
            }
            else if (Parent is Window window)
            {
                window.Close();
            }
        }
    }
}
