namespace Installer_Online.Objects
{
    class Extension
    {
        public Extension(string extension, string appName, string descr, string openWith, string iconPath = "")
        {
            Ext = extension;
            AppName = appName;
            Description = descr;
            OpenWith = openWith;
            IconPath = iconPath;
        }

        public string Ext { get; set; }

        public string AppName { get; set; }

        public string Description { get; set; }

        public string OpenWith { get; set; }

        public string IconPath { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(IconPath))
                return string.Format("{0};{1};{2};{3}", Ext, AppName, Description, OpenWith);
            else
                return string.Format("{0};{1};{2};{3};{4}", Ext, AppName, Description, OpenWith, IconPath);
        }
    }
}
