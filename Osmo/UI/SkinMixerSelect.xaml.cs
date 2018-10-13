using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Objects;
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
    /// Interaction logic for SkinMixerSelect.xaml
    /// </summary>
    public partial class SkinMixerSelect : Grid, IHotkeyHelper
    {
        public event EventHandler<EventArgs> DialogClosed;

        public SkinMixerSelect()
        {
            InitializeComponent();
        }

        private void Skins_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadSkin(true);
        }

        private void SelectSkin_Click(object sender, RoutedEventArgs e)
        {
            LoadSkin(false);
        }

        private async void LoadSkin(bool closeDialog)
        {
            if (Tag != null)
            {
                string target = Tag.ToString();
                if (target.Equals("mixer"))
                {
                    await SkinMixer.Instance.LoadSkin(lv_skins.SelectedItem as Skin, false);
                }
                else if (target.Equals("template"))
                {
                    TemplateEditor.Instance.MakePreview(lv_skins.SelectedItem as Skin);
                }
            }

            if (closeDialog)
            {
                if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                    DialogHost.CloseDialogCommand.Execute(null, null);
                OnDialogClosed(EventArgs.Empty);
            }
        }

        public bool ForwardKeyboardInput(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O && lv_skins.SelectedIndex != -1)
            {
                e.Handled = true;
                LoadSkin(true);
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                if (DialogHost.CloseDialogCommand.CanExecute(null, null))
                    DialogHost.CloseDialogCommand.Execute(null, null);
            }

            return e.Handled;
        }

        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ForwardKeyboardInput(e);
        }

        protected virtual void OnDialogClosed(EventArgs e)
        {
            DialogClosed?.Invoke(this, e);
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogClosed(EventArgs.Empty);
        }
    }
}
