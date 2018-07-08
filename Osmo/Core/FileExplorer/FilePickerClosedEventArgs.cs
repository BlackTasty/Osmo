using System.Windows;

namespace Osmo.Core.FileExplorer
{
    public class FilePickerClosedEventArgs : RoutedEventArgs
    {
        private string path;

        public string Path { get => path; }

        public FilePickerClosedEventArgs(RoutedEvent routedEvent, string path) : base(routedEvent)
        {
            this.path = path;
        }
    }
}
