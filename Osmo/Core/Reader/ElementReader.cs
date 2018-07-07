namespace Osmo.Core.Reader
{
    class ElementReader
    {
        protected string Version { get; private set; }

        protected bool VersionEqualOrHigher { get; private set; } //Is set to true if a + is found in the version column

        protected bool VersionEqualOrLower { get; private set; } //Is set to true if a - is found in the version column

        protected string[] ReadLine(string line)
        {
            return line.Trim()[0] != '#' ? line.Split('|') : null;
        }

        protected void SetVersion(string version)
        {
            VersionEqualOrHigher = version.EndsWith("+");
            VersionEqualOrLower = version.EndsWith("-");
            Version = version.Replace("+", "").Replace("-", "");
        }

        internal bool VersionMatches(string version)
        {
            if (!string.IsNullOrWhiteSpace(Version))
            {
                double targetVersion = Parser.TryParse(version, 1.0);
                double entryVersion = Parser.TryParse(Version, 1.0);

                if (VersionEqualOrHigher)
                    return targetVersion >= entryVersion;

                if (VersionEqualOrLower)
                    return targetVersion <= entryVersion;

                return targetVersion == entryVersion;
            }
            return true;
        }
    }
}
