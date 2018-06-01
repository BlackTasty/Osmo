using System;
using System.Collections.Generic;

namespace Osmo.Core.Reader
{
    class SkinSoundReader : ElementGenerator
    {
        public SkinSoundReader(string list) : base(true)
        {
            string[] content = list.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    Files.Add(new SoundEntry(content[i]));
                }
            }
        }
    }
}
