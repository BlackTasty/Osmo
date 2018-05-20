using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Osmo.Core.LibraryValidatior;

namespace Osmo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread()]
        //[DebuggerNonUserCode()]
        public static void Main()
        {
            bool canRun = true;
            if (ValidateLibraries())
            {
                canRun = !LibraryValidator.RequiredLibrariesMissing;

                if (canRun)
                {
                    canRun = MessageBox.Show(string.Format("I've detected something is missing!\n{0}\n\nOsmo can be started, but only limited support is given!", LibraryValidator.GetFailedLibraries())
                        , "", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                }
                else
                {
                    MessageBox.Show(string.Format("I've detected something is missing!\n{0}\n\nOsmo can not be started!!", LibraryValidator.GetFailedLibraries())
                       , "", MessageBoxButton.OK);
                }
            }

            if (canRun)
            {
                Un4seen.Bass.BassNet.Registration("raphael10@live.at", "2X373361752918");
                App app = new App()
                {
                    ShutdownMode = ShutdownMode.OnMainWindowClose
                };
                app.InitializeComponent();
                app.Run();
            }
        }

        private static bool ValidateLibraries()
        {
            List<Library> libraries = new List<Library>()
            {
                new Library(AppDomain.CurrentDomain.BaseDirectory, "ControlzEx.dll"),
                new Library(AppDomain.CurrentDomain.BaseDirectory, "MahApps.Metro.dll"),
                new Library(AppDomain.CurrentDomain.BaseDirectory, "MaterialDesignColors.dll"),
                new Library(AppDomain.CurrentDomain.BaseDirectory, "MaterialDesignThemes.MahApps.dll"),
                new Library(AppDomain.CurrentDomain.BaseDirectory, "MaterialDesignThemes.Wpf.dll"),
                new Library(AppDomain.CurrentDomain.BaseDirectory, "System.Windows.Interactivity.dll"),
            };
            LibraryValidator.ValidateLibraries(libraries);
            return LibraryValidator.LibrariesMissing;
        }
    }
}
