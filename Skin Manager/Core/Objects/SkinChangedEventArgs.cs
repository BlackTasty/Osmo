using System.IO;

namespace Osmo.Core.Objects
{
    /// <summary>
    /// Required to properly update skins inside the <see cref="ViewModel.OsmoViewModel"/>
    /// </summary>
    public class SkinChangedEventArgs
    {
        private string path;
        private WatcherChangeTypes changeType;

        public string Path { get => path; }

        public WatcherChangeTypes ChangeType { get => changeType; }

        public SkinChangedEventArgs(string path, WatcherChangeTypes changeType)
        {
            this.path = path;
            this.changeType = changeType;
        }
    }
}
