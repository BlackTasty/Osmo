using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class SkinWizardViewModel : NewSkinViewModel
    {
        private List<string> versions = new List<string>()
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

        public List<string> Versions => versions;

        public void ApplyBaseData(NewSkinViewModel vm)
        {
            Name = vm.Name;
            Author = vm.Author;
            AddDummyFiles = vm.AddDummyFiles;
        }
    }
}
