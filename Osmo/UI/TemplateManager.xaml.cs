using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for TemplateManager.xaml
    /// </summary>
    public partial class TemplateManager : Grid, IHotkeyHelper
    {
        #region Singleton implementation
        private static TemplateManager instance;

        public static TemplateManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TemplateManager();

                return instance;
            }
        }
        #endregion

        private OsmoViewModel master;

        private TemplateManager()
        {
            InitializeComponent();
        }

        internal void SetMasterViewModel(OsmoViewModel vm)
        {
            master = vm;
        }

        private void Templates_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_templates.SelectedIndex > 0)
            {
                TemplateEditor.Instance.LoadTemplate(lv_templates.SelectedItem as ForumTemplate);
                master.SelectedSidebarIndex = FixedValues.TEMPLATE_EDITOR_INDEX;
            }
        }

        private void Templates_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lv_templates.SelectedIndex == 0)
            {
                if (DialogHost.OpenDialogCommand.CanExecute(btn_newTemplate.CommandParameter, btn_newTemplate))
                    DialogHost.OpenDialogCommand.Execute(btn_newTemplate.CommandParameter, btn_newTemplate);
            }
        }

        private void TemplateDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteTemplate((sender as Button).Tag.ToString());
        }

        private async void DeleteTemplate(string name)
        {
            var msgBox = MaterialMessageBox.Show(Helper.FindString("templateManager_deleteTitle"),
                Helper.FindString("templateManager_deleteDescription"),
                OsmoMessageBoxButton.YesNo);

            await DialogHelper.Instance.ShowDialog(msgBox);

            if (msgBox.Result == OsmoMessageBoxResult.Yes)
            {
                (DataContext as TemplateManagerViewModel).DeleteTemplate(name);
            }
        }

        private void EditTemplate_Click(object sender, RoutedEventArgs e)
        {
            TemplateEditor.Instance.LoadTemplate(lv_templates.SelectedItem as ForumTemplate);
            master.SelectedSidebarIndex = FixedValues.TEMPLATE_EDITOR_INDEX;
        }
        
        private void Dialog_TemplateCreated(object sender, RoutedEventArgs e)
        {
            var args = e as TemplateCreatedEventArgs;
            (DataContext as TemplateManagerViewModel).Templates.Add(args.Template);
            TemplateEditor.Instance.LoadTemplate(args.Template);
            master.SelectedSidebarIndex = FixedValues.TEMPLATE_EDITOR_INDEX;
        }

        private void NewTemplateDialog_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as NewTemplateDialog).TemplateCreated += Dialog_TemplateCreated;
        }

        public bool ForwardKeyboardInput(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        e.Handled = true;
                        if (DialogHost.OpenDialogCommand.CanExecute(btn_newTemplate.CommandParameter, btn_newTemplate))
                            DialogHost.OpenDialogCommand.Execute(btn_newTemplate.CommandParameter, btn_newTemplate);
                        break;
                    case Key.O:
                        e.Handled = true;
                        if (lv_templates.SelectedIndex > 0)
                        {
                            TemplateEditor.Instance.LoadTemplate(lv_templates.SelectedItem as ForumTemplate);
                            master.SelectedSidebarIndex = FixedValues.TEMPLATE_EDITOR_INDEX;
                        }
                        break;
                }
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                if (lv_templates.SelectedIndex > 0)
                {
                    DeleteTemplate((lv_templates.SelectedItem as Skin).Name);
                }
            }

            return e.Handled;
        }

        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ForwardKeyboardInput(e);
        }
    }
}
