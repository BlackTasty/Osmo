using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class TemplateEditorViewModel : ViewModelBase
    {
        private ForumTemplate mTemplate;
        private string mPreviewText;

        public ForumTemplate Template
        {
            get => mTemplate ?? new ForumTemplate();
            set
            {
                mTemplate = value;
                InvokePropertyChanged("Template");
            }
        }

        public Skin TargetSkin
        {
            get;set;
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

        public string Name
        {
            get => mTemplate?.Name;
            set
            {
                mTemplate.Rename(value);
                InvokePropertyChanged("Template");
            }
        }
    }
}
