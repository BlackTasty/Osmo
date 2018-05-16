using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Patcher
{
    class PatchList
    {
        public int Count { get { return Patches.Count; } }

        public List<PatchData> Patches { get; private set; } = new List<PatchData>();

        public PatchData GetPatch(string version)
        {
            foreach (var data in Patches)
            {
                if (data.PatchNumber.Equals(version))
                    return data;
            }
            return null;
        }

        public PatchData GetPatch(int index)
        {
            return Patches[index];
        }

        public void Add(PatchData data)
        {
            Patches.Add(data);
        }

        public void AddRange(List<PatchData> patches)
        {
            this.Patches = patches;
        }
    }
}
