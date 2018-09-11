using Osmo.Controls;
using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Osmo.Core.Simulation
{
    internal static class ElementTransformHelper
    {
        internal static Storyboard GetApproachAnimation(SimulatedElement targetControl, double fromScale, double toScale, TimeSpan duration)
        {
            Storyboard storyboard = new Storyboard
            {
                Duration = new Duration(duration),
                RepeatBehavior = RepeatBehavior.Forever
            };
            DoubleAnimation approachXAnimation = new DoubleAnimation()
            {
                From = fromScale,
                To = toScale,
                Duration = storyboard.Duration,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(approachXAnimation, targetControl.img_animated);
            Storyboard.SetTargetProperty(approachXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            
            DoubleAnimation approachYAnimation = new DoubleAnimation()
            {
                From = fromScale,
                To = toScale,
                Duration = storyboard.Duration,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(approachYAnimation, targetControl.img_animated);
            Storyboard.SetTargetProperty(approachYAnimation, new PropertyPath("RenderTransform.ScaleY"));

            storyboard.Children.Add(approachXAnimation);
            storyboard.Children.Add(approachYAnimation);

            return storyboard;
        }
    }
}
