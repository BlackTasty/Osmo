using System;
using System.Reflection;
using System.Windows;
using Uninstaller.Objects;

namespace Uninstaller
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private static string appName;

        public static string AppName { get => appName; }

        [STAThread()]
        //[DebuggerNonUserCode()]
        public static void Main()
        {
            App app = new App()
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            EmbeddedAssembly.Load("Uninstaller.MaterialDesignColors.dll", "MaterialDesignColors.dll");
            EmbeddedAssembly.Load("Uninstaller.MaterialDesignThemes.Wpf.dll", "MaterialDesignThemes.Wpf.dll");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            appName = Helper.FindString("appName"); 

            app.InitializeComponent();
            app.Run();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
