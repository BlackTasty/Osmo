using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for SkinCard.xaml
    /// </summary>
    public partial class SkinCard : Card
    {
        public SkinCard()
        {
            InitializeComponent();
        }

        public SkinCard InitializeSkin(Skin skin)
        {
            (DataContext as SkinCardViewModel).Skin = skin;
            return this;
        }
    }
}
