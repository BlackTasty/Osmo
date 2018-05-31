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

        private bool mComponentInterface = true;
        private bool mComponentSounds = true;
        private bool mComponentOsu;
        private bool mComponentCTB;
        private bool mComponentTaiko;
        private bool mComponentMania;

        private bool mIsCreating;
        private double mCurrentFileCount;
        private double mFilesToCreate;
        private string mCurrentFileName;

        public List<string> Versions => mVersions;

        public bool IsConfirmEnabled
        {
            get => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Author);
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

        public bool IsCreating
        {
            get => mIsCreating;
            set
            {
                mIsCreating = value;
                InvokePropertyChanged("IsCreating");
            }
        }

        public double CurrentFileCount
        {
            get => mCurrentFileCount;
            set
            {
                mCurrentFileCount = value;
                InvokePropertyChanged("Current");
            }
        }

        public double FilesToCreate
        {
            get => mFilesToCreate;
            set
            {
                mFilesToCreate = value;
                InvokePropertyChanged("FilesToCreate");
            }
        }

        public string CurrentFileName
        {
            get => mCurrentFileName;
            set
            {
                mCurrentFileName = value;
                InvokePropertyChanged("CurrentFileName");
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
