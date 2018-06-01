using System;
using System.Collections.Generic;

namespace Osmo.Core.Reader
{
    class SkinElementReader : ElementGenerator
    {
        public SkinElementReader(string list) : base(false)
        {
            string[] content = list.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    Files.Add(new SkinningEntry(content[i]));
                }
            }
        }
    }
}
