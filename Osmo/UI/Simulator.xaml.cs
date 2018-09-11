using Osmo.Controls;
using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.Core.Simulation;
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
    /// Interaction logic for Simulator.xaml
    /// </summary>
    public partial class Simulator : DockPanel
    {
        private static Simulator instance;
        
        public static Simulator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Simulator();
                }

                return instance;
            }
        }

        private Simulator()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Skin test = SkinManager.Instance.Skins.FirstOrDefault(x => !x.IsEmpty && x.Name.Contains("Chrom"));

            if (test != null)
            {
                element_combo100.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hit100"));
                element_menuBack.AnimatedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("menu-back"));
                element_hitcircle.AnimationType = AnimationType.RenderTransform;
                element_hitcircle.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hitcircle"));
                element_hitcircle.AnimatedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("approachcircle"));
                element_hitcircle.CountElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-1"));
                element_hitcircle.TransformAnimation = ElementTransformHelper.GetApproachAnimation(element_hitcircle,
                    2, 1, TimeSpan.FromSeconds(2));
                element_hitcircle.img_animated.RenderTransform = new ScaleTransform(2, 2);

                element_hitcircle.StartAnimation();
                element_menuBack.StartAnimation();
                element_combo100.StartAnimation();

            }
            else
            {
                MessageBox.Show("Damn it wrong skin name!");
            }
        }

        public void StopAllAnimations()
        {
            foreach (TabItem item in tabs.Items.OfType<TabItem>()) //Go through each tab item
            {
                if (item.Content is Panel panel)
                {
                    foreach (SimulatedElement element in panel.Children.OfType<SimulatedElement>()) //Go through each control of type SimulatedElement
                    {
                        element.StopAnimation();
                    }
                }
            }
        }
    }
}
