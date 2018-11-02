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
        private const string txt_edgePlayer = "Vibrance Player",
            txt_desktopIcon = "Desktop Icon",
            txt_startmenu = "Startmenu shortcuts*",
            txt_associate = "Associate audio files*",
            txt_lyricsCreator = "Lyrics Creator",
            txt_visualizerStudio = "Visualizer Studio";

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
            vm.Components.Add(new Component(txt_edgePlayer, true, 11627.86719, ComponentType.EDGE_PLAYER, false));
            vm.Components.Add(new Component(txt_desktopIcon, false, 179, ComponentType.SHORTCUT));
            vm.Components.Add(new Component(txt_startmenu, false, 0, ComponentType.STARTMENU));
            vm.Components.Add(new Component(txt_associate, true, 0, ComponentType.ASSOCIATE_EXTENSIONS));
            vm.Components.Add(new Component(txt_lyricsCreator, true, 220, ComponentType.LYRICS_CREATOR));
            vm.Components.Add(new Component(txt_visualizerStudio, true, 424, ComponentType.VISUALIZER_STUDIO));
            CalculateSpace();

            window.ViewModel = vm;

            RegistryKey edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player", false);
            if (edgeKey != null)
            {
                uacComponentsChecked[2] = edgeKey.GetValue("LyricsCreator", 0).ToString() == "0";
                uacComponentsChecked[3] = edgeKey.GetValue("VisualizerStudio", 0).ToString() == "0";
                edgeKey.Close();
            }
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            CheckBox cb = (CheckBox)((ContentControl)sender).Content;
            string name = cb.Content.ToString();
            switch (name)
            {
                case txt_edgePlayer:
                    txt_info.Text = "The application, where you can listen your music from osu!, put them into playlists and export them.";
                    break;
                case txt_desktopIcon:
                    txt_info.Text = "Creates a shortcut to the application on your desktop.";
                    break;
                case txt_startmenu:
                    txt_info.Text = "Creates shortcuts to the player in your startmenu.";
                    break;
                case txt_associate:
                    txt_info.Text = "Play audio files (.mp3, .wav, .ogg, .mp2, .mp1, .aif) with the Vibrance Player by default.";
                    break;
                case txt_lyricsCreator:
                    txt_info.Text = "Installs the Lyrics Creator, with which you can easily create and edit lyrics for your songs!";
                    break;
                case txt_visualizerStudio:
                    txt_info.Text = "Installs the Visualizer Studio, with which you can create your own visualizer!";
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
            RegistryKey edgeKey = Registry.CurrentUser.OpenSubKey(@"Software\Vibrance Player", false);
            switch (cb.Content.ToString())
            {
                case txt_associate:
                    uacComponentsChecked[0] = (bool)cb.IsChecked;
                    break;
                case txt_startmenu:
                    uacComponentsChecked[1] = (bool)cb.IsChecked;
                    break;
                case txt_lyricsCreator:
                    if (cb.IsChecked == true)
                        uacComponentsChecked[2] = edgeKey.GetValue("LyricsCreator", 0).ToString() == "0";
                    else
                        uacComponentsChecked[2] = false;
                    break;
                case txt_visualizerStudio:
                    if (cb.IsChecked == true)
                        uacComponentsChecked[3] = edgeKey.GetValue("VisualizerStudio", 0).ToString() == "0";
                    else
                        uacComponentsChecked[3] = false;
                    break;
            }
            edgeKey.Close();

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
