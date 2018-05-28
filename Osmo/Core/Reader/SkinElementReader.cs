using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Reader
{
    class SkinElementReader
    {
        List<SkinningEntry> skinningEntries;

        public SkinElementReader(string skinningList)
        {
            string[] content = skinningList.Split(new string[] { "\r\n" },
                StringSplitOptions.RemoveEmptyEntries);
            skinningEntries = new List<SkinningEntry>();

            for (int i = 0; i< content.Length; i++)
            {
                if (content[i].Trim()[0] != '#')
                {
                    skinningEntries.Add(new SkinningEntry(content[i]));
                }
            }
        }
    }
}
