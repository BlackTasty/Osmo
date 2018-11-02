using System.IO;

namespace Uninstaller.Objects
{
    static class Helper
    {
        static string[] extensions = new string[] { ".mp3", ".ogg", ".wav", ".mp2", ".mp1", ".aif" };

        public static void DeleteDirectory(string path, bool recreate)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            if (recreate)
                Directory.CreateDirectory(path);
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
