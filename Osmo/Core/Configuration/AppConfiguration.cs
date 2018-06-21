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
        public static AppConfiguration GetInstance()
        {
            if (instance == null)
                instance = new AppConfiguration();

            return instance;
        }
        #endregion

        //TODO: Append all changed settings to EventArgs (custom class required)
        public event EventHandler<EventArgs> SettingsSaved;

        #region Properties
        private string mBackupDirectory;
        private string mDefaultBackupDirectory;

        private string mTemplateDirectory;
        private string mDefaultTemplateDirectory;

        public bool IsValid
        {
            get
            {
                return File.Exists(FilePath) && !string.IsNullOrWhiteSpace(OsuDirectory)
                    && Directory.Exists(OsuDirectory);
            }
        }

        public bool DisclaimerRead { get; set; }

        public string OsuDirectory { get; set; }

        public string BackupDirectory {
            get => !string.IsNullOrWhiteSpace(mBackupDirectory) ? 
                mBackupDirectory : mDefaultBackupDirectory;
            set => mBackupDirectory = value; }

        public string TemplateDirectory
        {
            get => !string.IsNullOrWhiteSpace(mTemplateDirectory) ?
                mTemplateDirectory : mDefaultTemplateDirectory;
            set => mTemplateDirectory = value;
        }

        public bool BackupBeforeMixing { get; set; }

        public Color BackgroundEditor { get; set; }

        public bool ReopenLastSkin { get; set; }

        public double Volume { get; set; }

        public bool IsMuted { get; set; }
        #endregion

        private AppConfiguration() : base("settings")
        {
            BackupBeforeMixing = true;
            BackgroundEditor = Colors.Black;
            ReopenLastSkin = true;
            mDefaultBackupDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backups\\";
            mDefaultTemplateDirectory = AppDomain.CurrentDomain.BaseDirectory + "Templates\\";
            OsuDirectory = "";
            Volume = .8;

            Load();
        }

        public void Save()
        {
            #region Properties
            Content = new string[]
            {
                "OsuDirectory:" + OsuDirectory,
                "TemplateDirectory:" + TemplateDirectory,
                "BackupBeforeMixing:"+ BackupBeforeMixing,
                "BackgroundEditor:" + BackgroundEditor.ToString(),
                "ReopenLastSkin:" + ReopenLastSkin,
                "Volume:" + Volume,
                "IsMuted:" + IsMuted,
                "DisclaimerRead:" + DisclaimerRead
            };
            #endregion

            base.Save(Content);
            OnSettingsSaved(EventArgs.Empty);
        }

        public void Load()
        {
            Content = base.LoadFile(this);

            if (Content != null)
            {
                foreach (string str in Content)
                {
                    string[] property = GetPropertyPair(str);
                    switch (property[0])
                    {
                        case "OsuDirectory":
                            OsuDirectory = property[1];
                            break;

                        case "BackupDirectory":
                            BackupDirectory = property[1];
                            break;

                        case "TemplateDirectory":
                            TemplateDirectory = property[1];
                            break;

                        case "BackupBeforeMixing":
                            BackupBeforeMixing = Parser.TryParse(property[1], true);
                            break;

                        case "BackgroundEditor":
                            BackgroundEditor = (Color)ColorConverter.ConvertFromString(property[1]);
                            break;

                        case "ReopenLastSkin":
                            ReopenLastSkin = Parser.TryParse(property[1], true);
                            break;
                            
                        case "Volume":
                            Volume = Parser.TryParse(property[1], .8);
                            break;

                        case "IsMuted":
                            IsMuted = Parser.TryParse(property[1], false);
                            break;

                        case "DisclaimerRead":
                            DisclaimerRead = Parser.TryParse(property[1], false);
                            break;
                    }
                }
            }
        }

        protected virtual void OnSettingsSaved(EventArgs e)
        {
            SettingsSaved?.Invoke(this, e);
        }
    }
}
