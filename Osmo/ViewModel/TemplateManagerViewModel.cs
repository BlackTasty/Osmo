using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class TemplateManagerViewModel : ViewModelBase
    {
        public VeryObservableCollection<ForumTemplate> Templates { get; set; } = 
            new VeryObservableCollection<ForumTemplate>("Templates");

        public TemplateManagerViewModel()
        {
            Templates.Add(new ForumTemplate());
            LoadTemplates();
        }

        public void DeleteTemplate(string name)
        {
            ForumTemplate template = Templates.FirstOrDefault(x => !x.IsEmpty && x.Name.Equals(name)) ?? new ForumTemplate();

            if (!template.IsEmpty)
            {
                File.Delete(template.Path);
                Templates.Remove(template);
            }
        }

        private void LoadTemplates()
        {
            foreach (FileInfo fi in new DirectoryInfo(AppConfiguration.GetInstance().TemplateDirectory).EnumerateFiles("*.oft"))
            {
                Templates.Add(new ForumTemplate(fi.FullName));
            }
        }
    }
}
