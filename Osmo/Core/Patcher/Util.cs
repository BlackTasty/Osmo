using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Patcher
{
    static class Util
    {
        public static List<string> MoveDirectory(string sourcePath, string destPath)
        {
            return MoveFiles(null, new DirectoryInfo(sourcePath), destPath);

            //Directory.Move(destPath, destPath + ".BAK");
            //Directory.Move(sourcePath, destPath);
        }

        public static List<string> MoveFiles(string root, DirectoryInfo di, string destPath)
        {
            List<string> paths = new List<string>();
            foreach (FileInfo fi in di.GetFiles())
            {
                paths.Add(MoveFile(fi.FullName, destPath + "\\" + fi.Name));
            }

            return paths;
        }

        public static string MoveFile(string sourcePath, string destPath, string[] whitelist)
        {
            if (!IsWhitelistFile(sourcePath, whitelist))
            {
                MoveFile(sourcePath, destPath);
            }
            return destPath + ".BAK";
        }

        public static string MoveFile(string sourcePath, string destPath)
        {
            if (File.Exists(destPath))
            {
                if (File.Exists(destPath + ".BAK"))
                    File.Delete(destPath + ".BAK");
                File.Move(destPath, destPath + ".BAK");
            }
            File.Move(sourcePath, destPath);

            return destPath + ".BAK";
        }

       public static bool IsWhitelistFile(string path, string[] whitelist)
        {
            if (whitelist != null)
            {
                foreach (string str in whitelist)
                {
                    if (path.Contains(str))
                        return true;
                }
            }
            return false;
        }
    }
}
