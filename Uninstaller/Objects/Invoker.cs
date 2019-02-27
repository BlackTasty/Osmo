using System.Windows.Controls;

namespace Uninstaller.Objects
{
    static class Invoker
    {
        public static void InvokeProgress(ProgressBar control, double value)
        {
            control.Dispatcher.Invoke(() =>
            {
                control.Value = value;
            });
        }

        public static void InvokeProgress(ProgressBar control, double value, double max)
        {
            control.Dispatcher.Invoke(() =>
            {
                control.IsIndeterminate = false;
                control.Maximum = max;
            });
            InvokeProgress(control, value);
        }

        public static void InvokeStatus(ProgressBar progress, TextBlock log, TextBlock status, string message)
        {
            progress.Dispatcher.Invoke(() =>
            {
                progress.Value += 1;
            });

            log.Dispatcher.Invoke(() =>
            {
                log.Text += string.Format("\n{0}", message);
                ScrollViewer parent = (ScrollViewer)log.Parent;
                parent.Dispatcher.Invoke(parent.ScrollToBottom);
            });

            status.Dispatcher.Invoke(() =>
            {
                status.Text = message;
            });
        }
    }
}
