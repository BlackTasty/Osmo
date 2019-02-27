namespace Osmo.Core.Patcher
{
    public class UpdateFoundEventArgs
    {
        private string versionCurrent;
        private string versionNew;

        public string VersionCurrent { get => versionCurrent; }

        public string VersionNew { get => versionNew; }

        public UpdateFoundEventArgs(string versionCurrent, string versionNew)
        {
            this.versionCurrent = versionCurrent;
            this.versionNew = versionNew;
        }
    }
}