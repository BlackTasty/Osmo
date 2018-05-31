using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Osmo.Core.Reader
{
    class SkinningEntry : ElementReader
    {
        private string name;
        private bool animatable;
        private string description;

        private List<VersionSizeDescriptor> sizeDescriptors;

        public string Name => name;

        public Size SuggestedSDSize =>
            sizeDescriptors.FirstOrDefault(x => !x.SuggestedSDSize.Equals(new Size()))?.SuggestedSDSize ?? new Size();

        public Size MaximumSize =>
            sizeDescriptors.FirstOrDefault(x => !x.MaximumSize.Equals(new Size()))?.MaximumSize ?? new Size();

        public Size MinimumSize =>
            sizeDescriptors.FirstOrDefault(x => !x.MinimumSize.Equals(new Size()))?.MinimumSize ?? new Size();


        public bool Animatable => animatable;

        public string Description => description ?? "";

        internal SkinningEntry(string line)
        {
            sizeDescriptors = new List<VersionSizeDescriptor>();
            string[] content = ReadLine(line);
            for (int i = 0; i < content.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(content[i]))
                {
                    switch (i)
                    {
                        case 0:
                            name = content[i];
                            break;
                        case 1:
                            string[] sizeDefinitions = content[i].Split(';');
                            for (int j = 0; j < sizeDefinitions.Length; j++)
                            {
                                sizeDescriptors.Add(new VersionSizeDescriptor(sizeDefinitions[j]));
                            }
                            break;
                        case 2:
                            animatable = Parser.TryParse(content[i], false);
                            break;
                        case 3:
                            SetVersion(content[i]);
                            break;
                        case 4:
                            description = content[i];
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Description);
        }

        public static bool operator ==(SkinningEntry entry, string name)
        {
            return entry.Name.Equals(name);
        }

        public static bool operator !=(SkinningEntry entry, string name)
        {
            return !entry.Name.Equals(name);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return name.Equals((obj as SkinningEntry).name);
        }
    }
}
