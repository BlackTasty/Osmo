using Installer_Online.ViewModel;

namespace Installer_Online.Objects
{
    public class Component : ViewModelBase
    {
        public Component(string name, bool isChecked, double space, ComponentType type)
        {
            Name = name;
            IsChecked = isChecked;
            SpaceRequired = space;
            Type = type;
        }
        public Component(string name, bool isChecked, double space, ComponentType type, bool isEnabled) : this(name, isChecked, space, type)
        {
            IsEnabled = isEnabled;
        }

        private string name;
        private bool isChecked;
        private bool isEnabled = true;
        private double spaceRequired = 0;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                InvokePropertyChanged("IsChecked");
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                InvokePropertyChanged("IsEnabled");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                InvokePropertyChanged("Name");
            }
        }

        public ComponentType Type { get; set; }

        public double SpaceRequired
        {
            get { return spaceRequired; }
            set { spaceRequired = value; }
        }
    }
}
