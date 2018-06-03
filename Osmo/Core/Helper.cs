using System.IO;
using System.Windows.Media.Imaging;

namespace Osmo.Core
{
    public static class Helper
    {

        public static BitmapImage LoadImage(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                BitmapImage bmp = new BitmapImage();

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = fs;
                    bmp.EndInit();
                }

                return bmp;
            }
            else return null;
        }
    }
}
