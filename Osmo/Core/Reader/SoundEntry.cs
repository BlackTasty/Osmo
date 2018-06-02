namespace Osmo.Core.Reader
{
    class SoundEntry : ElementReader, IEntry
    {
        private readonly string description;

        public string Name { get; private set; }

        public string PreferredFormat => SupportedFormats?.Length > 0 ? SupportedFormats[0] : "";

        public string[] SupportedFormats { get; private set; }
        
        public bool MultipleElementsAllowed { get; private set; }

        public bool IsSound => true;

        public string Description => description ?? "";

        internal SoundEntry(string line)
        {
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
                            MultipleElementsAllowed = Parser.TryParse(content[i], false);
                            break;
                        case 3:
                            description = content[i].Replace('\\', '\n');
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Description);
        }

        public static bool operator ==(SoundEntry entry, string name)
        {
            return entry.Name.Equals(name);
        }

        public static bool operator !=(SoundEntry entry, string name)
        {
            return !entry.Name.Equals(name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Name.Equals((obj as SoundEntry).Name);
        }
    }
}
