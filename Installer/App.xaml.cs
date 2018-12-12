using Installer.Objects;
using System;
using System.Reflection;
using System.Windows;

namespace Installer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private static string appName;

        public const string AppComponentName = "Osmo";


        [STAThread()]
        //[DebuggerNonUserCode()]
        public static void Main()
        {
            App app = new App()
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            EmbeddedAssembly.Load("Installer.MaterialDesignColors.dll", "MaterialDesignColors.dll");
            EmbeddedAssembly.Load("Installer.MaterialDesignThemes.Wpf.dll", "MaterialDesignThemes.Wpf.dll");

            appName = Helper.FindString("appName");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            app.InitializeComponent();
            app.Run();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

        //public void ChangeBaseTheme(bool isDark)
        //{
        //    ResourceDictionary theme = new ResourceDictionary();

        //    if (isDark)
        //    {
        //        theme.Source = new Uri(@"Resources/Theme/Dark.xaml", UriKind.RelativeOrAbsolute);
        //    }
        //    else
        //    {
        //        theme.Source = new Uri(@"Resources/Theme/Light.xaml", UriKind.RelativeOrAbsolute);
        //    }

        //    Resources.MergedDictionaries[0] = theme;
        //    new PaletteHelper().SetLightDark(isDark);
        //}
    }
}
