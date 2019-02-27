using Installer_Online.Objects;
using System;
using System.Reflection;
using System.Windows;

namespace Installer_Online
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string AppComponentName = "Osmo";

        [STAThread()]
        //[DebuggerNonUserCode()]
        public static void Main()
        {
            App app = new App()
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            EmbeddedAssembly.Load("Installer_Online.MaterialDesignColors.dll", "MaterialDesignColors.dll");
            EmbeddedAssembly.Load("Installer_Online.MaterialDesignThemes.Wpf.dll", "MaterialDesignThemes.Wpf.dll");
            
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
