using System.Drawing;
using System.IO;

namespace Osmo.Core.Reader
{
    class ElementGenerator
    {
        private bool isSound;

        protected ElementGenerator(bool isSound)
        {
            this.isSound = isSound;
        }

        internal void Generate(string path)
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
    }
}
