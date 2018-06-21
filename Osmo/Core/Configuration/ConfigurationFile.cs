using System;
using System.IO;

namespace Osmo.Core.Configuration
{
    public partial class ConfigurationFile
    {
        private string fileName;

        /// <summary>
        /// Defines a new configuration file with the given name. 
        /// This file is created with the .cfg extension and is saved in the root directory of the application.
        /// </summary>
        /// <param name="fileName">Configuration file name</param>
        protected ConfigurationFile(string fileName) : this(fileName, ".cfg", "") { }

        /// <summary>
        /// Defines a new configuration file with the given values
        /// </summary>
        /// <param name="fileName">Configuration file name</param>
        /// <param name="extension">Optional: The extension of the file (default: .cfg)</param>
        /// <param name="subdir">Optonal: Save the configuration to the specified sub directory</param>
        protected ConfigurationFile(string fileName, string extension, string subDir)
        {
            if (fileName != "")
            {
                this.fileName = fileName;
                FilePath = AppDomain.CurrentDomain.BaseDirectory + subDir + fileName + extension;
            }
        }
        
        public string FilePath { get; private set; }

        protected string[] Content { get; set; }

        protected void Save(string[] properties)
        {
            File.WriteAllLines(FilePath, properties);
        }

        protected string[] LoadFile(ConfigurationFile file)
        {
            return File.Exists(FilePath) ? File.ReadAllLines(FilePath) : null;
        }

        protected void RecreateFile(string[] oldProp, ConfigurationFile newFile)
        {
            File.Delete(FilePath);
            //if (FilePath.Contains("settings.cfg"))
            //{
            //    AppSettings set = (AppSettings)newFile;
            //    set.Save();
            //}

            string[] newProp = File.ReadAllLines(FilePath);

            for (int i = 0; i < oldProp.Length; i++)
                newProp[i] = oldProp[i];
            Save(newProp);
        }

        internal bool Exists()
        {
            return File.Exists(FilePath);
        }

        internal void Reset()
        {
            File.Delete(FilePath);
        }
    }
}
