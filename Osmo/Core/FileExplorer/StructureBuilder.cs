using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Osmo.Core.FileExplorer
{
    public class StructureBuilder
    {
        List<KeyValuePair<int, string>> treePaths = new List<KeyValuePair<int, string>>();

        public string RootFolder { get => GetPathAtTreeLayer(0); }

        public string TargetPath { get; private set; }

        public string GetPathAtTreeLayer(int layer)
        {
            KeyValuePair<int, string> target = treePaths.FirstOrDefault(x => x.Key == layer);
            if (!target.Equals(default(KeyValuePair<int, string>)))
            {
                return target.Value;
            }
            else
            {
                return null;
            }
        }

        public StructureBuilder(string path)
        {
            TargetPath = path;
            string[] tree = path.Split('\\');

            string previousPath = null;
            foreach (string directory in tree)
            {
                if (previousPath == null)
                {
                    previousPath = directory + "\\";
                }
                else
                {
                    previousPath = Path.Combine(previousPath, directory);
                }

                treePaths.Add(new KeyValuePair<int, string>(treePaths.Count, previousPath));
            }
        }

        public static void CacheDriveContent(FolderEntry root)
        {
            File.AppendAllLines(FixedValues.FILE_EXPLORER_CACHEFILE, root.GetFolderStructure(true));
        }
    }
}
