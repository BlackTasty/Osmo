using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for NewTemplateDialog.xaml
    /// </summary>
    public partial class NewTemplateDialog : DockPanel
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
                AppConfiguration.GetInstance().TemplateDirectory, vm.Name);

            File.WriteAllText(templatePath, "");

            RaiseTemplateCreatedEvent(new ForumTemplate(templatePath));
        }
    }
}
