using Osmo.Core.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Osmo.ViewModel
{
    class SimulatorViewModel
    {
        private List<Resolution> mResolutions;

        public List<Resolution> Resolutions { get => mResolutions; }

        public SimulatorViewModel()
        {
            mResolutions = new List<Resolution>()
            {
                new Resolution(new Size(1024,768), false),
                new Resolution(new Size(2048,1536), false),
                new Resolution(new Size(1366,768), false),
                new Resolution(new Size(2732,1536), true),
                new Resolution(new Size(1920,1080), true)
            };
        }
    }
}
