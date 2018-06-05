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
    /// Interaction logic for SkinMixer.xaml
    /// </summary>
    public partial class SkinMixer : Grid
    {
        private static SkinMixer instance;

        public static SkinMixer Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkinMixer();

                return instance;
            }
        }

        private SkinMixer()
        {
            InitializeComponent();
        }
    }
}
