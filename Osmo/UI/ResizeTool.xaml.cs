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

        public void SetSkin(Skin skin)
        {
            (DataContext as ResizeToolViewModel).SelectedSkin = skin;
        }
    }
}
