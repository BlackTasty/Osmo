using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.ViewModel;
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
            (DataContext as ResizeToolViewModel).SelectedSkin = skin;
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
    }
}
