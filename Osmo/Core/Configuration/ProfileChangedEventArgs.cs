using Osmo.Core.Configuration;

namespace Osmo.Core.Configuration
{
    public class ProfileChangedEventArgs 
    {
        private AppConfiguration oldProfile;
        private AppConfiguration newProfile;

        public AppConfiguration OldProfile { get => oldProfile; }

        public AppConfiguration NewProfile { get => newProfile; }

        public ProfileChangedEventArgs(AppConfiguration oldProfile, AppConfiguration newProfile)
        {
            this.oldProfile = oldProfile;
            this.newProfile = newProfile;
        }
    }
}
