using Osmo.Core.Logging;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;

namespace Osmo.Core.Reader
{
    class SkinElementReader : ElementGenerator
    {
        public SkinElementReader(string list, string listName, ElementType elementType) : base(false)
        {
            string[] content = list.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    Files.Add(new SkinningEntry(content[i], elementType));
                }
            }

            Logger.Instance.WriteLog("{0}: {1} element details have been loaded!", listName, Files.Count);
        }
    }
}
