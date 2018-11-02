using System.Text;

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
    }
}
