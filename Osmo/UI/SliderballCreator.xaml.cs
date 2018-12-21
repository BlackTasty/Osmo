using Osmo.Core;
using Osmo.Core.FileExplorer;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for SliderballCreator.xaml
    /// </summary>
    public partial class SliderballCreator : Grid
    {
        private static SliderballCreator instance;

        public static SliderballCreator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SliderballCreator();
                }

                return instance;
            }
        }

        private SliderballCreator()
        {
            InitializeComponent();
        }

        private void FilePicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            if (sender is FilePicker fp)
            {
                FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

                if (args.Path != null)
                {
                    (DataContext as SBCreatorViewModel).SourceImagePath = args.Path;
                }
            }
        }
       
        private void FilePicker_DialogOpened(object sender, RoutedEventArgs e)
        {
            string path = (DataContext as SBCreatorViewModel).SourceImagePath;
            if (path != null)
            {
                (sender as FilePicker).InitialDirectory = path;
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SBCreatorViewModel).GenerateAnimation();
            SBCreatorViewModel vm = DataContext as SBCreatorViewModel;
            for (int i = 0; i < vm.DummyNames.Count; i++)
            {
                var value = (BitmapSource)vm.Animation[i];
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as SBCreatorViewModel).StopAnimation();
        }

        private void StartStopAnimation_Click(object sender, RoutedEventArgs e)
        {
            SBCreatorViewModel vm = DataContext as SBCreatorViewModel;
            if (!vm.IsAnimationPlaying)
            {
                vm.StartAnimation();
                //Logger.Instance.WriteLog("Animation started!");
            }
            else
            {
                vm.StopAnimation();
                //Logger.Instance.WriteLog("Animation stopped!");
            }
        }

        private void PreviousFrame_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SBCreatorViewModel).PreviousFrame();
        }

        private void NextFrame_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SBCreatorViewModel).NextFrame();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SBCreatorViewModel).Reset();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FolderPicker_DialogClosed(object sender, RoutedEventArgs e)
        {
            FilePickerClosedEventArgs args = e as FilePickerClosedEventArgs;

            if (args.Path != null)
            {
                SBCreatorViewModel vm = DataContext as SBCreatorViewModel;
                for (int i = 0; i < vm.DummyNames.Count; i++)
                {
                    Helper.SaveImage((BitmapSource)vm.Animation[i], args.Path + "\\" + vm.DummyNames[i]);
                }

                string message = Helper.FindString("sb_sliderballSaved");
                snackbar.MessageQueue.Enqueue(message, Helper.FindString("sb_sliderballSavedButton"),
                    param =>
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer",
                            Arguments = string.Format("/e, /select, \"{0}\"", args.Path + "\\" + vm.DummyNames[0])
                        });
                    }, message, false, true);
            }
        }
    }
}
