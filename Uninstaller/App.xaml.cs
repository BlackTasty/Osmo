using System;
using System.Windows;

namespace Uninstaller
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public void ChangeTheme(Uri uri)
        {
            if (uri != null)
                Resources.MergedDictionaries[0] = new ResourceDictionary() { Source = uri };
        }
    }
}
