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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Osmo.Core.Objects;
using Osmo.Core.Simulation;
using Osmo.ViewModel;

namespace Osmo.Controls
{
    /// <summary>
    /// Interaction logic for SimulatedElement.xaml
    /// </summary>
    public partial class SimulatedElement : Grid
    {
        public SimulatedElement()
        {
            InitializeComponent();
        }

        #region AnimationType
        public AnimationType AnimationType
        {
            get { return (AnimationType)GetValue(AnimationTypeProperty); }
            set
            {
                SetValue(AnimationTypeProperty, value);
                (DataContext as SimulatedElementViewModel).AnimationType = value;
            }
        }

        // Using a DependencyProperty as the backing store for AnimationType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register("AnimationType", typeof(AnimationType), typeof(SimulatedElement), new PropertyMetadata(AnimationType.NoAnimation));
        #endregion

        #region ElementType
        public SimulatedElementType ElementType
        {
            get { return (SimulatedElementType)GetValue(ElementTypeProperty); }
            set
            {
                SetValue(ElementTypeProperty, value);
                (DataContext as SimulatedElementViewModel).ElementType = value;
            }
        }

        // Using a DependencyProperty as the backing store for ElementType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementTypeProperty =
            DependencyProperty.Register("ElementType", typeof(SimulatedElementType), typeof(SimulatedElement), new PropertyMetadata(SimulatedElementType.FixedOnly));
        #endregion

        #region FixedElement
        public SkinElement FixedElement
        {
            get { return (SkinElement)GetValue(FixedElementProperty); }
            set
            {
                SetValue(FixedElementProperty, value);
                (DataContext as SimulatedElementViewModel).FixedElement = value;
            }
        }

        // Using a DependencyProperty as the backing store for FixedElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FixedElementProperty =
            DependencyProperty.Register("FixedElement", typeof(SkinElement), typeof(SimulatedElement), new PropertyMetadata(null));
        #endregion

        #region CountElement
        public SkinElement CountElement
        {
            get { return (SkinElement)GetValue(CountElementProperty); }
            set
            {
                SetValue(CountElementProperty, value);
                (DataContext as SimulatedElementViewModel).CountElement = value;
            }
        }

        // Using a DependencyProperty as the backing store for CountElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountElementProperty =
            DependencyProperty.Register("CountElement", typeof(SkinElement), typeof(SimulatedElement), new PropertyMetadata(null));
        #endregion

        #region AnimatedElement
        public SkinElement AnimatedElement
        {
            get { return (SkinElement)GetValue(AnimatedElementProperty); }
            set
            {
                SetValue(AnimatedElementProperty, value);
                (DataContext as SimulatedElementViewModel).AnimatedElement = value;
            }
        }

        // Using a DependencyProperty as the backing store for AnimatedElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimatedElementProperty =
            DependencyProperty.Register("AnimatedElement", typeof(SkinElement), typeof(SimulatedElement), new PropertyMetadata(null));
        #endregion

        #region TransformAnimation
        public Storyboard TransformAnimation
        {
            get { return (Storyboard)GetValue(TransformAnimationProperty); }
            set
            {
                value.Freeze();
                SetValue(TransformAnimationProperty, value);
                (DataContext as SimulatedElementViewModel).TransformAnimation = value;
            }
        }

        // Using a DependencyProperty as the backing store for TransformAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformAnimationProperty =
            DependencyProperty.Register("TransformAnimation", typeof(Storyboard), typeof(SimulatedElement), new PropertyMetadata(null));
        #endregion

        public void StartAnimation()
        {
            if (AnimationType == AnimationType.Predefined)
            {
                (DataContext as SimulatedElementViewModel).StartAnimation();
            }
            else if (AnimationType == AnimationType.RenderTransform)
            {
                TransformAnimation.Begin();
            }
        }

        public void StopAnimation()
        {
            if (AnimationType == AnimationType.Predefined)
            {
                (DataContext as SimulatedElementViewModel).StopAnimation();
            }
            else if (AnimationType == AnimationType.RenderTransform)
            {
                TransformAnimation.Stop();
            }
        }
    }
}
