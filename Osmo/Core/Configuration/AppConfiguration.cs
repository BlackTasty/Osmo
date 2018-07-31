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
        private static AppConfiguration instance;
        public static AppConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppConfiguration();

                return instance;
            }
        }
        #endregion

        //TODO: Append all changed settings to EventArgs (custom class required)
        public event EventHandler<EventArgs> SettingsSaved;
        public event EventHandler<EventArgs> SettingsLoaded;

        #region Properties
        private string mBackupDirectory;
        private string mDefaultBackupDirectory;

        private string mTemplateDirectory;
        private string mDefaultTemplateDirectory;

        private string mOsuDirectory;
        private Color mBackgroundEditor;
        private bool mBackupBeforeMixing;
        private bool mReopenLastSkin;
        private Language mLanguage;

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
            get => !string.IsNullOrWhiteSpace(mBackupDirectory) ? 
                mBackupDirectory : mDefaultBackupDirectory;
            set
            {
                SetUnsavedChanges(mBackupDirectory, value);
                mBackupDirectory = value;
            }
        }

        public string TemplateDirectory
        {
            get => !string.IsNullOrWhiteSpace(mTemplateDirectory) ?
                mTemplateDirectory : mDefaultTemplateDirectory;
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
        #endregion

        private AppConfiguration() : base("settings")
        {
            Load();
        }

        public void Save()
        {
            #region Properties
            Content = new string[]
            {
                "OsuDirectory:" + OsuDirectory,
                "BackupDirectory:" + BackupDirectory,
                "TemplateDirectory:" + TemplateDirectory,
                "BackupBeforeMixing:"+ BackupBeforeMixing,
                "BackgroundEditor:" + BackgroundEditor.ToString(),
                "ReopenLastSkin:" + ReopenLastSkin,
                "Language:" + (int)Language,
                "DisclaimerRead:" + DisclaimerRead
            };
            #endregion

            base.Save(Content);
            UnsavedChanges = false;
            OnSettingsSaved(EventArgs.Empty);
        }

        public void Load()
        {
            Content = base.LoadFile(this);
            LoadDefaults();

            if (Content != null)
            {
                foreach (string str in Content)
                {
                    string[] property = GetPropertyPair(str);
                    switch (property[0])
                    {
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
            mBackupBeforeMixing = true;
            mBackgroundEditor = Colors.Black;
            mReopenLastSkin = true;
            mDefaultBackupDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backups\\";
            mDefaultTemplateDirectory = AppDomain.CurrentDomain.BaseDirectory + "Templates\\";
            mOsuDirectory = "";
            mLanguage = Language.Default;
        }

        private void SetUnsavedChanges(object oldValue, object value)
        {
            if (!oldValue.Equals(value))
            {
                UnsavedChanges = true;
            }
        }

        protected virtual void OnSettingsSaved(EventArgs e)
        {
            SettingsSaved?.Invoke(this, e);
        }

        protected virtual void OnSettingsLoaded(EventArgs e)
        {
            SettingsLoaded?.Invoke(this, e);
        }
    }
}
