namespace Installer.Objects
{
    class StartmenuEntry
    {
        private string appName;
        private string icon;
        private string path;

        public StartmenuEntry(string name, string icon, string path)
        {
            appName = name;
            this.icon = icon;
            this.path = path;
        }

        public string AppName { get { return appName; } }

        public string Icon { get { return icon; } }

        public string Path { get { return path; } }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}", AppName, Icon, Path);
        }
    }
}
