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

        private void Select_AllElements_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SkinElement item in lv_skins.Items)
            {
                if (item.FileType == FileType.Image && item.ImageSize.Width > 1 && item.ImageSize.Height > 1)
                {
                    item.IsResizeSelected = true;
                }
            }
        }

        private void Select_HDElements_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SkinElement item in lv_skins.Items)
            {
                if (item.FileType == FileType.Image && item.ImageSize.Width > 1 && item.ImageSize.Height > 1)
                {
                    item.IsResizeSelected = item.IsHighDefinition;
                }
            }
        }

        private void Select_NonHDElements_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SkinElement item in lv_skins.Items)
            {
                if (item.FileType == FileType.Image && item.ImageSize.Width > 1 && item.ImageSize.Height > 1)
                {
                    item.IsResizeSelected = !item.IsHighDefinition;
                }
            }
        }

        private void Select_NoElement_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SkinElement item in lv_skins.Items)
            {
                if (item.FileType == FileType.Image)
                {
                    item.IsResizeSelected = false;
                }
            }
        }

        private void Rezize_Click(object sender, RoutedEventArgs e)
        {
            bool keepOriginalFiles = (DataContext as ResizeToolViewModel).FileOption_keepOriginal;
            bool optimalSize = (DataContext as ResizeToolViewModel).ResizeOption_OptimalSize;
            foreach (SkinElement item in lv_skins.Items)
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

                    TransformedBitmap source = ResizeImage(item, optimalSize);

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

        private TransformedBitmap ResizeImage(SkinElement element, bool resize_optimalSize)
        {
            BitmapSource image = new BitmapImage(new Uri(element.Path, UriKind.Absolute));
            Size targetSize;
            SkinningEntry entry = element.ElementDetails as SkinningEntry;

            if (resize_optimalSize && entry != null && 
                entry.SuggestedSDSize != null && !entry.SuggestedSDSize.Equals(new Size()))
            {
                targetSize = entry.SuggestedSDSize;
            }
            else
            {
                targetSize = new Size(element.ImageSize.Width / 2, element.ImageSize.Height / 2);
            }

            return new TransformedBitmap(image,
                new ScaleTransform(
                    targetSize.Width / image.PixelWidth,
                    targetSize.Height / image.PixelHeight));
            
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
