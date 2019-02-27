using Osmo.Core.Objects;
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

        /// <summary>
        /// Setting this value to 0 tells engine that infinite frames are allowed.
        /// </summary>
        public int MaximumFrames { get; private set; }

        /// <summary>
        /// If this isn't defined, no frame order is used.
        /// </summary>
        public int[] FrameOrder { get; private set; }

        public bool ContainsHyphen { get; private set; }

        public bool IsSound => false;

        public string Description => description ?? "";

        public string Flags { get; private set; } = "";

        public ElementType ElementType { get; private set; }

        internal SkinningEntry(string line, ElementType elementType)
        {
            ElementType = elementType;
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
                            string[] subContent = content[i].Split(';');
                            for (int j = 0; j < subContent.Length; j++)
                            {
                                switch (j)
                                {
                                    case 0: //Animatable
                                        MultipleElementsAllowed = Parser.TryParse(subContent[j], false);
                                        break;
                                    case 1: //Contains hyphen
                                        ContainsHyphen = Parser.TryParse(subContent[j], false);
                                        break;
                                    case 2: //Maximum frames
                                        MaximumFrames = Parser.TryParse(subContent[j], 0);
                                        break;
                                    case 3: //Frame order
                                        string[] rawFrameOrder = subContent[j].Split(',');
                                        FrameOrder = new int[rawFrameOrder.Length];
                                        for (int frameIndex = 0; frameIndex < rawFrameOrder.Length; frameIndex++)
                                        {
                                            FrameOrder[frameIndex] = Parser.TryParse(rawFrameOrder[frameIndex], 0);
                                        }
                                        break;
                                }
                            }
                            break;
                        case 4:
                            SetVersion(content[i]);
                            break;
                        case 5:
                            description = content[i].Replace('\\', '\n');
                            break;
                        case 6:
                            Flags = content[i].ToLower();
                            break;
                    }
                }
            }
        }

        public Size GetSuggestedSizeForVersion(string version)
        {
            return sizeDescriptors.FirstOrDefault(x => x.VersionMatches(version))?.SuggestedSDSize ?? new Size();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Description);
        }

        //public static bool operator ==(SkinningEntry entry, string name)
        //{
        //    return entry.Name.Equals(name);
        //}

        //public static bool operator !=(SkinningEntry entry, string name)
        //{
        //    return !entry.Name.Equals(name);
        //}

        //public override int GetHashCode()
        //{
        //    return Name.GetHashCode();
        //}

        //public override bool Equals(object obj)
        //{
        //    return Name.Equals((obj as SkinningEntry).Name);
        //}

        public string GetRegexName()
        {
            return ContainsHyphen ? Name + "[-]" : Name;
        }
    }
}
