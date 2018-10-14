using Osmo.Core.Logging;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Osmo.Core.Configuration
{
    public class AppConfiguration : ConfigurationFile
    {
        #region Singleton implementation
        //private static AppConfiguration instance;
        //public static AppConfiguration Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //            instance = new AppConfiguration();

        //        return instance;
        //    }
        //}
        #endregion

        //TODO: Append all changed settings to EventArgs (custom class required)
        public event EventHandler<ProfileEventArgs> SettingsSaved;
        public event EventHandler<EventArgs> SettingsLoaded;
        public event EventHandler<ProfileRenamedEventArgs> SettingsRenamed;

        #region Properties
        private string mBackupDirectory;
        private string mDefaultBackupDirectory;

        private string mTemplateDirectory;
        private string mDefaultTemplateDirectory;

        private string mProfileName;
        private string mOsuDirectory;
        private Color mBackgroundEditor;
        private bool mBackupBeforeMixing;
        private bool mReopenLastSkin;
        private Language mLanguage;

        private bool isInit = true;

        public string ProfilePath { get; set; }

        public string ProfileName {
            get => mProfileName;
            set
            {
                string oldName = mProfileName;
                SetUnsavedChanges(mProfileName, value);
                mProfileName = value;
                if (!isInit)
                {
                    RenameFile(mProfileName);
                    redirectPath = FilePath;
                    Save();
                    OnSettingsRenamed(new ProfileRenamedEventArgs(this, oldName, value));
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return File.Exists(FilePath) && !string.IsNullOrWhiteSpace(OsuDirectory)
                    && Directory.Exists(OsuDirectory);
            }
        }

        public bool UnsavedChanges { get; private set; }

        public bool DisclaimerRead { get; set; }

        public string OsuDirectory
        {
            get => mOsuDirectory;
            set
            {
                SetUnsavedChanges(mOsuDirectory, value);
                    mOsuDirectory = value;
            }
        }

        public string BackupDirectory {
            get
            {
                return !string.IsNullOrWhiteSpace(mBackupDirectory) ?
                    mBackupDirectory : mDefaultBackupDirectory;
            }
            set
            {
                SetUnsavedChanges(mBackupDirectory, value);
                mBackupDirectory = value;
            }
        }

        public string TemplateDirectory
        {
            get
            {
                return !string.IsNullOrWhiteSpace(mTemplateDirectory) ?
                    mTemplateDirectory : mDefaultTemplateDirectory;
            }
            set
            {
                SetUnsavedChanges(mTemplateDirectory, value);
                mTemplateDirectory = value;
            }
        }

        public bool BackupBeforeMixing
        {
            get => mBackupBeforeMixing; set
            {
                SetUnsavedChanges(mBackupBeforeMixing, value);
                mBackupBeforeMixing = value;
            }
        }

        public Color BackgroundEditor
        {
            get => mBackgroundEditor;
            set
            {
                SetUnsavedChanges(mBackgroundEditor, value);
                mBackgroundEditor = value;
            }
        }

        public bool ReopenLastSkin
        {
            get => mReopenLastSkin;
            set
            {
                SetUnsavedChanges(mReopenLastSkin, value);
                mReopenLastSkin = value;
            }
        }

        public Language Language { get => mLanguage;
            set
            {
                SetUnsavedChanges(mLanguage, value);
                mLanguage = value;
            }
        }

        public bool IsDefault { get => isDefault; }
        #endregion

        private string redirectPath;
        private bool isDefault;

        /// <summary>
        /// This initalizes a new configuration profile with a given name
        /// </summary>
        /// <param name="profileName"></param>
        public AppConfiguration(string profileName) : base(profileName, ".cfg", "Profiles/")
        {
            ProfileName = profileName; 
            isDefault = false;
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles"));
            redirectPath = FilePath;
            Load(base.LoadFile(this));
            isInit = false;
        }

        /// <summary>
        /// This loads the default settings
        /// </summary>
        public AppConfiguration() : base("settings")
        {
            isDefault = true;
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles"));
            Load();
        }

        public void LoadProfile(AppConfiguration profile)
        {
        }

        public void Save()
        {
            #region Properties
            string[] content;
            if (isDefault)
            {
                content = new string[]
                {
                "ProfilePath:" + ProfilePath,
                "OsuDirectory:" + OsuDirectory,
                "BackupDirectory:" + BackupDirectory,
                "TemplateDirectory:" + TemplateDirectory,
                "BackupBeforeMixing:"+ BackupBeforeMixing,
                "BackgroundEditor:" + BackgroundEditor.ToString(),
                "ReopenLastSkin:" + ReopenLastSkin,
                "Language:" + (int)Language,
                "DisclaimerRead:" + DisclaimerRead
                };
            }
            else
            {
                content = new string[]
                {
                "ProfileName:" + ProfileName,
                "OsuDirectory:" + OsuDirectory,
                "BackupDirectory:" + BackupDirectory,
                "TemplateDirectory:" + TemplateDirectory,
                "BackupBeforeMixing:"+ BackupBeforeMixing,
                "BackgroundEditor:" + BackgroundEditor.ToString(),
                "ReopenLastSkin:" + ReopenLastSkin,
                "Language:" + (int)Language,
                "DisclaimerRead:" + DisclaimerRead
                };
            }
            #endregion

            if (string.IsNullOrWhiteSpace(redirectPath))
            {
                base.Save(content);
            }
            else
            {
                base.SaveTo(redirectPath, content);
            }
            UnsavedChanges = false;
            OnSettingsSaved(new ProfileEventArgs(this));
        }

        public void Load()
        {
            Load(base.LoadFile(this));
        }

        private void Load(string[] content)
        {
            LoadDefaults();
            
            if (content != null)
            {
                foreach (string str in content)
                {
                    string[] property = GetPropertyPair(str);
                    switch (property[0])
                    {
                        case "ProfilePath":
                            ProfilePath = property[1];
                            break;

                        case "ProfileName":
                            ProfileName = property[1];
                            break;

                        case "OsuDirectory":
                            mOsuDirectory = property[1];
                            break;

                        case "BackupDirectory":
                            mBackupDirectory = property[1];
                            break;

                        case "TemplateDirectory":
                            mTemplateDirectory = property[1];
                            break;

                        case "BackupBeforeMixing":
                            mBackupBeforeMixing = Parser.TryParse(property[1], true);
                            break;

                        case "BackgroundEditor":
                            mBackgroundEditor = (Color)ColorConverter.ConvertFromString(property[1]);
                            break;

                        case "ReopenLastSkin":
                            mReopenLastSkin = Parser.TryParse(property[1], true);
                            break;

                        case "Language":
                            mLanguage = Parser.TryParse(property[1], Language.Default);
                            break;

                        case "DisclaimerRead":
                            DisclaimerRead = Parser.TryParse(property[1], false);
                            break;
                    }
                }
            }

            UnsavedChanges = false;
            OnSettingsLoaded(EventArgs.Empty);
        }

        private void LoadDefaults()
        {
            ProfilePath = "";
            mBackupBeforeMixing = true;
            mBackgroundEditor = Colors.Black;
            mReopenLastSkin = true;
            mDefaultBackupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups\\");
            mDefaultTemplateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\");
            mOsuDirectory = "";
            mLanguage = Language.Default;
        }

        private void SetUnsavedChanges(object oldValue, object value)
        {
            if (oldValue != null && !oldValue.Equals(value))
            {
                UnsavedChanges = true;
            }
        }

        protected virtual void OnSettingsSaved(ProfileEventArgs e)
        {
            SettingsSaved?.Invoke(this, e);
        }

        protected virtual void OnSettingsLoaded(EventArgs e)
        {
            SettingsLoaded?.Invoke(this, e);
        }

        protected virtual void OnSettingsRenamed(ProfileRenamedEventArgs e)
        {
            SettingsRenamed?.Invoke(this, e);
        }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(ProfileName) ? ProfileName : "Default";
        }
    }
}
