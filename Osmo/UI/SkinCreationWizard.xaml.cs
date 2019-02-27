using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Logging;
using Osmo.Core.Objects;
using Osmo.Core.Reader;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for SkinCreationWizard.xaml
    /// </summary>
    public partial class SkinCreationWizard : Grid
    {
        private static SkinCreationWizard instance;

        public static SkinCreationWizard Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinCreationWizard();

                return instance;
            }
        }

        private SkinCreationWizard()
        {
            InitializeComponent();
        }

        internal void SetMasterViewModel(OsmoViewModel vm)
        {
            (DataContext as SkinWizardViewModel).Master = vm;
        }

        internal void ApplyData(NewSkinViewModel vm)
        {
            SkinWizardViewModel wizardVm = DataContext as SkinWizardViewModel;
            wizardVm.Name = vm.Name;
            wizardVm.Author = vm.Author;
            wizardVm.AddDummyFiles = vm.AddDummyFiles;
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text))
                (DataContext as SkinWizardViewModel).Name = "";
        }

        private void Author_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text))
                (DataContext as SkinWizardViewModel).Author = "";
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SkinWizardViewModel).Master.SelectedSidebarIndex = 0;
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            SkinWizardViewModel vm = DataContext as SkinWizardViewModel;
            vm.IsCreating = true;
            vm.FilesToCreate = 0;
            vm.CurrentFileCount = 0;
            vm.CurrentFileName = "";

            if (vm.ComponentInterface)
                vm.FilesToCreate += FixedValues.readerInterface.FileCount;
            if (vm.ComponentOsu)
                vm.FilesToCreate += FixedValues.readerStandard.FileCount;
            if (vm.ComponentMania)
                vm.FilesToCreate += FixedValues.readerMania.FileCount;
            if (vm.ComponentCTB)
                vm.FilesToCreate += FixedValues.readerCatch.FileCount;
            if (vm.ComponentTaiko)
                vm.FilesToCreate += FixedValues.readerTaiko.FileCount;
            if (vm.ComponentSounds)
                vm.FilesToCreate += FixedValues.readerSounds.FileCount;

            Logger.Instance.WriteLog("Creating new skin \"{0}\"... ({1} files to generate)", 
                vm.Name, vm.FilesToCreate);

            string skinPath = App.ProfileManager.Profile.OsuDirectory + "\\" + vm.Name;
            Directory.CreateDirectory(skinPath);
            bool spinnerNewChecked = (bool)cb_spinnerNew.IsChecked;
            bool catcherNewChecked = (bool)cb_catcherNew.IsChecked;

            new Thread(() =>
            {
                if (vm.ComponentInterface)
                {
                    WriteFilesFromReader(vm, FixedValues.readerInterface, skinPath);
                    Logger.Instance.WriteLog("Generating interface elements... ({0} files)", 
                        FixedValues.readerInterface.FileCount);
                }
                if (vm.ComponentOsu)
                {
                    WriteFilesFromReader(vm, FixedValues.readerStandard, skinPath, spinnerNewChecked);
                    Logger.Instance.WriteLog("Generating osu! Standard elements... ({0} files)",
                        FixedValues.readerStandard.FileCount);
                }
                if (vm.ComponentMania)
                {
                    WriteFilesFromReader(vm, FixedValues.readerMania, skinPath);
                    Logger.Instance.WriteLog("Generating Mania elements... ({0} files)",
                        FixedValues.readerMania.FileCount);
                }
                if (vm.ComponentCTB)
                {
                    WriteFilesFromReader(vm, FixedValues.readerCatch, skinPath, catcherNewChecked);
                    Logger.Instance.WriteLog("Generating CTB elements... ({0} files)",
                        FixedValues.readerCatch.FileCount);
                }
                if (vm.ComponentTaiko)
                {
                    WriteFilesFromReader(vm, FixedValues.readerTaiko, skinPath);
                    Logger.Instance.WriteLog("Generating Taiko elements... ({0} files)",
                        FixedValues.readerTaiko.FileCount);
                }
                if (vm.ComponentSounds)
                {
                    WriteSoundsFromReader(vm, FixedValues.readerSounds, skinPath);
                    Logger.Instance.WriteLog("Generating sounds... ({0} files)",
                        FixedValues.readerSounds.FileCount);
                }
            }).Start();

            string skinIniRaw = Properties.Resources.DefaulSkinIni;
            skinIniRaw = skinIniRaw.Replace("[NAME]", vm.Name)
                .Replace("[AUTHOR]", vm.Author)
                .Replace("[VERSION]", combo_version.Text);

            File.WriteAllText(Path.Combine(skinPath, "skin.ini"), skinIniRaw);
            Logger.Instance.WriteLog("skin.ini created!");
            vm.IsCreating = false;
            Skin skin = new Skin(skinPath);
            vm.Master.Skins.Add(skin);
            Logger.Instance.WriteLog("Skin \"{0}\" successfully created!", vm.Name);
            if (await SkinEditor.Instance.LoadSkin(skin))
            {
                vm.Master.SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
            }
        }

        /// <summary>
        /// This method iterates through all entries inside the given <see cref="SkinElementReader"/> and writes them into the given path.
        /// </summary>
        /// <param name="vm">The viewmodel of the skin creation wizard</param>
        /// <param name="reader">The reader which contains all information about skin elements</param>
        /// <param name="skinPath">The path where all elements should be saved to</param>
        /// <param name="useNewStyle">Optional: If true, this method will only export elements without flag or with the flag "New".
        /// <para>If false, this method will only export elements without flag or with the flag "Old".</para>
        /// <para>If null, flags are ignored.</para></param>
        private void WriteFilesFromReader(SkinWizardViewModel vm, SkinElementReader reader, string skinPath, bool? useNewStyle = null)
        {
            foreach (SkinningEntry entry in reader.Files)
            {
                if (useNewStyle == null)
                {
                    WriteFile(vm, skinPath, entry.Name + entry.PreferredFormat, reader);
                }
                else if (useNewStyle == true && !entry.Flags.Contains("old"))
                {
                    WriteFile(vm, skinPath, entry.Name + entry.PreferredFormat, reader);
                }
                else if (useNewStyle == false && !entry.Flags.Contains("new"))
                {
                    WriteFile(vm, skinPath, entry.Name + entry.PreferredFormat, reader);
                }
            }
        }

        private void WriteSoundsFromReader(SkinWizardViewModel vm, SkinSoundReader reader, string skinPath)
        {
            foreach (SoundEntry entry in reader.Files)
            {
                WriteFile(vm, skinPath, entry.Name + entry.PreferredFormat, reader);
            }
        }

        private void WriteFile(SkinWizardViewModel vm, string skinPath, string name, ElementGenerator gen)
        {
            vm.CurrentFileCount++;
            vm.CurrentFileName = name;
            gen.Generate(Path.Combine(skinPath, name));
        }

        private void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo_version.SelectedIndex == 0)
            {
                cb_spinnerOld.IsChecked = true;
            }

            if (combo_version.SelectedIndex < 4)
            {
                cb_catcherOld.IsChecked = true;
            }
        }
    }
}
