using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.Core.Reader;
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
    /// Interaction logic for ResizeTool.xaml
    /// </summary>
    public partial class ResizeTool : Grid
    {
        private static ResizeTool _instance;

        public static ResizeTool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ResizeTool();
                }

                return _instance;
            }
        }

        private ResizeTool()
        {
            InitializeComponent();
        }

        public void SelectSkin(Skin skin)
        {
            ResizeToolViewModel vm = DataContext as ResizeToolViewModel;
            vm.SelectedSkinIndex = vm.Skins.IndexOf(skin);
        }

        private async void Rezize_Click(object sender, RoutedEventArgs e)
        {
            ResizeToolViewModel vm = DataContext as ResizeToolViewModel;
            bool keepOriginalFiles = vm.FileOption_keepOriginal;
            bool optimalSize = vm.ResizeOption_OptimalSize;
            string version = vm.SelectedSkin.Version;

            foreach (SkinElement item in vm.SelectedSkin.Elements)
            {
                string newPath = null;
                if (item.IsResizeSelected)
                {
                    if (keepOriginalFiles)
                    {
                        if (item.IsHighDefinition)
                        {
                            newPath = item.Path.Replace("@2x", "");
                        }
                        else
                        {
                            newPath = item.Path.Replace(item.Name, item.Name + "@2x");
                        }
                    }
                    else
                    {
                        newPath = item.Path;
                    }

                    TransformedBitmap source = ResizeImage(item, optimalSize, version, out bool isDistorted);
                    
                    if (isDistorted)
                    {
                        var msgBox = MaterialMessageBox.Show(Helper.FindString("dlg_distortedImageTitle"),
                            string.Format("{0}{1}{2}", Helper.FindString("dlg_distortedImageDescription1"), item.Name, Helper.FindString("dlg_distortedImageDescription2")),
                            Helper.FindString("dlg_distortedImageUseHalveSize"),
                            Helper.FindString("dlg_distortedImageProceed"),
                            Helper.FindString("dlg_distortedImageSkip"), 450);

                        await DialogHost.Show(msgBox);

                        if (msgBox.Result == OsmoMessageBoxResult.CustomActionLeft) //Skip the current element
                        {
                            continue;
                        }
                        else if (msgBox.Result == OsmoMessageBoxResult.CustomActionRight) //Resize the image again but halve image size
                        {
                            source = ResizeImage(item, false, version, out isDistorted);
                        }
                    }

                    if (item.Extension.Contains("png"))
                    {
                        TransformedBitmapToFile<PngBitmapEncoder>(newPath, source);
                    }
                    else
                    {
                        TransformedBitmapToFile<JpegBitmapEncoder>(newPath, source);
                    }
                }
            }
        }

        private TransformedBitmap ResizeImage(SkinElement element, bool resize_optimalSize, string skinVersion,
            out bool isDistorted)
        {
            BitmapSource image = new BitmapImage(new Uri(element.Path, UriKind.Absolute));
            Size targetSize;
            SkinningEntry entry = element.ElementDetails as SkinningEntry;

            if (resize_optimalSize && entry != null && 
                entry.SuggestedSDSize != null && !entry.SuggestedSDSize.Equals(new Size()))
            {
                targetSize = entry.GetSuggestedSizeForVersion(skinVersion);
            }
            else
            {
                targetSize = new Size(Math.Round(element.ImageSize.Width / 2), Math.Round(element.ImageSize.Height / 2));
            }

            double scaleX = targetSize.Width / image.PixelWidth,
                scaleY = targetSize.Height / image.PixelHeight;

            double distortionFactor = scaleX - scaleY;

            isDistorted = distortionFactor > 0.2 || distortionFactor < -0.2;

            return new TransformedBitmap(image,
                new ScaleTransform(
                    scaleX,
                    scaleY));
        }

        private bool TransformedBitmapToFile<T>(string targetPath, BitmapSource source) where T : BitmapEncoder, new()
        {
            if (string.IsNullOrWhiteSpace(targetPath) || source == null)
            {
                return false;
            }

            var frame = BitmapFrame.Create(source);
            var encoder = new T();
            encoder.Frames.Add(frame);
            try
            {
                using (var fs = new FileStream(targetPath, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
