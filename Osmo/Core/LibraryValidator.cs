using Osmo.Core.Objects;
using System.Collections.Generic;

namespace Osmo.Core
{
    internal static class LibraryValidator
    {
        private static List<Library> failedRequiredLibs = new List<Library>();
        private static List<Library> failedOptionalLibs = new List<Library>();

        public static bool LibrariesMissing { get { return RequiredLibrariesMissing || OptionalLibrariesMissing; } }

        public static bool RequiredLibrariesMissing { get { return failedRequiredLibs.Count > 0; } }

        public static bool OptionalLibrariesMissing { get { return failedOptionalLibs.Count > 0; } }

        public static void ValidateLibraries(List<Library> targets)
        {
            failedRequiredLibs.Clear();
            foreach (Library library in targets)
            {
                library.Exists(out bool exists, out bool isRequired, out string message);
                if (!exists && isRequired)
                    failedRequiredLibs.Add(library);
                else if (!exists && !isRequired)
                    failedOptionalLibs.Add(library);
            }
        }

        public static bool ValidateLibrary(Library library)
        {
            failedRequiredLibs.Clear();
            library.Exists(out bool exists, out bool isRequired, out string message);
            return exists;
        }

        public static string GetFailedLibraries()
        {
            string output = "";

            if (RequiredLibrariesMissing)
            {
                output = "Following libraries are required to run:";
                foreach (Library library in failedRequiredLibs)
                {
                    output += "\n- " + library.ToString();
                }

                if (OptionalLibrariesMissing)
                    output += "\n\n";
            }

            if (OptionalLibrariesMissing)
            {
                output += "Following libraries aren't required to run, but can lead to crashes if not present:";
                foreach (Library library in failedOptionalLibs)
                {
                    if (string.IsNullOrWhiteSpace(library.Message))
                        output += "\n- " + library.ToString();
                    else
                        output += string.Format("\n- {0} ({1})", library.ToString(), library.Message);
                }
            }

            return output;
        }
    }
}
