using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class TemplatePreviewViewModel : ViewModelBase
    {
        private ForumTemplate mTemplate;
        private string mPreviewText;

        public ObservableCollection<ForumTemplate> Templates { get; set; }

        public Skin Skin { get; set; }

        public ForumTemplate Template
        {
            get => mTemplate;
            set
            {
                mTemplate = value;
                PreviewText = Helper.ApplyForumTemplate(value, Skin);
                InvokePropertyChanged("Template");
            }
        }

        public string PreviewText
        {
            get => mPreviewText;
            set
            {
                mPreviewText = value;
                InvokePropertyChanged("PreviewText");
            }
        }

        public void LoadTemplates()
        {
            Templates = new ObservableCollection<ForumTemplate>();

            foreach (FileInfo fi in new DirectoryInfo(AppConfiguration.GetInstance().TemplateDirectory).EnumerateFiles("*.oft"))
            {
                Templates.Add(new ForumTemplate(fi.FullName));
            }
            InvokePropertyChanged("Templates");
        }
    }
}
