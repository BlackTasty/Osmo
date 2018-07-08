using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Osmo.Core.FileExplorer
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }

    class NaturalFilePickerEntryComparer : IComparer<IFilePickerEntry>
    {
        public int Compare(IFilePickerEntry a, IFilePickerEntry b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a.Name, b.Name);
        }
    }
}
