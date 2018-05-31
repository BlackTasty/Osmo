using System;
using System.Collections.Generic;

namespace Osmo.Core.Reader
{
    class SkinElementReader : ElementGenerator
    {
        List<SkinningEntry> entries;

        public List<SkinningEntry> Files => entries;

        public int FileCount => entries?.Count ?? 0;

        public SkinElementReader(string list) : base(false)
        {
            string[] content = list.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);
            entries = new List<SkinningEntry>();

            for (int i = 0; i< content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    entries.Add(new SkinningEntry(content[i]));
                }
            }
        }
    }
}
