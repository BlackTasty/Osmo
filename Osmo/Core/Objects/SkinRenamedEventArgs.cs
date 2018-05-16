namespace Osmo.Core.Objects
{
    /// <summary>
    /// Required to properly update skins inside the <see cref="ViewModel.OsmoViewModel"/>
    /// </summary>
    public class SkinRenamedEventArgs : SkinChangedEventArgs
    {
        private string pathAfter;
        public string PathAfter { get => pathAfter; }

        public SkinRenamedEventArgs(string pathBefore, string pathAfter) : 
            base(pathBefore, System.IO.WatcherChangeTypes.Renamed)
        {
            this.pathAfter = pathAfter;
        }
    }
}
