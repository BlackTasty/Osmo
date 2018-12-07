using System.IO;
using System.Windows;

namespace Uninstaller.Objects
{
    static class Helper
    {
        static string[] extensions = new string[] { ".osk" };

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

        public static string FindString(string targetName)
        {
            try
            {
                return (string)Application.Current.FindResource(targetName);
            }
            catch
            {
                return "NO STRING FOUND!";
            }
        }
    }
}
