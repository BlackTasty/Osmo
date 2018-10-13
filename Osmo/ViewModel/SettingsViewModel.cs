using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System;
using System.IO;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        private int mSelectedProfileIndex = -1;

        public VeryObservableCollection<AppConfiguration> Profiles
        {
            get => App.ProfileManager.Profiles;
            set
            {
                App.ProfileManager.Profiles = value;
                InvokePropertyChanged("Profiles");
            }
        }

        public AppConfiguration SelectedProfile
        {
            get => App.ProfileManager.Profile;
            set
            {
                SelectedProfile.SettingsLoaded -= Configuration_SettingsLoaded;
                App.ProfileManager.ChangeActiveProfile(value.ProfileName);
                SelectedProfile.SettingsLoaded += Configuration_SettingsLoaded;
                InvokePropertyChanged("SelectedProfile");
            }
        }

        public int SelectedProfileIndex
        {
            get => mSelectedProfileIndex;
            set
            {
                mSelectedProfileIndex = value;
                InvokePropertyChanged("SelectedProfileIndex");
            }
        }

        public SettingsViewModel()
        {
            SelectedProfile.SettingsLoaded += Configuration_SettingsLoaded;
            App.ProfileManager.ProfileChanged += ProfileManager_ProfileChanged;
            App.ProfileManager.ProfileCreated += ProfileManager_ProfileCreated;

            SelectedProfileIndex = Profiles.IndexOf(SelectedProfile);
            //FileSystemWatcher fileWatcher = new FileSystemWatcher(config.FilePath + "\\Profiles", "*.cfg");
            //fileWatcher.Created += FileWatcher_Created;
            //fileWatcher.Deleted += FileWatcher_Deleted;
            //fileWatcher.Changed += FileWatcher_Changed;
            //fileWatcher.Renamed += FileWatcher_Renamed;
        }

        private void ProfileManager_ProfileCreated(object sender, ProfileEventArgs e)
        {
            App.ProfileManager.ChangeActiveProfile(e.Profile);
        }

        private void ProfileManager_ProfileChanged(object sender, ProfileChangedEventArgs e)
        {
            Configuration_SettingsLoaded(this, EventArgs.Empty);
            InvokePropertyChanged("SelectedProfile");
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {

        }

        private void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Configuration_SettingsLoaded(object sender, System.EventArgs e)
        {
            InvokePropertyChanged("OsuDirectory");
            InvokePropertyChanged("BackupDirectory");
            InvokePropertyChanged("TemplateDirectory");
            InvokePropertyChanged("BackupBeforeMixing");
            InvokePropertyChanged("BackgroundEditor");
            InvokePropertyChanged("ReopenLastSkin");
            InvokePropertyChanged("Language");
            InvokePropertyChanged("ProfileName");
            InvokePropertyChanged("Profiles");
        }

        public string OsuDirectory
        {
            get => SelectedProfile.OsuDirectory;
            set
            {
                SelectedProfile.OsuDirectory = value;
                InvokePropertyChanged("OsuDirectory");
            }
        }

        public string BackupDirectory
        {
            get => SelectedProfile.BackupDirectory;
            set
            {
                SelectedProfile.BackupDirectory = value;
                InvokePropertyChanged("BackupDirectory");
            }
        }

        public string TemplateDirectory
        {
            get => SelectedProfile.TemplateDirectory;
            set
            {
                SelectedProfile.TemplateDirectory = value;
                InvokePropertyChanged("TemplateDirectory");
            }
        }

        public bool BackupBeforeMixing
        {
            get => SelectedProfile.BackupBeforeMixing;
            set
            {
                SelectedProfile.BackupBeforeMixing = value;
                InvokePropertyChanged("BackupBeforeMixing");
            }
        }

        public Color BackgroundEditor
        {
            get => SelectedProfile.BackgroundEditor;
            set
            {
                SelectedProfile.BackgroundEditor = value;
                InvokePropertyChanged("BackgroundEditor");
            }
        }

        public bool ReopenLastSkin
        {
            get => SelectedProfile.ReopenLastSkin;
            set
            {
                SelectedProfile.ReopenLastSkin = value;
                InvokePropertyChanged("ReopenLastSkin");
            }
        }

        public int Language
        {
            get => (int)SelectedProfile.Language;
            set
            {
                SelectedProfile.Language = (Language)value;
                InvokePropertyChanged("Language");
            }
        }

        public string ProfileName
        {
            get => SelectedProfile.ProfileName;
            set
            {
                SelectedProfile.ProfileName = value;
                InvokePropertyChanged("ProfileName");
            }
        }
    }
}
