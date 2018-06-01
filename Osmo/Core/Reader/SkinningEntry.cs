using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Osmo.Core.Reader
{
    class SkinningEntry : ElementReader, IEntry
    {
        private readonly string description;
        
        private List<VersionSizeDescriptor> sizeDescriptors;

        public string Name { get; private set; }

        public string PreferredFormat => SupportedFormats?.Length > 0 ? SupportedFormats[0] : "";

        public string[] SupportedFormats { get; private set; }

        public Size SuggestedSDSize =>
            sizeDescriptors.FirstOrDefault(x => !x.SuggestedSDSize.Equals(new Size()))?.SuggestedSDSize ?? new Size();

        public Size MaximumSize =>
            sizeDescriptors.FirstOrDefault(x => !x.MaximumSize.Equals(new Size()))?.MaximumSize ?? new Size();

        public Size MinimumSize =>
            sizeDescriptors.FirstOrDefault(x => !x.MinimumSize.Equals(new Size()))?.MinimumSize ?? new Size();


        public bool MultipleElementsAllowed { get; private set; }

        public bool IsSound => false;

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
                            Name = content[i];
                            break;
                        case 1:
                            SupportedFormats = content[i].Split(',');
                            break;
                        case 2:
                            string[] sizeDefinitions = content[i].Split(';');
                            for (int j = 0; j < sizeDefinitions.Length; j++)
                            {
                                sizeDescriptors.Add(new VersionSizeDescriptor(sizeDefinitions[j]));
                            }
                            break;
                        case 3:
                            MultipleElementsAllowed = Parser.TryParse(content[i], false);
                            break;
                        case 4:
                            SetVersion(content[i]);
                            break;
                        case 5:
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
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Name.Equals((obj as SkinningEntry).Name);
        }
    }
}
