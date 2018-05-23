using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using Osmo.Core;
using Osmo.Core.Configuration;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Linq;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for SkinEditor.xaml
    /// </summary>
    public partial class SkinEditor : Grid
    {
        private static SkinEditor instance;
        private string lastPath = null;
        private AudioEngine audio;
        private bool isEnteringProperty;

        private CompletionWindow completionWindow;

        private List<CompletionData> skinIniCompletion = new List<CompletionData>();

        public static SkinEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinEditor();
                return instance;
            }
        }

        #region AudioPosition
        public double AudioPosition
        {
            get { return (double)GetValue(AudioPositionProperty); }
            set { SetValue(AudioPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AudioPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AudioPositionProperty =
            DependencyProperty.Register("AudioPosition", typeof(double), typeof(SkinEditor), new PropertyMetadata(0d));
        #endregion

        //public double AudioPosition { get => audio != null ? audio.AudioPosition : 0; }

        private SkinEditor()
        {
            InitializeComponent();
            audio = new AudioEngine((SkinViewModel)DataContext);
            skinIniCompletion.AddRange(FixedValues.skinIniGeneralCompletionData);
            skinIniCompletion.AddRange(FixedValues.skinIniColoursCompletionData);
            skinIniCompletion.AddRange(FixedValues.skinIniFontsCompletionData);
            skinIniCompletion.AddRange(FixedValues.skinIniCTBCompletionData);
            skinIniCompletion.AddRange(FixedValues.skinIniManiaCompletionData);
        }

        public void LoadSkin(Skin skin)
        {
            ((SkinViewModel)DataContext).LoadedSkin = skin;
        }

        public void SaveSkin()
        {
            ((SkinViewModel)DataContext).SaveSkin();
        }

        private void lv_elements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SkinViewModel vm = (SkinViewModel)DataContext;

            if (lv_elements.SelectedIndex != -1)
                vm.SelectedElement = (SkinElement)lv_elements.SelectedItem;
            else
                vm.SelectedElement = new SkinElement();

            audio.StopAudio();
            vm.PlayStatus = 0;

            vm.ShowIcon = vm.SelectedElement.FileType == FileType.Image ? Visibility.Hidden : Visibility.Visible;
            if (vm.ShowIcon == Visibility.Visible)
            {
                switch (vm.SelectedElement.FileType)
                {
                    case FileType.Audio:
                        vm.Icon = MaterialDesignThemes.Wpf.PackIconKind.FileMusic;
                        break;
                    case FileType.Configuration:
                        vm.Icon = MaterialDesignThemes.Wpf.PackIconKind.FileXml;
                        LoadDocument(vm.SelectedElement.Path);
                        break;
                    case FileType.Unknown:
                        vm.Icon = MaterialDesignThemes.Wpf.PackIconKind.File;
                        break;
                }
            }

            vm.ShowEditor = vm.SelectedElement.Name.ToLower() == "skin.ini" ? Visibility.Visible : Visibility.Hidden;
        }

        private void LoadDocument(string path)
        {
            textEditor.Document = null; // immediately remove old document
            TextDocument doc = new TextDocument(new StringTextSource(File.ReadAllText(path)));
            doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
            Application.Current.Dispatcher.BeginInvoke(
                  new Action(
                      delegate
                      {
                          textEditor.Document = doc;
                      }), DispatcherPriority.Normal);
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            SkinViewModel vm = DataContext as SkinViewModel;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = GetFileFilter(vm.SelectedElement.FileType),
                InitialDirectory = vm.LoadedSkin.Path,
                Title = "Select a file as a replacement..."
            };

            if (!string.IsNullOrWhiteSpace(lastPath))
            {
                openFileDialog.InitialDirectory = lastPath;
            }
            
            if (openFileDialog.ShowDialog() == true)
            {
                //File.Copy(openFileDialog.FileName, vm.SelectedElement.Path, true);
                vm.SelectedElement.ReplaceBackup(new FileInfo(openFileDialog.FileName));
                ((SkinViewModel)DataContext).RefreshImage();

                //Save the last visited directory
                lastPath = Path.GetDirectoryName(openFileDialog.FileName);
                ((SkinViewModel)DataContext).ResetEnabled = true;
            }
        }

        private string GetFileFilter(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Audio:
                    return "Supported audio files|*.mp3;*.wav;*.ogg";
                case FileType.Configuration:
                    return "Supported configuration files|*.ini";
                case FileType.Image:
                    return "Supported image files|*.jpg;*.jpeg;*.png";
                default:
                    return "";
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Reset" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to revert all changes made to this image?",
                "Revert changes?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SkinViewModel vm = (SkinViewModel)DataContext;
                vm.SelectedElement.Reset();
                /*string path = AppConfiguration.GetInstance().BackupDirectory + "\\" + 
                    vm.LoadedSkin.Name + "\\";
                File.Copy(path + vm.SelectedElement.Name, vm.SelectedElement.Path, true);*/
                vm.RefreshImage();
                vm.ResetEnabled = false;
            }
        }

        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Erase" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to erase the image?", 
                "Erase image?", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Exclamation, 
                MessageBoxResult.No);

            string path = ((SkinViewModel)DataContext).SelectedElement.ReplaceBackup(null);

            if (result == MessageBoxResult.Yes)
            {
                using (FileStream stream = new FileStream(
                    ((SkinViewModel)DataContext).SelectedElement.ReplaceBackup(null), 
                    FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(
                        new BitmapImage(new Uri("pack://application:,,,/Osmo;component/Resources/empty.png", UriKind.Absolute))));
                    encoder.Save(stream);
                }
                ((SkinViewModel)DataContext).RefreshImage();
                ((SkinViewModel)DataContext).ResetEnabled = true;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Replace "Delete" MessageBox with MaterialDesign dialog
            var result = MessageBox.Show("Do you really want to delete this element?",
                "Erase image?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                SkinElement element = ((SkinViewModel)DataContext).SelectedElement;
                if (lv_elements.SelectedIndex < lv_elements.Items.Count - 1)
                    lv_elements.SelectedIndex++;
                else
                    lv_elements.SelectedIndex--;
                element.Delete();
                ((SkinViewModel)DataContext).ResetEnabled = false;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            audio.PlayAudio((DataContext as SkinViewModel).SelectedElement.Path);
            if (cb_mute.IsChecked == true)
                audio.SetVolume(0);
            else
                audio.SetVolume(slider_volume.Value);
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            audio.PauseAudio();
        }

        private void Slider_volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audio != null)
            {
                cb_mute.IsChecked = false;
                AppConfiguration.GetInstance().Volume = slider_volume.Value;
                audio.SetVolume(slider_volume.Value);
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            AppConfiguration.GetInstance().IsMuted = cb_mute.IsChecked == true;
            if (cb_mute.IsChecked == true)
                audio.SetVolume(0);
            else
                audio.SetVolume(slider_volume.Value);
        }

        private void TextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            using (Stream s = Application.GetResourceStream(new Uri("pack://application:,,,/Osmo;component/Resources/SkinIniSyntax.xshd", UriKind.Absolute)).Stream)
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
        }

        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!isEnteringProperty && (DataContext as SkinViewModel).SelectedElement.Name.Equals("skin.ini", StringComparison.InvariantCultureIgnoreCase))
            {
                completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                foreach (CompletionData item in skinIniCompletion)
                {
                    if (item.Text.Contains(e.Text))
                    data.Add(item);
                }

                if (data.Count > 0)
                {
                    completionWindow.Show();
                    completionWindow.Closed += delegate { completionWindow = null; };
                }
            }
            else
            {
                completionWindow = null;
            }
        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!isEnteringProperty && e.Text == ":")
            {
                isEnteringProperty = true;
            }
            else if (isEnteringProperty && e.Text == "\n")
            {
                isEnteringProperty = false;
            }

            if (completionWindow != null && !isEnteringProperty)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        private void Container_Loaded(object sender, RoutedEventArgs e)
        {
            slider_volume.Value = AppConfiguration.GetInstance().Volume;
            cb_mute.IsChecked = AppConfiguration.GetInstance().IsMuted;
        }

        private void Slider_Audio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue % 1 != 0)
            {
                audio.SetPosition(slider_audio.Value);
            }
        }

        private void Slider_Audio_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            audio.EnableSliderChange = true;
        }

        private void Slider_Audio_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            audio.EnableSliderChange = false;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            audio.StopAudio();
        }
    }
}
