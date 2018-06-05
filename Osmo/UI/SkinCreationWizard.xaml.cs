using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Configuration;
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

        internal void ApplyData(NewSkinViewModel vm)
        {
            SkinWizardViewModel wizardVm = DataContext as SkinWizardViewModel;
            wizardVm.Name = vm.Name;
            wizardVm.Author = vm.Author;
            wizardVm.AddDummyFiles = vm.AddDummyFiles;
            wizardVm.Master = vm.Master;
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

        private void Confirm_Click(object sender, RoutedEventArgs e)
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

            string skinDirectory = AppConfiguration.GetInstance().OsuDirectory + "\\Skins\\" + vm.Name;
            Directory.CreateDirectory(skinDirectory);

            new Thread(() =>
            {
                if (vm.ComponentInterface)
                {
                    WriteFilesFromReader(vm, FixedValues.readerInterface, skinDirectory);
                }
                if (vm.ComponentOsu)
                {
                    WriteFilesFromReader(vm, FixedValues.readerStandard, skinDirectory);
                }
                if (vm.ComponentMania)
                {
                    WriteFilesFromReader(vm, FixedValues.readerMania, skinDirectory);
                }
                if (vm.ComponentCTB)
                {
                    WriteFilesFromReader(vm, FixedValues.readerCatch, skinDirectory);
                }
                if (vm.ComponentTaiko)
                {
                    WriteFilesFromReader(vm, FixedValues.readerTaiko, skinDirectory);
                }
                if (vm.ComponentSounds)
                {
                    WriteSoundsFromReader(vm, FixedValues.readerSounds, skinDirectory);
                }
            }).Start();

            string skinIniRaw = Properties.Resources.DefaulSkinIni;
            skinIniRaw = skinIniRaw.Replace("[NAME]", vm.Name)
                .Replace("[AUTHOR]", vm.Author)
                .Replace("[VERSION]", combo_version.Text);

            File.WriteAllText(Path.Combine(skinDirectory, "skin.ini"), skinIniRaw);
            vm.IsCreating = false;
            Skin skin = new Skin(skinDirectory);
            vm.Master.Skins.Add(skin);
            SkinEditor.Instance.LoadSkin(skin);
            vm.Master.SelectedSidebarIndex = FixedValues.EDITOR_INDEX;
        }

        private void WriteFilesFromReader(SkinWizardViewModel vm, SkinElementReader reader, string skinPath)
        {
            foreach (SkinningEntry entry in reader.Files)
            {
                WriteFile(vm, skinPath, entry.Name + entry.PreferredFormat, reader);
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
    }
}
