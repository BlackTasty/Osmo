namespace Osmo.Core.Configuration
{
    public class ProfileEventArgs
    {
        private AppConfiguration sender;

        public AppConfiguration Profile { get => sender; }

        public string ProfileName { get => sender.ProfileName; }

        public ProfileEventArgs(AppConfiguration sender)
        {
            this.sender = sender;
        }
    }
}
