using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Osmo.Core.Simulation
{
    class Resolution
    {
        public Size TargetResolution { get; private set; }

        public bool UseHDScaling { get; private set; }

        public string AspectRatio { get; private set; }

        public Resolution(Size resolution, bool useHDScaling)
        {
            TargetResolution = resolution;
            UseHDScaling = useHDScaling;
            if (resolution.Width / resolution.Height > 1.7)
            {
                AspectRatio = "16:9";
            }
            else
            {
                AspectRatio = "4:3";
            }
        }
    }
}
