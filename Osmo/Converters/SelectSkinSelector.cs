using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Osmo.Converters
{
    public class SelectSkinSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate NewSkinTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item.GetType() == typeof(Skin))
            {
                if (container is FrameworkElement element)
                {
                    Skin skin = item as Skin;
                    if (!skin.IsEmpty)
                    {
                        DefaultTemplate = element.FindResource("SkinDataTemplate") as DataTemplate;
                        return DefaultTemplate;
                    }
                    else
                    {
                        NewSkinTemplate = element.FindResource("NewSkinDataTemplate") as DataTemplate;
                        return NewSkinTemplate;
                    }
                }
            }

            return null;
        }
    }
}
