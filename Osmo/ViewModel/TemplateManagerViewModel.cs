using Osmo.Core;
using Osmo.Core.Objects;
using System.IO;
using System.Linq;

namespace Osmo.ViewModel
{
    class TemplateManagerViewModel : ViewModelBase
    {
        public VeryObservableCollection<ForumTemplate> Templates { get; set; } = 
            new VeryObservableCollection<ForumTemplate>("Templates");

        public TemplateManagerViewModel()
        {
            if (!App.IsDesigner)
            {
                Templates.Add(new ForumTemplate());

                if (!Directory.Exists(App.ProfileManager.Profile.TemplateDirectory))
                {
                    Directory.CreateDirectory(App.ProfileManager.Profile.TemplateDirectory);
                    File.WriteAllText(App.ProfileManager.Profile.TemplateDirectory + "Default.oft",
                        Properties.Resources.DefaultTemplate);
                    File.WriteAllText(App.ProfileManager.Profile.TemplateDirectory + "Official.oft",
                        Properties.Resources.OfficialTemplate);
                }
                LoadTemplates();
            }
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
            foreach (FileInfo fi in new DirectoryInfo(App.ProfileManager.Profile.TemplateDirectory).EnumerateFiles("*.oft"))
            {
                Templates.Add(new ForumTemplate(fi.FullName));
            }
        }
    }
}
