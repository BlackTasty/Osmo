using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.FileExplorer
{
    public interface IFolderEntry
    {
        string Name { get; }

        string Path { get; }
    }
}
