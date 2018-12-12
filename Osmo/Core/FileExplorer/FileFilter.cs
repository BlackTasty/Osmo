namespace Osmo.Core.FileExplorer
{
    class FileFilter
    {
        public string Description { get; private set; }

        public string[] Extensions { get; private set; }

        public FileFilter(string description, string rawExtensions)
        {
            Description = description;
            Extensions = rawExtensions.Replace("*", "").Split(';');
        }

        //TODO: Use Regex expressions to match filter
        public bool FilterMatch(string input)
        {
            for (int i = 0; i < Extensions.Length; i++)
            {
                if (input.Contains(Extensions[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
