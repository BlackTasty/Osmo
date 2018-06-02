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
