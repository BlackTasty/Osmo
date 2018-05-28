using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.Core.Reader
{
    class SkinningReader
    {
        private string version;
        private bool versionEqualOrLower; //Is set to true if a - is found in the version column
        private bool versionEqualOrHigher; //Is set to true if a + is found in the version column

        protected string Version => version;

        protected bool VersionEqualOrHigher => versionEqualOrHigher;

        protected bool VersionEqualOrLower => versionEqualOrLower;

        protected string[] ReadLine(string line)
        {
            return line.Trim()[0] != '#' ? line.Split('|') : null;
        }

        protected void SetVersion(string version)
        {
            versionEqualOrHigher = version.EndsWith("+");
            versionEqualOrLower = version.EndsWith("-");
            this.version = version.Replace("+", "").Replace("-", "");
        }

        internal bool VersionMatches(string version)
        {
            if (!string.IsNullOrWhiteSpace(this.version))
            {
                double targetVersion = Parser.TryParse(version, 1.0);
                double entryVersion = Parser.TryParse(this.version, 1.0);

                if (versionEqualOrHigher)
                    return targetVersion >= entryVersion;

                if (versionEqualOrLower)
                    return targetVersion <= entryVersion;

                return targetVersion == entryVersion;
            }
            return true;
        }
    }
}
