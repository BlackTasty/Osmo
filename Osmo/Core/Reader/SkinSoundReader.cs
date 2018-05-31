using System;
using System.Collections.Generic;

namespace Osmo.Core.Reader
{
    class SkinSoundReader : ElementGenerator
    {
        List<SoundEntry> entries;

        public List<SoundEntry> Files => entries;

        public int FileCount => entries?.Count ?? 0;

        public SkinSoundReader(string list) : base(true)
        {
            string[] content = list.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);
            entries = new List<SoundEntry>();

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    entries.Add(new SoundEntry(content[i]));
                }
            }
        }
    }
}
