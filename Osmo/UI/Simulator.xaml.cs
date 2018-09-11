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
                #region osu! elements
                element_combo0.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hit0"));
                element_combo50.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hit50"));
                element_combo100.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hit100"));
                element_combo300.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hit300"));

                #region Score Number initialization
                element_score0.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-0"));
                element_score1.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-1"));
                element_score2.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-2"));
                element_score3.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-3"));
                element_score4.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-4"));
                element_score5.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-5"));
                element_score6.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-6"));
                element_score7.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-7"));
                element_score8.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-8"));
                element_score9.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-9"));
                #endregion

                #region Hitcircle initialization
                element_hitcircle.AnimationType = AnimationType.RenderTransform;
                element_hitcircle.FixedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("hitcircle"));
                element_hitcircle.AnimatedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("approachcircle"));
                element_hitcircle.CountElement = test.Elements.FirstOrDefault(x => x.Name.Contains("default-1"));
                element_hitcircle.TransformAnimation = ElementTransformHelper.GetApproachAnimation(element_hitcircle,
                    2, 1, TimeSpan.FromSeconds(2));
                element_hitcircle.img_animated.RenderTransform = new ScaleTransform(2, 2);
                #endregion
                #endregion

                element_menuBack.AnimatedElement = test.Elements.FirstOrDefault(x => x.Name.Contains("menu-back"));

                element_hitcircle.StartAnimation();
                element_menuBack.StartAnimation();

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
                    StopAllAnimations(panel);
                }
            }
        }

        /// <summary>
        /// Recursive function to stop all animations of nested <see cref="SimulatedElement"/> controls
        /// </summary>
        /// <param name="panel"></param>
        private void StopAllAnimations(Panel panel)
        {
            foreach (Panel childPanel in panel.Children.OfType<Panel>())
            {
                StopAllAnimations(childPanel);
            }

            foreach (SimulatedElement element in panel.Children.OfType<SimulatedElement>()) //Go through each control of type SimulatedElement
            {
                element.StopAnimation();
            }
        }
    }
}
