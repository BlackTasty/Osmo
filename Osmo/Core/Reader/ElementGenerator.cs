using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Osmo.Core.Reader
{
    class ElementGenerator
    {
        private bool isSound;

        public List<IEntry> Files { get; private set; }

        public int FileCount => Files?.Count ?? 0;

        protected ElementGenerator(bool isSound)
        {
            this.isSound = isSound;
            Files = new List<IEntry>();
        }

        /// <summary>
        /// This method differs from the FileCount property as it only counts files which are used with the provided version.
        /// This needs a better name...
        /// </summary>
        /// <param name="version">The target version</param>
        /// <returns>All files which match the version criteria</returns>
        internal int CountFiles(string version)
        {
            return Files?.Count(x => (x as ElementReader).VersionMatches(version)) ?? 0;
        }

        internal void Generate(string path)
        {
            Generate(path, isSound);
        }

        internal static void Generate(string path, bool isSound)
        {
            if (!isSound)
            {
                using (Bitmap bmp = Properties.Resources.emptyImage)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    bmp.Save(path);
                }
            }
            else
            {
                using (FileStream fs = File.Create(path))
                {
                    using (UnmanagedMemoryStream ums = Properties.Resources.emptySound)
                    {
                        byte[] data = new byte[ums.Length];
                        ums.Read(data, 0, data.Length);
                        fs.Write(data, 0, data.Length);
                    }
                }
            }
        }

        internal IEntry FindElement(string name)
        {
            return Files.FirstOrDefault(x => name.Contains(x.Name));
        }
    }
}
