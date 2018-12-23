using Installer.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Installer.Objects;
using Microsoft.Win32;

namespace Installer.UI
{
    /// <summary>
    /// Interaktionslogik für Components.xaml
    /// </summary>
    public partial class Components : UserControl, IManagedUI
    {
        public ComponentsViewModel vm;
        private MainWindow window;
        private bool[] uacComponentsChecked = new bool[] { true, false, true, true};
        private const string txt_application = App.AppComponentName,
            txt_desktopIcon = "Desktop Icon",
            txt_startmenu = "Startmenu shortcuts*";

        public Components()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void RegisterParent(MainWindow window)
        {
            this.window = window;
            vm = (ComponentsViewModel)DataContext;
            vm.Components.Add(new Component(txt_application, true, 10092.24, ComponentType.APPLICATION, false));
            vm.Components.Add(new Component(txt_desktopIcon, false, 179, ComponentType.SHORTCUT));
            vm.Components.Add(new Component(txt_startmenu, false, 0, ComponentType.STARTMENU));
            CalculateSpace();

            window.ViewModel = vm;
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            CheckBox cb = (CheckBox)((ContentControl)sender).Content;
            string name = cb.Content.ToString();
            switch (name)
            {
                case txt_application:
                    txt_info.Text = "Install " + txt_application + " and it's required components";
                    break;
                case txt_desktopIcon:
                    txt_info.Text = "Creates a shortcut to the application on your desktop.";
                    break;
                case txt_startmenu:
                    txt_info.Text = "Creates shortcuts to the player in your startmenu.";
                    break;
            }
        }

        private void CheckBox_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_info.Text = "Position your mouse over a component to see its description.";
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CalculateSpace();
            CheckBox cb = (CheckBox)sender;
            string content = cb.Content.ToString();

            if (!window.IsUpgrade)
                window.ShowUACIcon(true);
            else
                window.ShowUACIcon(IsAtLeastOneChecked(cb));
        }

        private bool IsAtLeastOneChecked(CheckBox cb)
        {
            switch (cb.Content.ToString())
            {
                case txt_startmenu:
                    uacComponentsChecked[0] = (bool)cb.IsChecked;
                    break;
            }

            foreach (bool isChecked in uacComponentsChecked)
            {
                if (isChecked)
                    return true;
            }

            return false;
        }

        private void CalculateSpace()
        {
            double total = 0;
            foreach (Component component in vm.Components)
                if (component.IsChecked)
                    total += component.SpaceRequired;

            vm.SpaceRequired = Math.Round(total / 1024, 2);
        }
    }
}
