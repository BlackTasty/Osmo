using Installer.Objects;
using System.Collections.ObjectModel;

namespace Installer.ViewModel
{
    public class ComponentsViewModel : ViewModelBase
    {
        private ObservableCollection<Component> m_components = new ObservableCollection<Component>();
        private double m_spaceRequired = 0;

        public ObservableCollection<Component> Components
        {
            get { return m_components; }
            set { m_components = value; }
        }

        public double SpaceRequired
        {
            get { return m_spaceRequired; }
            set
            {
                m_spaceRequired = value;
                InvokePropertyChanged();
            }
        }

        public Component GetComponent(ComponentType type)
        {
            foreach (Component component in m_components)
            {
                if (component.Type == type)
                    return component;
            }

            return null;
        }
    }
}
