using System;
using System.Collections.Generic;

namespace Osmo.Core.Reader
{
    class SkinSoundReader
    {
        List<SoundEntry> soundEntries;

        public SkinSoundReader(string soundList)
        {
            string[] content = soundList.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);
            soundEntries = new List<SoundEntry>();

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    soundEntries.Add(new SoundEntry(content[i]));
                }
            }
        }
    }
}
