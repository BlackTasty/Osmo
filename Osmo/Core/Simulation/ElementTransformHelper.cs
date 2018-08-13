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
        internal static Storyboard GetApproachAnimation(SimulatedElement targetControl, Size startSize, Size endSize)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(5));
            DoubleAnimation approachXAnimation = new DoubleAnimation()
            {
                From = startSize.Width,
                To = endSize.Width,
                Duration = storyboard.Duration,
                RepeatBehavior = RepeatBehavior.Forever
            };
            DoubleAnimation approachYAnimation = new DoubleAnimation()
            {
                From = startSize.Height,
                To = endSize.Height,
                Duration = storyboard.Duration,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(approachXAnimation, targetControl.img_animated);
            Storyboard.SetTargetProperty(approachXAnimation, new PropertyPath(Image.WidthProperty));
            Storyboard.SetTarget(approachYAnimation, targetControl.img_animated);
            Storyboard.SetTargetProperty(approachYAnimation, new PropertyPath(Image.HeightProperty));

            storyboard.Children.Add(approachXAnimation);
            storyboard.Children.Add(approachYAnimation);

            return storyboard;
        }
    }
}
