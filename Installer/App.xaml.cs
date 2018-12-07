using Installer.Objects;
using System;
using System.IO;
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

        public static string AppName { get => appName; }

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
    }
}
