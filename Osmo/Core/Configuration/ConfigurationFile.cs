using Osmo.Core.Logging;
using System;
using System.IO;

namespace Osmo.Core.Configuration
{
    public partial class ConfigurationFile
    {
        private string fileName;
        private string subDir;
        private string extension;

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
                this.subDir = subDir;
                this.extension = extension;
                FilePath = GetFilePath(fileName);
            }
        }
        
        public string FilePath { get; private set; }

        protected void Save(string[] properties)
        {
            SaveTo(FilePath, properties);
        }

        /// <summary>
        /// Defines where an array of properties should be saved
        /// </summary>
        /// <param name="properties">An array of properties</param>
        /// <param name="filePath">The target file location where the configuration shall be saved to</param>
        protected void SaveTo(string filePath, string[] properties)
        {
            File.WriteAllLines(filePath, properties);
            Logger.Instance.WriteLog("Configuration file \"{0}\" has been saved!", fileName + extension);
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

        protected void RenameFile(string newName)
        {
            Logger.Instance.WriteLog("Renaming configuration file \"{0}\" to \"{1}\"...", fileName + extension, newName + extension);
            fileName = newName;
            string newPath = GetFilePath(newName);
            File.Move(FilePath, newPath);
            FilePath = newPath;
        }

        internal bool Exists()
        {
            return File.Exists(FilePath);
        }

        internal void Reset()
        {
            File.Delete(FilePath);
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subDir, fileName + extension);
        }
    }
}
