using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osmo.Core
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Replaces multiple strings inside the target string
        /// </summary>
        /// <param name="source">Any string where strings shall be replaced</param>
        /// <param name="oldValues">A number of strings which shall be replaced</param>
        /// <param name="newValue">The new value which replaces all occurences</param>
        /// <returns></returns>
        public static string Replace(this string source, string[] oldValues, string newValue = "")
        {
            StringBuilder b = new StringBuilder(source);
            foreach (var str in oldValues)
                if (str != "")
                    b.Replace(str, newValue);
            return b.ToString();
        }

        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ToImageSource(this Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
