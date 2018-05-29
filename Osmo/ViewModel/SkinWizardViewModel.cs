using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class SkinWizardViewModel : NewSkinViewModel
    {
        private List<string> mVersions = new List<string>()
        {
            "1.0",
            "2.0",
            "2.1",
            "2.2",
            "2.3",
            "2.4",
            "2.5",
            "latest"
        };

        private string mSelectedVersion;

        private bool mComponentInterface = true;
        private bool mComponentSounds = true;
        private bool mComponentOsu;
        private bool mComponentCTB;
        private bool mComponentTaiko;
        private bool mComponentMania;

        public List<string> Versions => mVersions;

        public string SelectedVersion
        {
            get => mSelectedVersion;
            set
            {
                mSelectedVersion = value;
                InvokePropertyChanged("SelectedVersion");
            }
        }

        public bool ComponentInterface
        {
            get => mComponentInterface;
            set
            {
                mComponentInterface = value;
                InvokePropertyChanged("ComponentInterface");
            }
        }

        public bool ComponentSounds
        {
            get => mComponentSounds;
            set
            {
                mComponentSounds = value;
                InvokePropertyChanged("ComponentSounds");
            }
        }

        public bool ComponentOsu
        {
            get => mComponentOsu;
            set
            {
                mComponentOsu = value;
                InvokePropertyChanged("ComponentOsu");
            }
        }

        public bool ComponentCTB
        {
            get => mComponentCTB;
            set
            {
                mComponentCTB = value;
                InvokePropertyChanged("ComponentCTB");
            }
        }

        public bool ComponentTaiko
        {
            get => mComponentTaiko;
            set
            {
                mComponentTaiko = value;
                InvokePropertyChanged("ComponentTaiko");
            }
        }

        public bool ComponentMania
        {
            get => mComponentMania;
            set
            {
                mComponentMania = value;
                InvokePropertyChanged("ComponentMania");
            }
        }

        public void ApplyBaseData(NewSkinViewModel vm)
        {
            Name = vm.Name;
            Author = vm.Author;
            AddDummyFiles = vm.AddDummyFiles;
        }
    }
}
