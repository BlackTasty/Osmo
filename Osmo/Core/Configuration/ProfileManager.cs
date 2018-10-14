using Osmo.Core.Logging;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Configuration
{
    public class ProfileManager
    {
        private AppConfiguration defaultProfile;
        private AppConfiguration activeProfile;

        public event EventHandler<ProfileChangedEventArgs> ProfileChanged;
        public event EventHandler<ProfileEventArgs> SettingsSaved;
        public event EventHandler<ProfileEventArgs> ProfileCreated;
        public event EventHandler<ProfileRenamedEventArgs> ProfileRenamed;

        /// <summary>
        /// Returns a reference to the current active profile
        /// </summary>
        public AppConfiguration Profile { get => activeProfile == null ? defaultProfile : activeProfile; }

        /// <summary>
        /// Returns a reference to the default profile
        /// </summary>
        public AppConfiguration DefaultProfile { get => defaultProfile; }

        /// <summary>
        /// Holds a list of profiles
        /// </summary>
        public VeryObservableCollection<AppConfiguration> Profiles { get; set; }
               = new VeryObservableCollection<AppConfiguration>("Profiles", false);

        public ProfileManager(AppConfiguration defaultProfile)
        {
            Logger.Instance.WriteLog("Initializing profile manager...");
            this.defaultProfile = defaultProfile;
            LoadProfiles();
            ChangeActiveProfile(GetProfileByPath(defaultProfile.ProfilePath));
            Logger.Instance.WriteLog("Profile manager is ready!");
        }

        /// <summary>
        /// Refresh the list of available profiles
        /// </summary>
        public void LoadProfiles()
        {
            Profiles.Clear();
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Profiles"))
            {
                Profiles.Add(defaultProfile);
                foreach (FileInfo fi in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/Profiles")
                    .EnumerateFiles("*cfg"))
                {
                    AppConfiguration config = new AppConfiguration(fi.Name.Replace(fi.Extension, ""));
                    config.SettingsSaved += Config_SettingsSaved;
                    config.SettingsRenamed += Config_SettingsRenamed;
                    Profiles.Add(config);
                }
            }
            Logger.Instance.WriteLog("{0} profiles have beend loaded!", LogType.INFO, Profiles.Count);
        }

        private void Config_SettingsRenamed(object sender, ProfileRenamedEventArgs e)
        {
            OnProfileRenamed(e);
        }

        private void Config_SettingsSaved(object sender, ProfileEventArgs e)
        {
            OnSettingsSaved(e);
        }

        /// <summary>
        /// Swaps the current configuration with another one with the given name.
        /// </summary>
        /// <param name="profileName">The name of the profile to load. Pass null or an empty string to load the default configuration.</param>
        /// <returns>Returns True if a profile with this name has been loaded, otherwise it returns false</returns>
        public bool ChangeActiveProfile(string profileName)
        {
             if (!string.IsNullOrWhiteSpace(profileName))
            {
                AppConfiguration newProfile = Profiles.FirstOrDefault(x => x.ProfileName?.Equals(profileName) ?? false);
                return ChangeActiveProfile(newProfile);
            }
            else
            {
                return ChangeActiveProfile(defaultProfile);
            }
        }


        /// <summary>
        /// Swaps the current configuration with the configuration which is passed.
        /// </summary>
        /// <param name="newProfile">The profile to load. Pass null to load the default configuration.</param>
        /// <returns>Returns True if the profile has been loaded, otherwise it returns false</returns>
        public bool ChangeActiveProfile(AppConfiguration newProfile)
        {
            try
            {
                if (newProfile == null)
                {
                    newProfile = defaultProfile;
                }

                AppConfiguration oldProfile = Profile;
                if (!oldProfile.Equals(newProfile))
                {
                    if (newProfile != null)
                    {
                        activeProfile = newProfile;
                    }
                    else
                    {
                        activeProfile = defaultProfile;
                    }

                    Logger.Instance.WriteLog("Active profile changed to \"{0}\".", LogType.INFO, activeProfile.ProfileName);
                    OnProfileChanged(new ProfileChangedEventArgs(oldProfile, activeProfile));
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLog("Unable to swap configuration! Target configuration: {0}",
                    LogType.ERROR, ex, newProfile);

                return false;
            }
        }

        /// <summary>
        /// Add a new profile to the manager
        /// </summary>
        /// <param name="profile">The profile to add</param>
        public void AddProfile(AppConfiguration profile)
        {
            profile.SettingsSaved += Config_SettingsSaved;
            Profiles.Add(profile);
            Logger.Instance.WriteLog("Profile \"{0}\" has been added!", profile.ProfileName);
        }

        /// <summary>
        /// Creates a new profile with the given name
        /// </summary>
        /// <param name="name">The name of the new profile</param>
        /// <param name="workingDir">The home directory of this profile</param>
        public void CreateProfile(string name, string workingDir)
        {
            AppConfiguration configuration = new AppConfiguration(name);
            configuration.OsuDirectory = workingDir;
            configuration.Save();
            AddProfile(configuration);
            OnProfileCreated(new ProfileEventArgs(configuration));
        }

        /// <summary>
        /// Removes a profile with the given name
        /// </summary>
        /// <param name="profileName">The name of the profile to delete</param>
        /// <returns>Returns true if the profile has been found and removed, otherwise false</returns>
        public bool RemoveProfile(string profileName)
        {
            AppConfiguration target = Profiles.FirstOrDefault(x => x.ProfileName?.Equals(profileName) ?? false);

            if (target != null)
            {
                RemoveProfile(target);
                Logger.Instance.WriteLog("Profile \"{0}\" has removed!", profileName);
                return true;
            }
            else
            {
                Logger.Instance.WriteLog("A profile with the name \"{0}\" does not exist!", LogType.WARNING, profileName);
                return false;
            }
        }
        
        /// <summary>
        /// Removes a profile
        /// </summary>
        /// <param name="profileName">The profile to delete</param>
        /// <returns>Returns true if the profile has been removed, otherwise false</returns>
        public bool RemoveProfile(AppConfiguration profile)
        {
            try
            {
                if (profile.Equals(Profile))
                {
                    Logger.Instance.WriteLog("Active profile has been removed! Resetting to default profile...");
                    ChangeActiveProfile("");
                }
                profile.SettingsSaved -= Config_SettingsSaved;
                Profiles.Remove(profile);
                File.Delete(profile.FilePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLog("Unable to remove configuration! Target configuration: {0}",
                    LogType.WARNING, ex, profile);

                return false;
            }
        }

        /// <summary>
        /// Returns the profile with a given name
        /// </summary>
        /// <param name="profileName">The name of the profile to return. Passing null returns the default profile.</param>
        /// <returns>The target profile or null if none has been found.</returns>
        public AppConfiguration GetProfileByName(string profileName)
        {
            if (!string.IsNullOrWhiteSpace(profileName))
            {
                return Profiles.FirstOrDefault(x => x.ProfileName?.Equals(profileName) ?? false);
            }
            else
            {
                return defaultProfile;
            }
        }

        /// <summary>
        /// Returns the name of a profile with a given path
        /// </summary>
        /// <param name="profilePath">The path of the profile whose name should be returned. Passing null returns null.</param>
        /// <returns>The target profile or null if none has been found.</returns>
        public AppConfiguration GetProfileByPath(string profilePath)
        {
            if (!string.IsNullOrWhiteSpace(profilePath))
            {
                return Profiles.FirstOrDefault(x => x.FilePath?.Equals(profilePath) ?? false);
            }
            else
            {
                return null;
            }
        }

        protected virtual void OnProfileChanged(ProfileChangedEventArgs e)
        {
            ProfileChanged?.Invoke(this, e);
        }

        protected virtual void OnSettingsSaved(ProfileEventArgs e)
        {
            SettingsSaved?.Invoke(this, e);
        }

        protected virtual void OnProfileCreated(ProfileEventArgs e)
        {
            ProfileCreated?.Invoke(this, e);
        }

        protected virtual void OnProfileRenamed(ProfileRenamedEventArgs e)
        {
            ProfileRenamed?.Invoke(this, e);
        }
    }
}
