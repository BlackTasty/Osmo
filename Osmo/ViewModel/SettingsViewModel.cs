using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System.Windows.Media;

namespace Osmo.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        AppConfiguration config = AppConfiguration.Instance;

        public SettingsViewModel()
        {
            config.SettingsLoaded += Configuration_SettingsLoaded;
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
        }

        public string OsuDirectory
        {
            get => config.OsuDirectory;
            set
            {
                config.OsuDirectory = value;
                InvokePropertyChanged("OsuDirectory");
            }
        }

        public string BackupDirectory
        {
            get => config.BackupDirectory;
            set
            {
                config.BackupDirectory = value;
                InvokePropertyChanged("BackupDirectory");
            }
        }

        public string TemplateDirectory
        {
            get => config.TemplateDirectory;
            set
            {
                config.TemplateDirectory = value;
                InvokePropertyChanged("TemplateDirectory");
            }
        }

        public bool BackupBeforeMixing
        {
            get => config.BackupBeforeMixing;
            set
            {
                config.BackupBeforeMixing = value;
                InvokePropertyChanged("BackupBeforeMixing");
            }
        }

        public Color BackgroundEditor
        {
            get => config.BackgroundEditor;
            set
            {
                config.BackgroundEditor = value;
                InvokePropertyChanged("BackgroundEditor");
            }
        }

        public bool ReopenLastSkin
        {
            get => config.ReopenLastSkin;
            set
            {
                config.ReopenLastSkin = value;
                InvokePropertyChanged("ReopenLastSkin");
            }
        }

        public Language Language
        {
            get => config.Language;
            set
            {
                config.Language = value;
                InvokePropertyChanged("Language");
            }
        }
    }
}
