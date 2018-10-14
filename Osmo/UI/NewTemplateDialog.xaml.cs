using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Logging;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for NewTemplateDialog.xaml
    /// </summary>
    public partial class NewTemplateDialog : DockPanel, IHotkeyHelper
    {
        public static readonly RoutedEvent TemplateCreatedEvent = EventManager.RegisterRoutedEvent("TemplateCreated",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewTemplateDialog));

        public event RoutedEventHandler TemplateCreated
        {
            add { AddHandler(TemplateCreatedEvent, value); }
            remove { RemoveHandler(TemplateCreatedEvent, value); }
        }

        private void RaiseTemplateCreatedEvent(ForumTemplate template)
        {
            TemplateCreatedEventArgs eventArgs = new TemplateCreatedEventArgs(TemplateCreatedEvent, template);
            RaiseEvent(eventArgs);
            Logger.Instance.WriteLog("New template \"{0}\" created!", template.Name);
        }

        public NewTemplateDialog()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_create.IsEnabled = !string.IsNullOrWhiteSpace(txt_name.Text);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            NewTemplateViewModel vm = DataContext as NewTemplateViewModel;

            string templatePath = string.Format("{0}\\{1}.oft",
                App.ProfileManager.Profile.TemplateDirectory, vm.Name);

            File.WriteAllText(templatePath, "");

            RaiseTemplateCreatedEvent(new ForumTemplate(templatePath));
        }

        public bool ForwardKeyboardInput(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                    DialogHost.CloseDialogCommand.Execute(null, null);
            }

            return e.Handled;
        }

        private void DockPanel_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ForwardKeyboardInput(e);
        }
    }
}
