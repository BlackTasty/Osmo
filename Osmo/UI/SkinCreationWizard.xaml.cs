using MaterialDesignThemes.Wpf;
using Osmo.Core.Configuration;
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
        private static SkinElementReader readerInterface;
        private static SkinElementReader readerStandard;
        private static SkinElementReader readerCatch;
        private static SkinElementReader readerMania;
        private static SkinElementReader readerTaiko;
        private static SkinSoundReader readerSounds;

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
            readerInterface = new SkinElementReader(Properties.Resources.SkinningInterface);
            readerStandard = new SkinElementReader(Properties.Resources.SkinningStandard);
            readerCatch = new SkinElementReader(Properties.Resources.SkinningCatch);
            readerMania = new SkinElementReader(Properties.Resources.SkinningMania);
            readerTaiko = new SkinElementReader(Properties.Resources.SkinningTaiko);
            readerSounds = new SkinSoundReader(Properties.Resources.SkinningSounds);
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
                vm.FilesToCreate += readerInterface.FileCount;
            if (vm.ComponentOsu)
                vm.FilesToCreate += readerStandard.FileCount;
            if (vm.ComponentMania)
                vm.FilesToCreate += readerMania.FileCount;
            if (vm.ComponentCTB)
                vm.FilesToCreate += readerCatch.FileCount;
            if (vm.ComponentTaiko)
                vm.FilesToCreate += readerTaiko.FileCount;
            if (vm.ComponentSounds)
                vm.FilesToCreate += readerSounds.FileCount;

            string skinDirectory = AppConfiguration.GetInstance().OsuDirectory + "\\Skins\\" + vm.Name;
            Directory.CreateDirectory(skinDirectory);

            new Thread(() =>
            {
                if (vm.ComponentInterface)
                {
                    WriteFilesFromReader(vm, readerInterface, skinDirectory);
                }
                if (vm.ComponentOsu)
                {
                    WriteFilesFromReader(vm, readerStandard, skinDirectory);
                }
                if (vm.ComponentMania)
                {
                    WriteFilesFromReader(vm, readerMania, skinDirectory);
                }
                if (vm.ComponentCTB)
                {
                    WriteFilesFromReader(vm, readerCatch, skinDirectory);
                }
                if (vm.ComponentTaiko)
                {
                    WriteFilesFromReader(vm, readerTaiko, skinDirectory);
                }
                if (vm.ComponentSounds)
                {
                    WriteSoundsFromReader(vm, readerSounds, skinDirectory);
                }
            }).Start();

            vm.IsCreating = false;
        }

        private void WriteFilesFromReader(SkinWizardViewModel vm, SkinElementReader reader, string skinPath)
        {
            foreach (SkinningEntry entry in reader.Files)
            {
                WriteFile(vm, skinPath, entry.Name, reader);
            }
        }

        private void WriteSoundsFromReader(SkinWizardViewModel vm, SkinSoundReader reader, string skinPath)
        {
            foreach (SoundEntry entry in reader.Files)
            {
                WriteFile(vm, skinPath, entry.Name + ".wav", reader);
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
