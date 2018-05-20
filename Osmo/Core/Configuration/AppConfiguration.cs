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
        public bool IsValid
        {
            get
            {
                return File.Exists(FilePath) && !string.IsNullOrWhiteSpace(OsuDirectory)
                    && Directory.Exists(OsuDirectory);
            }
        }

        public string DefaultBackupDirectory
        {
            get => "[Installation folder]\\Backups\\";
        }

        public string OsuDirectory { get; set; }

        public string BackupDirectory { get; set; }

        public bool BackupBeforeMixing { get; set; }

        public Color BackgroundEditor { get; set; }

        public bool ReopenLastSkin { get; set; }

        public bool PlaySoundWhenHovering { get; set; }
        #endregion

        private AppConfiguration() : base("settings")
        {
            BackupBeforeMixing = true;
            BackgroundEditor = Colors.Black;
            ReopenLastSkin = true;
            PlaySoundWhenHovering = false;
            BackupDirectory = AppDomain.CurrentDomain.BaseDirectory + "Backups";
            OsuDirectory = "";

            Load();
        }

        public void Save()
        {
            #region Properties
            Content = new string[]
            {
                "OsuDirectory:" + OsuDirectory,
                "BackupBeforeMixing:"+ BackupBeforeMixing,
                "BackgroundEditor:" + BackgroundEditor.ToString(),
                "ReopenLastSkin:" + ReopenLastSkin,
                "PlaySoundWhenHovering:" + PlaySoundWhenHovering
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

                        case "BackupBeforeMixing":
                            BackupBeforeMixing = Parser.TryParse(property[1], true);
                            break;

                        case "BackgroundEditor":
                            BackgroundEditor = (Color)ColorConverter.ConvertFromString(property[1]);
                            break;

                        case "ReopenLastSkin":
                            ReopenLastSkin = Parser.TryParse(property[1], true);
                            break;

                        case "PlaySoundWhenHovering":
                            PlaySoundWhenHovering = Parser.TryParse(property[1], true);
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
