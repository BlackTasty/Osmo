using System;
using System.Windows;

namespace Installer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public void ChangeTheme(Uri uri)
        {
            Resources.MergedDictionaries[0] = new ResourceDictionary() { Source = uri };
        }
    }
}
