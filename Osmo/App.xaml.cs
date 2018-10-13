using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Logging;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Osmo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static int sessionId;
        private static MainWindow window;
        private static ProfileManager profileManager;
        
        public static event EventHandler<EventArgs> LanguageChanged;

        public static int SessionID { get => sessionId; }

        public static ProfileManager ProfileManager { get => profileManager; }

        [STAThread()]
        //[DebuggerNonUserCode()]
        public static void Main()
        {
            bool canRun = true;

            Logger.Instance.WriteLog("Validating if all libraries exist...");
            if (ValidateLibraries())
            {
                canRun = !LibraryValidator.RequiredLibrariesMissing;

                if (canRun)
                {
                    Logger.Instance.WriteLog("There are missing libraries, but Osmo is still able to start! {0}", LogType.WARNING, LibraryValidator.GetFailedLibraries());
                    canRun = MessageBox.Show(string.Format("I've detected something is missing!\n{0}\n\nOsmo can be started, but only limited support is given!", LibraryValidator.GetFailedLibraries())
                        , "", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                }
                else
                {
                    Logger.Instance.WriteLog("There are missing libraries and Osmo cannot be started! {0}", LogType.ERROR, LibraryValidator.GetFailedLibraries());
                    MessageBox.Show(string.Format("I've detected something is missing!\n{0}\n\nOsmo can not be started!!", LibraryValidator.GetFailedLibraries())
                       , "", MessageBoxButton.OK);
                }
            }

            if (canRun)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Logger.Instance.WriteLog("Initializing Osmo...");
                sessionId = Logger.Instance.SessionID;
                profileManager = new ProfileManager(new AppConfiguration());
                Un4seen.Bass.BassNet.Registration("raphael10@live.at", "2X373361752918");
                App app = new App()
                {
                    ShutdownMode = ShutdownMode.OnMainWindowClose
                };
                app.InitializeComponent();
                window = (MainWindow)app.MainWindow;
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
                new Library(AppDomain.CurrentDomain.BaseDirectory, "System.Windows.Interactivity.dll")
            };
            LibraryValidator.ValidateLibraries(libraries);
            return LibraryValidator.LibrariesMissing;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Logger logger = new Logger();

            //logger.WriteLog("An error has occurred! See exception log for details!", Vibrance.Core.Enums.LoggingType.FATAL, (Exception)e.ExceptionObject);
            SystemInformation.CreateCrashLog((Exception)e.ExceptionObject, window);
        }

        public void ChangeLanguage(Language lang)
        {
            ResourceDictionary localisation = new ResourceDictionary();
            string threadLang = "en";

            switch (lang)
            {
                case Language.Spanish:
                    localisation.Source = new Uri(@"Localisation\StringResources.es.xaml", UriKind.Relative);
                    threadLang = "es";
                    break;
                case Language.German:
                    localisation.Source = new Uri(@"Localisation\StringResources.de.xaml", UriKind.Relative);
                    threadLang = "de";
                    break;
                default:
                    localisation.Source = new Uri(@"Localisation\StringResources.xaml", UriKind.Relative);
                    threadLang = "en";
                    break;
            }

            Thread.CurrentThread.CurrentUICulture =
               new System.Globalization.CultureInfo(threadLang);
            Resources.MergedDictionaries[2] = localisation;
            OnLanguageChanged(EventArgs.Empty);
        }

        protected virtual void OnLanguageChanged(EventArgs e)
        {
            LanguageChanged?.Invoke(this, e);
        }
    }
}
